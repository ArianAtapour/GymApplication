using Microsoft.Maui.Controls;
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
                    if (workoutIndex < workouts.Count - 1)
                    {
                        workoutIndex++;
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
            timer.Start();
        }

        private void Pause_Click(object sender, EventArgs e)
        {
            exerciseLabel.Text = currentExercise?.Name + " PAUSED";
            timer.Stop();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            timer.Stop();
            workoutIndex = 0;
            exerciseIndex = 0;
            currentWorkout = workouts.FirstOrDefault();
            if (currentWorkout != null)
                currentExercise = currentWorkout.Exercises.FirstOrDefault();
            exerciseTime = currentExercise?.Duration ?? TimeSpan.Zero;
            UpdateUI();

            // Display "Workout Finished"
            Device.InvokeOnMainThreadAsync(() =>
            {
                exerciseLabel.Text = "WORKOUT FINISHED";
                timerLabel.Text = "00:00:00";
            });
        }

        private void UpdateUI()
        {
            if (currentWorkout != null)
            {
                workoutNameLabel.Text = "Workout Name: " + currentWorkout.WorkoutName;
                exerciseLabel.Text = "Exercise: " + (currentExercise?.Name ?? "No exercise");
                timerLabel.Text = exerciseTime.ToString(@"hh\:mm\:ss");
            }
        }
    }
}
