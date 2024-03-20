using System.Collections.ObjectModel;

class Workout
{
    public string WorkoutName { get; set; }
    public TimeSpan TotalDuration
    {
        get
        {
            TimeSpan totalDuration = TimeSpan.Zero;
            foreach (Exercise exercise in Exercises)
            {
                totalDuration += exercise.Duration;
            }
            return totalDuration;
        }
    }
    public ObservableCollection<Exercise> Exercises { get; private set; }

    public Workout(string workoutName)
    {
        WorkoutName = workoutName;
        Exercises = new ObservableCollection<Exercise>();
    }

    public void AddExercise(Exercise exercise)
    {
        Exercises.Add(exercise);
    }

    public void RemoveExercise(Exercise exercise)
    {
        Exercises.Remove(exercise);
    }
}
