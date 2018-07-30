using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AudioScheduler.Model;
using Microsoft.EntityFrameworkCore;

namespace AudioScheduler
{
    public partial class App
    {
        public const string DatabaseFile = "data.db";
        public const string SoundDirectory = @".\sounds\";
        public const string NextDayTime = "02:00";
        public static readonly int NextDayTimeInt = int.Parse(NextDayTime.Remove(2, 1));

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

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
                    "Sounds directory not found. It will be created in the same directory as this executable. The directory of sounds may have deleted or misplaced.");
                Directory.CreateDirectory(SoundDirectory);
            }

            // Delete Sounds with missing files
//            else
//            {
//                foreach (var sound in Sound.Fetch())
//                {
//                    if (File.Exists(sound.FilePath)) continue;
//                    InfoMessage("Startup Warning",
//                        $"Audio file for sound {sound.Name} ({sound.FilePath}) could not be found. This sound will be deleted from the database");
//                    Sound.Remove(sound.Id);
//                }
//            }

            // Start the Scheduler as a Task
            Task.Factory.StartNew(Scheduler);

            // Open main window
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }


        #region Static Helpers

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

        // Runs once a minute and checks if a Sound should play
        private static void Scheduler()
        {
            while (true)
            {
                // Calculate ms until next minute
                var now = DateTime.Now.TimeOfDay;
                var nextMin = TimeSpan.FromMinutes(Math.Ceiling(now.TotalMinutes));
                var wait = (nextMin - now).TotalMilliseconds;

                // Wait
                Thread.Sleep((int) wait);

                // Get the Sound
                var sound = Day.CurrentSound(DateTime.Now);

                // Play Sound if one is found
                if (sound != null) AudioController.PlaySound(sound);
            }

            // ReSharper disable once FunctionNeverReturns
        }

        #endregion
    }
}