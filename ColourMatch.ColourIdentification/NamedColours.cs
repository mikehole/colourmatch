using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ColourMatch.ColourIdentification
{
    public class NamedColours
    {
        public static async Task<IEnumerable<NamedColour>> GetStandardNamedColours()
        {
            List<NamedColour> NamedColours = new List<NamedColour>();

            var serializer = new JsonSerializer();

            var httpClient = new HttpClient();

            Stream dataStream = await httpClient.GetStreamAsync("https://raw.githubusercontent.com/codebrainz/color-names/master/output/colors.json");

            using (var sr = new StreamReader(dataStream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                var data = ((JObject)serializer.Deserialize(jsonTextReader)).Children();

                foreach (JProperty item in data)
                {
                    var rgb = item.Values<JToken>("rgb").First();
                    var newColour = new NamedColour() { Name = item.Values<string>("name").First(), ColourValue = new Rgba32(rgb[0].Value<byte>(), rgb[1].Value<byte>(), rgb[2].Value<byte>()) };
                    NamedColours.Add(newColour);
                }
            }

            return NamedColours;
        }

        public static async Task<IEnumerable<NamedColour>> GetKnownNamedColours()
        {
            List<NamedColour> NamedColours = new List<NamedColour>();

            var serializer = new JsonSerializer();

            var httpClient = new HttpClient();

            Stream dataStream = await httpClient.GetStreamAsync("https://colourmatch.blob.core.windows.net/data/KnownColours.json");

            using (var sr = new StreamReader(dataStream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                var data = (JArray)serializer.Deserialize(jsonTextReader);

                foreach (JToken item in data)
                {
                    var rgb = item["rgb"];
                    var newColour = new NamedColour() { Name = item["name"].ToString(), ColourValue = new Rgba32(rgb[0].Value<byte>(), rgb[1].Value<byte>(), rgb[2].Value<byte>()) };
                    NamedColours.Add(newColour);
                }
            }

            return NamedColours;
        }
    }
}