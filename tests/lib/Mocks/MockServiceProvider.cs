using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shipstone.Test.Mocks;

public sealed class MockServiceProvider : IServiceProvider
{
    private readonly IDictionary<Type, Object> _instances;
    private readonly Object _locker;
    private readonly IServiceCollection _services;

    public MockServiceProvider(IServiceCollection services)
    {
        this._instances = new Dictionary<Type, Object>();
        this._locker = new();
        this._services = services;
    }

    private Object? GetService(ConstructorInfo constructor)
    {
        IReadOnlyList<ParameterInfo> parameters = constructor.GetParameters();
        int count = parameters.Count;
        Object?[] arguments = new Object?[count];

        for (int i = 0; i < count; i ++)
        {
            Object? arg = this.GetService(parameters[i].ParameterType);

            if (arg is null)
            {
                return null;
            }

            arguments[i] = arg;
        }

        return constructor.Invoke(arguments);
    }

    private Object? GetService(Type serviceType)
    {
        lock (this._locker)
        {
            if (serviceType.Equals(typeof (IServiceProvider)))
            {
                return this;
            }

            if (this._instances.TryGetValue(serviceType, out Object? instance))
            {
                return instance;
            }

            ServiceDescriptor? descriptor =
                this._services.FirstOrDefault(s =>
                    s.ServiceType.Equals(serviceType));

            if (descriptor is null)
            {
                return this.GetServiceOptions(serviceType);
            }

            instance = descriptor.ImplementationInstance;

            if (instance is not null)
            {
                return instance;
            }

            Func<IServiceProvider, Object>? factory =
                descriptor.ImplementationFactory;

            if (factory is not null)
            {
                instance = factory(this);
                this._instances.Add(serviceType, instance);
                return instance;
            }

            Type? type = descriptor.ImplementationType;

            if (type is null)
            {
                return null;
            }

            IEnumerable<ConstructorInfo> constructors =
                type
                    .GetConstructors()
                    .OrderByDescending(p =>
                    {
                        IReadOnlyCollection<ParameterInfo> parameters =
                            p.GetParameters();

                        return parameters.Count;
                    });

            foreach (ConstructorInfo constructor in constructors)
            {
                instance = this.GetService(constructor);

                if (instance is not null)
                {
                    this._instances.Add(serviceType, instance);
                    return instance;
                }
            }
        }

        return null;
    }

    private Object? GetServiceOptions(Type serviceType)
    {
        Type optionsType = typeof (IOptions<>);
        Type? typeArgument;

        if (serviceType.IsGenericType)
        {
            Type genericType = serviceType.GetGenericTypeDefinition();

            if (genericType.Equals(optionsType))
            {
                typeArgument =
                    serviceType
                        .GetGenericArguments()
                        .First();
            }

            else
            {
                return null;
            }
        }

        else
        {
            typeArgument =
                serviceType
                    .GetInterfaces()
                    .FirstOrDefault(i =>
                    {
                        if (!i.IsGenericType)
                        {
                            return false;
                        }

                        return i
                            .GetGenericTypeDefinition()
                            .Equals(optionsType);
                    });
        }

        if (typeArgument is null)
        {
            return null;
        }

        Type configureOptionsType =
            typeof (IConfigureOptions<>).MakeGenericType(typeArgument);

        MethodInfo? configureMethod =
            configureOptionsType.GetMethod(nameof (IConfigureOptions<Object>.Configure));

        Object? configureInstance =
            this.GetService(configureOptionsType);

        Object? instance = Activator.CreateInstance(typeArgument);

        if (
            configureMethod is null
            || configureInstance is null
            || instance is null
        )
        {
            return null;
        }

        Object?[]? arguments = new Object?[1] { instance };
        configureMethod.Invoke(configureInstance, arguments);
        return instance;
    }

    Object? IServiceProvider.GetService(Type serviceType) =>
        this.GetService(serviceType);
}
