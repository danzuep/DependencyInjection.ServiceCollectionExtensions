namespace DependencyInjectionServiceCollectionExtensions;

using System;
using Microsoft.Extensions.DependencyInjection;

// Minimal marker used to register the current factory for T
internal sealed class InnerFactory<T> { }

public static class AddDecoratorServiceCollectionExtensions
{
    // Register a concrete implementation as the seed of the pipeline.
    public static IServiceCollection AddDecorator<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime)
        where TImplementation : class, TService
    {
        var innerFactoryType = typeof(InnerFactory<TService>);
        var implType = typeof(TImplementation);

        // Register the concrete implementation itself
        services.Add(new ServiceDescriptor(implType, implType, lifetime));

        // Register the inner factory Func<TService> which resolves TImplementation (the "seed" factory).
        services.Add(new ServiceDescriptor(innerFactoryType, provider =>
        {
            // seed factory: resolves the concrete implementation (implType) and returns as TService
            Func<TService> seed = () => (TService)provider.GetRequiredService(implType);
            return seed;
        }, lifetime));

        // Register the public service type as invoking the current pipeline factory.
        services.Add(new ServiceDescriptor(typeof(TService), provider =>
        {
            var factory = (Func<TService>)provider.GetRequiredService(innerFactoryType);
            return factory();
        }, lifetime));

        return services;
    }

    // Register a decorator that wraps the current pipeline. The factory receives (provider, next)
    // and must return an instance of TDecorator (which implements TService). This keeps the API tiny.
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
