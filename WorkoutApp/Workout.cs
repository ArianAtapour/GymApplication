using System.Collections;

class Workout
{
    public string WorkoutName { get; set; }
    public TimeSpan TotalDuration
    {
        get
        {
            TimeSpan totalDuration = TimeSpan.Zero;
            foreach (Exercise exercise in exercises)
            {
                totalDuration += exercise.Duration;
            }
            return totalDuration;
        }
    }
    private ArrayList exercises;

    public Workout(string workoutName)
    {
        WorkoutName = workoutName;
        exercises = new ArrayList();
    }

    public void AddExercise(Exercise exercise)
    {
        exercises.Add(exercise);
    }

    public void RemoveExercise(Exercise exercise)
    {
        exercises.Remove(exercise);
    }
}