using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;

namespace WorkoutApp
{
    public partial class WorkoutPage : ContentPage
    {
        private ObservableCollection<Exercise> exercises = new ObservableCollection<Exercise>();
        private ObservableCollection<Workout> workouts = new ObservableCollection<Workout>();
        private List<Workout> selectedWorkouts = new List<Workout>();

        public WorkoutPage()
        {
            InitializeComponent();
            exerciseListView.ItemsSource = exercises;
            workoutListView.ItemsSource = workouts;

            workoutListView.ItemSelected += OnWorkoutItemSelected;

            workoutListView.SelectionMode = (ListViewSelectionMode)SelectionMode.Multiple;
        }
        private void OnAddExerciseClicked(object sender, EventArgs e)
        {
            string exerciseName = exerciseNameEntry.Text;
            if (string.IsNullOrEmpty(exerciseName))
            {
                DisplayAlert("Error", "Please enter exercise name", "OK");
                return;
            }

            if (!int.TryParse(hoursDurationEntry.Text, out int hours) || !int.TryParse(minutesDurationEntry.Text, out int minutes) || !int.TryParse(secondsDurationEntry.Text, out int seconds))
            {
                DisplayAlert("Error", "Please enter valid time values", "OK");
                return;
            }

            if (hours < 0 || minutes < 0 || seconds < 0)
            {
                DisplayAlert("Error", "Time values cannot be negative", "OK");
                return;
            }

            TimeSpan duration = TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds);
            exercises.Add(new Exercise(exerciseNameEntry.Text, duration));
        }

        private void OnSaveWorkoutClicked(object sender, EventArgs e)
        {
            string workoutName = workoutNameEntry.Text;
            if (string.IsNullOrEmpty(workoutName))
            {
                DisplayAlert("Error", "Please enter workout name", "OK");
                return;
            }

            Workout workout = new Workout(workoutName);
            foreach (Exercise exercise in exercises)
            {
                workout.AddExercise(exercise);
            }

            workouts.Add(workout);
            exercises.Clear();
            DisplayAlert("Success", "Workout saved successfully", "OK");
        }

        private async void OnBeginWorkoutClicked(object sender, EventArgs e)
        {
            if (selectedWorkouts.Count == 0)
            {
                await DisplayAlert("Error", "Please select at least one workout.", "OK");
                return;
            }

            
            var gymApplication = new GymApplication(selectedWorkouts);
            await Navigation.PushAsync(gymApplication);
        }

        private void OnWorkoutItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var selectedWorkout = e.SelectedItem as Workout;

            if (!selectedWorkouts.Contains(selectedWorkout))
            {
                selectedWorkouts.Add(selectedWorkout);
            }

            ((ListView)sender).SelectedItem = null;
        }





    }
}
