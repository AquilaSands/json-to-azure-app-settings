using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonToAzureAppSettings
{
    class Program
    {
        private const char SEPARATOR = ':';

        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter input file path");
            string inFilePath = Console.ReadLine();

            List<Setting> flattenedJson = await FlattenJson(inFilePath);

            Console.WriteLine("Enter output file path");
            string outFilePath = Console.ReadLine();

            await WriteAppSettings(flattenedJson, outFilePath);

        }

        private static async Task WriteAppSettings(List<Setting> flattenedJson, string outFilePath)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            await using (FileStream fileStream = new FileStream(
                outFilePath,
                FileMode.Append,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                useAsync: true))
            {
                await JsonSerializer.SerializeAsync<List<Setting>>(fileStream, flattenedJson, options);
            };
        }

        private static async Task<List<Setting>> FlattenJson(string filePath)
        {
            var flattenedJson = new List<Setting>();

            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            };

            await using (FileStream fileStream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 4096,
                useAsync: true))
            using (JsonDocument document = await JsonDocument.ParseAsync(fileStream, options))
            {
                foreach (JsonProperty property in document.RootElement.EnumerateObject())
                {
                    var keys = new List<string> { property.Name };

                    Parse(flattenedJson, keys, property);
                }
            }

            return flattenedJson;
        }

        private static void Parse(List<Setting> flattenedJson, List<string> keys, JsonProperty property)
        {
            if (property.Value.ValueKind == JsonValueKind.Object)
            {
                JsonElement childElement = property.Value;
                ParseSettings(flattenedJson, keys, childElement);
            }
            else
            {
                // var options = new JsonSerializerOptions
                // {
                //     Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                // };

                string value = property.Value.ValueKind == JsonValueKind.Array ?
                    JsonSerializer.Serialize<JsonElement>(property.Value) :
                    property.Value.ToString();

                var setting = new Setting
                {
                    Name = string.Join(SEPARATOR, keys),
                    Value = value
                };

                flattenedJson.Add(setting);
            }
        }

        // private static async Task<string> ParseArray(JsonElement jsonElement)
        // {
        //     if (jsonElement.ValueKind != JsonValueKind.Array )
        //     {
        //         throw new InvalidOperationException("Json element is not an array");
        //     }

        //     var options = new JsonSerializerOptions
        //     {
        //         WriteIndented = false
        //     };
        //     await JsonSerializer.DeserializeAsync<List<object>>(jsonElement.GetRawText())
        //     return "";
        // }

        private static void ParseSettings(List<Setting> flattenedJson, List<string> keys, JsonElement jsonElement)
        {
            foreach (JsonProperty property in jsonElement.EnumerateObject())
            {
                keys.Add(property.Name);

                Parse(flattenedJson, keys, property);

                keys.RemoveAt(keys.Count - 1);
            }
        }
    }
}
