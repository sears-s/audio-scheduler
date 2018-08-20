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
            var yesterday = DateTime.Today.AddDays(-1);
            using (var db = new Context())
            {
                var oldDays = db.Days.Where(o => o.Date < yesterday).Include("Events");
                foreach (var day in oldDays)
                {
                    foreach (var dayEvent in day.Events)
                    {
                        db.Events.Remove(dayEvent);
                    }

                    db.Days.Remove(day);
                }
            }
            
            App.InfoMessage("Success", "Old days cleared.");
        }
    }
}