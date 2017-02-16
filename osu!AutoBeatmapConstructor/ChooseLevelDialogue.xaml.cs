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
    /// Логика взаимодействия для ChooseLevelDialogue.xaml
    /// </summary>
    public partial class ChooseLevelDialogue : Window
    {
        public int level;
        public int count;
        public int phase;

        public ChooseLevelDialogue()
        {
            InitializeComponent();
        }

        private void OKbutton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            try
            {
                level = comboBox.SelectedIndex + 1;
                count = int.Parse(numberOfPatternsTextbox.Text);
                phase = int.Parse(breakyEveryPatternTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR! Incorrent input\n" + ex.ToString());
            }
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}