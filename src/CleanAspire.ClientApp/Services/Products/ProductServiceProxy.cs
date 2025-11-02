// This class acts as a service proxy for managing products, supporting both online and offline operations.
// It integrates caching, offline synchronization, and API interactions to enhance the application's reliability and user experience.

// Purpose:
// 1. **Online/Offline Mode Support**:
//    - Provides seamless functionality in offline mode with caching and deferred synchronization.
//    - Automatically syncs cached operations (create, update, delete) when the application returns online.

// 2. **Caching**:
//    - Uses a caching mechanism (via `ProductCacheService` and `ApiClientServiceProxy`) to store and retrieve product data efficiently.
//    - Improves performance by reducing redundant API calls and enabling offline access to product data.

// 3. **Error Handling**:
//    - Wraps API calls with robust error handling to manage exceptions like `HttpValidationProblemDetails`, `ProblemDetails`, and general exceptions.
//    - Logs errors to facilitate debugging and provides detailed error responses when necessary.

using CleanAspire.Api.Client;
using CleanAspire.Api.Client.Models;
using CleanAspire.ClientApp.Services.JsInterop;
using CleanAspire.ClientApp.Services.PushNotifications;
using Microsoft.AspNetCore.Components;
using Microsoft.Kiota.Abstractions;
using OneOf;


namespace CleanAspire.ClientApp.Services.Products;

/// <summary>
/// This class acts as a service proxy for managing products, supporting both online and offline operations.
/// It integrates caching, offline synchronization, and API interactions to enhance the application's reliability and user experience.
/// </summary>
public class ProductServiceProxy
{
    private readonly NavigationManager _navigationManager;
    private readonly ProductCacheService _productCacheService;
    private readonly IWebpushrService _webpushrService;
    private readonly ApiClientServiceProxy _apiClientServiceProxy;
    private readonly ApiClient _apiClient;
    private readonly OnlineStatusInterop _onlineStatusInterop;
    private readonly OfflineModeState _offlineModeState;
    private readonly OfflineSyncService _offlineSyncService;
    private readonly string[] _cacheTags = new[] { "caching" };
    private bool _previousOnlineStatus;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductServiceProxy"/> class.
    /// </summary>
    /// <param name="navigationManager">The navigation manager.</param>
    /// <param name="productCacheService">The product cache service.</param>
    /// <param name="webpushrService">The web push notification service.</param>
    /// <param name="apiClientServiceProxy">The API client service proxy.</param>
    /// <param name="apiClient">The API client.</param>
    /// <param name="onlineStatusInterop">The online status interop service.</param>
    /// <param name="offlineModeState">The offline mode state.</param>
    /// <param name="offlineSyncService">The offline sync service.</param>
    public ProductServiceProxy(
        NavigationManager navigationManager,
        ProductCacheService productCacheService,
        IWebpushrService webpushrService,
        ApiClientServiceProxy apiClientServiceProxy,
        ApiClient apiClient,
        OnlineStatusInterop onlineStatusInterop,
        OfflineModeState offlineModeState,
        OfflineSyncService offlineSyncService)
    {
        _navigationManager = navigationManager;
        _productCacheService = productCacheService;
        _webpushrService = webpushrService;
        _apiClientServiceProxy = apiClientServiceProxy;
        _apiClient = apiClient;
        _onlineStatusInterop = onlineStatusInterop;
        _offlineModeState = offlineModeState;
        _offlineSyncService = offlineSyncService;
        Initialize();
    }

    /// <summary>
    /// Initializes the service proxy by setting up the online status change event handler.
    /// </summary>
    private void Initialize()
    {
        _onlineStatusInterop.OnlineStatusChanged -= OnOnlineStatusChanged;
        _onlineStatusInterop.OnlineStatusChanged += OnOnlineStatusChanged;
    }

    /// <summary>
    /// Handles the online status change event.
    /// </summary>
    /// <param name="isOnline">if set to <c>true</c> [is online].</param>
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

