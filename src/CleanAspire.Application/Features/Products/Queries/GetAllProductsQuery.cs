using CleanAspire.Application.Features.Products.DTOs;

namespace CleanAspire.Application.Features.Products.Queries;
/// <summary>
/// Query to fetch all products.
/// Implements IFusionCacheRequest to enable caching for performance optimization.
/// </summary>
public record GetAllProductsQuery() : IFusionCacheRequest<List<ProductDto>>
{
    /// <summary>
    /// Cache key for storing the results of this query.
    /// </summary>
    public string CacheKey => "getallproducts";

    /// <summary>
    /// Tags for cache invalidation, categorizing this query under "products".
    /// </summary>
    public IEnumerable<string>? Tags => new[] { "products" };
}

/// <summary>
/// Handler for the GetAllProductsQuery.
/// Fetches and returns a list of ProductDto objects from the database.
/// </summary>
public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// Constructor to initialize the database context.
    /// </summary>
    /// <param name="dbContext">Application database context.</param>
    public GetAllProductsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Handles the query by fetching all products, mapping them to ProductDto, and returning the result.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A list of ProductDto objects.</returns>
    public async ValueTask<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _dbContext.Products
            .OrderBy(x => x.Name) // Sort by product name
            .Select(t => new ProductDto // Map to ProductDto
            {
                Id = t.Id,
                SKU = t.SKU,
                Name = t.Name,
                Category = t.Category, // Cast to ProductCategoryDto
                Description = t.Description,
                Price = t.Price,
                Currency = t.Currency,
                UOM = t.UOM
            })
            .ToListAsync(cancellationToken); // Execute query and return list

        return products;
    }
}
