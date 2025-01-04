namespace DependencyInjectionServiceCollectionExtensions;

using Microsoft.Extensions.DependencyInjection;

public static class AddTypeServiceCollectionExtensions
{
    public static IServiceCollection AddAllOfType(
        this IServiceCollection serviceCollection,
        Type type,
        IEnumerable<Type> assemblyTypes,
        ServiceLifetime lifetime)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(assemblyTypes);
        foreach (var assemblyType in assemblyTypes)
        {
            if (!assemblyType.IsClass || assemblyType.IsAbstract)
            {
                continue;
            }
            foreach (var serviceType in assemblyType.GetInterfaces())
            {
                if (serviceType != type &&
                    (!serviceType.IsGenericType ||
                    serviceType.GetGenericTypeDefinition() != type))
                {
                    continue;
                }
                serviceCollection.Add(new ServiceDescriptor(serviceType, assemblyType, lifetime));
            }
        }
        return serviceCollection;
    }

    public static IServiceCollection AddAllOfType<T>(
        this IServiceCollection serviceCollection,
        ServiceLifetime lifetime)
    {
        var type = typeof(T);
        var assemblyTypes = type.Assembly.GetTypes();
        return serviceCollection.AddAllOfType(type, assemblyTypes, lifetime);
    }
}
