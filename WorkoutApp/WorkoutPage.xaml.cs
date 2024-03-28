using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
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

        private void AddWorkoutToView(Workout workout)
        {
            var stackLayout = new StackLayout { Padding = new Thickness(5) };

            var checkBox = new CheckBox { VerticalOptions = LayoutOptions.Center };

            checkBox.CheckedChanged += (sender, e) =>
            {
                var chkBox = (CheckBox)sender;
                var selectedWorkout = chkBox.BindingContext as Workout;

                if (e.Value && !selectedWorkouts.Contains(selectedWorkout))
                {
                    selectedWorkouts.Add(selectedWorkout);
                }
                else if (!e.Value && selectedWorkouts.Contains(selectedWorkout))
                {
                    selectedWorkouts.Remove(selectedWorkout);
                }
            };

            var workoutNameLabel = new Label { Text = workout.WorkoutName, VerticalOptions = LayoutOptions.Center };
            var durationLabel = new Label { Text = workout.TotalDuration.ToString(), VerticalOptions = LayoutOptions.Center };

            stackLayout.Children.Add(checkBox);
            stackLayout.Children.Add(workoutNameLabel);
            stackLayout.Children.Add(durationLabel);

            var exerciseListView = new ListView { ItemsSource = workout.Exercises };
            exerciseListView.ItemTemplate = new DataTemplate(() =>
            {
                var exerciseNameLabel = new Label { VerticalOptions = LayoutOptions.Center };
                exerciseNameLabel.SetBinding(Label.TextProperty, "Name");

                var exerciseDurationLabel = new Label { VerticalOptions = LayoutOptions.Center };
                exerciseDurationLabel.SetBinding(Label.TextProperty, "Duration");

                return new ViewCell
                {
                    View = new StackLayout
                    {
                        Padding = new Thickness(5),
                        Children =
                {
                    exerciseNameLabel,
                    exerciseDurationLabel
                }
                    }
                };
            });

            stackLayout.Children.Add(exerciseListView);

            var viewCell = new ViewCell { View = stackLayout };
            workoutListView.ItemTemplate = new DataTemplate(() => viewCell);
        }


        private void OnCheckboxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkbox = (CheckBox)sender;
            var workout = (Workout)checkbox.BindingContext;

            if (e.Value && !selectedWorkouts.Contains(workout))
            {
                selectedWorkouts.Add(workout);
            }
            else if (!e.Value && selectedWorkouts.Contains(workout))
            {
                selectedWorkouts.Remove(workout);
            }
        }

    }
}
