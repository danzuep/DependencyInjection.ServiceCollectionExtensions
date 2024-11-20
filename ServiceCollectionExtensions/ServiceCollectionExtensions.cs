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

    private static IServiceCollection AddInheritedTypes(
        IServiceCollection collection,
        ServiceLifetime lifetime,
        Type implementationType,
        params Type[] inheritedTypes)
    {
        // Add a ServiceDescriptors for each service type the implemented type inherits from
        if (inheritedTypes?.Length > 0)
        {
            foreach (var serviceType in inheritedTypes)
            {
                // Ensure each implementationType is assignable to the serviceType
                if (!serviceType.IsAssignableFrom(implementationType))
                {
                    throw new ArgumentException($"{serviceType.Name} must be assignable from {implementationType.Name}.");
                }

                // Add a new ServiceDescriptor for each interface of the service type using the previously registered concrete service
                var serviceDescriptor = new ServiceDescriptor(serviceType, provider => provider.GetRequiredService(implementationType), lifetime);
                collection.Add(serviceDescriptor);
            }
        }
        return collection;
    }

    public static IServiceCollection Add<TImplementation>(
        this IServiceCollection collection,
        ServiceLifetime serviceLifetime,
        Func<IServiceProvider, TImplementation> implementationFactory,
        params Type[] inheritedTypes)
        where TImplementation : class
    {
        ArgumentNullException.ThrowIfNull(implementationFactory);
        var implementationType = typeof(TImplementation);
        // Transient lifestyle breaks this pattern by definition, so use scoped instead
        var concreteLifetime = serviceLifetime == ServiceLifetime.Transient ? ServiceLifetime.Scoped : serviceLifetime;
        // Add the implemented service type ServiceDescriptor
        collection.Add(new ServiceDescriptor(implementationType, implementationFactory, concreteLifetime));
        // Add a ServiceDescriptor for each inherited service type
        AddInheritedTypes(collection, serviceLifetime, implementationType, inheritedTypes);
        return collection;
    }

    public static IServiceCollection Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection serviceCollection,
        ServiceLifetime serviceLifetime,
        params Type[] inheritedTypes)
    {
        var implementationType = typeof(TImplementation);
        // Transient lifestyle breaks this pattern by definition, so use scoped instead
        var concreteLifetime = serviceLifetime == ServiceLifetime.Transient ? ServiceLifetime.Scoped : serviceLifetime;
        // Add the implemented service type ServiceDescriptor
        serviceCollection.Add(new ServiceDescriptor(implementationType, implementationType, concreteLifetime));
        // Add a ServiceDescriptor for each inherited service type
        AddInheritedTypes(serviceCollection, serviceLifetime, implementationType, inheritedTypes);
        return serviceCollection;
    }

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
}
