using System.Diagnostics;

namespace WorkoutApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            Debug.WriteLine("App constructor called.");
        }
    }
}
