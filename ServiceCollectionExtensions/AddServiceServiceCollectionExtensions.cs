namespace DependencyInjectionServiceCollectionExtensions;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

public static class AddServiceServiceCollectionExtensions
{
    public static IServiceCollection AddService<TService, TImplementation>(
        this IServiceCollection serviceCollection,
        TImplementation implementationInstance,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TService : class
        where TImplementation : class, TService
    {
        serviceCollection.Add(new ServiceDescriptor(typeof(TImplementation), implementationInstance));
        serviceCollection.Add(new ServiceDescriptor(typeof(TService), provider => provider.GetRequiredService<TImplementation>(), lifetime));
        return serviceCollection;
    }

    public static IServiceCollection AddService<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection serviceCollection,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TService : class
        where TImplementation : class, TService
    {
        var serviceType = typeof(TService);
        var implementationType = typeof(TImplementation);
        var implementationLifetime = lifetime == ServiceLifetime.Transient ? ServiceLifetime.Scoped : lifetime;
        serviceCollection.Add(new ServiceDescriptor(implementationType, implementationType, implementationLifetime));
        serviceCollection.Add(new ServiceDescriptor(serviceType, provider => provider.GetRequiredService(implementationType), lifetime));
        return serviceCollection;
    }

    public static IServiceCollection AddService<TService, TImplementation>(
        this IServiceCollection serviceCollection,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TService : class
        where TImplementation : class, TService
    {
        var serviceType = typeof(TService);
        var implementationType = typeof(TImplementation);
        var implementationLifetime = lifetime == ServiceLifetime.Transient ? ServiceLifetime.Scoped : lifetime;
        serviceCollection.Add(new ServiceDescriptor(implementationType, implementationFactory, implementationLifetime));
        serviceCollection.Add(new ServiceDescriptor(serviceType, provider => provider.GetRequiredService(implementationType), lifetime));
        return serviceCollection;
    }
}
