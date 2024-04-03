using SongPlayer;
using System.Diagnostics;

namespace WorkoutApp;

public partial class WeatherInput : ContentPage
{
    public string Country { get; set; }
    public string City { get; set; }
    public WeatherInput()
    {
        InitializeComponent();
        Debug.WriteLine("WeatherInput constructor called.");
    }

    private async void OnButtonClicked(object sender, EventArgs e)
    {
        weatherData = new WeatherDataModule();
        if (string.IsNullOrEmpty(EntryCountry.Text) || string.IsNullOrEmpty(EntryCity.Text))
        {
            await DisplayAlert("Error", "Country and City cannot be empty", "OK");
        }
        else
        {
            var country = EntryCountry.Text.ToLowerInvariant();
            country = char.ToUpperInvariant(country[0]) + country.Substring(1);

            var city = EntryCity.Text.ToLowerInvariant();
            city = char.ToUpperInvariant(city[0]) + city.Substring(1);

            WeatherDataModule.Instance.Country = country;
            WeatherDataModule.Instance.City = city;
            Debug.WriteLine(WeatherDataModule.Instance.Country + " " + WeatherDataModule.Instance.City);
            await WeatherDataModule.Instance.GetWeatherAsync();

            string result = await WeatherDataModule.Instance.SearchForGenre();

            if (result == "nothing found")
            {
                await DisplayAlert("Error", "City or country not found !", "OK");
            }
            else
            {
                await Shell.Current.Navigation.PushAsync(new WorkoutPage());
            }
        }
    }



}
