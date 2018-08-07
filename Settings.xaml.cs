using System.Windows;
using AudioScheduler.Model;

namespace AudioScheduler
{
    public partial class Settings
    {
        public Settings()
        {
            InitializeComponent();

            // Set initial NextDayStart value
            TextBox.Text = Setting.Get("NextDayStart");
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            // Make Time
            Time time = TextBox.Text;
            
            // Return if bad format
            if (time == null) return;
            
            // Change the Setting
            Setting.AddOrChange("NextDayStart", time);
            App.NextDayStart = time;
            Close();
        }
    }
}