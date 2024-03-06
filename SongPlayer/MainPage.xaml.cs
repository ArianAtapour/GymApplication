using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

namespace SongPlayer;

public partial class MainPage : ContentPage
{
     List<Song> songs = new List<Song>();

        public MainPage()
        {
            InitializeComponent();
            LoadSongs();
        }
    private string GetProjectDirectory()
    {
        string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
        string parentDirectory = basePath;

        for (int i = 0; i < 6; i++)
        {
            DirectoryInfo parentInfo = Directory.GetParent(parentDirectory);
            if (parentInfo != null)
            {
                parentDirectory = parentInfo.FullName;
            }
            else
            {
                // Handle the case where there are fewer than five parent directories
                break;
            }
        }
        return parentDirectory;
    }
    private void LoadSongs()
    {
        AddDebugMessage("Loading songs...");

        string songsDirectory = System.IO.Path.Combine(GetProjectDirectory(), $"songs/");


        if (!Directory.Exists(songsDirectory))
        {
            AddDebugMessage("Songs directory not found.");
            // Handle case where Songs directory doesn't exist
            return;
        }

        string[] songFiles = Directory.GetFiles(songsDirectory);

        AddDebugMessage($"Found {songFiles.Length} song files.");

        foreach (string songFile in songFiles)
        {
            AddDebugMessage($"Loading song: {songFile}");

            try
            {
                string fileName = Path.GetFileNameWithoutExtension(songFile);
                string[] parts = fileName.Split(';');

                if (parts.Length != 3)
                {
                    AddDebugMessage($"Invalid format for song file: {songFile}. Skipping.");
                    // Skip files with invalid format
                    continue;
                }

                string artist = parts[0];
                string title = parts[1];
                if (!Enum.TryParse(parts[2], out WeatherCondition condition))
                {
                    AddDebugMessage($"Invalid condition for song file: {songFile}. Skipping.");
                    // Skip files with invalid condition
                    continue;
                }

                songs.Add(new Song { Title = title, Artist = artist, Condition = condition });
                AddDebugMessage($"Song loaded: {artist} - {title}");
            }
            catch (Exception ex)
            {
                AddDebugMessage($"Error loading song file: {songFile}. Details: {ex.Message}");
            }
        }

        totalSongsLabel.Text = $"Total Songs: {songs.Count}";
        AddDebugMessage($"Total songs loaded: {songs.Count}");
    }

    private void AddDebugMessage(string message)
    {
        Label debugLabel = new Label
        {
            Text = message,
        };

        debugOutputContainer.Children.Add(debugLabel);
    }



    private void OnCounterClicked(object sender, EventArgs e)
    {
        if (songs.Any())
        {
            Random random = new Random();
            int index = random.Next(songs.Count);
            var selectedSong = songs[index];

            string filePath = System.IO.Path.Combine(GetProjectDirectory(), $"songs/{selectedSong.Artist};{selectedSong.Title};{selectedSong.Condition}.mp3");
            AddDebugMessage(filePath);
            mediaElement.Source = new Uri(filePath);
            mediaElement.Play();

            nowPlayingLabel.Text = $"Now playing: {selectedSong.Artist} - {selectedSong.Title}";
        }
        else
        {
            // Display a message or handle the case where no songs are available.
        }
    }

    private void OnWeatherButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            WeatherCondition condition = (WeatherCondition)Enum.Parse(typeof(WeatherCondition), button.Text);

            var filteredSongs = songs.Where(song => song.Condition == condition).ToList();
            if (filteredSongs.Any())
            {
                Random random = new Random();
                int index = random.Next(filteredSongs.Count);
                var selectedSong = filteredSongs[index];

                string filePath = System.IO.Path.Combine(GetProjectDirectory(), $"songs/{selectedSong.Artist};{selectedSong.Title};{selectedSong.Condition}.mp3");
                nowPlayingLabel.Text = $"Now playing: {selectedSong.Artist} - {selectedSong.Title}";
                mediaElement.Source = new Uri(filePath);
                mediaElement.Play();
        }
            else
            {
                // Display a message or handle the case where no songs are available for the selected weather condition.
            }
        }
}
