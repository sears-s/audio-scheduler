using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using AudioScheduler.Days;
using AudioScheduler.Model;
using Microsoft.EntityFrameworkCore;
using Application = System.Windows.Application;
using Day = AudioScheduler.Model.Day;

namespace AudioScheduler
{
    public partial class MainWindow
    {
        private readonly CollectionViewSource _eventViewSource;
        private Context _db;

        public MainWindow()
        {
            InitializeComponent();

            // Setup minimize to tray functionality
            var contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add("Quit", (sender, args) => Application.Current.Shutdown());
            var notifyIcon = new NotifyIcon
            {
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath),
                Visible = true,
                ContextMenu = contextMenu
            };
            notifyIcon.DoubleClick += delegate
            {
                Show();
                WindowState = WindowState.Normal;
            };

            // Set playing DataContext
            TextBlock.DataContext = App.AudioController;

            // Load Events
            _eventViewSource = (CollectionViewSource) FindResource("EventViewSource");
            UpdateEvents();
        }

        // Minimize to tray when closed
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            OnClosed(e);
        }

        public void UpdateEvents()
        {
            // Set Context
            _db = new Context();

            // Load Sounds
            UpdateSounds();

            // Get today's date
            var today = DateTime.Today;

            // Add if needed
            Day.AddIfNotExists(today);

            // Find the Day
            var day = _db.Days.FirstOrDefault(o => o.Date.Equals(today));
            if (day == null)
            {
                App.ErrorMessage($"Could not find day with date {today.ToShortDateString()} in database.");
                return;
            }

            // Load Events
            _db.Entry(day).Collection(o => o.Events).Query().OrderBy(o => o.Time).Load();
            _eventViewSource.Source = _db.Events.Local.ToObservableCollection();
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
            App.AudioController.PlaySound((Sound) SoundCb.SelectedItem);
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
            App.AudioController.Stop();
        }

        private void Reveille(object sender, RoutedEventArgs e)
        {
            App.AudioController.PlaySound(_db.Sounds.FirstOrDefault(o => o.Name == "Reveille"));
        }

        private void ToTheColors(object sender, RoutedEventArgs e)
        {
            App.AudioController.PlaySound(_db.Sounds.FirstOrDefault(o => o.Name == "To the Colors"));
        }

        private void Retreat(object sender, RoutedEventArgs e)
        {
            App.AudioController.PlaySound(_db.Sounds.FirstOrDefault(o => o.Name == "Retreat"));
        }

        private void NationalAnthem(object sender, RoutedEventArgs e)
        {
            App.AudioController.PlaySound(_db.Sounds.FirstOrDefault(o => o.Name == "National Anthem"));
        }

        private void Taps(object sender, RoutedEventArgs e)
        {
            App.AudioController.PlaySound(_db.Sounds.FirstOrDefault(o => o.Name == "TAPS"));
        }
    }
}