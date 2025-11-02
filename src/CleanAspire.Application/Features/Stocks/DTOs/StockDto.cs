// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanAspire.Application.Features.Products.DTOs;
using CleanAspire.Domain.Common;

namespace CleanAspire.Application.Features.Stocks.DTOs;
/// <summary>
/// Data Transfer Object for Stock.
/// </summary>
public class StockDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the stock.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    public string ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product details.
    /// </summary>
    public ProductDto Product { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the stock.
    /// </summary>
    public int Quantity { get; set; } = 0;

    /// <summary>
    /// Gets or sets the location of the stock.
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the stock was created.
    /// </summary>
    public DateTime? Created { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the stock was last modified.
    /// </summary>
    public DateTime? LastModified { get; set; }
}

