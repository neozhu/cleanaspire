// Summary:
// This file defines a command and its handler for importing products from a CSV file.
// The ImportProductsCommand encapsulates the input stream, while the ImportProductsCommandHandler 
// reads the CSV data, maps it to product entities, triggers domain events like ProductCreatedEvent, 
// and commits the changes to the database.

using System.Globalization;
using CleanAspire.Application.Features.Products.DTOs;
using CleanAspire.Application.Features.Products.EventHandlers;
using CleanAspire.Application.Pipeline;
using CsvHelper;

namespace CleanAspire.Application.Features.Products.Commands;

// Command object that encapsulates the input stream containing CSV data.
public record ImportProductsCommand(Stream Stream)
    : IFusionCacheRefreshRequest<Unit>,
      IRequiresValidation
{
    public IEnumerable<string>? Tags => new[] { "products" };
}

public class ImportProductsCommandHandler : IRequestHandler<ImportProductsCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public ImportProductsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(ImportProductsCommand request, CancellationToken cancellationToken)
    {
        request.Stream.Position = 0;

        using (var reader = new StreamReader(request.Stream))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<ProductDto>();

            foreach (var product in records.Select(x => new Product
            {
                SKU = x.SKU,
                Name = x.Name,
                Category = x.Category,
                Description = x.Description,
                Price = x.Price,
                Currency = x.Currency,
                UOM = x.UOM
            }))
            {
                product.AddDomainEvent(new ProductCreatedEvent(product));
                _context.Products.Add(product);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
