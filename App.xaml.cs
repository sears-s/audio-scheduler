using System;
using System.IO;
using System.Threading;
using System.Windows;
using AudioScheduler.Model;
using Microsoft.EntityFrameworkCore;

namespace AudioScheduler
{
    public partial class App
    {
        private static readonly string BaseDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"AudioScheduler\");

        public static readonly string DatabaseFile = Path.Combine(BaseDirectory, "data.db");
        public static readonly string SoundDirectory = Path.Combine(BaseDirectory, @"sounds\");
        public static Time NextDayStart;
        public static readonly AudioController AudioController = new AudioController();

        [STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {
            // Check if duplicate instance
            using (var mutex = new Mutex(false, "Global\\83a4c0e1-eb14-483f-8612-a41ef86048ae"))
            {
                if (!mutex.WaitOne(0, false))
                {
                    ErrorMessage("Audio Scheduler already running. Exiting this duplicate instance.");
                    Current.Shutdown();
                }
            }
            
            base.OnStartup(e);

            // Create BaseDirectory
            if (!Directory.Exists(BaseDirectory))
            {
                InfoMessage("Startup Warning",
                    $"Data folder not found. It will be created at {BaseDirectory}. " +
                    "The data folder may have deleted or misplaced or this is the first time using the program on this computer.");
                Directory.CreateDirectory(BaseDirectory);
            }

            // Create SoundDirectory
            if (!Directory.Exists(SoundDirectory)) Directory.CreateDirectory(SoundDirectory);

            // Set default Settings
            var nextDayStartSetting = Setting.Get("NextDayStart");
            if (nextDayStartSetting == null)
            {
                Setting.AddOrChange("NextDayStart", "02:00");
                nextDayStartSetting = Setting.Get("NextDayStart");
            }

            // Get Settings
            NextDayStart = nextDayStartSetting;

            // Test database connection
            try
            {
                using (var db = new Context())
                {
                    db.Database.OpenConnection();
                    db.Database.CloseConnection();
                }
            }
            catch (Exception error)
            {
                ErrorMessage("Database connection error. Exiting.", error);
                Current.Shutdown();
            }

            // Start the Scheduler
            Scheduler.Start();

            // Open main window
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        // Display an error message
        public static void ErrorMessage(string message, Exception e = null)
        {
            var show = message;
            if (e != null) show = $"{message}\n{e.Message}";
            MessageBox.Show(show, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // Display an info message
        public static void InfoMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}