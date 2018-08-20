using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AudioScheduler.Model
{
    public class Template
    {
        [Key] public int Id { get; set; }

        [Required] public string Name { get; private set; }

        public virtual ICollection<Event> Events { get; set; }

        // Add Template
        public static void Add(string name)
        {
            using (var db = new Context())
            {
                // Check if name is duplicate
                if (db.Templates.Any(o => o.Name.Equals(name)))
                {
                    App.ErrorMessage($"Template {name} already exists.");
                    return;
                }

                // Create Template
                var newTemplate = new Template
                {
                    Name = name
                };

                // Add Template to database
                db.Templates.Add(newTemplate);
                db.SaveChanges();
            }
        }

        // Remove Template
        public static void Remove(int id)
        {
            using (var db = new Context())
            {
                // Find Template with id
                var template = db.Templates.Find(id);
                if (template == null)
                {
                    App.ErrorMessage($"Could not find template with ID #{id} in database.");
                    return;
                }

                // Delete Template and its Events from database
                db.Events.RemoveRange(template.Events);
                db.Templates.Remove(template);
                db.SaveChanges();
            }
        }

        // Rename Template
        public static void Rename(int id, string name)
        {
            using (var db = new Context())
            {
                // Check if name is duplicate
                if (db.Templates.Any(o => o.Name.Equals(name)))
                {
                    App.ErrorMessage($"Template {name} already exists.");
                    return;
                }

                // Find Template with id
                var template = db.Templates.Find(id);
                if (template == null)
                {
                    App.ErrorMessage($"Could not find template with ID #{id} in database.");
                    return;
                }

                // Rename in database
                template.Name = name;
                db.SaveChanges();
            }
        }

        // Returns all Templates, sorted alphabetically
        public static IEnumerable<Template> Fetch()
        {
            using (var db = new Context())
            {
                return db.Templates.OrderBy(o => o.Name).ToList();
            }
        }

        // Returns all Events for a Template
        public static IEnumerable<Event> FetchEvents(int id, Context db = null)
        {
            // Declare the Template
            Template template;

            if (db == null)
                using (db = new Context())
                {
                    // Find Template with id
                    template = db.Templates.Include("Events.Sound").FirstOrDefault(o => o.Id == id);

                    // Return Events
                    if (template != null) return template.Events.ToList();

                    // If Template not found
                    App.ErrorMessage($"Could not find template with ID #{id} in database.");
                    return null;
                }

            // Find Template with id
            template = db.Templates.Include("Events.Sound").FirstOrDefault(o => o.Id == id);

            // Return Events
            if (template != null) return template.Events.ToList();

            // If Template not found
            App.ErrorMessage($"Could not find template with ID #{id} in database.");
            return null;
        }
    }
}