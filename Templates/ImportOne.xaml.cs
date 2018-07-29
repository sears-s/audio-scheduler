using System.Windows;
using System.Windows.Controls;

namespace AudioScheduler.Templates
{
    public partial class ImportOne
    {
        public ImportOne()
        {
            InitializeComponent();

            // Initialize TemplateId
            TemplateId = -1;

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

        public int TemplateId { get; set; }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Import(object sender, RoutedEventArgs e)
        {
            // Do nothing if nothing selected
            if (Templates.SelectedItem == null) return;

            // Set TemplateId
            TemplateId = (int) ((ListViewItem) Templates.SelectedValue).Tag;
            Close();
        }
    }
}