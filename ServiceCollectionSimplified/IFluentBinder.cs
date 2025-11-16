namespace ServiceCollectionSimplified;

using Microsoft.Extensions.DependencyInjection;

public interface IFluentBinder
{
    public IDecorateSyntax<TService> Decorate<TService>();
}

public interface IDecorateSyntax<TService>
{
    public IDecorateSyntax<TService> AddWithLifetime<TDecorator>(ServiceLifetime lifetime)
        where TDecorator : class, TService;
}
