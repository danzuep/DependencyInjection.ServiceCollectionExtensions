namespace DependencyInjectionServiceCollectionExtensions;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

public static class AddServiceCollectionExtensions
{
    [Experimental("AddImplementation01", Message = "Untested")]
    public static IServiceCollection AddImplementation<TImplementation>(
        this IServiceCollection serviceCollection,
        TImplementation implementationInstance,
        params Type[]? inheritedTypes)
    {
        ArgumentNullException.ThrowIfNull(implementationInstance);
        var implementationType = typeof(TImplementation);
        // Add a ServiceDescriptor for each inherited service type
        _ = serviceCollection.AddInheritedTypes(null, ServiceLifetime.Singleton, implementationType, inheritedTypes);
        // Add the implemented service type ServiceDescriptor
        serviceCollection.Add(new ServiceDescriptor(implementationType, implementationInstance.GetType(), ServiceLifetime.Singleton));
        return serviceCollection;
    }

    [Experimental("AddImplementation02", Message = "Untested")]
    public static IServiceCollection AddImplementation<TImplementation>(
        this IServiceCollection serviceCollection,
        ServiceLifetime serviceLifetime,
        Func<IServiceProvider, TImplementation> implementationFactory,
        params Type[]? inheritedTypes)
    {
        ArgumentNullException.ThrowIfNull(implementationFactory);
        var implementationType = typeof(TImplementation);
        // Add a ServiceDescriptor for each inherited service type
        var implementationLifetime = serviceCollection.AddInheritedTypes(null, serviceLifetime, implementationType, inheritedTypes);
        // Add the implemented service type ServiceDescriptor
        serviceCollection.Add(new ServiceDescriptor(implementationType, implementationFactory, implementationLifetime));
        return serviceCollection;
    }

    public static IServiceCollection Add<TImplementation>(
        this IServiceCollection serviceCollection,
        TImplementation implementationInstance,
        params Type[]? inheritedTypes)
        where TImplementation : class =>
        serviceCollection.AddKeyed(null, implementationInstance, inheritedTypes);

    public static IServiceCollection Add<TService, TImplementation>(
        this IServiceCollection services,
        TImplementation implementationInstance)
        where TService : class
        where TImplementation : class, TService =>
        services.Add(implementationInstance, typeof(TService));

    public static IServiceCollection Add<T1, T2, TImplementation>(
        this IServiceCollection services,
        TImplementation implementationInstance)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add(implementationInstance, typeof(T1), typeof(T2));

    public static IServiceCollection Add<T1, T2, T3, TImplementation>(
        this IServiceCollection services,
        TImplementation implementationInstance)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add(implementationInstance, typeof(T1), typeof(T2), typeof(T3));

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

    public static IServiceCollection Add<TImplementation>(
        this IServiceCollection serviceCollection,
        ServiceLifetime serviceLifetime,
        Func<IServiceProvider, TImplementation> implementationFactory,
        params Type[]? inheritedTypes)
        where TImplementation : class =>
        serviceCollection.AddKeyed(null, serviceLifetime, implementationFactory, inheritedTypes);

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
}
