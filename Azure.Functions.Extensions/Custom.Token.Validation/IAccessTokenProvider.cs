using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Microsoft.Azure.Functions.Extensions.JwtCustomHandler.Interface
{
    /// <summary>Validates access tokes that have been submitted as part of a request.</summary>
    public interface IAccessTokenProvider
    {
        /// <summary>
        /// Validate the access token, returning the security principal in a result.
        /// </summary>
        /// <param name="request">The HTTP request containing the access token.</param>
        /// <returns>A result that contains the security principal.</returns>
        AccessTokenResult ValidateToken(HttpRequest request);

        /// <summary>
        /// Validate the Firebase Id Token
        /// </summary>
        /// <param name="request">The HTTP request containing the access token.</param>
        /// <returns>A result that contains the security principal.</returns>
        Task<AccessTokenResult> ValidateFirebaseToken(HttpRequest request);
    }
}
