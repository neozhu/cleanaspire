// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.OpenApi;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.OpenApi;

namespace CleanAspire.Api;

public sealed class DecimalAndIntegerSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var clrType = context.JsonTypeInfo?.Type;

        if (clrType == typeof(decimal) || clrType == typeof(decimal?))
        {
            schema.Type = JsonSchemaType.Number;
            schema.Format = "decimal";
        }
        else if (clrType == typeof(int) || clrType == typeof(int?))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Format = "int32";
        }
        else if (clrType == typeof(long) || clrType == typeof(long?))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Format = "int64";
        }

        return Task.CompletedTask;
    }
}
