// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanAspire.Application.Common.Interfaces;
using CleanAspire.Application.Features.Stocks.DTOs;
using CleanAspire.Application.Pipeline;

namespace CleanAspire.Application.Features.Stocks.Commands;
public record StockReceivingCommand : IFusionCacheRefreshRequest<Unit>, IRequiresValidation
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty;
    public IEnumerable<string>? Tags => new[] { "stocks" };
}
public class StockReceivingCommandHandler : IRequestHandler<StockReceivingCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public StockReceivingCommandHandler(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async ValueTask<Unit> Handle(StockReceivingCommand request, CancellationToken cancellationToken)
    {
        // Validate that the product exists
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product == null)
        {
            throw new KeyNotFoundException($"Product with Product ID '{request.ProductId}' was not found.");
        }

        // Check if the stock record already exists for the given ProductId and Location
        var existingStock = await _context.Stocks
            .FirstOrDefaultAsync(s => s.ProductId == request.ProductId && s.Location == request.Location, cancellationToken);

        if (existingStock != null)
        {
            // If the stock record exists, update the quantity
            existingStock.Quantity += request.Quantity;
            _context.Stocks.Update(existingStock);
        }
        else
        {
            // If no stock record exists, create a new one
            var newStockEntry = new Stock
            {
                ProductId = request.ProductId,
                Location = request.Location,
                Quantity = request.Quantity,
            };

            _context.Stocks.Add(newStockEntry);
        }

        // Save changes to the database
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
