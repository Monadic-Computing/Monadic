using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Monadic.Effect.Attributes;
using Monadic.Effect.Configuration.MonadicEffectBuilder;
using Monadic.Effect.Configuration.MonadicEffectConfiguration;
using Monadic.Effect.Services.EffectProviderFactory;
using Monadic.Effect.Services.EffectRunner;
using Monadic.Effect.Utils;

namespace Monadic.Effect.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddMonadicEffects(
        this IServiceCollection serviceCollection,
        Action<MonadicEffectConfigurationBuilder>? options = null
    )
    {
        var configuration = BuildConfiguration(serviceCollection, options);

        return serviceCollection
            .AddSingleton<IMonadicEffectConfiguration>(configuration)
            .AddTransient<IEffectRunner, EffectRunner>();
    }

    private static MonadicEffectConfiguration BuildConfiguration(
        IServiceCollection serviceCollection,
        Action<MonadicEffectConfigurationBuilder>? options
    )
    {
        // Create Builder to be used after Options are invoked
        var builder = new MonadicEffectConfigurationBuilder(serviceCollection);

        // Options able to be null since all values have defaults
        options?.Invoke(builder);

        return builder.Build();
    }

    public static MonadicEffectConfigurationBuilder AddEffect<TIEffectFactory, TEffectFactory>(
        this MonadicEffectConfigurationBuilder builder,
        TEffectFactory factory
    )
        where TIEffectFactory : class, IEffectProviderFactory
        where TEffectFactory : class, TIEffectFactory
    {
        builder
            .ServiceCollection.AddSingleton<TIEffectFactory>(factory)
            .AddSingleton<IEffectProviderFactory>(factory);

        return builder;
    }

    public static MonadicEffectConfigurationBuilder AddEffect<TIEffectFactory, TEffectFactory>(
        this MonadicEffectConfigurationBuilder builder
    )
        where TIEffectFactory : class, IEffectProviderFactory
        where TEffectFactory : class, TIEffectFactory
    {
        builder
            .ServiceCollection.AddSingleton<IEffectProviderFactory, TEffectFactory>()
            .AddSingleton<TIEffectFactory, TEffectFactory>();

        return builder;
    }

    public static MonadicEffectConfigurationBuilder AddEffect<TEffectFactory>(
        this MonadicEffectConfigurationBuilder builder,
        TEffectFactory factory
    )
        where TEffectFactory : class, IEffectProviderFactory
    {
        builder.ServiceCollection.AddSingleton<IEffectProviderFactory>(factory);

        return builder;
    }

    public static MonadicEffectConfigurationBuilder AddEffect<TEffectFactory>(
        this MonadicEffectConfigurationBuilder builder
    )
        where TEffectFactory : class, IEffectProviderFactory, new()
    {
        var factory = new TEffectFactory();

        return builder.AddEffect(factory);
    }

    public static void InjectProperties(this IServiceProvider serviceProvider, object instance)
    {
        var properties = instance
            .GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.IsDefined(typeof(InjectAttribute)) && p.CanWrite);

        foreach (var property in properties)
        {
            if (property.GetValue(instance) != null)
                continue;

            var propertyType = property.PropertyType;
            object? service = null;

            // Handle IEnumerable<T>
            if (
                propertyType.IsGenericType
                && propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
            )
            {
                var serviceType = propertyType.GetGenericArguments()[0];
                var serviceCollectionType = typeof(IEnumerable<>).MakeGenericType(serviceType);
                service = serviceProvider.GetService(serviceCollectionType);
            }
            else
            {
                service = serviceProvider.GetService(propertyType);
            }

            if (service != null)
            {
                property.SetValue(instance, service);
            }
        }
    }

    public static IServiceCollection AddScopedMonadicWorkflow<TService, TImplementation>(
        this IServiceCollection services
    )
        where TService : class
        where TImplementation : class, TService
    {
        services.AddScoped<TImplementation>();
        services.AddScoped<TService>(sp =>
        {
            var instance = sp.GetRequiredService<TImplementation>();
            sp.InjectProperties(instance);
            return instance;
        });

        return services;
    }

    public static IServiceCollection AddScopedMonadicWorkflow(
        this IServiceCollection services,
        Type serviceInterface,
        Type serviceImplementation
    )
    {
        services.AddScoped(serviceImplementation);
        services.AddScoped(
            serviceInterface,
            sp =>
            {
                var instance = sp.GetRequiredService(serviceImplementation);
                sp.InjectProperties(instance);
                return instance;
            }
        );

        return services;
    }

    public static IServiceCollection AddTransientMonadicWorkflow<TService, TImplementation>(
        this IServiceCollection services
    )
        where TService : class
        where TImplementation : class, TService
    {
        services.AddTransient<TImplementation>();
        services.AddTransient<TService>(sp =>
        {
            var instance = sp.GetRequiredService<TImplementation>();
            sp.InjectProperties(instance);
            return instance;
        });

        return services;
    }

    public static IServiceCollection AddTransientMonadicWorkflow(
        this IServiceCollection services,
        Type serviceInterface,
        Type serviceImplementation
    )
    {
        services.AddTransient(serviceImplementation);
        services.AddTransient(
            serviceInterface,
            sp =>
            {
                var instance = sp.GetRequiredService(serviceImplementation);
                sp.InjectProperties(instance);
                return instance;
            }
        );

        return services;
    }

    public static IServiceCollection AddSingletonMonadicWorkflow<TService, TImplementation>(
        this IServiceCollection services
    )
        where TService : class
        where TImplementation : class, TService
    {
        services.AddSingleton<TImplementation>();
        services.AddSingleton<TService>(sp =>
        {
            var instance = sp.GetRequiredService<TImplementation>();
            sp.InjectProperties(instance);
            return instance;
        });

        return services;
    }

    public static IServiceCollection AddSingletonMonadicWorkflow(
        this IServiceCollection services,
        Type serviceInterface,
        Type serviceImplementation
    )
    {
        services.AddSingleton(serviceImplementation);
        services.AddSingleton(
            serviceInterface,
            sp =>
            {
                var instance = sp.GetRequiredService(serviceImplementation);
                sp.InjectProperties(instance);
                return instance;
            }
        );

        return services;
    }
}
