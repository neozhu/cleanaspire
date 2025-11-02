// Summary:
// This file defines a command and its handler for creating a new product in the database. 
// The CreateProductCommand encapsulates the necessary data for a product, while the 
// CreateProductCommandHandler processes the command, creates a product entity, 
// triggers domain events such as ProductCreatedEvent, and commits the changes. This ensures 
// a structured and efficient approach to handling product creation.

using CleanAspire.Application.Features.Products.DTOs;
using CleanAspire.Application.Features.Products.EventHandlers;
using CleanAspire.Application.Pipeline;

namespace CleanAspire.Application.Features.Products.Commands;

// Command object that encapsulates the data required for creating a new product. 
// Its fields directly map to the properties of ProductDto.
public record CreateProductCommand(
    string SKU,
    string Name,
    ProductCategory? Category,
    string? Description,
    decimal Price,
    string? Currency,
    string? UOM
) : IFusionCacheRefreshRequest<ProductDto>,
    IRequiresValidation
{
    public IEnumerable<string>? Tags => new[] { "products" };
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            SKU = request.SKU,
            Name = request.Name,
            Category = request.Category?? ProductCategory.Electronics,
            Description = request.Description,
            Price = request.Price,
            Currency = request.Currency,
            UOM = request.UOM
        };

        product.AddDomainEvent(new ProductCreatedEvent(product));
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return new ProductDto() { Id = product.Id, Name = product.Name, SKU = product.SKU };
    }
}
