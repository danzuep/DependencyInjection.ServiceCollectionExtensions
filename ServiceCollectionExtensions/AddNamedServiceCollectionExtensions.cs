namespace DependencyInjectionServiceCollectionExtensions;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

public static class AddNamedServiceCollectionExtensions
{
    private static ServiceLifetime AddNamed<TService, TImplementation>(
        IServiceCollection services,
        ServiceLifetime lifetime,
        object? serviceKey = null)
        where TService : class
        where TImplementation : class, TService
    {
        var serviceType = typeof(TService);
        var implementationType = typeof(TImplementation);
        serviceKey ??= implementationType.Name;
        services.Add(new ServiceDescriptor(serviceType, provider => provider.GetRequiredService(implementationType), lifetime));
        services.Add(new ServiceDescriptor(serviceType, serviceKey, (provider, _) => provider.GetRequiredService(implementationType), lifetime));
        return lifetime == ServiceLifetime.Transient ? ServiceLifetime.Scoped : lifetime;
    }

    public static IServiceCollection AddNamed<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TService : class
        where TImplementation : class, TService
    {
        var implementationType = typeof(TImplementation);
        var implementationLifetime = AddNamed<TService, TImplementation>(serviceCollection, lifetime, serviceKey);
        serviceCollection.Add(new ServiceDescriptor(implementationType, implementationType, implementationLifetime));
        return serviceCollection;
    }

    public static IServiceCollection AddNamed<TService, TImplementation>(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        TImplementation implementationInstance,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TService : class
        where TImplementation : class, TService
    {
        _ = AddNamed<TService, TImplementation>(serviceCollection, lifetime, serviceKey);
        serviceCollection.AddSingleton(implementationInstance);
        return serviceCollection;
    }

    public static IServiceCollection AddNamed<TService, TImplementation>(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TService : class
        where TImplementation : class, TService
    {
        var implementationLifetime = AddNamed<TService, TImplementation>(serviceCollection, lifetime, serviceKey);
        serviceCollection.Add(new ServiceDescriptor(typeof(TImplementation), implementationFactory, implementationLifetime));
        return serviceCollection;
    }

    public static IServiceCollection AddNamed<TService, TImplementation>(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        Func<IServiceProvider, object?, TImplementation> implementationFactory,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TService : class
        where TImplementation : class, TService =>
        serviceCollection.AddNamed<TService, TImplementation>(serviceKey, p => implementationFactory(p, null), lifetime);
}
