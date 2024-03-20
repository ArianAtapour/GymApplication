using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

namespace SongPlayer
{
    public partial class MainPage : ContentPage
    {
        private readonly WeatherDataModule _weatherDataModule;
        private readonly List<Song> _songs = new List<Song>();
        private Song _previousSong;

        public MainPage()
        {
            InitializeComponent();
            _weatherDataModule = new WeatherDataModule();
            LoadSongs();
        }

        private string GetProjectDirectory()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string parentDirectory = basePath;

            for (int i = 0; i < 6; i++)
            {
                DirectoryInfo parentInfo = Directory.GetParent(parentDirectory);
                if (parentInfo != null)
                    parentDirectory = parentInfo.FullName;
                else
                    break;
            }
            return parentDirectory;
        }

        private void LoadSongs()
        {
            AddDebugMessage("Loading songs...");

            string songsDirectory = Path.Combine(GetProjectDirectory(), "songs");

            if (!Directory.Exists(songsDirectory))
            {
                AddDebugMessage("Songs directory not found.");
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
                        continue;
                    }

                    string fileName = Path.GetFileNameWithoutExtension(songFile);
                    string[] parts = fileName.Split(';');

                    if (parts.Length != 3)
                    {
                        AddDebugMessage($"Invalid format for song file: {songFile}. Skipping.");
                        continue;
                    }

                    string artist = parts[0];
                    string title = parts[1];

                    if (!Enum.TryParse(parts[2], out Genre genre))
                    {
                        AddDebugMessage($"Invalid condition for song file: {songFile}. Skipping.");
                        continue;
                    }

                    _songs.Add(new Song { Title = title, Artist = artist, Genre = genre });
                    AddDebugMessage($"Song loaded: {artist} - {title}");
                }
                catch (Exception ex)
                {
                    AddDebugMessage($"Error loading song file: {songFile}. Details: {ex.Message}");
                }
            }

            totalSongsLabel.Text = $"Total Songs: {_songs.Count}";
            AddDebugMessage($"Total songs loaded: {_songs.Count}");
        }

        private void AddDebugMessage(string message)
        {
            Label debugLabel = new Label
            {
                Text = message,
            };

            debugOutputContainer.Children.Add(debugLabel);
        }

        private void OnRandomSongClicked(object sender, EventArgs e)
        {
            var filteredSongs = _songs.Where(song => song != _previousSong).ToList();
            if (filteredSongs.Any())
            {
                Random random = new Random();
                int index = random.Next(filteredSongs.Count);
                var selectedSong = filteredSongs[index];

                string filePath = Path.Combine(GetProjectDirectory(), $"songs/{selectedSong.Artist};{selectedSong.Title};{selectedSong.Genre}.mp3");
                AddDebugMessage(filePath);
                mediaElement.Source = new Uri(filePath);
                mediaElement.Play();

                nowPlayingLabel.Text = $"Now playing: {selectedSong.Artist} - {selectedSong.Title}";

                _previousSong = selectedSong;
            }
            else
            {
                // nothing when no songs found
            }
        }
        private void PlaySong(Genre genre)
        {
                var filteredSongs = _songs.Where(song => song.Genre == genre && song != _previousSong).ToList();
                if (filteredSongs.Any())
                {
                    Random random = new Random();
                    int index = random.Next(filteredSongs.Count);
                    var selectedSong = filteredSongs[index];

                    string filePath = Path.Combine(GetProjectDirectory(), $"songs/{selectedSong.Artist};{selectedSong.Title};{selectedSong.Genre}.mp3");
                    nowPlayingLabel.Text = $"Now playing: {selectedSong.Artist} - {selectedSong.Title}";
                    mediaElement.Source = new Uri(filePath);
                    mediaElement.Play();

                    _previousSong = selectedSong;
                }
                else
                {
                    // nothing when no songs found
                }
        }

        private async void OnGetGenreButtonClicked(object sender, EventArgs e)
        {
            string genre;
            AddDebugMessage(_weatherDataModule.country);
            AddDebugMessage(_weatherDataModule.city);
            genre = await _weatherDataModule.SearchForGenre();
            AddDebugMessage(genre);

            if (Enum.TryParse(genre, out Genre parsedGenre))
            {
                PlaySong(parsedGenre);
            }
            else
            {
                AddDebugMessage($"Invalid genre: {genre}");
            }
        }
    }
}
