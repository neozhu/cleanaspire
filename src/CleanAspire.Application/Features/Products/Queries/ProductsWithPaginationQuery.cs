using CleanAspire.Application.Features.Products.DTOs;

namespace CleanAspire.Application.Features.Products.Queries;
/// <summary>
/// Query to fetch products with pagination, filtering, and sorting options.
/// Implements IFusionCacheRequest to enable caching for performance optimization.
/// </summary>
public record ProductsWithPaginationQuery(
    string Keywords,
    int PageNumber = 0,
    int PageSize = 15,
    string OrderBy = "Id",
    string SortDirection = "Descending"
) : IFusionCacheRequest<PaginatedResult<ProductDto>>
{
    /// <summary>
    /// Tags for cache invalidation, categorizing this query under "products".
    /// </summary>
    public IEnumerable<string>? Tags => new[] { "products" };

    /// <summary>
    /// Cache key for storing the result of this query, unique to its parameters.
    /// </summary>
    public string CacheKey => $"productswithpagination_{Keywords}_{PageNumber}_{PageSize}_{OrderBy}_{SortDirection}";
}

/// <summary>
/// Handler for the ProductsWithPaginationQuery.
/// Retrieves paginated, filtered, and sorted product data from the database.
/// </summary>
public class ProductsWithPaginationQueryHandler : IRequestHandler<ProductsWithPaginationQuery, PaginatedResult<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Constructor to initialize the database context.
    /// </summary>
    /// <param name="context">Application database context.</param>
    public ProductsWithPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the query by retrieving paginated product data, applying filtering, sorting, and mapping to ProductDto.
    /// </summary>
    /// <param name="request">The query request containing pagination, filtering, and sorting parameters.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A paginated result of ProductDto objects.</returns>
    public async ValueTask<PaginatedResult<ProductDto>> Handle(ProductsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        // Retrieves and paginates data, applying filters and mapping to ProductDto
        var data = await _context.Products
                    .OrderBy(request.OrderBy, request.SortDirection) // Dynamic ordering
                    .ProjectToPaginatedDataAsync(
                        condition: x => x.SKU.Contains(request.Keywords)
                                     || x.Name.Contains(request.Keywords)
                                     || x.Description.Contains(request.Keywords), // Filter by keywords
                        pageNumber: request.PageNumber,
                        pageSize: request.PageSize,
                        mapperFunc: t => new ProductDto // Map to ProductDto
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Description = t.Description,
                            Price = t.Price,
                            SKU = t.SKU,
                            UOM = t.UOM,
                            Currency = t.Currency,
                            Category = t.Category
                        },
                    cancellationToken: cancellationToken);

        return data;
    }
}
