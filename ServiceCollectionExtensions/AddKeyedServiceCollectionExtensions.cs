namespace DependencyInjectionServiceCollectionExtensions;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

public static class AddKeyedServiceCollectionExtensions
{
    internal static ServiceLifetime AddInheritedTypes(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        ServiceLifetime serviceLifetime,
        Type implementationType,
        params Type[]? inheritedTypes)
    {
        var implementationLifetime = serviceLifetime;
        if (inheritedTypes?.Length > 0)
        {
            // Transient lifestyle breaks this pattern by definition, so use scoped instead
            if (implementationLifetime == ServiceLifetime.Transient)
            {
                implementationLifetime = ServiceLifetime.Scoped;
            }
            // Add a ServiceDescriptor for each service type the implemented type inherits from
            foreach (var serviceType in inheritedTypes)
            {
                // Ensure each implementationType is assignable to the serviceType
                if (!serviceType.IsAssignableFrom(implementationType))
                {
                    throw new ArgumentException($"{serviceType.Name} must be assignable from {implementationType.Name}.");
                }

                // Add a new ServiceDescriptor for each interface of the service type using the previously registered concrete service
                var serviceDescriptorInherited = new ServiceDescriptor(serviceType, provider => provider.GetRequiredService(implementationType), serviceLifetime);
                serviceCollection.Add(serviceDescriptorInherited);
                if (serviceKey != null)
                {
                    var serviceDescriptorInheritedKeyed = new ServiceDescriptor(serviceType, serviceKey, (provider, _) => provider.GetRequiredService(implementationType), serviceLifetime);
                    serviceCollection.Add(serviceDescriptorInheritedKeyed);
                }
            }
            if (serviceKey != null)
            {
                var serviceDescriptorKeyed = new ServiceDescriptor(implementationType, serviceKey, (provider, _) => provider.GetRequiredService(implementationType), serviceLifetime);
                serviceCollection.Add(serviceDescriptorKeyed);
            }
        }
        return implementationLifetime;
    }

    public static IServiceCollection AddKeyed<TImplementation>(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        TImplementation implementationInstance,
        params Type[]? inheritedTypes)
        where TImplementation : class
    {
        ArgumentNullException.ThrowIfNull(implementationInstance);
        // Add a ServiceDescriptor for each inherited service type
        _ = AddInheritedTypes(serviceCollection, serviceKey, ServiceLifetime.Singleton, typeof(TImplementation), inheritedTypes);
        // Add the implemented service type ServiceDescriptor
        serviceCollection.AddSingleton(implementationInstance);
        return serviceCollection;
    }

    public static IServiceCollection AddKeyed<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        ServiceLifetime serviceLifetime,
        params Type[]? inheritedTypes)
    {
        var implementationType = typeof(TImplementation);
        // Add a ServiceDescriptor for each inherited service type
        var implementationLifetime = AddInheritedTypes(serviceCollection, serviceKey, serviceLifetime, implementationType, inheritedTypes);
        // Add the implemented service type ServiceDescriptor
        serviceCollection.Add(new ServiceDescriptor(implementationType, implementationType, implementationLifetime));
        return serviceCollection;
    }

    public static IServiceCollection AddKeyed<TImplementation>(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        ServiceLifetime serviceLifetime,
        Func<IServiceProvider, TImplementation> implementationFactory,
        params Type[]? inheritedTypes)
        where TImplementation : class
    {
        ArgumentNullException.ThrowIfNull(implementationFactory);
        var implementationType = typeof(TImplementation);
        // Add a ServiceDescriptor for each inherited service type
        var implementationLifetime = AddInheritedTypes(serviceCollection, serviceKey, serviceLifetime, implementationType, inheritedTypes);
        // Add the implemented service type ServiceDescriptor
        serviceCollection.Add(new ServiceDescriptor(implementationType, implementationFactory, implementationLifetime));
        return serviceCollection;
    }
}
