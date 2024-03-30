using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WorkoutApp;

namespace SongPlayer
{
    internal class WeatherDataModule
    {
        private static WeatherDataModule instance;

        public static WeatherDataModule Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WeatherDataModule();
                }
                return instance;
            }
        }

        public string weatherCondition;
        public int weatherCode;
        private WeatherInput weatherInput;
        public const string apiKey = "e83bddd9808ae125d3d77a6fe13cbcfe";
        public string Country { get; set; }
        public string City { get; set; }
        public static string CityChosen { get; set; }

        public WeatherDataModule()
        {

        }
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

            if (data != null)
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> SearchForGenre()
        {

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string targetPath = Path.GetDirectoryName(basePath);

            while (Path.GetFileName(targetPath) != "WorkoutApp" && !string.IsNullOrEmpty(targetPath))
            {
                targetPath = Path.GetDirectoryName(targetPath); // Go up one more directory
            }

            string filePath = Path.GetFullPath(Path.Combine(targetPath, "cities.csv"));

            await GetWeatherAsync();

            try
            {
                var result = await SearchCsvAsync(filePath, Country, City, weatherCondition);

                if (result != null)
                {
                    Debug.WriteLine(result);
                    return result;
                    //await DisplayAlert("Result", $"Found: Country: {result} ", "OK");
                }
                else
                {
                    Debug.WriteLine("NOTHING FOUND");
                    return "nothing found";
                    //await DisplayAlert("Result", "Entry not found.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CSV ERROR SEARCHING {ex.Message}");
                return ex.ToString();
                //Console.WriteLine($"Error searching CSV: {ex.Message}");
            }
        }
        public async Task GetWeatherAsync()
        {
            Debug.WriteLine(this.City);

            string url = $"https://api.openweathermap.org/data/2.5/weather?q={this.City}&appid={apiKey}";

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

                    Debug.WriteLine($"Weather description: {this.weatherCondition}");
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"Error parsing JSON: {ex.Message}");
                }
            }
            else
            {
                Debug.WriteLine($"Error getting weather: {response.StatusCode}" + url);
            }
        }
    }
}
