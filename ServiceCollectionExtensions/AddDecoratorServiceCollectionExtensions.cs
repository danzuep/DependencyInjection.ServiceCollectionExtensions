namespace DependencyInjectionServiceCollectionExtensions;

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

// Minimal marker used to register the current factory for T
internal sealed class InnerFactory<T> { }

public static class AddDecoratorServiceCollectionExtensions
{
    /// <summary>
    /// A pipeline of decorators for the specified service type.
    /// </summary>
    //[Experimental("AddDecorator01", Message = "Untested")]
    //public static IServiceCollection AddDecorator<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TDecorator>(
    //    this IServiceCollection services,
    //    ServiceLifetime lifetime)
    //    where TDecorator : class, TService
    //{
    //    var serviceType = typeof(TService);
    //    var decoratorType = typeof(TDecorator);
    //    services.Add(new ServiceDescriptor(decoratorType, decoratorType, lifetime));
    //    services.Add(new ServiceDescriptor(serviceType, provider => provider.GetRequiredService(decoratorType), lifetime));
    //    return services;
    //}

    /// <summary>
    /// Adds a decorator for <typeparamref name="TService"/> where the decorator type
    /// <typeparamref name="TDecorator"/> wraps the previously registered implementation(s).
    /// The decorator type itself is registered as a concrete service, and a factory
    /// (Func<TService>) is used to build the decorator pipeline. Each resolution of
    /// <typeparamref name="TService"/> invokes the current pipeline factory; the last
    /// registered decorator becomes the outermost decorator returned to callers.
    /// </summary>
    /// <typeparam name="TService">The service contract/interface being decorated.</typeparam>
    /// <typeparam name="TDecorator">The decorator implementation type that implements <typeparamref name="TService"/>.</typeparam>
    /// <param name="services">The service collection to which the decorator registrations are added.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> used for the decorator, its concrete registration, and the inner factory.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so additional registrations can be chained.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the inner factory (Func<TService>) cannot be resolved — i.e. no prior service/factory
    /// for <typeparamref name="TService"/> is available to seed the decorator pipeline.
    /// </exception>
    public static IServiceCollection AddDecorator<TService, TDecorator>(
        this IServiceCollection services,
        ServiceLifetime lifetime)
        where TDecorator : class, TService
    {
        var serviceType = typeof(TService);
        var innerFactoryType = typeof(InnerFactory<TService>);
        var decoratorType = typeof(TDecorator);

        // Register the concrete implementation of the decorator itself
        services.Add(new ServiceDescriptor(decoratorType, decoratorType, lifetime));

        // Register the inner factory Func<TService> which resolves TDecorator (the "seed" factory).
        services.Add(new ServiceDescriptor(innerFactoryType, provider =>
        {
            // seed factory: resolves the concrete implementation (implType) and returns as TService
            Func<TService> seed = () => (TService)provider.GetRequiredService(decoratorType);
            return seed;
        }, lifetime));

        // Register the public service type as invoking the current pipeline factory.
        services.Add(new ServiceDescriptor(typeof(TService), provider =>
        {
            var factory = (Func<TService>)provider.GetRequiredService(innerFactoryType) ??
                throw new InvalidOperationException($"No service registered for type '{serviceType.FullName}'.");
            return factory()!;
        }, lifetime));

        return services;
    }

    // Register a decorator that wraps the current pipeline. The factory receives (provider, next)
    // and must return an instance of TDecorator (which implements TService). This keeps the API tiny.
    [Experimental("AddDecorator02", Message = "Untested")]
    public static IServiceCollection AddDecorator<TService, TDecorator>(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        Func<IServiceProvider, Func<TService>, TDecorator> factory)
        where TDecorator : class, TService
    {
        ArgumentNullException.ThrowIfNull(factory);

        var innerFactoryType = typeof(InnerFactory<TService>);
        var publicServiceType = typeof(TService);

        // Append a new InnerFactory<TService> registration that, when resolved, returns a Func<TService>
        // which creates the decorator and calls into the previous factory for next().
        services.Add(new ServiceDescriptor(innerFactoryType, provider =>
        {
            // Resolve the previous factory at invocation time (so nesting works)
            var previousFactory = (Func<TService>)provider.GetRequiredService(innerFactoryType);

            Func<TService> newFactory = () =>
            {
                // Use the supplied factory to build the decorator given provider and previousFactory
                var decorator = factory(provider, previousFactory);
                if (decorator == null)
                    throw new InvalidOperationException($"Decorator factory for {typeof(TService)} returned null.");
                return decorator;
            };

            return newFactory;
        }, lifetime));

        // Re-register the public service factory that invokes the current top-of-pipeline factory.
        services.Add(new ServiceDescriptor(publicServiceType, provider =>
        {
            var f = (Func<TService>)provider.GetRequiredService(innerFactoryType);
            return f();
        }, lifetime));

        return services;
    }
}
