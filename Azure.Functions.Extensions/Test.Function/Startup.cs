using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.JwtCustomHandler;
using Microsoft.Azure.Functions.Extensions.JwtCustomHandler.Interface;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Test.Function.Startup))]
namespace Test.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IFirebaseTokenProvider, CustomTokenProvider>(provider => new CustomTokenProvider(
                audience: "firebase-app-name",
                issuer: "firebase-app-issuer"));
        }
    }
}