    /// <summary>
    /// Retrieves paginated product data, using cache if offline.
    /// </summary>
    /// <param name="paginationQuery">The pagination query.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the paginated result of product DTO.</returns>
    public async Task<PaginatedResultOfProductDto> GetPaginatedProductsAsync(ProductsWithPaginationQuery paginationQuery)
    {
        var isOnline = await _onlineStatusInterop.GetOnlineStatusAsync();
        var cacheKey = _productCacheService.GeneratePaginationCacheKey(paginationQuery);
        if (!isOnline)
        {
            var cachedResult = await _productCacheService.GetPaginatedProductsAsync(cacheKey);
            return cachedResult ?? new PaginatedResultOfProductDto();
        }
        try
        {
            var paginatedProducts = await _apiClientServiceProxy.QueryAsync($"_{cacheKey}", () => _apiClient.Products.Pagination.PostAsync(paginationQuery), tags: _cacheTags, expiration: _cacheExpiration);
            if (paginatedProducts != null && _offlineModeState.Enabled)
            {
                await _productCacheService.SaveOrUpdatePaginatedProductsAsync(cacheKey, paginatedProducts);
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

    /// <summary>
    /// Fetches a product by ID, falling back to cached data in offline mode.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the product DTO or a KeyNotFoundException.</returns>
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
            var product = await _apiClientServiceProxy.QueryAsync($"_{productId}", () => _apiClient.Products[productId].GetAsync(), tags: _cacheTags, expiration: _cacheExpiration);
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

    /// <summary>
    /// Creates a new product, supporting both online and offline scenarios.
    /// </summary>
    /// <param name="command">The create product command.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the product DTO or error details.</returns>
    public async Task<OneOf<ProductDto, HttpValidationProblemDetails, ProblemDetails>> CreateProductAsync(CreateProductCommand command)
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
                await _apiClientServiceProxy.ClearCache(_cacheTags);
                return response;
            }
            catch (HttpValidationProblemDetails ex)
            {
                return ex;
            }
            catch (ProblemDetails ex)
            {
                return ex;
            }
            catch (ApiException ex)
            {
                return new ProblemDetails
                {
                    Title = ex.Message,
                    Detail = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ProblemDetails
                {
                    Title = ex.Message,
                    Detail = ex.Message
                };
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
                    //Category =command.Category,
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
                return new ProblemDetails
                {
                    Title = "Offline mode is disabled.",
                    Detail = "Offline mode is disabled. Please enable offline mode to create products in offline mode."
                };
            }
        }
    }

    /// <summary>
    /// Updates an existing product, with offline mode support.
    /// </summary>
    /// <param name="command">The update product command.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success or error details.</returns>
    public async Task<OneOf<bool, HttpValidationProblemDetails, ProblemDetails>> UpdateProductAsync(UpdateProductCommand command)
    {
        var isOnline = await _onlineStatusInterop.GetOnlineStatusAsync();
        if (isOnline)
        {
            try
            {
                var response = await _apiClient.Products.PutAsync(command);
                await _apiClientServiceProxy.ClearCache(_cacheTags);
                return true;
            }
            catch (HttpValidationProblemDetails ex)
            {
                return ex;
            }
            catch (ProblemDetails ex)
            {
                return ex;
            }
            catch (ApiException ex)
            {
                return new ProblemDetails
                {
                    Title = ex.Message,
                    Detail = ex.InnerException?.Message ?? ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ProblemDetails
                {
                    Title = ex.Message,
                    Detail = ex.Message
                };
            }
        }
        else if (_offlineModeState.Enabled)
        {
            await _productCacheService.StoreOfflineUpdateCommandAsync(command);

            var productDto = new ProductDto()
            {
                Id = command.Id,
                //Category =new ProductDto.ProductDto_category() { ProductCategory = command.Category.ProductCategory },
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
        return new ProblemDetails
        {
            Title = "Offline mode is disabled.",
            Detail = "Offline mode is disabled. Please enable offline mode to update products in offline mode."
        };
    }

    /// <summary>
    /// Deletes products, queuing commands for later synchronization if offline.
    /// </summary>
    /// <param name="productIds">The product identifiers.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success or error details.</returns>
    public async Task<OneOf<bool, ProblemDetails>> DeleteProductsAsync(List<string> productIds)
    {
        var isOnline = await _onlineStatusInterop.GetOnlineStatusAsync();
        if (isOnline)
        {
            try
            {
                await _apiClient.Products.DeleteAsync(new DeleteProductCommand() { Ids = productIds });
                await _productCacheService.UpdateDeletedProductsAsync(productIds);
                await _apiClientServiceProxy.ClearCache(_cacheTags);
                return true;
            }
            catch (ProblemDetails ex)
            {
                return ex;
            }
            catch (ApiException ex)
            {
                return new ProblemDetails
                {
                    Title = ex.Message,
                    Detail = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ProblemDetails
                {
                    Title = ex.Message,
                    Detail = ex.Message
                };
            }
        }
        else if (_offlineModeState.Enabled)
        {
            var cmd = new DeleteProductCommand { Ids = productIds };
            await _productCacheService.StoreOfflineDeleteCommandAsync(cmd);
            await _productCacheService.UpdateDeletedProductsAsync(productIds);
            return true;
        }
        return new ProblemDetails
        {
            Title = "Offline mode is disabled.",
            Detail = "Offline mode is disabled. Please enable offline mode to delete products in offline mode."
        };
    }

    /// <summary>
    /// Synchronizes offline cached commands (create, update, delete) with the server upon reconnecting to the internet.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
        await _productCacheService.ClearCommands();
        await _apiClientServiceProxy.ClearCache(_cacheTags);
        _offlineSyncService.SetSyncStatus(SyncStatus.Idle, "", 0, 0);
    }
}

