using System.Windows;
using AudioScheduler.Model;

namespace AudioScheduler.Sounds
{
    public partial class AddTts
    {
        public AddTts()
        {
            InitializeComponent();

            // Populate voices drop down
            foreach (var v in App.AudioController.GetVoices()) Voices.Items.Add(v);

            // Select first item
            Voices.SelectedIndex = 0;
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Test(object sender, RoutedEventArgs e)
        {
            // Play the test TTS
            App.AudioController.PlayTts(TextBox.Text, Voices.SelectedItem.ToString());
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            // Open input window
            var inputWindow = new Input("Add TTS Sound", "Choose a name this sound:")
            {
                Owner = this
            };
            inputWindow.ShowDialog();

            // Exit if canceled
            if (inputWindow.Result == null) return;

            // Add the Sound
            Sound.AddTts(inputWindow.Result, TextBox.Text, Voices.SelectedItem.ToString());

            Close();
        }
    }
}