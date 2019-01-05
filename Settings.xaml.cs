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
            NextDayStartTextBox.Text = Setting.Get("NextDayStart");

            // Set initial AdvanceSchedule value
            AdvanceScheduleTextBox.Text = Setting.Get("AdvanceSchedule");
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            // Make Time
            Time time = NextDayStartTextBox.Text;

            // Return if bad format
            if (time == null) return;

            // Check if int
            int advanceSchedule;
            try
            {
                advanceSchedule = int.Parse(AdvanceScheduleTextBox.Text);
            }
            catch (Exception)
            {
                App.ErrorMessage("Days in advance should be a number.");
                return;
            }

            // Change the Setting
            Setting.AddOrChange("NextDayStart", time);
            Setting.AddOrChange("AdvanceSchedule", AdvanceScheduleTextBox.Text);
            App.NextDayStart = time;
            App.AdvanceSchedule = advanceSchedule;
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
            App.Log("Old days cleared");
        }
    }
}