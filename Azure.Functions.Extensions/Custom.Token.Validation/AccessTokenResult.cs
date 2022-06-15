using System;
using System.Security.Claims;

namespace Microsoft.Azure.Functions.Extensions.JwtCustomHandler
{
    /// <summary>Contains the result of an access token check.</summary>
    public sealed class AccessTokenResult
    {
        /// <summary>Gets the security principal associated with a valid token.</summary>
        public ClaimsPrincipal Principal { get; private set; }

        /// <summary>Gets the status of the token, i.e. whether it is valid.</summary>
        public AccessTokenStatus Status { get; private set; }

        /// <summary>Gets any exception encountered when validating a token.</summary>
        public Exception Exception { get; private set; }

        /// <summary>Returns a valid token.</summary>
        public static AccessTokenResult Success(ClaimsPrincipal principal) => new()
        {
            Principal = principal,
            Status = AccessTokenStatus.Valid
        };

        /// <summary>Returns a result that indicates the submitted token has expired.</summary>
        public static AccessTokenResult Expired() => new()
        {
            Status = AccessTokenStatus.Expired
        };

        /// <summary>Returns a result to indicate that there was an error when processing the token.</summary>
        public static AccessTokenResult Error(Exception ex) => new()
        {
            Status = AccessTokenStatus.Error,
            Exception = ex
        };

        /// <summary>Returns a result in response to no token being in the request.</summary>
        public static AccessTokenResult NoToken() => new()
        {
            Status = AccessTokenStatus.NoToken
        };
    }
}
