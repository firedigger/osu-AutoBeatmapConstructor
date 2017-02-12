using BMAPI.v1.Events;
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
    /// Логика взаимодействия для AddBreakDialogue.xaml
    /// </summary>
    public partial class AddBreakDialogue : Window
    {
        public ConfiguredBreak pattern = null;

        public AddBreakDialogue()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKbutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int seconds = int.Parse(numberOfSecondsTextbox.Text);

                pattern = new ConfiguredBreak(seconds);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void randomSeconds_Click(object sender, RoutedEventArgs e)
        {
            numberOfSecondsTextbox.Text = Utils.rng.Next(3,20).ToString();
        }
    }
}
