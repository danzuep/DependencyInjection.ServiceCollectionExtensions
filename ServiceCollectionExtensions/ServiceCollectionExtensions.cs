namespace DependencyInjectionServiceCollectionExtensions;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingleton<T1, T2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add(implementationFactory, ServiceLifetime.Singleton, typeof(T1), typeof(T2));

    public static IServiceCollection AddSingleton<T1, T2, T3, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add(implementationFactory, ServiceLifetime.Singleton, typeof(T1), typeof(T2), typeof(T3));

    public static IServiceCollection AddTransient<T1, T2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add(implementationFactory, ServiceLifetime.Transient, typeof(T1), typeof(T2));

    public static IServiceCollection AddTransient<T1, T2, T3, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add(implementationFactory, ServiceLifetime.Transient, typeof(T1), typeof(T2), typeof(T3));

    public static IServiceCollection AddScoped<T1, T2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add(implementationFactory, ServiceLifetime.Scoped, typeof(T1), typeof(T2));

    public static IServiceCollection AddScoped<T1, T2, T3, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add(implementationFactory, ServiceLifetime.Scoped, typeof(T1), typeof(T2), typeof(T3));

    public static IServiceCollection Add<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime)
        where TService : class
        where TImplementation : class, TService =>
        services.Add(implementationFactory, serviceLifetime, typeof(TService));

    public static IServiceCollection Add<T1, T2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add(implementationFactory, serviceLifetime, typeof(T1), typeof(T2));

    public static IServiceCollection Add<T1, T2, T3, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add(implementationFactory, serviceLifetime, typeof(T1), typeof(T2), typeof(T3));

    public static IServiceCollection Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime,
        params Type[] serviceTypes)
        where TImplementation : class
    {
        var serviceDescriptors = GetServiceDescriptors(serviceLifetime, implementationFactory, serviceTypes);

        // Register the services
        foreach (var serviceDescriptor in serviceDescriptors)
        {
            services.Add(serviceDescriptor);
        }

        return services;
    }

    private static List<ServiceDescriptor> Add(this List<ServiceDescriptor> serviceDescriptors, ServiceLifetime lifetime, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType, params Type[] inheritedTypes)
    {
        ArgumentNullException.ThrowIfNull(serviceDescriptors);

        if (inheritedTypes?.Length > 0)
        {
            foreach (var serviceType in inheritedTypes)
            {
                // Ensure TImplementation is assignable to the serviceType
                if (!serviceType.IsAssignableFrom(implementationType))
                {
                    throw new ArgumentException($"{serviceType.Name} must be assignable from {implementationType.Name}.");
                }

                // Add a new ServiceDescriptor for each interface of the service type using the previously registered concrete service
                serviceDescriptors.Add(new ServiceDescriptor(serviceType, provider => provider.GetRequiredService(implementationType), lifetime));
            }
        }

        return serviceDescriptors;
    }

    public static IList<ServiceDescriptor> GetServiceDescriptors<TImplementation>(ServiceLifetime lifetime, Func<IServiceProvider, TImplementation> factory, params Type[] serviceTypes)
        where TImplementation : class
    {
        ArgumentNullException.ThrowIfNull(factory);

        var serviceDescriptors = new List<ServiceDescriptor>();
        var implementationType = typeof(TImplementation);

        // Transient lifestyle breaks this pattern by definition, so use scoped instead
        var concreteLifetime = lifetime == ServiceLifetime.Transient ? ServiceLifetime.Scoped : lifetime;

        // Add a ServiceDescriptor for the concrete service type
        serviceDescriptors.Add(new ServiceDescriptor(implementationType, factory, concreteLifetime));

        // Add a ServiceDescriptors for each inherited service type
        serviceDescriptors.Add(lifetime, implementationType, serviceTypes);

        return serviceDescriptors;
    }

    public static IList<ServiceDescriptor> GetServiceDescriptors(ServiceLifetime lifetime, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType, params Type[] inheritedTypes)
    {
        ArgumentNullException.ThrowIfNull(implementationType);

        var serviceDescriptors = new List<ServiceDescriptor>();

        // Transient lifestyle breaks this pattern by definition, so use scoped instead
        var concreteLifetime = lifetime == ServiceLifetime.Transient ? ServiceLifetime.Scoped : lifetime;

        // Add a ServiceDescriptor for the implemented service type
        serviceDescriptors.Add(new ServiceDescriptor(implementationType, concreteLifetime));

        // Add a ServiceDescriptors for each inherited service type
        serviceDescriptors.Add(lifetime, implementationType, inheritedTypes);

        return serviceDescriptors;
    }

    public static IServiceCollection Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime,
        params Type[] serviceTypes)
        where TImplementation : class
    {
        var serviceDescriptors = GetServiceDescriptors(serviceLifetime, typeof(TImplementation), serviceTypes);

        // Register the services
        foreach (var serviceDescriptor in serviceDescriptors)
        {
            services.Add(serviceDescriptor);
        }

        return services;
    }

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
}
