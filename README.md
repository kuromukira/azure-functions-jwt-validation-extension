# Custom Token Validation Provider for Azure Functions
A custom access token validation provider for Azure Functions via Dependency Injection. Inspired by https://github.com/BenMorris/FunctionsCustomSercuity with extra implementation for Firebase Auth.

## [NuGet](https://www.nuget.org/packages/Azure.Functions.JwtCustomHandler/)

## Custom Token Validator
Add in your Function's `Startup.cs`
```cs
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.JwtCustomHandler;
using Microsoft.Azure.Functions.Extensions.JwtCustomHandler.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(ExampleFunction.Startup))]
namespace ExampleFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            /// Custom Token Validator
            builder.Services.AddSingleton<IAccessTokenProvider, AccessTokenProvider>(provider => new AccessTokenProvider(
                issuer: Environment.GetEnvironmentVariable("issuer"),
                audience: Environment.GetEnvironmentVariable("audience"),
                issuerSigningKey: Environment.GetEnvironmentVariable("issuerSigningKey")));
        }
    }
}
```
Inside your Function Class
```cs
public class ExamplesFunction
{
    private readonly IAccessTokenProvider IAccessTokenProvider;
    public ExamplesFunction(IAccessTokenProvider accessTokenProvider) => IAccessTokenProvider = accessTokenProvider;

    [FunctionName("Example_Function")]
    public IActionResult HelloWorldFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
    {
        try
        {
            if (req.Headers is null)
                throw new NullReferenceException();
            else if (req.Body is null)
                throw new NullReferenceException();

            AccessTokenResult _tokenResult = IAccessTokenProvider.ValidateToken(req);
            if (_tokenResult.Status != AccessTokenStatus.Valid)
                return new UnauthorizedResult();

            return new OkObjectResult($"Hello there, ${req.Query["name"]}");
        }
        catch (Exception)
        { return new BadRequestObjectResult("Web server encountered an error."); }
    }
}
```

or you can use the

## Firebase Auth Id Token Validator
Add in your Function's `Startup.cs`
```cs
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.JwtCustomHandler;
using Microsoft.Azure.Functions.Extensions.JwtCustomHandler.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(ExampleFunction.Startup))]
namespace ExampleFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            /// Firebase Id Token Validator
            builder.Services.AddSingleton<IAccessTokenProvider, AccessTokenProvider>(provider => new AccessTokenProvider(
                issuer: "https://securetoken.google.com/<your-firebase-app-name>",
                audience: "<your-firebase-app-name>"));
        }
    }
}
```
Inside your Function Class
```cs
public class ExamplesFunction
{
    private readonly IAccessTokenProvider IAccessTokenProvider;
    public ExamplesFunction(IAccessTokenProvider accessTokenProvider) => IAccessTokenProvider = accessTokenProvider;

    [FunctionName("Example_Function")]
    public async Task<IActionResult> HelloWorldFunction(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
    {
        try
        {
            if (req.Headers is null)
                throw new NullReferenceException();
            else if (req.Body is null)
                throw new NullReferenceException();

            AccessTokenResult _tokenResult = await IAccessTokenProvider.ValidateFirebaseToken(req);
            if (_tokenResult.Status != AccessTokenStatus.Valid)
                return new UnauthorizedResult();

            return new OkObjectResult($"Hello there, ${req.Query["name"]}");
        }
        catch (Exception)
        { return new BadRequestObjectResult("Web server encountered an error."); }
    }
}
```

## Contributors
- [kuromukira](https://www.twitter.com/norgelera)

Install the following to get started

**IDE**
1. [Visual Studio Code](https://code.visualstudio.com/) 
2. [Visual Studio Community](https://visualstudio.microsoft.com/downloads/)

**Exntesions**
1. [C# Language Extension for VSCode](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp)

**Frameworks**
1. [.NET](https://www.microsoft.com/net/download)


Do you want to contribute? Send me an email or DM me in [twitter](https://www.twitter.com/norgelera).