namespace DependencyInjectionServiceCollectionExtensions;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingleton<T1, T2, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add(implementationFactory, ServiceLifetime.Singleton, typeof(T1), typeof(T2));

    public static IServiceCollection AddSingleton<T1, T2, T3, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add(implementationFactory, ServiceLifetime.Singleton, typeof(T1), typeof(T2), typeof(T3));

    public static IServiceCollection AddTransient<T1, T2, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add(implementationFactory, ServiceLifetime.Transient, typeof(T1), typeof(T2));

    public static IServiceCollection AddTransient<T1, T2, T3, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add(implementationFactory, ServiceLifetime.Transient, typeof(T1), typeof(T2), typeof(T3));

    public static IServiceCollection AddScoped<T1, T2, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add(implementationFactory, ServiceLifetime.Scoped, typeof(T1), typeof(T2));

    public static IServiceCollection AddScoped<T1, T2, T3, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add(implementationFactory, ServiceLifetime.Scoped, typeof(T1), typeof(T2), typeof(T3));

    public static IServiceCollection Add<TService, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime)
        where TService : class
        where TImplementation : class, TService =>
        services.Add(implementationFactory, serviceLifetime, typeof(TService));

    public static IServiceCollection Add<T1, T2, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add(implementationFactory, serviceLifetime, typeof(T1), typeof(T2));

    public static IServiceCollection Add<T1, T2, T3, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add(implementationFactory, serviceLifetime, typeof(T1), typeof(T2), typeof(T3));

    public static IServiceCollection Add<TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime,
        params Type[] serviceTypes)
        where TImplementation : class
    {
        var serviceDescriptors = GetServiceDescriptors(implementationFactory, serviceLifetime, serviceTypes);

        // Register the services
        foreach (var serviceDescriptor in serviceDescriptors)
        {
            services.Add(serviceDescriptor);
        }

        return services;
    }

    public static IList<ServiceDescriptor> GetServiceDescriptors<TImplementation>(Func<IServiceProvider, TImplementation> factory, ServiceLifetime lifetime, params Type[] serviceTypes)
        where TImplementation : class
    {
        ArgumentNullException.ThrowIfNull(factory);

        var serviceDescriptors = new List<ServiceDescriptor>();

        // Transient lifestyle breaks this pattern by definition, so use scoped instead
        var concreteLifetime = lifetime == ServiceLifetime.Transient ? ServiceLifetime.Scoped : lifetime;

        // Add a ServiceDescriptor for the concrete service type
        serviceDescriptors.Add(new ServiceDescriptor(typeof(TImplementation), factory, concreteLifetime));

        if (serviceTypes?.Length > 0)
        {
            foreach (var serviceType in serviceTypes)
            {
                // Ensure TImplementation is assignable to the serviceType
                if (!serviceType.IsAssignableFrom(typeof(TImplementation)))
                {
                    throw new ArgumentException($"{serviceType.Name} must be assignable from {typeof(TImplementation).Name}.");
                }

                // Add a new ServiceDescriptor for each interface of the service type using the previously registered concrete service
                serviceDescriptors.Add(new ServiceDescriptor(serviceType, provider => provider.GetRequiredService<TImplementation>(), lifetime));
            }
        }

        return serviceDescriptors;
    }
}
