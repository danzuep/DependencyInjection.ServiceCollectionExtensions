namespace ServiceCollectionSimplified;

using DependencyInjectionServiceCollectionExtensions;
using Microsoft.Extensions.DependencyInjection;
using ServiceCollectionExtensions;

public static class DemoProgram
{
    public static void Main()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IService, ServiceImpl>();
        // the last registered decorator is the one that will be injected
        services.AddDecorator<IService, DecoratorA>(ServiceLifetime.Transient, (sp, next) => new DecoratorA(next));
        services.AddDecorator<IService, DecoratorB>(ServiceLifetime.Transient, (sp, next) => new DecoratorB(next, Guid.NewGuid()));

        services.Build(b =>
        {
            b.Add<ServiceImpl>().As<IService>().AddDecorator<IService, DecoratorA>();
        });

        var provider = services.BuildServiceProvider();

        var svc = provider.GetRequiredService<IService>();
        svc.Write("Hello stacked decorators");
    }
}
