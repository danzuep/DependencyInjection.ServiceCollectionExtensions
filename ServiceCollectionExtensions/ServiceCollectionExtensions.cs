namespace DependencyInjectionServiceCollectionExtensions;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Add<TService, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        where TService : class
        where TImplementation : class, TService =>
        services.Add(implementationFactory, serviceLifetime, typeof(TService));

    public static IServiceCollection Add<T1, T2, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add(implementationFactory, serviceLifetime, typeof(T1), typeof(T2));

    public static IServiceCollection Add<T1, T2, T3, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(implementationFactory);

        // Register a factory method to create instances
        Func<IServiceProvider, TImplementation> factoryMethod = provider => provider.GetRequiredService<TImplementation>();

        // Register the concrete service type first
        var concreteLifetime = serviceLifetime == ServiceLifetime.Transient ? ServiceLifetime.Scoped : serviceLifetime;
        services.Add(new ServiceDescriptor(typeof(TImplementation), implementationFactory, concreteLifetime));

        // Register each interface of the service type
        foreach (var serviceType in serviceTypes)
        {
            // Ensure TImplementation is assignable to the serviceType
            if (!serviceType.IsAssignableFrom(typeof(TImplementation)))
            {
                throw new ArgumentException($"{serviceType.Name} must be assignable from {typeof(TImplementation).Name}.");
            }

            // Register the service
            services.Add(new ServiceDescriptor(serviceType, factoryMethod, serviceLifetime));
        }

        return services;
    }
}
