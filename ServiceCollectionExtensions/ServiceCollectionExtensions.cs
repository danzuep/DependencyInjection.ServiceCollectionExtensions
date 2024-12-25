namespace DependencyInjectionServiceCollectionExtensions;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingleton<T1, T2, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add(ServiceLifetime.Singleton, implementationFactory, typeof(T1), typeof(T2));

    public static IServiceCollection AddSingleton<T1, T2, T3, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add(ServiceLifetime.Singleton, implementationFactory, typeof(T1), typeof(T2), typeof(T3));

    public static IServiceCollection AddTransient<T1, T2, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add(ServiceLifetime.Transient, implementationFactory, typeof(T1), typeof(T2));

    public static IServiceCollection AddTransient<T1, T2, T3, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add(ServiceLifetime.Transient, implementationFactory, typeof(T1), typeof(T2), typeof(T3));

    public static IServiceCollection AddScoped<T1, T2, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add(ServiceLifetime.Scoped, implementationFactory, typeof(T1), typeof(T2));

    public static IServiceCollection AddScoped<T1, T2, T3, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add(ServiceLifetime.Scoped, implementationFactory, typeof(T1), typeof(T2), typeof(T3));

    public static IServiceCollection Add<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService =>
        services.Add(serviceLifetime, implementationFactory, typeof(TService));

    public static IServiceCollection Add<T1, T2, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add(serviceLifetime, implementationFactory, typeof(T1), typeof(T2));

    public static IServiceCollection Add<T1, T2, T3, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add(serviceLifetime, implementationFactory, typeof(T1), typeof(T2), typeof(T3));

    private static ServiceLifetime AddInheritedTypes(
        IServiceCollection serviceCollection,
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

    public static IServiceCollection Add<TImplementation>(
        this IServiceCollection serviceCollection,
        ServiceLifetime serviceLifetime,
        Func<IServiceProvider, TImplementation> implementationFactory,
        params Type[]? inheritedTypes)
        where TImplementation : class =>
        serviceCollection.AddKeyed(null, serviceLifetime, implementationFactory, inheritedTypes);

    public static IServiceCollection Add<TImplementation>(
        this IServiceCollection serviceCollection,
        TImplementation implementationInstance,
        params Type[]? inheritedTypes)
        where TImplementation : class =>
        serviceCollection.AddKeyed(null, implementationInstance, inheritedTypes);

    public static IServiceCollection Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection serviceCollection,
        ServiceLifetime serviceLifetime,
        params Type[]? inheritedTypes) =>
        serviceCollection.AddKeyed<TImplementation>(null, serviceLifetime, inheritedTypes);

    public static IServiceCollection Add<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime)
        where TService : class
        where TImplementation : class, TService =>
        services.Add<TImplementation>(serviceLifetime, typeof(TService));

    public static IServiceCollection Add<T1, T2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add<TImplementation>(serviceLifetime, typeof(T1), typeof(T2));

    public static IServiceCollection Add<T1, T2, T3, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add<TImplementation>(serviceLifetime, typeof(T1), typeof(T2), typeof(T3));

    public static IServiceCollection AddSingleton<T1, T2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add<TImplementation>(ServiceLifetime.Singleton, typeof(T1), typeof(T2));

    public static IServiceCollection AddSingleton<T1, T2, T3, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add<TImplementation>(ServiceLifetime.Singleton, typeof(T1), typeof(T2), typeof(T3));

    public static IServiceCollection AddTransient<T1, T2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add<TImplementation>(ServiceLifetime.Transient, typeof(T1), typeof(T2));

    public static IServiceCollection AddTransient<T1, T2, T3, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add<TImplementation>(ServiceLifetime.Transient, typeof(T1), typeof(T2), typeof(T3));

    public static IServiceCollection AddScoped<T1, T2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add<TImplementation>(ServiceLifetime.Scoped, typeof(T1), typeof(T2));

    public static IServiceCollection AddScoped<T1, T2, T3, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add<TImplementation>(ServiceLifetime.Scoped, typeof(T1), typeof(T2), typeof(T3));

    #region Other ideas

    //[Experimental("TypedServices")]
    public static IServiceCollection AddAllInterfaces(
        this IServiceCollection serviceCollection,
        Type implementationType,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        ArgumentNullException.ThrowIfNull(implementationType);
        foreach (var assemblyType in implementationType.Assembly.GetTypes())
        {
            foreach (var serviceType in assemblyType.GetInterfaces())
            {
                if (serviceType.IsAssignableFrom(implementationType))
                {
                    serviceCollection.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
                }
            }
        }
        return serviceCollection;
    }

    //[Experimental("ServiceInterfaces")]
    public static ServiceLifetime AddWithInterfaces<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection serviceCollection,
        ServiceLifetime serviceLifetime,
        object? serviceKey = null)
    {
        // Add a ServiceDescriptor for the implemented service type
        var implementationType = typeof(TImplementation);
        // Transient lifestyle breaks this pattern by definition, so use scoped instead
        var implementationLifetime = serviceLifetime == ServiceLifetime.Transient ? ServiceLifetime.Scoped : serviceLifetime;
        serviceCollection.Add(new ServiceDescriptor(implementationType, implementationType, implementationLifetime));
        if (serviceKey != null)
        {
            serviceCollection.Add(new ServiceDescriptor(implementationType, serviceKey, (provider, _) => provider.GetRequiredService(implementationType), serviceLifetime));
        }
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
        return implementationLifetime;
    }

    private static ServiceLifetime AddKeyed<TService, TImplementation>(
        IServiceCollection services,
        object? serviceKey,
        ServiceLifetime lifetime)
        where TService : class
        where TImplementation : class, TService
    {
        var serviceType = typeof(TService);
        var implementationType = typeof(TImplementation);
        serviceKey ??= implementationType.Name;
        services.Add(new ServiceDescriptor(serviceType, provider => provider.GetRequiredService(implementationType), lifetime));
        services.Add(new ServiceDescriptor(serviceType, serviceKey, (provider, _) => provider.GetRequiredService(implementationType), lifetime));
        services.Add(new ServiceDescriptor(implementationType, serviceKey, (provider, _) => provider.GetRequiredService(implementationType), lifetime));
        return lifetime == ServiceLifetime.Transient ? ServiceLifetime.Scoped : lifetime;
    }

    //[Experimental("NamedServices")]
    public static IServiceCollection AddNamed<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TService : class
        where TImplementation : class, TService
    {
        var implementationType = typeof(TImplementation);
        var implementationLifetime = AddKeyed<TService, TImplementation>(serviceCollection, serviceKey, lifetime);
        serviceCollection.Add(new ServiceDescriptor(implementationType, implementationType, implementationLifetime));
        return serviceCollection;
    }

    //[Experimental("NamedServiceInstance")]
    public static IServiceCollection AddNamed<TService, TImplementation>(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        TImplementation implementationInstance,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TService : class
        where TImplementation : class, TService
    {
        _ = AddKeyed<TService, TImplementation>(serviceCollection, serviceKey, lifetime);
        serviceCollection.AddSingleton(implementationInstance);
        return serviceCollection;
    }

    //[Experimental("NamedServiceFactory")]
    public static IServiceCollection AddNamed<TService, TImplementation>(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TService : class
        where TImplementation : class, TService
    {
        var implementationLifetime = AddKeyed<TService, TImplementation>(serviceCollection, serviceKey, lifetime);
        serviceCollection.Add(new ServiceDescriptor(typeof(TImplementation), implementationFactory, implementationLifetime));
        return serviceCollection;
    }

    //[Experimental("NamedServiceFactoryImplementation")]
    public static IServiceCollection AddNamed<TService, TImplementation>(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        Func<IServiceProvider, object?, TImplementation> implementationFactory,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TService : class
        where TImplementation : class, TService =>
        serviceCollection.AddNamed<TService, TImplementation>(serviceKey, p => implementationFactory(p, null), lifetime);

    #endregion
}
