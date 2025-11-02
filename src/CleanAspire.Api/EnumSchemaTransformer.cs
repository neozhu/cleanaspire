// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Text.Json.Nodes;

namespace CleanAspire.Api;

/// <summary>
/// OpenAPI schema transformer for enum types.
/// Ensures enums are properly represented as strings with allowed values in the OpenAPI schema.
/// For nullable enums, creates a reference to the enum schema that can be made nullable.
/// </summary>
public sealed class EnumSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext ctx, CancellationToken cancellationToken)
    {
        var clrType = ctx.JsonTypeInfo?.Type;

        if (clrType is null)
        {
            return Task.CompletedTask;
        }

        // Check if the type is an enum or nullable enum
        var underlyingType = Nullable.GetUnderlyingType(clrType);
        var isNullableEnum = underlyingType?.IsEnum == true;
        var isEnum = clrType.IsEnum || isNullableEnum;

        if (!isEnum)
        {
            return Task.CompletedTask;
        }

        var enumType = isNullableEnum ? underlyingType! : clrType;

        // For non-nullable enums, set the schema directly
        if (!isNullableEnum)
        {
            // Set the schema to represent a string enum
            schema.Type = JsonSchemaType.String;
            schema.Format = null;

            // Add enum values as JsonNode strings
            schema.Enum = Enum.GetNames(enumType)
                .Select(n => (JsonNode)JsonValue.Create(n)!)
                .ToList();
        }
        else
        {
            // For nullable enums, create a oneOf pattern with null and the enum reference
            // This allows the NullOneOfToNullableRefTransformer to convert it to nullable reference
            
            // Clear any existing properties
            schema.Type = null;
            schema.Enum = null;
            schema.Format = null;

            // Create the enum schema
            var enumSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Enum = Enum.GetNames(enumType)
                    .Select(n => (JsonNode)JsonValue.Create(n)!)
                    .ToList()
            };

            // Create the null schema
            var nullSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Null
            };

            // Set up oneOf with null and enum - cast to IList<IOpenApiSchema> for OpenAPI.NET v2
            schema.OneOf = new List<IOpenApiSchema>
            {
                nullSchema,
                enumSchema
            };
        }

        return Task.CompletedTask;
    }
}
