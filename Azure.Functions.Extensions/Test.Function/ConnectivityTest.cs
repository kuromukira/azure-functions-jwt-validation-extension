using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Extensions.JwtCustomHandler.Interface;
using Microsoft.Azure.Functions.Extensions.JwtCustomHandler;

namespace Test.Function
{
    public class ConnectivityTest
    {
        readonly IFirebaseTokenProvider TokenProvider;
        public ConnectivityTest(IFirebaseTokenProvider provider) => TokenProvider = provider;

        [FunctionName("Test_FirebaseAuth")]
        public async Task<IActionResult> AuthorizeSignIn([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "test-connection")] HttpRequest req)
        {
            try
            {
                if (req.Body is null)
                    throw new NullReferenceException();
                else
                {
                    AccessTokenResult _tokenResult = await TokenProvider.ValidateToken(req);
                    if (_tokenResult.Status == AccessTokenStatus.Valid)
                        return new OkResult();
                    else return new UnauthorizedResult();
                }
            }
            catch (Exception)
            { return new BadRequestObjectResult("Web server encountered an error."); }
        }
    }
}
