using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiApp2
{
    public partial class MainPage : ContentPage
    {
        public string weatherCondition;
        public int weatherCode;
        public const string apiKey = "e83bddd9808ae125d3d77a6fe13cbcfe";
        public string country = "Netherlands";
        public string city = "Emmen";

        public MainPage()
        {
            InitializeComponent();
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
            return data;
        }


        private async void OnSearchClicked(object sender, EventArgs e)
        {
            string filePath = "C:\\Users\\Nathan\\OneDrive - NHL Stenden\\NHL Uni work\\C# Resit\\Final Resit Exam\\MauiApp2\\MauiApp2\\cities.csv";
            
            await GetWeatherAsync();

            try
            {
                var result = await SearchCsvAsync(filePath, country, city, weatherCondition);

                if (result !=  null)
                {
                    await DisplayAlert("Result", $"Found: country: {result} ", "OK");
                }
                else
                {
                    await DisplayAlert("Result", "Entry not found.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching CSV: {ex.Message}");
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
