using System.Timers;

namespace workoutTimer;

public partial class MainPage : ContentPage
{
    System.Timers.Timer timer;
    TimeSpan exerciseTime;

    public MainPage()
    {
        InitializeComponent();
        timer = new System.Timers.Timer(1000);
        timer.Elapsed += Timer_Elapsed;

        //TEST INPUT FOR TIMER
        exerciseTime = new TimeSpan(0, 0, 30); // 30 
    }

    private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        exerciseTime = exerciseTime.Subtract(TimeSpan.FromSeconds(1));

        await Device.InvokeOnMainThreadAsync(() =>
        {
            timerL.Text = exerciseTime.ToString(@"hh\:mm\:ss");
        });

        if (exerciseTime.TotalSeconds <= 0)
        {
            timer.Stop();
            await Device.InvokeOnMainThreadAsync(() =>
            {
                exerciseL.Text = "WORKOUT FINISHED";
                timerL.Text = "00:00:00";
            });
        }
    }


    private void Start_Click(object sender, EventArgs e)
    {
        exerciseL.Text = "WORKOUT STARTED";
        timer.Start();
    }

    private void Pause_Click(object sender, EventArgs e)
    {
        exerciseL.Text = "WORKOUT PAUSED";
        timer.Stop();
    }

    private void Stop_Click(object sender, EventArgs e)
    {
        timer.Stop();
        exerciseTime = new TimeSpan(0, 0, 30); // 30 
        exerciseL.Text = "WORKOUT FINISHED";
        timerL.Text = "00:00:00";
    }
}


