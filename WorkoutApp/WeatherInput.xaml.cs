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

    private void OnButtonClicked(object sender, EventArgs e)
    {
        this.Country = EntryCountry.Text;
        this.City = EntryCity.Text;

        if (string.IsNullOrEmpty(this.Country) || string.IsNullOrEmpty(this.City))
        {
            DisplayAlert("Error","Country and City cannot be empty", "OK");
        }
        else
        {
            //weatherData.country = this.Country;
            //weatherData.city = this.City;
            DisplayAlert("Working", $"Country and City: {this.Country} {this.City}", "OK");
            //Needs to redirect to Workout app
        }
    }
}