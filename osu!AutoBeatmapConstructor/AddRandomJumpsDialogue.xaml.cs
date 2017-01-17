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
        public ConfiguredPattern pattern = null;

        private int numberOfNotes;

        public AddRandomJumpsDialogue()
        {
            InitializeComponent();
        }

        private void OKbutton_Click(object sender, RoutedEventArgs e)
        {
            string description;
            var notes = generatePattern();

            if (tillEndCheckbox.IsChecked.Value)
            {
                description = "Jumps till end";
            }
            else
            {
                description = numberOfNotes + "-notes jumps";
            }

            pattern = new ConfiguredPattern(notes, description);
            pattern.end = tillEndCheckbox.IsChecked.Value;
            DialogResult = true;
            Close();
        }

        private List<CircleObject> generatePattern()
        {
            MapContextAwareness context = ((MainWindow)Application.Current.MainWindow).generator.mapContext;
            PatternsGenerator generator = ((MainWindow)Application.Current.MainWindow).generator.patternGenerator;

            var notes = new List<CircleObject>();

            if (tillEndCheckbox.IsChecked.Value)
            {
                double endOffset = context.endOffset;
                double currOffset = context.offset;

                int n = (int)Math.Floor((endOffset - currOffset) / context.bpm) - 1;

                numberOfNotes = n;

            }
            else
            {
                numberOfNotes = int.Parse(numberOfNotesTextbox.Text);
            }
            int spacing = (int)spacingSlider.Value;

            notes.AddRange(generator.generateRandomJumps(numberOfNotes, spacing));

            return notes;
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
    }
}
