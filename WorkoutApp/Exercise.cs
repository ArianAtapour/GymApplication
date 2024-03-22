public class Exercise
{
    public string Name { get; set; }
    public TimeSpan Duration { get; set; }

    public Exercise(string name, TimeSpan duration)
    {
        Name = name;
        Duration = duration;
    }
}