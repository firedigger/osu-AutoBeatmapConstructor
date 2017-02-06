using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для InitialSettingsWindow.xaml
    /// </summary>
    public partial class InitialSettingsWindow : Window
    {
        public InitialSettingsWindow()
        {
            InitializeComponent();
        }

        private void OKbutton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void firstTimingChecked(object sender, RoutedEventArgs e)
        {
            beginOffsetTextbox.Visibility = Visibility.Hidden;
        }

        private void firstTimingUnchecked(object sender, RoutedEventArgs e)
        {
            beginOffsetTextbox.Visibility = Visibility.Visible;
        }

        private void lastObjectChecked(object sender, RoutedEventArgs e)
        {
            endOffsetTextbox.Visibility = Visibility.Hidden;
        }

        private void lastObjectUnchecked(object sender, RoutedEventArgs e)
        {
            endOffsetTextbox.Visibility = Visibility.Visible;
        }
    }
}
