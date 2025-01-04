namespace DependencyInjectionServiceCollectionExtensions;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingleton<T1, T2, TImplementation>(
        this IServiceCollection services,
        TImplementation implementationInstance)
        where T1 : class
        where T2 : class
        where TImplementation : class, T1, T2 =>
        services.Add(implementationInstance, typeof(T1), typeof(T2));

    public static IServiceCollection AddSingleton<T1, T2, T3, TImplementation>(
        this IServiceCollection services,
        TImplementation implementationInstance)
        where T1 : class
        where T2 : class
        where T3 : class
        where TImplementation : class, T1, T2, T3 =>
        services.Add(implementationInstance, typeof(T1), typeof(T2), typeof(T3));

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
