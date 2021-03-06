﻿using System;
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
        private readonly CollectionViewSource _eventViewSource;
        private Day _day;
        private Context _db;

        public EditAll()
        {
            InitializeComponent();

            // Load Events
            _eventViewSource = (CollectionViewSource) FindResource("EventViewSource");
            Calendar.SelectedDate = Day.Today();
            UpdateHighlightedDates();
        }

        private void UpdateEvents()
        {
            // Set Context
            _db = new Context();

            // Load Sounds
            Sound.ItemsSource = Model.Sound.Fetch(_db);

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
            _db.Entry(_day).Collection(o => o.Events).Query().OrderBy(o => o.Time).Load();
            _eventViewSource.Source = _db.Events.Local.ToObservableCollection();
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
            // Return if no Day
            if (_day == null) return;

            // Remove Events with no Sound or Time and add new ones
            foreach (var ev in _db.Events.Local.ToList())
                if (ev.Sound == null || ev.Time == null)
                {
                    _db.Events.Remove(ev);
                    App.Log($"Event removed from schedule with date '{_day.Date.ToShortDateString()}'");
                }
                else if (_day.Events.All(o => o.Id != ev.Id))
                {
                    _day.Events.Add(ev);
                    App.Log(
                        $"Event added to schedule with date '{_day.Date.ToShortDateString()}' and sound '{ev.Sound.Name}' and time '{ev.TimeString}'");
                }

            // Save changes
            _db.SaveChanges();
            UpdateHighlightedDates();
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            _db.Events.Local.Clear();
            App.Log($"Clear button pressed for schedule with date '{_day.Date.ToShortDateString()}'");
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

            App.Log(
                $"Template imported to schedule with date '{_day.Date.ToShortDateString()}' and template name '{templateName}'");
        }

        private void DateChanged(object sender, SelectionChangedEventArgs e)
        {
            Save();
            UpdateEvents();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            Save();
        }
    }
}