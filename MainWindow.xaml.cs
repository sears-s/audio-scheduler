using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
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
            contextMenu.MenuItems.Add("Quit", (sender, args) =>
            {
                App.Log("Application quit from tray");
                Application.Current.Shutdown();
            });
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
                App.Log("Main window restored from tray");
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
            App.Log("Main window hidden to tray");
        }

        public void UpdateEvents()
        {
            // Set Context
            _db = new Context();

            // Load Sounds
            UpdateSounds();

            // Get today's date
            var today = Day.Today();

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

            // Update status
            if (StatusCheck())
            {
                StatusText.Text = "Schedule is Good";
                StatusText.Foreground = new BrushConverter().ConvertFromString("Green") as SolidColorBrush;
            }
            else
            {
                StatusText.Text = $"WARNING: Schedule should be completed {App.AdvanceSchedule} days in advance";
                StatusText.Foreground = new BrushConverter().ConvertFromString("Red") as SolidColorBrush;
            }
        }

        private bool StatusCheck()
        {
            // Set Context
            _db = new Context();

            // Get today's date
            var dateToCheck = Day.Today();

            for (var i = 0; i < App.AdvanceSchedule; i++)
            {
                // Find the Day
                var day = _db.Days.Include(o => o.Events).FirstOrDefault(o => o.Date.Equals(dateToCheck));
                if (day == null) return false;

                // Check if any Events
                if (day.Events.Count < 1) return false;

                // Increment the date
                dateToCheck = dateToCheck.AddDays(1);
            }

            // Check was good
            return true;
        }

        private void UpdateSounds()
        {
            // Save currently selected Sound
            var index = -1;
            if (SoundCb.Items.Count > 0 && SoundCb.SelectedItem != null)
                index = SoundCb.Items.IndexOf((Sound) SoundCb.SelectedItem);

            // Populate the drop down for playing Sounds
            SoundCb.ItemsSource = Sound.Fetch();

            // Reload currently selected Sound
            if (index != -1) SoundCb.SelectedItem = SoundCb.Items[index];

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
            UpdateEvents();
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
            // Check if no Sound selected
            if (SoundCb.SelectedItem == null)
            {
                App.InfoMessage("No Sound", "No sound was selected to be played.");
                return;
            }

            App.AudioController.PlaySound((Sound) SoundCb.SelectedItem);
            App.Log($"Sound played from main window with name '{((Sound) SoundCb.SelectedItem).Name}'");
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
            App.Log("Stop playing button pressed on main window");
        }
    }
}