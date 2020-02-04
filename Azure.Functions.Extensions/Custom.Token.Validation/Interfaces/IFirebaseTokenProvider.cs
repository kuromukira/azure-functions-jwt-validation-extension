using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Microsoft.Azure.Functions.Extensions.JwtCustomHandler.Interface
{
    /// <summary>Validates Firebase Id Tokens that have been submitted as part of a request.</summary>
    public interface IFirebaseTokenProvider
    {
        /// <summary>
        /// Validate the Firebase Id Token
        /// </summary>
        /// <param name="request">The HTTP request containing the access token.</param>
        /// <returns>A result that contains the security principal.</returns>
        Task<AccessTokenResult> ValidateToken(HttpRequest request);
    }
}
