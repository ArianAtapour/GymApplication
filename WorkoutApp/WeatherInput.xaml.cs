using SongPlayer;

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
        weatherData.Country = EntryCountry.Text;
        weatherData.City = EntryCity.Text;

        if (string.IsNullOrEmpty(EntryCountry.Text) || string.IsNullOrEmpty(EntryCity.Text))
        {
            await DisplayAlert("Error", "Country and City cannot be empty", "OK");
        }
        else
        {
            await Shell.Current.Navigation.PushAsync(new WorkoutPage());
        }
    }
}
