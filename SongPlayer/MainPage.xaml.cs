using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

namespace SongPlayer;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
        string filePath = System.IO.Path.Combine(basePath, "song.mp3");
        mediaElement.Source = new Uri(filePath);
        mediaElement.Play();
    }
}
