using System.Linq;
using System.Windows;
using System.Windows.Data;
using AudioScheduler.Model;

namespace AudioScheduler.Templates
{
    public partial class EditOne
    {
        private readonly Context _db = new Context();
        private readonly Template _template;

        public EditOne(int templateId)
        {
            InitializeComponent();

            // Find Template with id
            _template = _db.Templates.Find(templateId);
            if (_template == null)
            {
                App.ErrorMessage($"Could not find template with ID #{templateId} in database.");
                Close();
                return;
            }

            // Change window title
            Title = $"Edit Template {_template.Name}";

            // Load Template Events from database
            _db.Entry(_template).Collection(o => o.Events).Load();
            var eventViewSource = (CollectionViewSource) FindResource("EventViewSource");
            eventViewSource.Source = _db.Events.Local.ToObservableCollection();

            // Load list of Sounds in same Context
            Sound.ItemsSource = Model.Sound.Fetch(_db);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            // Remove Events with no Sound or Time and add new ones
            foreach (var ev in _db.Events.Local.ToList())
                if (ev.Sound == null || ev.Time == null)
                {
                    _db.Events.Remove(ev);
                    App.Log($"Event removed from template with name '{_template.Name}'");
                }
                else if (_template.Events.All(o => o.Id != ev.Id))
                {
                    _template.Events.Add(ev);
                    App.Log(
                        $"Event added to template with name '{_template.Name}' and sound '{ev.Sound.Name}' and time '{ev.TimeString}'");
                }

            // Save changes
            _db.SaveChanges();
            Close();
        }

        private void Import(object sender, RoutedEventArgs e)
        {
            // Open the window
            var importOneWindow = new ImportOne
            {
                Owner = this
            };
            importOneWindow.ShowDialog();

            // Return if no Template selected
            if (importOneWindow.TemplateId == -1) return;

            // Add the Events
            var templateName = _db.Templates.Find(importOneWindow.TemplateId).Name;
            foreach (var ev in Model.Template.FetchEvents(importOneWindow.TemplateId))
            {
                var newEvent = new Event
                {
                    Sound = ev.Sound,
                    Time = ev.Time
                };
                _db.Events.Local.Add(newEvent);
            }

            App.Log($"Template imported to template with name '{_template.Name}' and template name '{templateName}'");
        }
    }
}