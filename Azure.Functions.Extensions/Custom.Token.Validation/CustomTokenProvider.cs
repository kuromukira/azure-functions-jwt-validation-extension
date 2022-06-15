using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Extensions.JwtCustomHandler.Interface;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Functions.Extensions.JwtCustomHandler;

/// <summary>Validates an incoming request and extracts any <see cref="ClaimsPrincipal"/> contained within the bearer token.</summary>
public class CustomTokenProvider : IClaimsTokenProvider, IFirebaseTokenProvider
{
    private HttpClient HttpClient { get; set; } = new();
    private readonly string _audience, _issuer, _issuerSigningKey, _authHeaderName, _bearerPrefix, _googleBaseUri, _googleTokenKeys;

    /// <summary></summary>
    public CustomTokenProvider(
        string audience,
        string issuer,
        string issuerSigningKey = null,
        string authHeaderName = "Authorization",
        string bearerPrefix = "Bearer ",
        string googleBaseUri = "https://www.googleapis.com/robot/v1/metadata/",
        string googlePublicKeys = "x509/securetoken@system.gserviceaccount.com")
    {
        _googleTokenKeys = googlePublicKeys ?? "x509/securetoken@system.gserviceaccount.com";
        _googleBaseUri = googleBaseUri ?? "https://www.googleapis.com/robot/v1/metadata/";
        _issuerSigningKey = issuerSigningKey ?? string.Empty;
        _authHeaderName = authHeaderName ?? "Authorization";
        _bearerPrefix = bearerPrefix ?? "Bearer ";
        _audience = audience;
        _issuer = issuer;

        HttpClient.BaseAddress = new Uri(_googleBaseUri);
    }

    async Task<AccessTokenResult> ValidateFirebaseToken(string token)
    {
        try
        {
            // Get signing keys from Google
            using HttpResponseMessage httpResponse = await HttpClient.GetAsync(_googleTokenKeys);
            IDictionary<string, string> x509Data = await httpResponse.Content.ReadAsAsync<Dictionary<string, string>>();
            SecurityKey[] keys = x509Data.Values.Select(CreateSecurityKeyFromPublicKey).ToArray();

            // Create the parameters
            TokenValidationParameters tokenParams = new()
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                IssuerSigningKeys = keys
            };

            // Validate Firebase Id Token
            JwtSecurityTokenHandler handler = new();
            ClaimsPrincipal result = handler.ValidateToken(token, tokenParams, out var securityToken);
            return AccessTokenResult.Success(result);
        }
        catch (SecurityTokenExpiredException)
        { return AccessTokenResult.Expired(); }
        catch (Exception ex)
        { return AccessTokenResult.Error(ex); }

        static SecurityKey CreateSecurityKeyFromPublicKey(string data) => new X509SecurityKey(new X509Certificate2(Encoding.UTF8.GetBytes(data)));
    }

    async Task<AccessTokenResult> IFirebaseTokenProvider.ValidateToken(HttpRequest request)
    {
        try
        {
            // Get the token from the header
            if (request != null &&
                request.Headers.ContainsKey(_authHeaderName) &&
                request.Headers[_authHeaderName].ToString().StartsWith(_bearerPrefix))
            {
                string token = request.Headers[_authHeaderName].ToString()[_bearerPrefix.Length..];
                return await ValidateFirebaseToken(token);
            }
            else return AccessTokenResult.NoToken();
        }
        catch (SecurityTokenExpiredException)
        { return AccessTokenResult.Expired(); }
        catch (Exception ex)
        { return AccessTokenResult.Error(ex); }
    }

    async Task<AccessTokenResult> IFirebaseTokenProvider.ValidateToken(HttpRequestData request)
    {
        try
        {
            // Get the token from the header
            if (request != null &&
                request.Headers.Any(h => h.Key == _authHeaderName) &&
                request.Headers.FirstOrDefault(h => h.Key == _authHeaderName).ToString().StartsWith(_bearerPrefix))
            {
                string token = request.Headers.FirstOrDefault(h => h.Key == _authHeaderName).ToString()[_bearerPrefix.Length..];
                return await ValidateFirebaseToken(token);
            }
            else return AccessTokenResult.NoToken();
        }
        catch (SecurityTokenExpiredException)
        { return AccessTokenResult.Expired(); }
        catch (Exception ex)
        { return AccessTokenResult.Error(ex); }
    }

    AccessTokenResult ValidateCustomJwt(string token)
    {
        // Create the parameters
        TokenValidationParameters tokenParams = new()
        {
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = !string.IsNullOrEmpty(_issuerSigningKey),
        };
        if (!string.IsNullOrEmpty(_issuerSigningKey))
            tokenParams.IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_issuerSigningKey));

        // Validate the token
        JwtSecurityTokenHandler handler = new();
        ClaimsPrincipal result = handler.ValidateToken(token, tokenParams, out var securityToken);
        return AccessTokenResult.Success(result);
    }

    AccessTokenResult IClaimsTokenProvider.ValidateToken(HttpRequest request)
    {
        try
        {
            // Get the token from the header
            if (request != null &&
                request.Headers.ContainsKey(_authHeaderName) &&
                request.Headers[_authHeaderName].ToString().StartsWith(_bearerPrefix))
            {
                string token = request.Headers[_authHeaderName].ToString()[_bearerPrefix.Length..];
                return ValidateCustomJwt(token);
            }
            else return AccessTokenResult.NoToken();
        }
        catch (SecurityTokenExpiredException)
        { return AccessTokenResult.Expired(); }
        catch (Exception ex)
        { return AccessTokenResult.Error(ex); }
    }

    AccessTokenResult IClaimsTokenProvider.ValidateToken(HttpRequestData request)
    {
        try
        {
            // Get the token from the header
            if (request != null &&
                request.Headers.Any(h => h.Key == _authHeaderName) &&
                request.Headers.FirstOrDefault(h => h.Key == _authHeaderName).ToString().StartsWith(_bearerPrefix))
            {
                string token = request.Headers.FirstOrDefault(h => h.Key == _authHeaderName).ToString()[_bearerPrefix.Length..];
                return ValidateCustomJwt(token);
            }
            else return AccessTokenResult.NoToken();
        }
        catch (SecurityTokenExpiredException)
        { return AccessTokenResult.Expired(); }
        catch (Exception ex)
        { return AccessTokenResult.Error(ex); }
    }
}
