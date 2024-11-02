namespace DependencyInjectionServiceCollectionExtensionsTests;

using DependencyInjectionServiceCollectionExtensions;
using Microsoft.Extensions.DependencyInjection;

public interface I1 { }
public interface I2 { }
public interface I3 { }

public class Concrete : I1, I2, I3 { }

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
        var serviceProvider = services.BuildServiceProvider();

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
        var serviceProvider = services.BuildServiceProvider();

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
        var serviceProvider = services.BuildServiceProvider();

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
        services.Add<I1, I2, I3, Concrete>((_) => new Concrete(), serviceLifetime);

        // Build the service provider
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var i1 = serviceProvider.GetRequiredService<I1>();
        var i2 = serviceProvider.GetRequiredService<I2>();
        var i3 = serviceProvider.GetRequiredService<I3>();

        // Assert
        Assert.Same(i1, i2);
        Assert.Same(i1, i3);
    }
}
