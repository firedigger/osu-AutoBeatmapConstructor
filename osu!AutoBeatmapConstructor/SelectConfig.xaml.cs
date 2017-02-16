using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace osu_AutoBeatmapConstructor
{
    /// <summary>
    /// Логика взаимодействия для SelectConfig.xaml
    /// </summary>
    public partial class SelectConfig : Window
    {
        public ObservableCollection<PatternConfiguration> Patterns { get; set; }
        public PatternConfiguration selected;

        public SelectConfig(ConfigStorage configStorage)
        {
            Patterns = new ObservableCollection<PatternConfiguration>(configStorage.configs);
            this.DataContext = this;
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (configsListView.SelectedIndex == -1)
            {
                MessageBox.Show("No config selected");
                return;
            }

            DialogResult = true;

            selected = Patterns[configsListView.SelectedIndex];

            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
