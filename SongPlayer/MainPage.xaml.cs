using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

namespace SongPlayer;

public partial class MainPage : ContentPage
{
     List<Song> songs = new List<Song>();
    private Song previousSong = null;

    public MainPage()
        {
            InitializeComponent();
            LoadSongs();
        }
    // because debugging is done in god knows what folder, i push it back to project directory because i need it in multiple places
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
                break;
            }
        }
        return parentDirectory;
    }
    //Loading all songs from the songs folder into an array list
    private void LoadSongs()
    {
        AddDebugMessage("Loading songs...");

        string songsDirectory = System.IO.Path.Combine(GetProjectDirectory(), $"songs/");


        if (!Directory.Exists(songsDirectory))
        {
            AddDebugMessage("Songs directory not found.");
            // if no directory - die
            return;
        }

        string[] songFiles = Directory.GetFiles(songsDirectory);

        AddDebugMessage($"Found {songFiles.Length} song files.");

        foreach (string songFile in songFiles)
        {
            AddDebugMessage($"Loading song: {songFile}");

            try
            {

                    if (Path.GetExtension(songFile) != ".mp3")
                    {
                        AddDebugMessage($"Skipping non-MP3 file: {songFile}");
                        // if a file is not mp3 - do not add
                        continue;
                    }

                    string fileName = Path.GetFileNameWithoutExtension(songFile);
                string[] parts = fileName.Split(';');

                if (parts.Length != 3)
                {
                    AddDebugMessage($"Invalid format for song file: {songFile}. Skipping.");
                    // wrong format of file structure (artist;title;condition.mp3) - do not add
                    continue;
                }

                string artist = parts[0];
                string title = parts[1];
                if (!Enum.TryParse(parts[2], out WeatherCondition condition))
                {
                    AddDebugMessage($"Invalid condition for song file: {songFile}. Skipping.");
                    // if no registered weather condition - do not add
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
    //pure debugging
    private void AddDebugMessage(string message)
    {
        Label debugLabel = new Label
        {
            Text = message,
        };

        debugOutputContainer.Children.Add(debugLabel);
    }


    //just play any random song
    private void OnCounterClicked(object sender, EventArgs e)
    {
        var filteredSongs = songs.Where(song => song != previousSong).ToList();
        if (filteredSongs.Any())
        {
            Random random = new Random();
            int index = random.Next(songs.Count);
            var selectedSong = songs[index];

            string filePath = System.IO.Path.Combine(GetProjectDirectory(), $"songs/{selectedSong.Artist};{selectedSong.Title};{selectedSong.Condition}.mp3");
            AddDebugMessage(filePath);
            mediaElement.Source = new Uri(filePath);
            mediaElement.Play();

            nowPlayingLabel.Text = $"Now playing: {selectedSong.Artist} - {selectedSong.Title}";

            // making sure the same song wont play twice in a row
            previousSong = selectedSong;

        }
        else
        {
            // nothing when no songs found
        }
    }
    //depending on which weather button clicked, it creates a new array with the condititon specified-only songs and pick one random one from them
    private void OnWeatherButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            WeatherCondition condition = (WeatherCondition)Enum.Parse(typeof(WeatherCondition), button.Text);

            var filteredSongs = songs.Where(song => song.Condition == condition && song != previousSong).ToList();
            if (filteredSongs.Any())
            {
                Random random = new Random();
                int index = random.Next(filteredSongs.Count);
                var selectedSong = filteredSongs[index];

                string filePath = System.IO.Path.Combine(GetProjectDirectory(), $"songs/{selectedSong.Artist};{selectedSong.Title};{selectedSong.Condition}.mp3");
                nowPlayingLabel.Text = $"Now playing: {selectedSong.Artist} - {selectedSong.Title}";
                mediaElement.Source = new Uri(filePath);
                mediaElement.Play();

            // making sure the same song wont play twice in a row
            previousSong = selectedSong;
        }
            else
            {
                // nothing when no songs found
            }
        }
}
