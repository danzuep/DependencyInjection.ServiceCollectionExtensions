namespace ServiceCollectionSimplified;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

public interface IService
{
    public void Write(string msg);
}

public class ServiceImpl : IService
{
    public void Write(string msg) => Console.WriteLine($"Impl: {msg}");
}

public class DecoratorA : IService
{
    private readonly Func<IService> _next;
    public DecoratorA(Func<IService> next) => _next = next;
    public void Write(string msg)
    {
        Console.WriteLine("[DecoratorA] before");
        _next().Write(msg);
        Console.WriteLine("[DecoratorA] after");
    }
}

// Another decorator that depends on something else from DI and takes Func<IService> next
public class DecoratorB : IService
{
    private readonly Func<IService> _next;
    private readonly Guid _id;
    public DecoratorB(Func<IService> next, Guid id)
    {
        _next = next;
        _id = id;
    }
    public void Write(string msg)
    {
        Console.WriteLine($"[DecoratorB {_id}] before");
        _next().Write(msg);
        Console.WriteLine($"[DecoratorB {_id}] after");
    }
}

internal sealed class InnerFactory<T> { }

public static class FluentServiceCollectionExtensions
{
    public static IServiceCollection WithBindings(this IServiceCollection services, Action<IFluentBinder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var binder = new FluentBinder(services);
        configure(binder);
        return services;
    }
}

internal class FluentBinder : IFluentBinder
{
    private readonly IServiceCollection _services;
    public FluentBinder(IServiceCollection services) => _services = services;

    public IBindSyntax<TService> Bind<TService>() => new BindSyntax<TService>(_services);

    public IDecorateSyntax<TService> Decorate<TService>() => new DecorateSyntax<TService>(_services);
}

internal class BindSyntax<TService> : IBindSyntax<TService>
{
    private readonly IServiceCollection _services;
    public BindSyntax(IServiceCollection services) => _services = services;

    public IBindSyntax<TService> Add<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TImplementation : class, TService
    {
        var innerFactoryType = typeof(InnerFactory<TService>);
        var implType = typeof(TImplementation);

        // Register the concrete implementation itself
        _services.Add(new ServiceDescriptor(implType, implType, lifetime));

        // Register the inner factory Func<TService> which resolves TImplementation (the "seed" factory).
        // We register InnerFactory<TService> as a Func<TService>.
        _services.Add(new ServiceDescriptor(innerFactoryType, provider =>
        {
            // seed factory: resolves the concrete implementation (implType) and returns as TService
            Func<TService> seed = () => (TService)provider.GetRequiredService(implType);
            return seed;
        }, lifetime));

        // Register the public service type if nobody decorates: we still add a factory that calls the current factory (could be overridden later by decorators)
        _services.Add(new ServiceDescriptor(typeof(TService), provider =>
        {
            var factory = (Func<TService>)provider.GetRequiredService(innerFactoryType);
            return factory();
        }, lifetime));

        return this;
    }
}

internal class DecorateSyntax<TService> : IDecorateSyntax<TService>
{
    private readonly IServiceCollection _services;
    private readonly List<Type> _decorators = [];
    public DecorateSyntax(IServiceCollection services) => _services = services;

    public IDecorateSyntax<TService> AddWithLifetime<TDecorator>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TDecorator : class, TService
    {
        _decorators.Add(typeof(TDecorator));

        // Each time With<TDecorator>() is called we wrap the existing InnerFactory<TService> with a new factory
        WrapFactoryWithDecorator(typeof(TDecorator), lifetime);

        return this;
    }

    private void WrapFactoryWithDecorator(Type decoratorType, ServiceLifetime lifetime)
    {
        var innerFactoryType = typeof(InnerFactory<TService>);
        var publicType = typeof(TService);

        // Get the currently registered factory for InnerFactory<TService>. We will replace it with a new factory that
        // returns a Func<TService> which, when invoked, constructs the decorator and passes the previous Func<TService> as next.
        // We must find the last registration for innerFactoryType in the IServiceCollection.
        // For simplicity we read the last registration's ImplementationFactory by building a closure around resolving the previous factory at runtime.

        // Remove any public TService registrations so we can re-add a new public factory that uses the updated inner factory.
        for (int i = _services.Count - 1; i >= 0; i--)
        {
            if (_services[i].ServiceType == publicType)
                _services.RemoveAt(i);
        }

        // Add a new InnerFactory<TService> descriptor that, when resolved, returns a Func<TService> that
        // builds decoratorType using ActivatorUtilities.CreateInstance(provider, decoratorType, next)
        _services.Add(new ServiceDescriptor(innerFactoryType, provider =>
        {
            // resolve the current inner factory (previous one) at runtime
            var previousFactory = (Func<TService>)provider.GetRequiredService(innerFactoryType);

            // new factory: when invoked, creates the decorator by passing previousFactory as Func<TService> next
            Func<TService> newFactory = () =>
            {
                // We want the decorator's ctor to accept Func<TService> next as a parameter.
                // ActivatorUtilities will attempt to fill other ctor dependencies from provider; we supply 'previousFactory' explicitly.
                var created = (TService)ActivatorUtilities.CreateInstance(provider, decoratorType, previousFactory);
                return created;
            };

            return newFactory;
        }, lifetime));

        // Add/update the public registration to resolve the top-level factory and invoke it.
        _services.Add(new ServiceDescriptor(publicType, provider =>
        {
            var factory = (Func<TService>)provider.GetRequiredService(innerFactoryType);
            return factory();
        }, lifetime));
    }
}
