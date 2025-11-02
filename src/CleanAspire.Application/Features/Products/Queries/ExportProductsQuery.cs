
using System.Globalization;
using CleanAspire.Application.Features.Products.DTOs;
using CsvHelper;

namespace CleanAspire.Application.Features.Products.Queries;
/// <summary>
/// Represents a query to export products based on specified keywords.
/// </summary>
/// <param name="Keywords">The keywords to filter products by SKU, Name, or Description.</param>
public record ExportProductsQuery(string Keywords) : IRequest<Stream>;


/// <summary>
/// Handles the export of products based on the provided query.
/// </summary>
public class ExportProductsQueryHandler : IRequestHandler<ExportProductsQuery, Stream>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportProductsQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public ExportProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the export products query and returns a CSV stream of the filtered products.
    /// </summary>
    /// <param name="request">The export products query request.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A stream containing the CSV data of the filtered products.</returns>
    public async ValueTask<Stream> Handle(ExportProductsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Products
            .Where(x => x.SKU.Contains(request.Keywords) || x.Name.Contains(request.Keywords) || x.Description.Contains(request.Keywords))
            .Select(t => new ProductDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Price = t.Price,
                SKU = t.SKU,
                UOM = t.UOM,
                Currency = t.Currency,
                Category = t.Category
            }).ToListAsync(cancellationToken);

        var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, leaveOpen: true))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(data);
            await writer.FlushAsync();
        }
        return stream;
    }
}
