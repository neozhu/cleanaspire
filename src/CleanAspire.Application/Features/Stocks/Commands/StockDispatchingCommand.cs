// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanAspire.Application.Pipeline;

namespace CleanAspire.Application.Features.Stocks.Commands;
public record StockDispatchingCommand : IFusionCacheRefreshRequest<Unit>, IRequiresValidation
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty;
    public IEnumerable<string>? Tags => new[] { "stocks" };
}
public class StockDispatchingCommandHandler : IRequestHandler<StockDispatchingCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public StockDispatchingCommandHandler(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async ValueTask<Unit> Handle(StockDispatchingCommand request, CancellationToken cancellationToken)
    {
        // Validate that the product exists
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product == null)
        {
            throw new KeyNotFoundException($"Product with Product ID '{request.ProductId}' was not found.");
        }

        // Check if the stock record exists for the given ProductId and Location
        var existingStock = await _context.Stocks
            .FirstOrDefaultAsync(s => s.ProductId == request.ProductId && s.Location == request.Location, cancellationToken);

        if (existingStock == null)
        {
            throw new KeyNotFoundException($"No stock record found for Product ID '{request.ProductId}' at Location '{request.Location}'.");
        }

        // Validate that the stock quantity is sufficient
        if (existingStock.Quantity < request.Quantity)
        {
            throw new InvalidOperationException($"Insufficient stock quantity. Available: {existingStock.Quantity}, Requested: {request.Quantity}");
        }

        // Reduce the stock quantity
        existingStock.Quantity -= request.Quantity;

        // If stock quantity is zero, remove the stock record
        if (existingStock.Quantity == 0)
        {
            _context.Stocks.Remove(existingStock);
        }
        else
        {
            // Update the stock record
            _context.Stocks.Update(existingStock);
        }

        // Save changes to the database
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

}
