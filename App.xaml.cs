using System;
using System.IO;
using System.Windows;
using AudioScheduler.Model;
using Microsoft.EntityFrameworkCore;

namespace AudioScheduler
{
    public partial class App
    {
        private static readonly string BaseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"AudioScheduler\");
        public static readonly string DatabaseFile = Path.Combine(BaseDirectory, "data.db");
        public static readonly string SoundDirectory = Path.Combine(BaseDirectory, @"sounds\");
        public static Time NextDayStart;
        public static readonly AudioController AudioController = new AudioController();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!Directory.Exists(BaseDirectory)) Directory.CreateDirectory(BaseDirectory);
            
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


            // Create Sounds directory if it doesn't exist
            if (!Directory.Exists(SoundDirectory))
            {
                InfoMessage("Startup Warning",
                    $"Sounds directory not found. It will be created at {SoundDirectory}. " +
                    "The directory of sounds may have deleted or misplaced or this is the first time using the program on this computer.");
                Directory.CreateDirectory(SoundDirectory);
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

        // Display a message
        public static void InfoMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}