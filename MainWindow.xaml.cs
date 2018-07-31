using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using AudioScheduler.Days;
using AudioScheduler.Model;
using Microsoft.EntityFrameworkCore;

namespace AudioScheduler
{
    public partial class MainWindow
    {
        private readonly Context _db = new Context();

        public MainWindow()
        {
            InitializeComponent();

            // Load Sounds
            UpdateSounds();

            // Load Events
            var eventViewSource = (CollectionViewSource) FindResource("EventViewSource");
            UpdateEvents();
            eventViewSource.Source = _db.Events.Local.ToObservableCollection();
        }

        private void UpdateEvents()
        {
            // Clear the Event list
            foreach (var x in _db.Events.Local.ToList())
            {
                _db.Entry(x).State = EntityState.Detached;
                _db.Events.Local.Remove(x);
            }

            // Get today's date
            var today = DateTime.Today;

            // Add if needed
            Day.AddIfNotExists(today);

            // Find the Day
            var day = _db.Days.FirstOrDefault(o => o.Date.Equals(today));
            if (day == null)
            {
                App.ErrorMessage(
                    $"Could not find day with date {today.ToString(CultureInfo.InvariantCulture)} in database.");
                return;
            }

            // Load Events
            _db.Entry(day).Collection(o => o.Events).Load();
        }

        private void UpdateSounds()
        {
            // Populate the drop down for playing Sounds
            SoundCb.ItemsSource = Sound.Fetch();

            // Populate the drop down for the DataGrid
            SoundDg.ItemsSource = Sound.Fetch(_db);
        }

        private void OpenEditDays(object sender, RoutedEventArgs e)
        {
            var editDaysWindow = new EditAll
            {
                Owner = this
            };
            editDaysWindow.ShowDialog();

            // Reload Events
            UpdateEvents();
        }

        private void OpenEditSounds(object sender, RoutedEventArgs e)
        {
            var editSoundsWindow = new Sounds.EditAll
            {
                Owner = this
            };
            editSoundsWindow.ShowDialog();

            // Reload Sounds
            UpdateSounds();
        }

        private void OpenEditTemplates(object sender, RoutedEventArgs e)
        {
            var editTemplatesWindow = new Templates.EditAll
            {
                Owner = this
            };
            editTemplatesWindow.ShowDialog();
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Settings
            {
                Owner = this
            };
            settingsWindow.ShowDialog();
        }

        private void OpenHelp(object sender, RoutedEventArgs e)
        {
            var helpWindow = new Help
            {
                Owner = this
            };
            helpWindow.ShowDialog();
        }

        private void PlaySound(object sender, RoutedEventArgs e)
        {
            AudioController.PlaySound((Sound) SoundCb.SelectedItem);
        }

        private void PlayTts(object sender, RoutedEventArgs e)
        {
            var playTtsWindow = new PlayTts
            {
                Owner = this
            };
            playTtsWindow.ShowDialog();
        }

        private void StopSound(object sender, RoutedEventArgs e)
        {
            AudioController.Stop();
        }
    }
}