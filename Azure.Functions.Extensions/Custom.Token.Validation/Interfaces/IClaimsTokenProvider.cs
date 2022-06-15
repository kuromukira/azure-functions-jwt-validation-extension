using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;

namespace Microsoft.Azure.Functions.Extensions.JwtCustomHandler.Interface;

/// <summary>Validates access tokens that have been submitted as part of a request.</summary>
public interface IClaimsTokenProvider
{
    /// <summary>
    /// Validate the access token, returning the security principal in a result.
    /// </summary>
    /// <param name="request">The HTTP request containing the access token.</param>
    /// <returns>A result that contains the security principal.</returns>
    AccessTokenResult ValidateToken(HttpRequest request);

    /// <summary>
    /// This works with Azure Functions Isolated mode. Validate the access token, returning the security principal in a result.
    /// </summary>
    /// <param name="request">The HTTP request containing the access token.</param>
    /// <returns>A result that contains the security principal.</returns>
    AccessTokenResult ValidateToken(HttpRequestData request);
}
