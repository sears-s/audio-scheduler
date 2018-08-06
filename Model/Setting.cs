using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AudioScheduler.Model
{
    public class Setting
    {
        [Key] public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Value { get; set; }

        public static string Get(string name)
        {
            Setting setting;

            using (var db = new Context())
            {
                setting = db.Settings.FirstOrDefault(o => o.Name == name);
            }

            return setting?.Value;
        }

        public static void AddOrChange(string name, string value)
        {
            using (var db = new Context())
            {
                if (db.Settings.Any(o => o.Name == name))
                {
                    var setting = db.Settings.FirstOrDefault(o => o.Name == name);
                    if (setting != null)
                    {
                        setting.Value = value;
                        db.SaveChanges();
                    }
                    else
                    {
                        App.ErrorMessage("Error finding setting.");
                    }
                }
                else
                {
                    var setting = new Setting
                    {
                        Name = name,
                        Value = value
                    };
                    db.Settings.Add(setting);
                    db.SaveChanges();
                }
            }
        }
    }
}