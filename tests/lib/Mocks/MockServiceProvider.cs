using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shipstone.Test.Mocks;

public sealed class MockServiceProvider : IServiceProvider
{
    private readonly IDictionary<Type, ICollection<Object>> _instances;
    private readonly Object _locker;
    private readonly IServiceCollection _services;

    public MockServiceProvider(IServiceCollection services)
    {
        this._instances = new Dictionary<Type, ICollection<Object>>();
        this._locker = new();
        this._services = services;
    }

    private Object Add(Type serviceType, Object instance)
    {
        if (!this._instances.TryGetValue(
            serviceType,
            out ICollection<Object>? instances
        ))
        {
            instances = new List<Object>();
            this._instances.Add(serviceType, instances);
        }

        instances.Add(instance);
        return instance;
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

    private Object? GetService(ServiceDescriptor descriptor)
    {
        Object? instance = descriptor.ImplementationInstance;

        if (instance is not null)
        {
            return instance;
        }

        Func<IServiceProvider, Object>? factory =
            descriptor.ImplementationFactory;

        Type serviceType = descriptor.ServiceType;

        if (factory is not null)
        {
            instance = factory(this);
            return this.Add(serviceType, instance);
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
                return this.Add(serviceType, instance);
            }
        }

        return null;
    }

    private Object? GetService(Type serviceType)
    {
        lock (this._locker)
        {
            if (serviceType.Equals(typeof (IServiceProvider)))
            {
                return this;
            }

            if (this._instances.TryGetValue(
                serviceType,
                out ICollection<Object>? instances
            ))
            {
                return instances.First();
            }

            if (this.TryGetEnumerable(serviceType, out Object? instance))
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

            return this.GetService(descriptor);
        }
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

        if (typeArgument.IsAssignableTo(serviceType))
        {
            return instance;
        }

        Type closedType = typeof (MockOptions<>).MakeGenericType(typeArgument);

        Type[] constructorTypes =
            new Type[1] { closedType.GenericTypeArguments[0] };

        ConstructorInfo? constructor =
            closedType.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                constructorTypes
            );

        return constructor?.Invoke(arguments);
    }

    private bool TryGetEnumerable(Type serviceType, out Object? instance)
    {
        if (!serviceType.IsGenericType)
        {
            instance = null;
            return false;
        }

        Type genericType = serviceType.GetGenericTypeDefinition();

        if (!genericType.Equals(typeof (IEnumerable<>)))
        {
            instance = null;
            return false;
        }

        IReadOnlyList<Type> typeArguments = serviceType.GetGenericArguments();
        Type typeArgument = typeArguments[0];

        if (!this._instances.TryGetValue(
            typeArgument,
            out ICollection<Object>? services
        ))
        {
            IEnumerable<ServiceDescriptor> descriptors =
                this._services.Where(s => s.ServiceType.Equals(typeArgument));

            foreach (ServiceDescriptor descriptor in descriptors)
            {
                this.GetService(descriptor);
            }
        }

        Type listType = typeof (List<>).MakeGenericType(typeArgument);
        instance = Activator.CreateInstance(listType)!;

        if (this._instances.TryGetValue(typeArgument, out services))
        {
            MethodInfo addMethod =
                listType.GetMethod(nameof (List<Object>.Add))!;

            Object?[]? arguments = new Object?[1] { null };

            foreach (Object service in services)
            {
                arguments[0] = service;
                addMethod.Invoke(instance, arguments);
            }
        }

        return true;
    }

    Object? IServiceProvider.GetService(Type serviceType) =>
        this.GetService(serviceType);
}
