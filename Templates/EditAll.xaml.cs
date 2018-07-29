using System.Windows;
using System.Windows.Controls;

namespace AudioScheduler.Templates
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
            Templates.Items.Clear();

            // Get the list
            var templates = Model.Template.Fetch();

            // Populate the list
            foreach (var template in templates)
            {
                var item = new ListViewItem
                {
                    Content = template.Name,
                    Tag = template.Id
                };
                Templates.Items.Add(item);
            }
        }

        private void EditTemplate(object sender, RoutedEventArgs e)
        {
            // Do nothing if nothing selected
            if (Templates.SelectedItem == null) return;

            // Open EditOne window
            var editTemplateWindow = new EditOne((int) ((ListViewItem) Templates.SelectedValue).Tag)
            {
                Owner = this
            };
            editTemplateWindow.ShowDialog();

            // Update the ListView
            UpdateList();
        }

        private void RenameTemplate(object sender, RoutedEventArgs e)
        {
            // Do nothing if nothing selected
            if (Templates.SelectedItem == null) return;

            // Open input window
            var inputWindow = new Input("Rename Sound",
                $"What would you like to rename {((ListViewItem) Templates.SelectedValue).Content} to?")
            {
                Owner = this
            };
            inputWindow.ShowDialog();

            // Exit if canceled
            if (inputWindow.Result == null) return;

            // Rename the Sound
            Model.Template.Rename((int) ((ListViewItem) Templates.SelectedValue).Tag, inputWindow.Result);

            // Update the ListView
            UpdateList();
        }

        private void DeleteTemplate(object sender, RoutedEventArgs e)
        {
            // Do nothing if nothing selected
            if (Templates.SelectedItem == null) return;

            // Confirm then delete
            if (MessageBox.Show(
                    $"Are you sure you want to delete template {((ListViewItem) Templates.SelectedValue).Content}?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                Model.Template.Remove((int) ((ListViewItem) Templates.SelectedValue).Tag);

            // Update the ListView
            UpdateList();
        }

        private void AddTemplate(object sender, RoutedEventArgs e)
        {
            // Open input window
            var inputWindow = new Input("Add Template", "Name of new template:")
            {
                Owner = this
            };
            inputWindow.ShowDialog();

            // Exit if canceled
            if (inputWindow.Result == null) return;

            // Add the Sound
            Model.Template.Add(inputWindow.Result);

            // Update the ListView
            UpdateList();
        }
    }
}