// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Http.Json;
using Projects;
using Aspire.Hosting;
using Newtonsoft.Json.Linq;

namespace CleanAspire.Tests;



[TestFixture]
public class StockEndpointTests
{
    // Adjust the backend route prefix to match your actual routes
    private const string ApiBaseUrl = "/stocks";

    private HttpClient _httpClient = null!;
    private HttpClientHandler _httpClientHandler = null!;

    private DistributedApplication? _app;
    private ResourceNotificationService? _resourceNotificationService;

    [SetUp]
    public async Task Setup()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<CleanAspire_AppHost>();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
            clientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });
        });

        _app = await appHost.BuildAsync();
        _resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();
        await _app.StartAsync();

        _httpClient = _app.CreateHttpClient("apiservice");

        await _resourceNotificationService
            .WaitForResourceAsync("apiservice", KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(30));

        await LoginAsync();
    }

    /// <summary>
    /// Performs a login request to obtain an authentication cookie.
    /// Make sure to modify it to your actual backend login route, for example "/account/login" or "/api/login".
    /// Here we use "/login?useCookies=true" as an example.
    /// </summary>
    private async Task LoginAsync()
    {
        var loginRequest = new
        {
            Email = "administrator",
            Password = "P@ssw0rd!"
        };

        // Ensure that your server-side Minimal API or Controller has defined POST /login
        // and that Cookie Auth is enabled
        var response = await _httpClient.PostAsJsonAsync("/login?useCookies=true", loginRequest);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            $"Login should return 200 OK. But was {response.StatusCode}");
    }

    /// <summary>
    /// This is a complete stock process test:
    /// 1) Pagination query of stocks
    /// 2) Receiving new stock
    /// 3) Dispatching stock
    /// </summary>
    [Test]
    public async Task FullStockProcessTest()
    {
        // -------- STEP 1: Test pagination query --------
        var query = new
        {
            keywords = "",
            pageNumber = 0,
            pageSize = 15,
            orderBy = "Id",
            sortDirection = "Descending",
        };

        var paginationResponse = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/pagination", query);
        Assert.That(paginationResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            $"Pagination query at {ApiBaseUrl}/pagination should return 200 OK, but was {paginationResponse.StatusCode}");
        var jsonString = await paginationResponse.Content.ReadAsStringAsync();
        var paginatedResult = JObject.Parse(jsonString);
        Assert.That(paginatedResult, Is.Not.Null, "Pagination result should not be null.");
        Assert.That(Convert.ToInt32(paginatedResult["totalItems"]), Is.GreaterThan(0), "Pagination should return at least one item.");

        // Retrieve the first ProductId for subsequent steps
        var productId = paginatedResult["items"][0]["productId"]?.ToString();
        Assert.That(productId, Is.Not.Null.And.Not.Empty, "ProductId should not be null or empty.");

        // -------- STEP 2: Test stock receiving --------
        var receiveCommand = new
        {
            ProductId = productId,
            Quantity = 50,
            Location = "WH-02"
        };

        var receiveResponse = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/receive", receiveCommand);
        Assert.That(receiveResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            $"Stock receiving at {ApiBaseUrl}/receive should return 200 OK, but was {receiveResponse.StatusCode}");

        // -------- STEP 3: Test stock dispatching --------
        var dispatchCommand = new
        {
            ProductId = productId,
            Quantity = 20,
            Location = "WH-02"
        };

        var dispatchResponse = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/dispatch", dispatchCommand);
        Assert.That(dispatchResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            $"Stock dispatching at {ApiBaseUrl}/dispatch should return 200 OK, but was {dispatchResponse.StatusCode}");
    }

    [TearDown]
    public async Task TearDown()
    {
        // Dispose of the HttpClient and Handler
        _httpClient?.Dispose();
        _httpClientHandler?.Dispose();

        // Dispose of the _resourceNotificationService if it's not null
        _resourceNotificationService?.Dispose();

        // Dispose of the _app if it's not null
        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }
}




