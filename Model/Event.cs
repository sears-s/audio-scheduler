using System.ComponentModel.DataAnnotations;

namespace AudioScheduler.Model
{
    public class Event
    {
        [Key] public int Id { get; set; }

        [Required] public virtual Sound Sound { get; set; }

        [Required] public string Time { get; set; }

        public int SortTime
        {
            get
            {
                var nextDayTime = int.Parse(App.NextDayTime.Remove(2, 1));
                if (App.FormatTime(Time) == null) return nextDayTime + 2400;

                var time = int.Parse(Time.Remove(2, 1));
                if (time < nextDayTime) time += 2400;
                return time;
            }
        }
    }
}