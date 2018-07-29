using System.IO;
using System.Windows;
using System.Windows.Controls;
using AudioScheduler.Model;
using Microsoft.Win32;

namespace AudioScheduler.Sounds
{
    public partial class EditAll
    {
        public EditAll()
        {
            InitializeComponent();

            // Populate ListView
            UpdateList();
        }

        private void UpdateList()
        {
            // Clear the list
            Sounds.Items.Clear();

            // Get the list
            var sounds = Sound.Fetch();

            // Exit if fetch failed
            if (sounds == null) return;

            // Populate the list
            foreach (var sound in sounds)
            {
                var item = new ListViewItem
                {
                    Content = sound.Name,
                    Tag = sound.Id
                };
                Sounds.Items.Add(item);
            }
        }

        private void AddFile(object sender, RoutedEventArgs e)
        {
            // Open file dialog
            var dlg = new OpenFileDialog
            {
                Filter = "Audio Files (*.mp3, *.wav)| *.mp3; *.wav"
            };
            var result = dlg.ShowDialog();

            // Exit if canceled
            if (result != true) return;

            // Open input window
            var inputWindow = new Input("Add Sound", $"Choose a name for {Path.GetFileName(dlg.FileName)}:")
            {
                Owner = this
            };
            inputWindow.ShowDialog();

            // Exit if canceled
            if (inputWindow.Result == null) return;

            // Add the Sound
            Sound.AddFile(inputWindow.Result, dlg.FileName);

            // Update the ListView
            UpdateList();
        }

        private void AddTts(object sender, RoutedEventArgs e)
        {
            // Open add TTS dialog
            var addTtsWindow = new AddTts
            {
                Owner = this
            };
            addTtsWindow.ShowDialog();

            // Update the ListView
            UpdateList();
        }

        private void DeleteSound(object sender, RoutedEventArgs e)
        {
            // Do nothing if nothing selected
            if (Sounds.SelectedItem == null) return;

            // Confirm then delete
            if (MessageBox.Show($"Are you sure you want to delete {((ListViewItem) Sounds.SelectedValue).Content}?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                Sound.Remove((int) ((ListViewItem) Sounds.SelectedValue).Tag);

            // Update the ListView
            UpdateList();
        }

        private void Rename(object sender, RoutedEventArgs e)
        {
            // Do nothing if nothing selected
            if (Sounds.SelectedItem == null) return;

            // Open input window
            var inputWindow = new Input("Rename Sound",
                $"What would you like to rename {((ListViewItem) Sounds.SelectedValue).Content} to?")
            {
                Owner = this
            };
            inputWindow.ShowDialog();

            // Exit if canceled
            if (inputWindow.Result == null) return;

            // Rename the Sound
            Sound.Rename((int) ((ListViewItem) Sounds.SelectedValue).Tag, inputWindow.Result);

            // Update the ListView
            UpdateList();
        }
    }
}