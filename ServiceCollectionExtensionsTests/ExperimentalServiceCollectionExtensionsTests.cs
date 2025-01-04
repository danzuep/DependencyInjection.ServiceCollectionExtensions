namespace DependencyInjectionServiceCollectionExtensionsTests;

using System;
using System.Globalization;
using DependencyInjectionServiceCollectionExtensions;
using Microsoft.Extensions.DependencyInjection;

public class ExperimentalServiceCollectionExtensionsTests
{
    [Fact]
    public void AddWithInterfaces()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddWithInterfaces<MemoryStream>(ServiceLifetime.Singleton);
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var test1 = serviceProvider.GetRequiredService<IDisposable>();

        // Assert
        Assert.NotNull(test1);
    }

    [Fact]
    public void AddNamed_DynamicallyAccessedConstructor()
    {
        // Arrange
        var services = new ServiceCollection();
        var key = nameof(MemoryStream);
        _ = services.AddNamed<IDisposable, MemoryStream>(null, ServiceLifetime.Singleton);
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var test1 = serviceProvider.GetRequiredService<MemoryStream>();
        var test2 = serviceProvider.GetRequiredKeyedService<IDisposable>(key);

        // Assert
        Assert.NotNull(test1);
        Assert.Same(test1, test2);
    }

    [Fact]
    public void AddNamed_Instance()
    {
        // Arrange
        var services = new ServiceCollection();
        var key = nameof(MemoryStream);
        _ = services.AddNamed<IDisposable, MemoryStream>(null, new MemoryStream());
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var test1 = serviceProvider.GetRequiredService<MemoryStream>();
        var test2 = serviceProvider.GetRequiredKeyedService<IDisposable>(key);

        // Assert
        Assert.NotNull(test1);
        Assert.Same(test1, test2);
    }

    [Fact]
    public void AddNamed_Factory()
    {
        // Arrange
        var services = new ServiceCollection();
        var key = nameof(MemoryStream);
        _ = services.AddNamed<IDisposable, MemoryStream>(null, (_) => new MemoryStream(), ServiceLifetime.Singleton);
        _ = services.AddNamed<IDisposable, MemoryStream>(null, (_, _) => new MemoryStream(), ServiceLifetime.Singleton);
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var test1 = serviceProvider.GetRequiredService<MemoryStream>();
        var test2 = serviceProvider.GetRequiredKeyedService<IDisposable>(key);

        // Assert
        Assert.NotNull(test1);
        Assert.Same(test1, test2);
    }

    [Fact]
    public void AddAllOfType_WithAbstract_AddsConcrete()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddAllOfType<I1>(ServiceLifetime.Singleton);
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var result = serviceProvider.GetServices<I1>().ToArray();

        // Assert
        Assert.Single(result);
        Assert.Equal(typeof(Concrete), result[0].GetType());
    }

    protected static T FunctionMaxInputs<T>(Func<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, T> test)
    {
        // Func has overloads for up to 16 inputs, how many overloads should `services.Add` have?
        var args = Enumerable.Range(1, 16).Select(static o => o.ToString(CultureInfo.InvariantCulture)).ToArray();
        return test(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14], args[15]);
    }
}
