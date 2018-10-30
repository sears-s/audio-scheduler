using System;
using System.Linq;
using System.Windows;
using AudioScheduler.Model;
using Microsoft.EntityFrameworkCore;

namespace AudioScheduler
{
    public partial class Settings
    {
        public Settings()
        {
            InitializeComponent();

            // Set initial NextDayStart value
            TextBox.Text = Setting.Get("NextDayStart");
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            // Make Time
            Time time = TextBox.Text;

            // Return if bad format
            if (time == null) return;

            // Change the Setting
            Setting.AddOrChange("NextDayStart", time);
            App.NextDayStart = time;
            Close();
        }

        private void ClearDays(object sender, RoutedEventArgs e)
        {
            // Get date for two days ago
            var yesterday = DateTime.Today.AddDays(-2);

            using (var db = new Context())
            {
                // Get the old Days
                var oldDays = db.Days.Where(o => o.Date < yesterday).Include("Events");
                foreach (var day in oldDays)
                {
                    // Delete the Events
                    foreach (var dayEvent in day.Events) db.Events.Remove(dayEvent);

                    // Delete the Day
                    db.Days.Remove(day);
                }

                // Save the changes
                db.SaveChanges();
            }

            App.InfoMessage("Success", "Old days cleared.");
        }
    }
}