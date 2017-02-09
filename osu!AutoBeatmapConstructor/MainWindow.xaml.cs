using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BMAPI.v1;
using System.Collections.ObjectModel;
using BMAPI.v1.HitObjects;

namespace osu_AutoBeatmapConstructor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string mapPath;
        private Beatmap baseBeatmap;
        public ObservableCollection<ConfiguredPattern> Patterns { get; set; }
        public BeatmapGenerator generator;

        public MainWindow()
        {
            Patterns = new ObservableCollection<ConfiguredPattern>();
            DataContext = this;
            InitializeComponent();
        }

        private bool mapSelected()
        {
            return generator != null;
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void osuSelectButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog osuFileDialog = new OpenFileDialog();
            osuFileDialog.Filter = "osu! beatmap file|*.osu";

            if (osuFileDialog.ShowDialog() ?? true)
            {
                mapPath = osuFileDialog.FileName;
                baseBeatmap = new Beatmap(mapPath);
                songTitle.Content = baseBeatmap.Artist + " - " + baseBeatmap.Title;
                generator = new BeatmapGenerator(baseBeatmap);
                InitialSettingsWindow initialSettingsDialogue = new InitialSettingsWindow();
                if (initialSettingsDialogue.ShowDialog() ?? true)
                    extractMapContextFromWindow(initialSettingsDialogue);
            }
        }

        private void generateBeatmapButton_Click(object sender, RoutedEventArgs e)
        {
            if (!mapSelected())
            {
                MessageBox.Show("You must select .osu file first!");
                return;
            }

            if (difficultyNameTextbox.Text.Length == 0)
            {
                MessageBox.Show("Please enter the difficulty name!");
                return;
            }

            generator.addPatterns(Patterns);
            Beatmap generatedMap = generator.generateBeatmap();
            generatedMap.Version = difficultyNameTextbox.Text;
            generatedMap.regenerateFilename();
            generatedMap.Save(generatedMap.Filename);
            MessageBox.Show("Map saved");
        }

        private void extractMapContextFromWindow(InitialSettingsWindow window)
        {
            TimingPoint current = baseBeatmap.TimingPoints[0];
            generator.mapContext.bpm = current.BpmDelay / 2;
            if (window.firstTimingCheckbox.IsChecked.Value)
                generator.mapContext.beginOffset = current.Time;
            else
            {
                double tmp;
                if (double.TryParse(window.beginOffsetTextbox.Text, out tmp))
                    generator.mapContext.beginOffset = tmp;
                else
                {
                    MessageBox.Show("Unable to parse the begin offset. Please input a valid number or check the First timing point checkbox");
                    return;
                }
            }
            generator.mapContext.offset = generator.mapContext.beginOffset;

            if (window.lastObjectCheckbox.IsChecked.Value)
                generator.mapContext.endOffset = findLastObjectTimingInMap(baseBeatmap);
            else
            {
                double tmp;
                if (double.TryParse(window.beginOffsetTextbox.Text, out tmp))
                    generator.mapContext.endOffset = tmp;
                else
                {
                    MessageBox.Show("Unable to parse the begin offset. Please input a valid number or check the Last object checkbox");
                    return;
                }
            }
            
            if (window.keepOriginalPartCheckbox.IsChecked.Value)
            {
                PatternGenerator.copyMap(baseBeatmap, generator.generatedMap, generator.mapContext.offset, generator.mapContext.endOffset);
            }

        }

        private double findLastObjectTimingInMap(Beatmap baseBeatmap)
        {
            return baseBeatmap.HitObjects.Last().StartTime;
        }

        private void addPolygonsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!mapSelected())
            {
                MessageBox.Show("You must select .osu file first!");
                return;
            }

            AddPolygonDialogue dialog = new AddPolygonDialogue();
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                Patterns.Add(dialog.pattern);
            }
        }

        private void addStreamsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!mapSelected())
            {
                MessageBox.Show("You must select .osu file first!");
                return;
            }

            AddStreamDialogue dialog = new AddStreamDialogue();
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                Patterns.Add(dialog.pattern);
            }
        }

        private void addJumpsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!mapSelected())
            {
                MessageBox.Show("You must select .osu file first!");
                return;
            }

            AddRandomJumpsDialogue dialog = new AddRandomJumpsDialogue();
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                Patterns.Add(dialog.pattern);
            }
        }

        private void randomPatternsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!mapSelected())
            {
                MessageBox.Show("You must select .osu file first!");
                return;
            }

            //TODO implement
            /*while (generator.mapContext.offset < generator.mapContext.endOffset)
            {
                int number = 10;
                int points = 10;
                int spacing = 10;
                int rotation = 10;
                int shift = 10;
                bool randomize = true;
                var pattern = generator.patternGenerator.generatePolygons(points,number,spacing,rotation,shift,randomize);
                generator.addPattern(pattern);
            }*/
        }

        private void addBreak_Click(object sender, RoutedEventArgs e)
        {
            if (!mapSelected())
            {
                MessageBox.Show("You must select .osu file first!");
                return;
            }

            AddBreakDialogue dialog = new AddBreakDialogue();
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                Patterns.Add(dialog.pattern);
            }
        }

        private void deletePatternButton_Click(object sender, RoutedEventArgs e)
        {
            int index = configuredPatterns.SelectedIndex;
            if (index == -1)
                MessageBox.Show("The pattern list is empty");
            else
                Patterns.RemoveAt(index);
        }

        private void addDoubleJumpsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!mapSelected())
            {
                MessageBox.Show("You must select .osu file first!");
                return;
            }

            AddDoubleJumpsDialogue dialog = new AddDoubleJumpsDialogue();
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                Patterns.Add(dialog.pattern);
            }
        }
    }
}
