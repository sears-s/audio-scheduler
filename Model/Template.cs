using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AudioScheduler.Model
{
    public class Template
    {
        [Key] public int Id { get; set; }

        [Required] public string Name { get; private set; }

        public virtual ICollection<Event> Events { get; set; }

        // Add Template to database
        public static void Add(string name)
        {
            try
            {
                using (var db = new Context())
                {
                    // Check if name is duplicate
                    if (db.Templates.Any(o => o.Name.Equals(name)))
                    {
                        App.InfoMessage("Duplicate Template", $"Template {name} already exists in template database.");
                        return;
                    }

                    // Create template object
                    var newTemplate = new Template
                    {
                        Name = name
                    };

                    // Add newTemplate to database
                    db.Templates.Add(newTemplate);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                App.ErrorMessage("Error adding template to database.", e);
            }
        }

        // Remove Template from database
        public static void Remove(int id)
        {
            try
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
            catch (Exception e)
            {
                App.ErrorMessage("Error removing template from database.", e);
            }
        }

        // Rename Template in database
        public static void Rename(int id, string name)
        {
            try
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

                    // Rename in database
                    template.Name = name;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                App.ErrorMessage("Error renaming template in database.", e);
            }
        }

        // Return all Templates in database
        public static List<Template> Fetch()
        {
            try
            {
                using (var db = new Context())
                {
                    return db.Templates.ToList();
                }
            }
            catch (Exception e)
            {
                App.ErrorMessage("Error fetching templates from database.", e);
                return null;
            }
        }

        // Return all Events for a Template
        public static List<Event> FetchEvents(int id, Context db = null)
        {
            if (db == null)
                using (db = new Context())
                {
                    // Find Template with id
                    var template = db.Templates.Find(id);
                    if (template == null)
                    {
                        App.ErrorMessage($"Could not find template with ID #{id} in database.");
                        return null;
                    }

                    // Return Events
                    return template.Events.ToList();
                }

            {
                // Find Template with id
                var template = db.Templates.Find(id);
                if (template == null)
                {
                    App.ErrorMessage($"Could not find template with ID #{id} in database.");
                    return null;
                }

                // Return Events
                return template.Events.ToList();
            }
        }
    }
}