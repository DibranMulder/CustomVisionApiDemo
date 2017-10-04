using Microsoft.Cognitive.CustomVision.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveConsoleApp
{
    static class Program
    {
        static void Main()
        {
            Console.Write("Enter image file path: ");
            string imageFilePath = Console.ReadLine();

            MakePredictionRequest(imageFilePath).Wait();

            Console.WriteLine("\n\n\nHit ENTER to exit...");
            Console.ReadLine();
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        static async Task MakePredictionRequest(string imageFilePath)
        {
            var client = new HttpClient();

            Console.WriteLine("Enter project id: ");
            string projectId = Console.ReadLine();

            Console.Write("Enter prediction key: ");
            string predictionKey = Console.ReadLine();

            // Request headers - replace this example key with your valid subscription key.
            client.DefaultRequestHeaders.Add("Prediction-Key", predictionKey);

            Console.Write("Enter iteration id: ");
            string iterationId = Console.ReadLine();

            // Prediction URL - replace this example URL with your valid prediction URL.
            string url = $"https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/{projectId}/image?iterationId={iterationId}";

            HttpResponseMessage response;

            // Request body. Try this sample with a locally stored image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                string resContent = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<ImagePredictionResultModel>(resContent);

                foreach (ImageTagPrediction prediction in model.Predictions)
                {
                    Console.WriteLine($"Tag: {prediction.Tag} - {prediction.Probability}");
                }
            }
        }
    }
}
