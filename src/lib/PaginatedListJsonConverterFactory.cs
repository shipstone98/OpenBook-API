using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

using Shipstone.Utilities.Collections;

namespace Shipstone.Utilities.Text.Json;

public class PaginatedListJsonConverterFactory : JsonConverterFactory
{
    public sealed override bool CanConvert(Type typeToConvert)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        return PaginatedListJsonConverterFactory.GetInterfaceType(typeToConvert) is not null;
    }

    public sealed override JsonConverter? CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        Type? interfaceType =
            PaginatedListJsonConverterFactory.GetInterfaceType(typeToConvert);

        if (interfaceType is null)
        {
            return null;
        }

        Type[] types = Array.Empty<Type>();

        ConstructorInfo? constructor =
            typeof (PaginatedListJsonConverter<>)
                .MakeGenericType(interfaceType.GenericTypeArguments[0])
                .GetConstructor(types);

        return constructor?.Invoke(null) as JsonConverter;
    }

    private static Type? GetInterfaceType(Type typeToConvert)
    {
        if (PaginatedListJsonConverterFactory.IsInstanceOfType(typeToConvert))
        {
            return typeToConvert;
        }

        Type[] interfaces = typeToConvert.GetInterfaces();

        return Array.Find(
            interfaces,
            PaginatedListJsonConverterFactory.IsInstanceOfType
        );
    }

    private static bool IsInstanceOfType(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }

        return typeToConvert
            .GetGenericTypeDefinition()
            .Equals(typeof (IReadOnlyPaginatedList<>));
    }
}
