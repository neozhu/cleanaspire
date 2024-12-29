﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CleanAspire.Api.Client;
using CleanAspire.Api.Client.Models;
using CleanAspire.ClientApp.Services.JsInterop;
using CleanAspire.ClientApp.Services.Products;
using CleanAspire.ClientApp.Services.PushNotifications;
using Microsoft.AspNetCore.Components;
using Microsoft.Kiota.Abstractions;
using OneOf;


namespace CleanAspire.ClientApp.Services.Proxies;

public class ProductServiceProxy
{

    private readonly NavigationManager _navigationManager;
    private readonly ProductCacheService _productCacheService;
    private readonly IWebpushrService _webpushrService;
    private readonly ApiClient _apiClient;

    private readonly OnlineStatusInterop _onlineStatusInterop;
    private readonly OfflineModeState _offlineModeState;
    private readonly OfflineSyncService _offlineSyncService;
    private bool _previousOnlineStatus;
    public ProductServiceProxy(NavigationManager navigationManager, ProductCacheService productCacheService, IWebpushrService webpushrService, ApiClient apiClient, OnlineStatusInterop onlineStatusInterop, OfflineModeState offlineModeState, OfflineSyncService offlineSyncService)
    {
        _navigationManager = navigationManager;
        _productCacheService = productCacheService;
        _webpushrService = webpushrService;
        _apiClient = apiClient;
        _onlineStatusInterop = onlineStatusInterop;
        _offlineModeState = offlineModeState;
        _offlineSyncService = offlineSyncService;
        Initialize();
    }
    private void Initialize()
    {
        _onlineStatusInterop.OnlineStatusChanged -= OnOnlineStatusChanged;
        _onlineStatusInterop.OnlineStatusChanged += OnOnlineStatusChanged;
    }

    private async void OnOnlineStatusChanged(bool isOnline)
    {
        if (_previousOnlineStatus == isOnline)
            return;
        _previousOnlineStatus = isOnline;
        if (isOnline)
        {
            await SyncOfflineCachedDataAsync();
        }
    }
    public async Task<PaginatedResultOfProductDto> GetPaginatedProductsAsync(ProductsWithPaginationQuery paginationQuery)
    {
        var isOnline = await _onlineStatusInterop.GetOnlineStatusAsync();
        if (!isOnline)
        {
            var cachedResult = await _productCacheService.GetPaginatedProductsAsync(paginationQuery);
            return cachedResult ?? new PaginatedResultOfProductDto();
        }
        try
        {
            var paginatedProducts = await _apiClient.Products.Pagination.PostAsync(paginationQuery);
            if (paginatedProducts != null && _offlineModeState.Enabled)
            {
                await _productCacheService.SaveOrUpdatePaginatedProductsAsync(paginationQuery, paginatedProducts);

                foreach (var productDto in paginatedProducts.Items)
                {
                    await _productCacheService.SaveOrUpdateProductAsync(productDto);
                }
            }
            return paginatedProducts ?? new PaginatedResultOfProductDto();
        }
        catch
        {
            return new PaginatedResultOfProductDto();
        }
    }

    public async Task<OneOf<ProductDto, KeyNotFoundException>> GetProductByIdAsync(string productId)
    {
        var isOnline = await _onlineStatusInterop.GetOnlineStatusAsync();
        if (!isOnline)
        {
            var cached = await _productCacheService.GetProductAsync(productId);
            if (cached != null)
            {
                return cached;
            }
            return new KeyNotFoundException($"Product '{productId}' not found in offline cache.");
        }
        try
        {
            var product = await _apiClient.Products[productId].GetAsync();
            if (product != null && _offlineModeState.Enabled)
            {
                await _productCacheService.SaveOrUpdateProductAsync(product);
            }
            return product!;
        }
        catch
        {
            return new KeyNotFoundException($"Product '{productId}' could not be fetched from API.");
        }
    }

