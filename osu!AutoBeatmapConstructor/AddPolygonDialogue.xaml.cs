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
        public ConfiguredPattern pattern = null;

        private int points;
        private int number;

        public AddPolygonDialogue()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private List<CircleObject> generatePattern()
        {
            MapContextAwareness context = ((MainWindow)Application.Current.MainWindow).generator.mapContext;
            PatternsGenerator generator = ((MainWindow)Application.Current.MainWindow).generator.patternGenerator;

            var notes = new List<CircleObject>();

            this.points = int.Parse(numberOfPointsComboBox.Text);
            this.number = int.Parse(numberOfPatternsTextbox.Text);
            if (tillEndCheckbox.IsChecked.Value)
            {
                double endOffset = context.endOffset;
                double currOffset = context.offset;

                int n = (int)Math.Floor((endOffset - currOffset) / context.bpm);

                number = n / points;

            }
            int spacing = (int)spacingSlider.Value;
            int rotation = (int)rotationSlider.Value;
            int shift = (int)shiftSlider.Value;
            bool randomize = randomizeNoteOrderCheckbox.IsChecked.Value;
            notes.AddRange(generator.generatePolygons(points,number,spacing,rotation,shift,randomize));

            return notes;
        }

        private void OKbutton_Click(object sender, RoutedEventArgs e)
        {
            string description;
            var notes = generatePattern();

            if (tillEndCheckbox.IsChecked.Value)
            {
                description = points + "-Polygons till end";
            }
            else
            {
                description = number + " " + points + "-Polygons";
            }

            pattern = new ConfiguredPattern(notes, description);
            pattern.end = tillEndCheckbox.IsChecked.Value;
            DialogResult = true;
            Close();
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            numberOfPatternsTextbox.Visibility = Visibility.Hidden;                
        }

        private void tillEndCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            numberOfPatternsTextbox.Visibility = Visibility.Visible;
        }
    }
}
