using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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

            try
            {
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
            catch (Exception e)
            {
                App.ErrorMessage($"Error adding date {date.ToShortDateString()} to database.", e);
            }
        }

        public static IEnumerable<DateTime> AllWithEvent()
        {
            try
            {
                using (var db = new Context())
                {
                    return db.Days.Where(o => o.Events.Count > 0).Select(o => o.Date).ToList();
                }
            }
            catch (Exception e)
            {
                App.ErrorMessage("Error getting dates from database.", e);
                return null;
            }
        }

        // Play Sound for current date and time
        public static void PlayCurrent()
        {
            // Get current date and time
            var today = DateTime.Now.Date;
            Time now = DateTime.Now.ToString("HH:mm");
            if (now.NextDay) today = today.AddDays(-1);
            Debug.WriteLine("CHECKING TIME: " + now);

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
            if (sound != null) App.AudioController.PlaySound(sound);
        }
    }
}