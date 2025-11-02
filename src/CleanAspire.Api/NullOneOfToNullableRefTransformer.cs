// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;


namespace CleanAspire.Api;

/// <summary>
/// OpenAPI schema transformer that converts oneOf patterns with null and $ref
/// into a simpler nullable $ref pattern for .NET 10 / OpenAPI.NET v2.
/// </summary>
/// <remarks>
/// Transforms schemas like:
/// <code>
/// "category": {
///   "oneOf": [
///     { "type": "null" },
///     { "$ref": "#/components/schemas/ProductCategory" }
///   ]
/// }
/// </code>
/// Into a simpler pattern by removing unnecessary oneOf wrappers.
/// Note: In .NET 10 with OpenAPI.NET v2, nullable handling has changed.
/// This transformer is kept for compatibility but may not be necessary
/// as the framework handles nullable references differently now.
/// </remarks>
public sealed class NullOneOfToNullableRefTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        // In .NET 10 with OpenAPI.NET v2 / JSON Schema draft 2020-12,
        // the handling of nullable types has changed. The oneOf pattern with null
        // is now the standard way to represent nullable references.
        // This transformer is kept for potential future customization but currently
        // does not modify the schema as the default behavior is already appropriate.
        
        return Task.CompletedTask;
    }
}
