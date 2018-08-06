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
                    _db.Events.Remove(ev);
                else if (_template.Events.All(o => o.Id != ev.Id)) _template.Events.Add(ev);

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
            foreach (var ev in Model.Template.FetchEvents(importOneWindow.TemplateId, _db))
            {
                _db.Events.Local.Add(ev);
                _db.Events.Add(ev);
            }
        }
    }
}