namespace DependencyInjectionServiceCollectionExtensionsTests;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using DependencyInjectionServiceCollectionExtensions;
using Microsoft.Extensions.DependencyInjection;

public class DecoratorA : I3
{
    public DecoratorA() => Console.WriteLine($"[{nameof(DecoratorA)}]");
}

public class DecoratorB : I3
{
    public DecoratorB() => Console.WriteLine($"[{nameof(DecoratorB)}]");
}

public class ExperimentalServiceCollectionExtensionsTests
{
    [Experimental("AddImplementation01", Message = "Untested")]
    [Fact(Skip = "Fails")]
    public void Services_Should_Be_Valid_When_Registered_With_AddImplementation()
    {
        // Arrange
        var services = new ServiceCollection();
        var guid = Guid.NewGuid();
        services.AddImplementation(guid, typeof(IComparable), typeof(IFormattable), typeof(ISpanFormattable), typeof(IUtf8SpanFormattable));
        using var serviceProvider = services.BuildServiceProvider();
        // Act
        var result = serviceProvider.GetService<Guid>();
        // Assert
        Assert.Equal(typeof(Guid), result.GetType());
    }

    [Experimental("AddImplementation02", Message = "Untested")]
    [Fact(Skip = "Fails")]
    public void Services_Should_Be_Valid_When_Registered_With_AddImplementation_Factory()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddImplementation(ServiceLifetime.Singleton, _ => Guid.NewGuid(), typeof(IComparable), typeof(IFormattable), typeof(ISpanFormattable), typeof(IUtf8SpanFormattable));
        using var serviceProvider = services.BuildServiceProvider();
        // Act
        var result = serviceProvider.GetService<Guid>();
        // Assert
        Assert.Equal(typeof(Guid), result.GetType());
    }

    [Experimental("AddDecorator01", Message = "Under test")]
    [Fact]
    public void AddDecorator()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddSingleton<Concrete>();
        services.AddSingleton<I1>(sp => sp.GetRequiredService<Concrete>());
        services.AddSingleton<I2>(sp => sp.GetRequiredService<Concrete>());
        services.AddSingleton<I3>(sp => sp.GetRequiredService<Concrete>());
        // the last registered decorator is the one that will be injected
        services.AddDecorator<I3, DecoratorA>(ServiceLifetime.Transient);
        services.AddDecorator<I3, DecoratorB>(ServiceLifetime.Transient);

        using var serviceProvider = services.BuildServiceProvider();
        // Act
        var result = serviceProvider.GetService<I3>();
        // Assert
        Assert.NotNull(result);
        Assert.Equal(typeof(DecoratorB), result.GetType());
    }

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
