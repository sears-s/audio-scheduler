using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AudioScheduler.Model
{
    public class Event
    {
        [Key] public int Id { get; set; }

        [Required] public virtual Sound Sound { get; set; }

        [Required]
        [Column("Time")]
        public string TimeString
        {
            get
            {
                if (Time == null) return "";
                return Time;
            }
            set => Time = value;
        }

        [NotMapped] public Time Time { get; set; }
    }
}