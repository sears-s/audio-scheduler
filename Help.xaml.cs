namespace AudioScheduler
{
    public partial class Help
    {
        public Help()
        {
            InitializeComponent();

            // Set help text
            TextBlock.Text =
                $"The database file is located at {App.DatabaseFile}\n\n" +
                $"Added sounds are copied and saved in {App.SoundDirectory}\n\n" +
                ".mp3 and .wav files are supported.\n\n" +
                "When editing a template or schedule, double click the last row to add a new event, and to delete an event, with a row selected, press the Delete key.\n\n" +
                "If you play a sound while another is playing, the first one will stop and the second will begin playing.\n\n" +
                "The time is based off of the computer's clock.\n\n" +
                "Sears Schulz '20";
        }
    }
}