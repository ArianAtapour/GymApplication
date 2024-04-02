using Microsoft.Maui.Controls;
using SongPlayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private List<Song> _songs = new List<Song>();
        private Song _previousSong;
        //Successor of Semaphore way better
        private SemaphoreSlim loadSongsSemaphore = new SemaphoreSlim(1, 1);
        //1 means the initial count is 1 the number of entries that can be granted concurently by the semaphore
        //the other 1 is the maximum count and that means the maxium amount of entries that can be granted by the semaphore
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
            LoadSongsAsync();

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
                        //When switching workouts we call this method to change the songs
                        ChangeSong();
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            exerciseLabel.Text = "ALL WORKOUTS FINISHED";
                            timerLabel.Text = "00:00:00";
                            //When all workouts finish we call the method asynchronously to change the song (cause why not let a man listen to music)
                            await ChangeSongAsync();
                        });
                    }
                }
            }
        }

        //TPL(async&await)
        private async Task ChangeSongAsync()
        {
            await Task.Delay(100);
            //Play me a random song
            await GetGenre();
        }

        private void ChangeSong()
        {
            //Play a random song
            GetGenre();
        }

        private async void Start_Click(object sender, EventArgs e)
        {
            exerciseLabel.Text = $"{currentExercise?.Name} STARTED";
            timer.Start();
            await ChangeSongAsync();
        }

        private void Pause_Click(object sender, EventArgs e)
        {
            exerciseLabel.Text = $"{currentExercise?.Name} PAUSED";
            timer.Stop();
            Device.InvokeOnMainThreadAsync(() => mediaElement.Pause());
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            timer.Stop();
            Device.InvokeOnMainThreadAsync(() =>
            {
                mediaElement.Stop();
                exerciseLabel.Text = "WORKOUT FINISHED";
                timerLabel.Text = "00:00:00";
            });

            workoutIndex = 0;
            exerciseIndex = 0;
            currentWorkout = workouts.FirstOrDefault();
            if (currentWorkout != null)
            {
                currentExercise = currentWorkout.Exercises.FirstOrDefault();
            }
            UpdateUI();
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
            string targetPath = Path.GetDirectoryName(basePath);

            while (Path.GetFileName(targetPath) != "WorkoutApp" && !string.IsNullOrEmpty(targetPath))
            {
                targetPath = Path.GetDirectoryName(targetPath); // Go up one more directory
            }
            return targetPath;
        }

        //TPL(async&await) and semaphore implementation and Asynchronous I/O
        private async Task LoadSongsAsync()
        {
            //Hey ! Wait to enter the semaphore before you proceed
            await loadSongsSemaphore.WaitAsync();
            try
            {
                string songsDirectory = Path.Combine(GetProjectDirectory(), "songs");
                // Check if the songs directory exists
                if (!Directory.Exists(songsDirectory))
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        Debug.WriteLine("SONGS DIRECTORY NOT FOUND !");
                    });
                    return;
                }

                /*
                 * Asynchronous I/O
                 * Loading songs from the directory asynchronously and updating UI elements in response to I/O operations
                 */
                // Get all song files in the directory
                string[] songFiles = await Task.Run(() => Directory.GetFiles(songsDirectory)).ConfigureAwait(false);

                var tempSongsList = new List<Song>();
                foreach (string songFile in songFiles)
                {
                    if (Path.GetExtension(songFile) != ".mp3")
                    {
                        //Skip the files that are not mp3 (we assume the user does not know what they are doing)
                        continue;
                    }
                    string fileName = Path.GetFileNameWithoutExtension(songFile);
                    string[] parts = fileName.Split(',');
                    if (parts.Length != 3 || !Enum.TryParse(parts[2], true, out Genre genre))
                    {
                        //Files do not respect the naming convention ? Skip them
                        continue;
                    }

                    //Add the song to the tempSongList
                    tempSongsList.Add(new Song { Title = parts[1], Artist = parts[0], Genre = genre });
                }

                //Update the song
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    _songs = tempSongsList;
                    totalSongsLabel.Text = $"Total Songs: {_songs.Count}";
                    //Debug.WriteLine($"Total Songs: {_songs.Count}");
                });
            }
            finally
            {
                //AAAAnd release from the semaphore
                loadSongsSemaphore.Release();
            }
        }


        //PLINQ
        private async Task<IEnumerable<Song>> FilterSongsAsync(Genre genre)
        {
            return await Task.Run(() =>
            {
                //PLINQ
                return _songs.AsParallel()
                             .Where(song => song.Genre == genre && song != _previousSong)
                             .ToList();
            });
        }

        private async Task PlaySong(Genre genre)
        {
            var filteredSongs = await FilterSongsAsync(genre);
            if (filteredSongs.Any())
            {
                Random random = new Random();
                int index = random.Next(filteredSongs.Count());
                var selectedSong = filteredSongs.ElementAt(index);

                string filePath = Path.Combine(GetProjectDirectory(), $"songs/{selectedSong.Artist},{selectedSong.Title},{selectedSong.Genre}.mp3");
                Debug.WriteLine($"Attempting to play song: {filePath}");
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    nowPlayingLabel.Text = $"Now playing: {selectedSong.Artist} - {selectedSong.Title} - {selectedSong.Genre}";
                    mediaElement.Source = new Uri(filePath);
                    mediaElement.Play();
                });

                _previousSong = selectedSong;
            }
            else
            {
                Debug.WriteLine("No songs found for the selected genre.");
            }
        }

        private async Task GetGenre()
        {
            await WeatherDataModule.Instance.GetWeatherAsync();
            string weatherString = WeatherDataModule.Instance.weatherCondition;
            Device.BeginInvokeOnMainThread(() =>
            {
                weatherLabel.Text = $"Current weather: {weatherString}";
            });


            string genre = await WeatherDataModule.Instance.SearchForGenre();

            if (Enum.TryParse(genre, out Genre parsedGenre))
            {
                await PlaySong(parsedGenre);
            }
            else
            {
                Debug.WriteLine("Failed to parse genre.");
            }
        }



    }
}
