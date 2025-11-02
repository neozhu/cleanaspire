// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanAspire.Application.Features.Products.DTOs;
using CleanAspire.Application.Features.Stocks.DTOs;

namespace CleanAspire.Application.Features.Stocks.Queryies;
public record StocksWithPaginationQuery(string Keywords, int PageNumber = 0, int PageSize = 15, string OrderBy = "Id", string SortDirection = "Descending") : IFusionCacheRequest<PaginatedResult<StockDto>>
{
    public IEnumerable<string>? Tags => new[] { "stocks" };
    public string CacheKey => $"stockswithpagination_{Keywords}_{PageNumber}_{PageSize}_{OrderBy}_{SortDirection}";
}

public class StocksWithPaginationQueryHandler : IRequestHandler<StocksWithPaginationQuery, PaginatedResult<StockDto>>
{
    private readonly IApplicationDbContext _context;

    public StocksWithPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<PaginatedResult<StockDto>> Handle(StocksWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Stocks.Include(x => x.Product).OrderBy(request.OrderBy, request.SortDirection)
                    .ProjectToPaginatedDataAsync(
                        condition: x => x.Location.Contains(request.Keywords) || (x.Product != null && (x.Product.Name.Contains(request.Keywords) || x.Product.SKU.Contains(request.Keywords) || x.Product.Description.Contains(request.Keywords))),
                        pageNumber: request.PageNumber,
                        pageSize: request.PageSize,
                        mapperFunc: t => new StockDto
                        {
                            Id = t.Id,
                            ProductId = t.ProductId,
                            Product = new ProductDto
                            {
                                Category = t.Product?.Category?? ProductCategory.Electronics,
                                Currency = t.Product?.Currency,
                                Description = t.Product?.Description,
                                Id = t.Product?.Id,
                                Name = t.Product?.Name,
                                Price = t.Product?.Price ?? 0,
                                SKU = t.Product?.SKU,
                                UOM = t.Product?.UOM,
                            },
                            Quantity = t.Quantity,
                            Location = t.Location
                        },
                    cancellationToken: cancellationToken);
        return data;
    }
}
