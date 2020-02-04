namespace Microsoft.Azure.Functions.Extensions.JwtCustomHandler
{
    public enum AccessTokenStatus
    {
        Valid,
        Expired,
        Error,
        NoToken
    }
}
