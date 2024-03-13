class Exercise
{
    private string name;
    private TimeSpan duration;

    public Exercise(string name, TimeSpan duration)
    {
        Name = name;
        Duration = duration;
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public TimeSpan Duration
    {
        get { return duration; }
        set { duration = value; }
    }
}