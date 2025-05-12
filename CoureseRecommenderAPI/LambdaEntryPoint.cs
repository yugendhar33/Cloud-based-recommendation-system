using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Amazon.Lambda.AspNetCoreServer;
using Microsoft.AspNetCore.Hosting;
using Amazon.Lambda.Core;


namespace CourseRecommenderAPI
{
    public class LambdaEntryPoint : APIGatewayHttpApiV2ProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder.UseStartup<Program>(); // Program class now supports this
        }

        // This method handles incoming Lambda requests
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apiGatewayRequest, ILambdaContext context)
        {
            // Log the incoming request data to troubleshoot the null issue
            try
            {
                // Log the incoming request as a JSON string
                Console.WriteLine("Incoming request: " + JsonConvert.SerializeObject(apiGatewayRequest));

                // Proceed with the usual logic here...
                // For example, checking if the body is null:
                if (apiGatewayRequest == null || apiGatewayRequest.Body == null)
                {
                    throw new ArgumentNullException("Request or Body is null");
                }

                // Process the request further...

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = "Response body" // Add your response body
                };
            }
            catch (Exception ex)
            {
                // Log any exception that happens during the process for debugging purposes
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }
}
