using System.Windows;

namespace AudioScheduler
{
    public partial class Input
    {
        public Input(string title, string text)
        {
            InitializeComponent();

            // Set title, TextBox text, and initialize Result
            Result = null;
            Title = title;
            TextBlock.Text = text;
        }

        public string Result { get; set; }

        private void Submit(object sender, RoutedEventArgs e)
        {
            // Set the Result and close
            Result = TextBox.Text;
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            // Remove Result and close
            Result = null;
            Close();
        }
    }
}