    public async Task<OneOf<ProductDto, ApiClientValidationError, ApiClientError>> CreateProductAsync(CreateProductCommand command)
    {
        var isOnline = await _onlineStatusInterop.GetOnlineStatusAsync();
        if (isOnline)
        {
            try
            {
                var response = await _apiClient.Products.PostAsync(command);
                var baseUrl = _navigationManager.BaseUri.TrimEnd('/');
                var productUrl = $"{baseUrl}/products/edit/{response.Id}";
                await _webpushrService.SendNotificationAsync(
                    "New Product Launched!",
                    $"Our new product, {response.Name}, is now available. Click to learn more!",
                    productUrl
                );

                return response;
            }
            catch (HttpValidationProblemDetails ex)
            {
                return new ApiClientValidationError(ex.Detail, ex);
            }
            catch (ProblemDetails ex)
            {
                return new ApiClientError(ex.Detail, ex);
            }
            catch (Exception ex)
            {
                return new ApiClientError(ex.Message, ex);
            }
        }
        else
        {
            if (_offlineModeState.Enabled)
            {
                await _productCacheService.StoreOfflineCreateCommandAsync(command);
                var productId = Guid.NewGuid().ToString();
                var productDto = new ProductDto()
                {
                    Id = productId,
                    Category = command.Category,
                    Currency = command.Currency,
                    Description = command.Description,
                    Name = command.Name,
                    Price = command.Price,
                    Sku = command.Sku,
                    Uom = command.Uom
                };
                await _productCacheService.SaveOrUpdateProductAsync(productDto);

                var cachedPaginatedProducts = await _productCacheService.GetAllCachedPaginatedResultsAsync();
                if (cachedPaginatedProducts.Any())
                {
                    foreach (var kvp in cachedPaginatedProducts)
                    {
                        var paginatedProducts = kvp.Value;
                        paginatedProducts.Items.Insert(0, productDto);
                        paginatedProducts.TotalItems++;
                        await _productCacheService.SaveOrUpdatePaginatedProductsAsync(kvp.Key, paginatedProducts);
                    }
                }
                return productDto;
            }
            else
            {
                return new ApiClientError("Offline mode is disabled. Please enable offline mode to create products in offline mode.", new Exception("Offline mode is disabled."));
            }
        }
    }
    public async Task<OneOf<bool, ApiClientValidationError, ApiClientError>> UpdateProductAsync(UpdateProductCommand command)
    {
        var isOnline = await _onlineStatusInterop.GetOnlineStatusAsync();
        if (isOnline)
        {
            try
            {
                var response = await _apiClient.Products.PutAsync(command);
                return true;
            }
            catch (HttpValidationProblemDetails ex)
            {
                return new ApiClientValidationError(ex.Detail, ex);
            }
            catch (ProblemDetails ex)
            {
                return new ApiClientError(ex.Detail, ex);
            }
            catch (ApiException ex)
            {
                return new ApiClientError(ex.Message, ex);
            }
            catch (Exception ex)
            {
                return new ApiClientError(ex.Message, ex);
            }
        }
        else if (_offlineModeState.Enabled)
        {
            await _productCacheService.StoreOfflineUpdateCommandAsync(command);

            var productDto = new ProductDto()
            {
                Id = command.Id,
                Category = command.Category,
                Currency = command.Currency,
                Description = command.Description,
                Name = command.Name,
                Price = command.Price,
                Sku = command.Sku,
                Uom = command.Uom
            };
            await _productCacheService.SaveOrUpdateProductAsync(productDto);

            var cachedPaginatedProducts = await _productCacheService.GetAllCachedPaginatedResultsAsync();
            if (cachedPaginatedProducts != null && cachedPaginatedProducts.Any())
            {
                foreach (var kvp in cachedPaginatedProducts)
                {
                    var key = kvp.Key;
                    var paginatedProducts = kvp.Value;
                    var item = paginatedProducts.Items.FirstOrDefault(x => x.Id == productDto.Id);
                    if (item != null)
                    {
                        item.Category = productDto.Category;
                        item.Currency = productDto.Currency;
                        item.Description = productDto.Description;
                        item.Name = productDto.Name;
                        item.Price = productDto.Price;
                        item.Sku = productDto.Sku;
                        item.Uom = productDto.Uom;
                    }
                    await _productCacheService.SaveOrUpdatePaginatedProductsAsync(kvp.Key, paginatedProducts);
                }
            }
            return true;
        }
        return new ApiClientError("Offline mode is disabled. Please enable offline mode to update products in offline mode.", new Exception("Offline mode is disabled."));
    }
    public async Task<OneOf<bool, ApiClientError>> DeleteProductsAsync(List<string> productIds)
    {
        var isOnline = await _onlineStatusInterop.GetOnlineStatusAsync();
        if (isOnline)
        {
            try
            {
                await _apiClient.Products.DeleteAsync(new DeleteProductCommand() { Ids = productIds });
                await _productCacheService.UpdateDeletedProductsAsync(productIds);
                return true;
            }
            catch (ProblemDetails ex)
            {
                return new ApiClientError(ex.Detail, ex);
            }
            catch (ApiException ex)
            {
                return new ApiClientError(ex.Message, ex);
            }
        }
        else if (_offlineModeState.Enabled)
        {
            var cmd = new DeleteProductCommand { Ids = productIds };
            await _productCacheService.StoreOfflineDeleteCommandAsync(cmd);
            await _productCacheService.UpdateDeletedProductsAsync(productIds);
            return true;
        }
        return new ApiClientError("Offline mode is disabled. Please enable offline mode to delete products in offline mode.", new Exception("Offline mode is disabled."));
    }
    public async Task SyncOfflineCachedDataAsync()
    {
        var (totalCount, cachedCreateProductCommands, cachedUpdateProductCommands, cachedDeleteProductCommands) = await _productCacheService.GetAllPendingCommandsAsync();
        if (totalCount > 0)
        {
            var processedCount = 0;
            _offlineSyncService.SetSyncStatus(SyncStatus.Syncing, $"Starting sync: 0/{totalCount} ...", totalCount, processedCount);
            await Task.Delay(500);

            async Task ProcessCommandsAsync<T>(IEnumerable<T> commands, Func<T, Task> action)
            {
                foreach (var command in commands)
                {
                    processedCount++;
                    await action(command);
                    _offlineSyncService.SetSyncStatus(SyncStatus.Syncing, $"Syncing {processedCount}/{totalCount} Success.", totalCount, processedCount);
                    await Task.Delay(500);
                }
            }

            if (cachedCreateProductCommands != null && cachedCreateProductCommands.Any())
            {
                await ProcessCommandsAsync(cachedCreateProductCommands, CreateProductAsync);
            }

            if (cachedUpdateProductCommands != null && cachedUpdateProductCommands.Any())
            {
                await ProcessCommandsAsync(cachedUpdateProductCommands, UpdateProductAsync);
            }

            if (cachedDeleteProductCommands != null && cachedDeleteProductCommands.Any())
            {
                await ProcessCommandsAsync(cachedDeleteProductCommands, command => DeleteProductsAsync(command.Ids));
            }

            _offlineSyncService.SetSyncStatus(SyncStatus.Completed, $"Sync completed: {processedCount}/{totalCount} processed.", totalCount, processedCount);
            await Task.Delay(1200);
        }

        _offlineSyncService.SetSyncStatus(SyncStatus.Idle, "", 0, 0);
    }
}

