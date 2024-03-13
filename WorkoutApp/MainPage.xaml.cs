using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;

namespace WorkoutApp
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Exercise> exercises = new ObservableCollection<Exercise>();
        private ObservableCollection<Workout> workouts = new ObservableCollection<Workout>();
        private Workout currentWorkout; 

        public MainPage()
        {
            InitializeComponent();
            exerciseListView.ItemsSource = exercises;
            workoutListView.ItemsSource = workouts;
        }

        private void OnAddExerciseClicked(object sender, EventArgs e)
        {
            string exerciseName = exerciseNameEntry.Text;
            if (string.IsNullOrEmpty(exerciseName))
            {
                DisplayAlert("Error", "Please enter exercise name", "OK");
                return;
            }

            int hours = string.IsNullOrEmpty(hoursDurationEntry.Text) ? 0 : int.Parse(hoursDurationEntry.Text);
            int minutes = string.IsNullOrEmpty(minutesDurationEntry.Text) ? 0 : int.Parse(minutesDurationEntry.Text);
            int seconds = string.IsNullOrEmpty(secondsDurationEntry.Text) ? 0 : int.Parse(secondsDurationEntry.Text);

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
    }
}