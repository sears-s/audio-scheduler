using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AudioScheduler.Model
{
    public class Day
    {
        [Key] public int Id { get; set; }

        [Required] public DateTime Date { get; private set; }

        public virtual ICollection<Event> Events { get; set; }

        // Add given Day if it doesn't exist
        public static void AddIfNotExists(DateTime date)
        {
            // Strip time
            date = date.Date;

            using (var db = new Context())
            {
                // Check if date is duplicate
                if (db.Days.Any(o => o.Date.Equals(date))) return;

                // Create day object
                var newDay = new Day
                {
                    Date = date
                };

                // Add newDay to database
                db.Days.Add(newDay);
                db.SaveChanges();
            }
        }

        // Returns list of Days with at least one Event
        public static IEnumerable<DateTime> AllWithEvent()
        {
            using (var db = new Context())
            {
                return db.Days.Where(o => o.Events.Count > 0).Select(o => o.Date).ToList();
            }
        }

        // Play Sound for current date and time
        public static void PlayCurrent()
        {
            // Get current date and time
            var today = Today();
            Time now = DateTime.Now.ToString("HH:mm");

            // Declare the Sound
            Sound sound;

            // Get the sound
            using (var db = new Context())
            {
                sound = db.Days.Include("Events.Sound")
                    .FirstOrDefault(o => o.Date.Equals(today))?.Events
                    .FirstOrDefault(o => o.Time == now)?.Sound;
            }

            // Play Sound if not null
            if (sound == null) return;
            App.AudioController.PlaySound(sound);
            App.Log($"Sound played from schedule with name '{sound.Name}'");
        }

        // Returns current date using next day setting
        public static DateTime Today()
        {
            var today = DateTime.Now.Date;
            Time now = DateTime.Now.ToString("HH:mm");
            if (now.NextDay) today = today.AddDays(-1);
            return today;
        }
    }
}