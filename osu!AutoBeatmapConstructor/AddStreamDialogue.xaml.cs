using System;
using System.Windows;

namespace osu_AutoBeatmapConstructor
{
    /// <summary>
    /// Логика взаимодействия для AddStreamDialogue.xaml
    /// </summary>
    public partial class AddStreamDialogue : Window
    {
        public ConfiguredStreams pattern = null;

        public AddStreamDialogue()
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
                int points = int.Parse(numberOfPointsTextbox.Text);

                int number = 0;
                bool end = false;
                if (tillEndCheckbox.IsChecked ?? true)
                    end = true;
                else
                    number = int.Parse(numberOfPatternsTextbox.Text);

                int spacing = (int)spacingSlider.Value;
                int shift = (int)shiftSlider.Value;
                int curviness = (int)curvinessSlider.Value;
                
                pattern = new ConfiguredStreams(points, number, spacing, curviness, shift, end);

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

        private void randomShiftButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.randomizeSlider(this.shiftSlider);
        }

        private void randomCuvinessButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.randomizeSlider(this.curvinessSlider);
        }
    }
}
