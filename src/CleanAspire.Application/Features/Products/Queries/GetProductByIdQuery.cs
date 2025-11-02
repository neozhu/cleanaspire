using CleanAspire.Application.Features.Products.DTOs;

namespace CleanAspire.Application.Features.Products.Queries;
/// <summary>
/// Query to fetch a product by its ID.
/// Implements IFusionCacheRequest to enable caching.
/// </summary>
public record GetProductByIdQuery(string Id) : IFusionCacheRequest<ProductDto?>
{
    /// <summary>
    /// Cache key for storing the result of this query, specific to the product ID.
    /// </summary>
    public string CacheKey => $"product_{Id}";

    /// <summary>
    /// Tags for cache invalidation, categorizing this query under "products".
    /// </summary>
    public IEnumerable<string>? Tags => new[] { "products" };
}

/// <summary>
/// Handler for the GetProductByIdQuery.
/// Fetches a single ProductDto by its ID from the database.
/// </summary>
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// Constructor to initialize the database context.
    /// </summary>
    /// <param name="dbContext">Application database context.</param>
    public GetProductByIdQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Handles the query by fetching the product with the specified ID, mapping it to ProductDto, and returning the result.
    /// </summary>
    /// <param name="request">The query request containing the product ID.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>The ProductDto if found, or null otherwise.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the product with the specified ID is not found.</exception>
    public async ValueTask<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
                            .Where(p => p.Id == request.Id) // Filter by product ID
                            .Select(p => new ProductDto // Map to ProductDto
                            {
                                Id = p.Id,
                                SKU = p.SKU,
                                Name = p.Name,
                                Category = p.Category, // Cast to ProductCategoryDto
                                Description = p.Description,
                                Price = p.Price,
                                Currency = p.Currency,
                                UOM = p.UOM
                            })
                            .SingleOrDefaultAsync(cancellationToken); // Get single result or default to null

        if (product == null)
        {
            throw new KeyNotFoundException($"Product with Id '{request.Id}' was not found."); // Handle not found case
        }

        return product;
    }
}
