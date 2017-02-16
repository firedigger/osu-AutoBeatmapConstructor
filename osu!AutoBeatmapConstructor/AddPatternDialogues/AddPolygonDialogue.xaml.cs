using BMAPI.v1.HitObjects;
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
    /// Логика взаимодействия для AddPolygonDialogue.xaml
    /// </summary>
    public partial class AddPolygonDialogue : Window
    {
        public ConfiguredPolygons pattern = null;

        public AddPolygonDialogue()
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
                int points = int.Parse(numberOfPointsComboBox.Text);

                int number = 0;
                bool end = false;
                if (tillEndCheckbox.IsChecked ?? true)
                    end = true;
                else
                    number = int.Parse(numberOfPatternsTextbox.Text);

                int spacing = (int)spacingSlider.Value;
                int rotation = (int)rotationSlider.Value;
                int shift = (int)shiftSlider.Value;

                bool randomize = randomizeNoteOrderCheckbox.IsChecked ?? true;

                pattern = new ConfiguredPolygons(points, number, spacing, rotation, shift, randomize, end);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            numberOfPatternsTextbox.Visibility = Visibility.Hidden;                
        }

        private void tillEndCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            numberOfPatternsTextbox.Visibility = Visibility.Visible;
        }

        private void randomSpacingButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.randomizeSlider(this.spacingSlider);
        }

        private void randomRotationgButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.randomizeSlider(this.rotationSlider);
        }

        private void randomShiftButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.randomizeSlider(this.shiftSlider);
        }
    }
}
