namespace ServiceCollectionSimplifiedTests;

using DependencyInjectionServiceCollectionExtensions;
using Microsoft.Extensions.DependencyInjection;

public interface I { }

public class C : I { }

public class FluentBinderTests
{
    [Fact]
    public void Test()
    {
        // Arrange
        var services = new ServiceCollection();

        // Register services
        services.WrapFactoryWithDecorator<I, C>();

        // Build the service provider
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var i = serviceProvider.GetRequiredService<I>();

        // Assert
        Assert.NotNull(i);
    }
}
