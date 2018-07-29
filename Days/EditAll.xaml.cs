using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using AudioScheduler.Model;
using AudioScheduler.Templates;
using Microsoft.EntityFrameworkCore;

namespace AudioScheduler.Days
{
    public partial class EditAll
    {
        private readonly Context _db = new Context();
        private Day _day;

        public EditAll()
        {
            InitializeComponent();

            // Select today
            Calendar.SelectedDate = DateTime.Today;

            // Load Sounds
            Sound.ItemsSource = Model.Sound.Fetch(_db);

            // Load Events
            var eventViewSource = (CollectionViewSource) FindResource("EventViewSource");
            eventViewSource.Source = _db.Events.Local.ToObservableCollection();
            UpdateEvents();
            UpdateHighlightedDates();
        }

        private void UpdateEvents()
        {
            // Clear the Event list
            foreach (var x in _db.Events.Local.ToList())
            {
                _db.Entry(x).State = EntityState.Detached;
                _db.Events.Local.Remove(x);
            }

            // Return if no date selected
            if (Calendar.SelectedDate == null) return;

            // Make Day if it doesn't exist
            var date = (DateTime) Calendar.SelectedDate;
            Day.AddIfNotExists(date);

            // Find the Day
            _day = _db.Days.FirstOrDefault(o => o.Date.Equals(date));
            if (_day == null)
            {
                App.ErrorMessage($"Could not find day with date {date.ToShortDateString()} in database.");
                return;
            }

            // Change window title
            Title = $"Edit Schedule {_day.Date.ToShortDateString()}";

            // Load Day Events from database
            _db.Entry(_day).Collection(o => o.Events).Load();
        }

        private void UpdateHighlightedDates()
        {
            // Create new Style
            var style = new Style
            {
                TargetType = typeof(CalendarDayButton)
            };

            // Add dates to the Style
            foreach (var date in Day.AllWithEvent())
            {
                var dataTrigger = new DataTrigger
                {
                    Binding = new Binding("Date"),
                    Value = date
                };
                dataTrigger.Setters.Add(new Setter(BackgroundProperty, Brushes.Silver));
                style.Triggers.Add(dataTrigger);
            }

            // Change the Style
            Resources["CdbKey"] = style;
        }

        private void Save()
        {
            if (_day == null) return;

            foreach (var ev in _db.Events.Local.ToList())
                // Remove Events with no Sound or Time and add new ones
                if (ev.Sound == null || ev.Time == null)
                    _db.Events.Remove(ev);
                else if (_day.Events.All(o => o.Id != ev.Id)) _day.Events.Add(ev);

            // Save changes
            _db.SaveChanges();
            UpdateHighlightedDates();
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            _db.Events.Local.Clear();
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

        private void DateChanged(object sender, SelectionChangedEventArgs e)
        {
            Save();
            UpdateEvents();
        }

        private void CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Exit if not Time column or cell text is null
            if (!(e.EditingElement is TextBox t) || !Equals(e.Column, Time)) return;

            // Get the formatted time
            var result = App.FormatTime(t.Text);

            // Show error if wrong format, then set to formatted time
            if (result == null)
                App.InfoMessage("Time Format", "Incorrect time format. Should be HHMM, HMM, HH:MM, or H:MM.");
            t.Text = result;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            Save();
        }
    }
}