﻿namespace AudioScheduler
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
                "To use the Reveille, To the Colors, Retreat, National Anthem, and TAPS buttons, you must add a sound with the exact same name as the button.\n\n" +
                "When you close the window, the program will not quit. You can reopen the program by double clicking the task bar icon. You can the quit the program by right clicking the task bar icon and clicking Quit.\n\n" +
                "Sears Schulz '20 - ***REMOVED***\n" +
                "https://github.com/sears-s/audio-scheduler";
        }
    }
}