namespace DependencyInjectionServiceCollectionExtensions;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

public static class AddInterfaceServiceCollectionExtensions
{
    private static ServiceLifetime AddInterfaces<TImplementation>(
        IServiceCollection serviceCollection,
        Type implementationType,
        ServiceLifetime serviceLifetime,
        object? serviceKey = null)
    {
        ArgumentNullException.ThrowIfNull(implementationType);
        // Add a ServiceDescriptor for each service type the implemented type inherits from
        foreach (var serviceType in implementationType.GetInterfaces())
        {
            if (serviceType.IsAssignableFrom(implementationType))
            {
                serviceCollection.Add(new ServiceDescriptor(serviceType, provider => provider.GetRequiredService(implementationType), serviceLifetime));
                if (serviceKey != null)
                {
                    serviceCollection.Add(new ServiceDescriptor(serviceType, serviceKey, (provider, _) => provider.GetRequiredService(implementationType), serviceLifetime));
                }
            }
        }
        // Transient lifestyle breaks this pattern by definition, so use scoped instead
        var implementationLifetime = serviceLifetime == ServiceLifetime.Transient ? ServiceLifetime.Scoped : serviceLifetime;
        return implementationLifetime;
    }

    public static IServiceCollection AddWithInterfaces<TImplementation>(
        this IServiceCollection serviceCollection,
        TImplementation implementationInstance,
        ServiceLifetime serviceLifetime,
        object? serviceKey = null)
    {
        ArgumentNullException.ThrowIfNull(implementationInstance);
        var implementationType = typeof(TImplementation);
        _ = AddInterfaces<TImplementation>(serviceCollection, implementationType, serviceLifetime, serviceKey);
        serviceCollection.Add(new ServiceDescriptor(implementationType, implementationInstance));
        return serviceCollection;
    }

    public static IServiceCollection AddWithInterfaces<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection serviceCollection,
        ServiceLifetime serviceLifetime,
        object? serviceKey = null)
    {
        var implementationType = typeof(TImplementation);
        var implementationLifetime = AddInterfaces<TImplementation>(serviceCollection, implementationType, serviceLifetime, serviceKey);
        serviceCollection.Add(new ServiceDescriptor(implementationType, implementationType, implementationLifetime));
        return serviceCollection;
    }

    public static IServiceCollection AddWithInterfaces<TImplementation>(
        this IServiceCollection serviceCollection,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime,
        object? serviceKey = null)
        where TImplementation : class
    {
        ArgumentNullException.ThrowIfNull(implementationFactory);
        var implementationType = typeof(TImplementation);
        var implementationLifetime = AddInterfaces<TImplementation>(serviceCollection, implementationType, serviceLifetime, serviceKey);
        serviceCollection.Add(new ServiceDescriptor(implementationType, implementationFactory, implementationLifetime));
        return serviceCollection;
    }
}
