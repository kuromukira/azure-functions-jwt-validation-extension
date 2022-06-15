namespace Microsoft.Azure.Functions.Extensions.JwtCustomHandler;

/// <summary></summary>
public enum AccessTokenStatus
{
    /// <summary></summary>
    Valid,

    /// <summary></summary>
    Expired,

    /// <summary></summary>
    Error,

    /// <summary></summary>
    NoToken
}
