namespace DependencyInjectionServiceCollectionExtensionsTests;

using System;
using DependencyInjectionServiceCollectionExtensions;
using Microsoft.Extensions.DependencyInjection;

public interface I1 { }
public interface I2 : I1 { }
public interface I3 { }

public class Concrete : I1, I2, I3 { }

public interface I4 { }
public interface I5 : I4 { }
public class Service(I2 i2) : I5 { }

public interface I6 { }
public interface I7 : I6 { }
public class Api(I5 i5) : I7 { }

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void Services_Should_Be_Same_Instance_Fails()
    {
        // Arrange
        var services = new ServiceCollection();

        // Register services
        services.AddSingleton<Concrete>();
        services.AddSingleton<I1, Concrete>();
        services.AddSingleton<I2, Concrete>();

        // Build the service provider
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var i1 = serviceProvider.GetRequiredService<I1>();
        var i2 = serviceProvider.GetRequiredService<I2>();

        // Assert
        Assert.NotSame(i1, i2); // We want these to be the same, but they're not
    }

    [Fact]
    public void Services_Should_Be_Same_Instance_When_Registered_With_Single_Concrete_Instance()
    {
        // Arrange
        var services = new ServiceCollection();
        var concrete = new Concrete();

        // Register the same instance for I1 and I2
        services.AddSingleton<I1>(concrete);
        services.AddSingleton<I2>(concrete);

        // Build the service provider
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var i1 = serviceProvider.GetService<I1>();
        var i2 = serviceProvider.GetService<I2>();

        // Assert
        Assert.Same(i1, i2);
    }

    [Fact]
    public void Services_Should_Be_Same_Instance_When_Registered_With_Singleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Register services
        services.AddSingleton<Concrete>();
        services.AddSingleton<I1>(sp => sp.GetRequiredService<Concrete>());
        services.AddSingleton<I2>(sp => sp.GetRequiredService<Concrete>());

        // Build the service provider
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var i1 = serviceProvider.GetRequiredService<I1>();
        var i2 = serviceProvider.GetRequiredService<I2>();

        // Assert
        Assert.Same(i1, i2);
    }

    [Theory]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void Services_Should_Be_Same_Instance_When_Registered_With_Custom_Factory(ServiceLifetime serviceLifetime)
    {
        // Arrange
        var services = new ServiceCollection();

        // Register services
        services.Add<I1, I2, I3, Concrete>(serviceLifetime, (_) => new Concrete());

        // Build the service provider
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var i1 = serviceProvider.GetRequiredService<I1>();
        var i2 = serviceProvider.GetRequiredService<I2>();
        var i3 = serviceProvider.GetRequiredService<I3>();

        // Assert
        Assert.Same(i1, i2);
        Assert.Same(i2, i3);
    }

    [Theory]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void Services_Should_Be_Same_Instance_When_Registered_With_Add(ServiceLifetime serviceLifetime)
    {
        // Arrange
        var services = new ServiceCollection();

        // Register services
        services.Add<I1, I2, I3, Concrete>(serviceLifetime);

        // Build the service provider
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var i1 = serviceProvider.GetRequiredService<I1>();
        var i2 = serviceProvider.GetRequiredService<I2>();
        var i3 = serviceProvider.GetRequiredService<I3>();

        // Assert
        Assert.Same(i1, i2);
        Assert.Same(i2, i3);
    }

    [Fact]
    public void Services_Should_Be_Same_Instance_When_Registered_With_AddTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Register services
        services.AddTransient<I1, I2, I3, Concrete>();
        services.AddTransient<I4, I5, Service>(provider => new Service(provider.GetRequiredService<I2>()));
        services.AddTransient<I6, I7, Api>(provider => new Api(provider.GetRequiredService<I5>()));

        // Build the service provider
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var i1 = serviceProvider.GetRequiredService<I1>();
        var i2 = serviceProvider.GetRequiredService<I2>();
        var i3 = serviceProvider.GetRequiredService<I3>();

        // Assert
        Assert.Same(i1, i2);
        Assert.Same(i2, i3);
    }

    [Fact]
    public void Services_Should_Be_Same_Instance_When_Registered_With_Add_Transient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Register services
        services.Add<Concrete>(ServiceLifetime.Transient, typeof(I1), typeof(I2), typeof(I3));
        services.Add<Service>(ServiceLifetime.Transient, provider => new Service(provider.GetRequiredService<I2>()), typeof(I4), typeof(I5));
        services.Add<Api>(ServiceLifetime.Transient, provider => new Api(provider.GetRequiredService<I5>()), typeof(I6), typeof(I7));

        // Build the service provider
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var i1 = serviceProvider.GetRequiredService<I1>();
        var i2 = serviceProvider.GetRequiredService<I2>();
        var i3 = serviceProvider.GetRequiredService<I3>();

        // Assert
        Assert.Same(i1, i2);
        Assert.Same(i2, i3);
    }

    [Fact]
    public void Services_Should_Be_Same_Instance_When_Registered_With_Key()
    {
        // Arrange
        var services = new ServiceCollection();
        var key = nameof(MemoryStream);
        services.AddSingleton<MemoryStream>();
        services.AddSingleton<IDisposable>(provider =>
            provider.GetRequiredService<MemoryStream>());
        services.AddKeyedSingleton(key, (provider, _) =>
            provider.GetRequiredService<MemoryStream>());
        services.AddKeyedSingleton(key, (provider, _) =>
            provider.GetRequiredService<IDisposable>());
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var test1 = serviceProvider.GetRequiredKeyedService<IDisposable>(key);
        var test2 = serviceProvider.GetRequiredKeyedService<MemoryStream>(key);

        // Assert
        Assert.Same(test1, test2);
    }

    [Theory]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void Services_Should_Be_Same_Instance_When_Registered_With_Add_Key(ServiceLifetime serviceLifetime)
    {
        // Arrange
        var services = new ServiceCollection();
        var key = nameof(MemoryStream);
        services.AddKeyed<MemoryStream>(key, serviceLifetime, typeof(IDisposable));
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var test1 = serviceProvider.GetRequiredKeyedService<IDisposable>(key);
        var test2 = serviceProvider.GetRequiredKeyedService<MemoryStream>(key);

        // Assert
        Assert.Same(test1, test2);
    }
}
