// Summary:
// This file defines a command and its handler for updating product details in the database. 
// The UpdateProductCommand encapsulates the necessary data to update a product, while the 
// UpdateProductCommandHandler validates the product's existence, updates its details, 
// triggers a domain event such as ProductUpdatedEvent, and commits the changes.

using CleanAspire.Application.Features.Products.DTOs;
using CleanAspire.Application.Features.Products.EventHandlers;
using CleanAspire.Application.Pipeline;

namespace CleanAspire.Application.Features.Products.Commands;

// Command object that encapsulates the data required to update a product. 
// Each field corresponds to a property in ProductDto.
public record UpdateProductCommand(
    string Id,
    string SKU,
    string Name,
    ProductCategory? Category,
    string? Description,
    decimal Price,
    string? Currency,
    string? UOM
) : IFusionCacheRefreshRequest<Unit>,
    IRequiresValidation
{
    public IEnumerable<string>? Tags => new[] { "products" };
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with Id '{request.Id}' was not found.");
        }

        product.SKU = request.SKU;
        product.Name = request.Name;
        product.Category = request.Category?? product.Category;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Currency = request.Currency;
        product.UOM = request.UOM;

        product.AddDomainEvent(new ProductUpdatedEvent(product));
        _context.Products.Update(product);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
