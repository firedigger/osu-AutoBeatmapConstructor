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
    /// Логика взаимодействия для AddRandomJumpsDialogue.xaml
    /// </summary>
    public partial class AddRandomJumpsDialogue : Window
    {
        public ConfiguredRandomJumps pattern = null;

        public AddRandomJumpsDialogue()
        {
            InitializeComponent();
        }

        private void OKbutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int number = 0;
                bool end = false;
                if (tillEndCheckbox.IsChecked ?? true)
                    end = true;
                else
                    number = int.Parse(numberOfNotesTextbox.Text);

                int spacing = (int)spacingSlider.Value;

                int maxStack = int.Parse(maxStacksComboBox.Text);
                bool onlySuch = onlySuchCheckBox.IsChecked ?? true;

                pattern = new ConfiguredRandomJumps(number, spacing, end, maxStack, onlySuch);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void tillEndCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            numberOfNotesTextbox.Visibility = Visibility.Visible;
        }

        private void tillEndCheckbox_checked(object sender, RoutedEventArgs e)
        {
            numberOfNotesTextbox.Visibility = Visibility.Hidden;
        }

        private void randomSpacingButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.randomizeSlider(this.spacingSlider);
        }
    }
}
