using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AudioScheduler.Model
{
    public class Setting
    {
        [Key] public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Value { get; set; }

        // Returns the value for a given Setting name
        public static string Get(string name)
        {
            // Declare the Setting
            Setting setting;

            // Get the Setting
            using (var db = new Context())
            {
                setting = db.Settings.FirstOrDefault(o => o.Name == name);
            }

            // Return the Setting
            return setting?.Value;
        }

        // Changes a Setting to a given value or adds it if it doesn't exist
        public static void AddOrChange(string name, string value)
        {
            using (var db = new Context())
            {
                // If the Setting exists, change it
                if (db.Settings.Any(o => o.Name == name))
                {
                    var setting = db.Settings.FirstOrDefault(o => o.Name == name);
                    if (setting != null)
                    {
                        setting.Value = value;
                    }
                    else
                    {
                        App.ErrorMessage($"Error finding setting named {name}.");
                        return;
                    }
                }

                // Otherwise add it
                else
                {
                    var setting = new Setting
                    {
                        Name = name,
                        Value = value
                    };
                    db.Settings.Add(setting);
                }

                // Save the changes
                db.SaveChanges();
            }
        }
    }
}