using SongPlayer;
using System.Diagnostics;

namespace WorkoutApp;

public partial class WeatherInput : ContentPage
{
    private WeatherDataModule weatherData;
    public string Country { get; set; }
    public string City { get; set; }
    public WeatherInput()
    {
        weatherData = new WeatherDataModule();
        InitializeComponent();
    }

    private async void OnButtonClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(EntryCountry.Text) || string.IsNullOrEmpty(EntryCity.Text))
        {
            await DisplayAlert("Error", "Country and City cannot be empty", "OK");
        }
        else
        {
            WeatherDataModule.Instance.Country = EntryCountry.Text;
            WeatherDataModule.Instance.City = EntryCity.Text;
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
