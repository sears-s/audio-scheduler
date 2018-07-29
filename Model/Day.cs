using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
                App.ErrorMessage("Error adding date to database.", e);
            }
        }

        public static List<DateTime> AllWithEvent()
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

        // Returns Sound Id of what to play for current minute
        public static Sound CurrentSound(DateTime dateTime)
        {
            // Declare the event
            Event e;

            // Get the sound
            using (var db = new Context())
            {
                // Get the Day
                var day = db.Days.FirstOrDefault(o => o.Date.Equals(dateTime.Date));

                // Return null if no Day found
                if (day?.Events == null) return null;

                // Get the Event
                e = day.Events.FirstOrDefault(o => o.Time == dateTime.ToString("HH:mm"));
            }

            // Return null if Sound not found
            return e?.Sound;
        }
    }
}