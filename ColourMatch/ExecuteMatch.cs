using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System;
using ColourMatch.ColourIdentification;

namespace ColourMatch
{
    public static class ExecuteMatch
    {
        [FunctionName("ExecuteMatch")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var results = new Results();

            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string imageUrl = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "url", true) == 0)
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            //Set name to query string or body data
            imageUrl = imageUrl ?? data?.url;


            if (!string.IsNullOrEmpty(imageUrl))
            {
                var knownNamedColours = await NamedColours.GetKnownNamedColours();
                var standardNamedColours = await NamedColours.GetStandardNamedColours();

                var httpClient = new HttpClient();
                Stream imageStream = await httpClient.GetStreamAsync(imageUrl);

                var x = new ColourMatch.ImageProcessing.PixelPallet();
                x.Load(imageStream, 25);

                var dominentColour = x.PalletPixelCount.First().Key;

                results.mainColourRGB = new int[] { dominentColour.R, dominentColour.G, dominentColour.B };

                //Do we know what this colour is?
                var knownColour = knownNamedColours.Where(C => C.Compare(dominentColour)).FirstOrDefault();

                if (knownColour != null)
                {
                    results.bestMatch = new ColourDetails() { name = knownColour.Name, rgb = new int[] { knownColour.ColourValue.R, knownColour.ColourValue.G, knownColour.ColourValue.B } , matchWeight = knownColour.GetWeight(dominentColour) };
                }

                List<ColourDetails> weightedList = new List<ColourDetails>();

                //Perform a match against the list of known colours
                foreach(var namedColour in standardNamedColours)
                {
                    var weight = namedColour.GetWeight(dominentColour);
                    var colourDetails = new ColourDetails() { name = namedColour.Name, rgb = new int[] { namedColour.ColourValue.R, namedColour.ColourValue.G, namedColour.ColourValue.B }, matchWeight = weight };
                    weightedList.Add(colourDetails);
                }

                if(weightedList.Any())
                {
                    results.otherMatches = weightedList.OrderBy(I => I.matchWeight).Take(10).ToArray();

                    if (results.bestMatch == null)
                        results.bestMatch = results.otherMatches.First();
                }

                return req.CreateResponse(HttpStatusCode.OK, results);
            }
            else
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass the url of the image to process on the query string or in the request body");
        }
    }
}
