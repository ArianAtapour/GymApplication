using Microsoft.Maui.Controls;
using SongPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace WorkoutApp
{
    public partial class GymApplication : ContentPage
    {
        private List<Workout> workouts;
        private Workout currentWorkout;
        private Exercise currentExercise;
        private int workoutIndex;
        private int exerciseIndex;
        private TimeSpan exerciseTime;
        private System.Timers.Timer timer;
        private readonly WeatherDataModule _weatherDataModule;
        private readonly List<Song> _songs = new List<Song>();
        private Song _previousSong;

        public GymApplication(List<Workout> workouts)
        {
            InitializeComponent();
            this.workouts = workouts;
            workoutIndex = 0;
            exerciseIndex = 0;
            currentWorkout = workouts.FirstOrDefault();
            if (currentWorkout != null)
                currentExercise = currentWorkout.Exercises.FirstOrDefault();
            UpdateUI();
            _weatherDataModule = new WeatherDataModule();
            LoadSongs();

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            exerciseTime = currentExercise?.Duration ?? TimeSpan.Zero;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            exerciseTime = exerciseTime.Subtract(TimeSpan.FromSeconds(1));

            Device.InvokeOnMainThreadAsync(() =>
            {
                timerLabel.Text = exerciseTime.ToString(@"hh\:mm\:ss");
            });

            if (exerciseTime.TotalSeconds <= 0)
            {
                timer.Stop();
                if (exerciseIndex < currentWorkout.Exercises.Count - 1)
                {
                    exerciseIndex++;
                    currentExercise = currentWorkout.Exercises[exerciseIndex];
                    exerciseTime = currentExercise.Duration;
                    UpdateUI();
                    timer.Start();
                }
                else
                {
                    var nextWorkoutIndex = workoutIndex + 1;
                    if (nextWorkoutIndex < workouts.Count)
                    {
                        workoutIndex = nextWorkoutIndex;
                        currentWorkout = workouts[workoutIndex];
                        exerciseIndex = 0;
                        currentExercise = currentWorkout.Exercises[exerciseIndex];
                        exerciseTime = currentExercise.Duration;
                        UpdateUI();
                        timer.Start();
                    }
                    else
                    {
                        Device.InvokeOnMainThreadAsync(() =>
                        {
                            exerciseLabel.Text = "ALL WORKOUTS FINISHED";
                            timerLabel.Text = "00:00:00";
                        });
                    }
                }
            }
        }


        private void Start_Click(object sender, EventArgs e)
        {
            exerciseLabel.Text = currentExercise?.Name + " STARTED";
            GetGenre();
            timer.Start();
        }

        private void Pause_Click(object sender, EventArgs e)
        {
            exerciseLabel.Text = currentExercise?.Name + " PAUSED";
            mediaElement.Pause();
            timer.Stop();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            timer.Stop();
            mediaElement.Stop();
            workoutIndex = 0;
            exerciseIndex = 0;
            currentWorkout = workouts.FirstOrDefault();
            if (currentWorkout != null)
                currentExercise = currentWorkout.Exercises.FirstOrDefault();
            exerciseTime = currentExercise?.Duration ?? TimeSpan.Zero;
            UpdateUI();

            Device.InvokeOnMainThreadAsync(() =>
            {
                exerciseLabel.Text = "WORKOUT FINISHED";
                timerLabel.Text = "00:00:00";
            });
        }

        private void UpdateUI()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (currentWorkout != null)
                {
                    workoutNameLabel.Text = "Workout Name: " + (currentWorkout.WorkoutName ?? "No workout name");
                    exerciseLabel.Text = "Exercise: " + (currentExercise?.Name ?? "No exercise");
                    timerLabel.Text = exerciseTime.ToString(@"hh\:mm\:ss");
                }
                else
                {
                    workoutNameLabel.Text = "No workout selected";
                    exerciseLabel.Text = "No exercise";
                    timerLabel.Text = "00:00:00";
                }
            });
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
            string songsDirectory = Path.Combine(GetProjectDirectory(), "songs");

            if (!Directory.Exists(songsDirectory))
            {
                  return;
            }

            string[] songFiles = Directory.GetFiles(songsDirectory);

            foreach (string songFile in songFiles)
            {

                try
                {
                    if (Path.GetExtension(songFile) != ".mp3")
                    {
                        continue;
                    }

                    string fileName = Path.GetFileNameWithoutExtension(songFile);
                    string[] parts = fileName.Split(';');

                    if (parts.Length != 3)
                    {
                        continue;
                    }

                    string artist = parts[0];
                    string title = parts[1];

                    if (!Enum.TryParse(parts[2], out Genre genre))
                    {
                        continue;
                    }

                    _songs.Add(new Song { Title = title, Artist = artist, Genre = genre });
                }
                catch (Exception ex)
                {

                }
            }

            totalSongsLabel.Text = $"Total Songs: {_songs.Count}";
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

        private async void GetGenre()
        {
            string genre;
            genre = await _weatherDataModule.SearchForGenre();

            if (Enum.TryParse(genre, out Genre parsedGenre))
            {
                PlaySong(parsedGenre);
            }
            else
            {
            }
        }


    }
}
