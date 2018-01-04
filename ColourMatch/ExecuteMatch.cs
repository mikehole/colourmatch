using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace ColourMatch
{
    public static class ExecuteMatch
    {
        [FunctionName("ExecuteMatch")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string imageUrl = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "url", true) == 0)
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            imageUrl = imageUrl ?? data?.url;

            return imageUrl == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass the url of the image to process on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, "You requested to check:" + imageUrl);
        }
    }
}
