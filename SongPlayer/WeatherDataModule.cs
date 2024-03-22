using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SongPlayer
{
    internal class WeatherDataModule
    {
        public string weatherCondition;
        public int weatherCode;
        public const string apiKey = "e83bddd9808ae125d3d77a6fe13cbcfe";
        public string country = "Netherlands";
        public string city = "Emmen";

        public WeatherDataModule() { }
        public async Task<string> SearchCsvAsync(string filePath, string country, string city, string weatherConditions)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"CSV file not found: {filePath}");
            }

            switch (weatherCondition)
            {
                case "Thunderstorm":
                    weatherCode = 2;
                    break;
                case "Drizzle":
                    weatherCode = 3;
                    break;
                case "Rain":
                    weatherCode = 4;
                    break;
                case "Snow":
                    weatherCode = 5;
                    break;
                case "Atmosphere":
                    weatherCode = 6;
                    break;
                case "Clear":
                    weatherCode = 7;
                    break;
                case "Clouds":
                    weatherCode = 8;
                    break;
            }

            // Use PLINQ with async methods
            var data = (from line in await File.ReadAllLinesAsync(filePath).ConfigureAwait(false)
                        let parts = line.Split(',')
                        where parts.Length >= 2 && parts[0] == country && parts[1] == city
                        select parts[weatherCode]).FirstOrDefault();
            return data;
        }

        public async Task<string> SearchForGenre()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string targetPath = Path.GetDirectoryName(basePath);

            while (Path.GetFileName(targetPath) != "SongPlayer" && !string.IsNullOrEmpty(targetPath))
            {
                targetPath = Path.GetDirectoryName(targetPath); // Go up one more directory
            }

            string filePath = Path.GetFullPath(Path.Combine(targetPath, "cities.csv"));

            await GetWeatherAsync();

            try
            {
                var result = await SearchCsvAsync(filePath, country, city, weatherCondition);

                if (result != null)
                {
                    return result;
                    //await DisplayAlert("Result", $"Found: country: {result} ", "OK");
                }
                else
                {
                    return "nothing found";
                    //await DisplayAlert("Result", "Entry not found.", "OK");
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
                //Console.WriteLine($"Error searching CSV: {ex.Message}");
            }
        }
        async Task GetWeatherAsync()
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}";

            using HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                try
                {
                    JsonDocument document = JsonDocument.Parse(jsonString);

                    // Access the weather description
                    this.weatherCondition = document.RootElement.GetProperty("weather").EnumerateArray().First().GetProperty("main").GetString();

                    Console.WriteLine($"Weather description: {this.weatherCondition}");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error parsing JSON: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Error getting weather: {response.StatusCode}");
            }
        }
    }
}
