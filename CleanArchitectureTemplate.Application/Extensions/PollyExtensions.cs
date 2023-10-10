using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;

namespace CleanArchitectureTemplate.Application.Extensions;

// <summary>
/// Contains the extension methods for Polly framework for common
/// use-cases that support infrastructure and application ops.
/// </summary>
public static class PollyExtensions
{
    /// <summary>
    /// The maximum number of retries for SQL before failing.
    /// Can be defined before initializing the Policy.
    /// </summary>
    private static int MaxSqlRetries = 5;

    /// <summary>
    /// The maximum HTTP retries before assuming broken.
    /// Can be defined before initializing the Policy.
    /// </summary>
    private static int MaxHttpRetries = 3;

    #region SQL Policies

    /// <summary>
    /// The SQL Policy for DB connections
    /// </summary>
    /// <param name="logger">The logger to log retries for connection, if necessary.</param>
    /// <returns>The <see cref="Policy"/>'ies for resilience.</returns>
    public static Policy SqlPolicy(ILogger logger = null) =>
        Policy.Wrap(DefaultSqlCircuitBreakerPolicy(), DefaultSqlWaitAndRetryPolicy(logger));

    /// <summary>
    /// The policy for handling circuit breaking connections in case of 60% failure rate.
    /// </summary>
    /// <returns>The AdvancedCircuitBreaker for resilience.</returns>
    private static Policy DefaultSqlCircuitBreakerPolicy() =>
        Policy
            .Handle<PostgresException>()
            .AdvancedCircuitBreaker(
                0.6, TimeSpan.FromSeconds(30), 15, TimeSpan.FromSeconds(30));
    

    /// <summary>
    /// The policy for retying <see cref="SqlException"/> errors.
    /// </summary>
    /// <param name="logger">The logger to log retries for connection, if necessary.</param>
    /// <returns>The WaitAndRetry for resilience</returns>
    private static Policy DefaultSqlWaitAndRetryPolicy(ILogger logger = null) =>
        Policy
            .Handle<PostgresException>()
            .WaitAndRetry(
                MaxSqlRetries,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, ctx) =>
                {
                    logger?.LogWarning("SqlException with: [{0}] after {1}", exception.Message, timeSpan.Seconds);
                });

    #endregion

    #region HTTP Policies

    /// <summary>
    /// Adds async Polly policy handlers for resilience.
    /// The preferred method compared to the non-async one.
    /// </summary>
    /// <param name="httpClientBuilder">The <see cref="IHttpClientBuilder"/> we are building.</param>
    /// <param name="logger">A logger, if logging retries is necessary.</param>
    /// <returns>The <see cref="IHttpClientBuilder"/> we built.</returns>
    public static IHttpClientBuilder AddDefaultPollyPolicyAsync(
        this IHttpClientBuilder httpClientBuilder,
        ILogger logger = null) =>
        httpClientBuilder.AddPolicyHandler(HttpPolicyAsync(logger));

    /// <summary>
    /// HTTP policies for resilient connectivity and connections.
    /// Needed if used directly without injecting into <see cref="IHttpClientFactory"/>.
    /// </summary>
    /// <param name="logger">The logger configuration, if any, in case we need to log retries.</param>
    /// <returns>The <see cref="Policy"/> of <see cref="HttpResponseMessage"/> to use for resilient connections.</returns>
    public static Policy<HttpResponseMessage> HttpPolicy(ILogger logger = null) =>
        Policy.Wrap(DefaultHttpCircuitBreakerPolicy(), DefaultHttpWaitAndRetryPolicy(logger));

    /// <summary>
    /// HTTP policies for resilient connectivity and connections.
    /// </summary>
    /// <param name="logger">The logger configuration, if any, in case we need to log retries.</param>
    /// <returns>The <see cref="Policy"/> of <see cref="HttpResponseMessage"/> to use for resilient connections.</returns>
    public static AsyncPolicy<HttpResponseMessage> HttpPolicyAsync(ILogger logger = null) =>
        Policy.WrapAsync(DefaultHttpCircuitBreakerPolicyAsync(), DefaultHttpWaitAndRetryPolicyAsync(logger));

    /// <summary>
    /// The policy for handling circuit breaking HTTP connections in case of 60% failure rate.
    /// </summary>
    /// <returns>The AdvancedCircuitBreaker for resilience.</returns>
    private static CircuitBreakerPolicy<HttpResponseMessage> DefaultHttpCircuitBreakerPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<OperationCanceledException>()
            .AdvancedCircuitBreaker(
                0.5, TimeSpan.FromSeconds(20), 15, TimeSpan.FromSeconds(20));

    /// <summary>
    /// The policy for handling circuit breaking HTTP connections in case of 60% failure rate
    /// in async setup.
    /// </summary>
    /// <returns>The AdvancedCircuitBreaker for resilience.</returns>
    private static AsyncCircuitBreakerPolicy<HttpResponseMessage> DefaultHttpCircuitBreakerPolicyAsync() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<OperationCanceledException>()
            .AdvancedCircuitBreakerAsync(
                0.5, TimeSpan.FromSeconds(20), 15, TimeSpan.FromSeconds(20));

    /// <summary>
    /// The policy for retying <see cref="HttpRequestException"/> errors.
    /// </summary>
    /// <param name="logger">The logger to log retries for connection, if necessary.</param>
    /// <returns>The WaitAndRetry for resilience</returns>
    private static RetryPolicy<HttpResponseMessage> DefaultHttpWaitAndRetryPolicy(ILogger logger = null) =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<OperationCanceledException>()
            .WaitAndRetry(
                MaxHttpRetries,
                retryAttempt => TimeSpan.FromSeconds(2 * retryAttempt),
                (exception, timeSpan, ctx) =>
                {
                    logger?.LogWarning("HttpRequestException with: [{0}] after {1}", exception.Exception?.Message, timeSpan.Seconds);

                });

    /// <summary>
    /// The policy for retying <see cref="HttpRequestException"/> errors and related
    /// in async setup.
    /// </summary>
    /// <param name="logger">The logger to log retries for connection, if necessary.</param>
    /// <returns>The WaitAndRetry for resilience</returns>
    private static AsyncRetryPolicy<HttpResponseMessage> DefaultHttpWaitAndRetryPolicyAsync(ILogger logger = null) =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<OperationCanceledException>()
            .WaitAndRetryAsync(
                MaxHttpRetries,
                retryAttempt => TimeSpan.FromSeconds(2 * retryAttempt),
                (exception, timeSpan, ctx) =>
                {
                    logger?.LogWarning("HttpRequestException with: [{0}] after {1}", exception.Exception?.Message, timeSpan.Seconds);

                });

    #endregion
}
