using System.Windows;

namespace AudioScheduler
{
    public partial class PlayTts
    {
        public PlayTts()
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

        private void Play(object sender, RoutedEventArgs e)
        {
            // Play the TTS
            App.AudioController.PlayTts(TextBox.Text, Voices.SelectedItem.ToString());
            Close();
        }
    }
}