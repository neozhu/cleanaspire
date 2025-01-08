// <auto-generated/>
#pragma warning disable CS0618
using CleanAspire.Api.Client.Models;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace CleanAspire.Api.Client.Stocks.Dispatch
{
    /// <summary>
    /// Builds and executes requests for operations under \stocks\dispatch
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class DispatchRequestBuilder : BaseRequestBuilder
    {
        /// <summary>
        /// Instantiates a new <see cref="global::CleanAspire.Api.Client.Stocks.Dispatch.DispatchRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public DispatchRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/stocks/dispatch", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::CleanAspire.Api.Client.Stocks.Dispatch.DispatchRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public DispatchRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/stocks/dispatch", rawUrl)
        {
        }
        /// <summary>
        /// Dispatches a specified quantity of stock from a location.
        /// </summary>
        /// <returns>A <see cref="global::CleanAspire.Api.Client.Models.Unit"/></returns>
        /// <param name="body">The request body</param>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
        /// <exception cref="global::CleanAspire.Api.Client.Models.ProblemDetails">When receiving a 400 status code</exception>
        /// <exception cref="global::CleanAspire.Api.Client.Models.HttpValidationProblemDetails">When receiving a 422 status code</exception>
        /// <exception cref="global::CleanAspire.Api.Client.Models.ProblemDetails">When receiving a 500 status code</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::CleanAspire.Api.Client.Models.Unit?> PostAsync(global::CleanAspire.Api.Client.Models.StockDispatchingCommand body, Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::CleanAspire.Api.Client.Models.Unit> PostAsync(global::CleanAspire.Api.Client.Models.StockDispatchingCommand body, Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = ToPostRequestInformation(body, requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "400", global::CleanAspire.Api.Client.Models.ProblemDetails.CreateFromDiscriminatorValue },
                { "422", global::CleanAspire.Api.Client.Models.HttpValidationProblemDetails.CreateFromDiscriminatorValue },
                { "500", global::CleanAspire.Api.Client.Models.ProblemDetails.CreateFromDiscriminatorValue },
            };
            return await RequestAdapter.SendAsync<global::CleanAspire.Api.Client.Models.Unit>(requestInfo, global::CleanAspire.Api.Client.Models.Unit.CreateFromDiscriminatorValue, errorMapping, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Dispatches a specified quantity of stock from a location.
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="body">The request body</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToPostRequestInformation(global::CleanAspire.Api.Client.Models.StockDispatchingCommand body, Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default)
        {
#nullable restore
#else
        public RequestInformation ToPostRequestInformation(global::CleanAspire.Api.Client.Models.StockDispatchingCommand body, Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default)
        {
#endif
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = new RequestInformation(Method.POST, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", "application/json");
            requestInfo.SetContentFromParsable(RequestAdapter, "application/json", body);
            return requestInfo;
        }
        /// <summary>
        /// Returns a request builder with the provided arbitrary URL. Using this method means any other path or query parameters are ignored.
        /// </summary>
        /// <returns>A <see cref="global::CleanAspire.Api.Client.Stocks.Dispatch.DispatchRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public global::CleanAspire.Api.Client.Stocks.Dispatch.DispatchRequestBuilder WithUrl(string rawUrl)
        {
            return new global::CleanAspire.Api.Client.Stocks.Dispatch.DispatchRequestBuilder(rawUrl, RequestAdapter);
        }
        /// <summary>
        /// Configuration for the request such as headers, query parameters, and middleware options.
        /// </summary>
        [Obsolete("This class is deprecated. Please use the generic RequestConfiguration class generated by the generator.")]
        [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
        public partial class DispatchRequestBuilderPostRequestConfiguration : RequestConfiguration<DefaultQueryParameters>
        {
        }
    }
}
#pragma warning restore CS0618
