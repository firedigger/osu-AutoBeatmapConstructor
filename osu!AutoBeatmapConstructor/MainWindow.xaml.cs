using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using BMAPI.v1;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;

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
        private MapContextAwareness baseContext;
        private BeatmapStats beatmapStats;
        private InitialSettingsWindow initialSettingsDialogue;
        private List<ConfiguredPattern> patternBuffer;

        public MainWindow()
        {
            Patterns = new ObservableCollection<ConfiguredPattern>();
            DataContext = this;
            beatmapStats = new BeatmapStats();
            InitializeComponent();
        }

        private bool mapSelected()
        {
            return initialSettingsDialogue != null;
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void initializeBeatmap()
        {
            generator = new BeatmapGenerator(baseBeatmap);
            extractMapContextFromWindow(initialSettingsDialogue);
            baseContext = (MapContextAwareness)generator.mapContext.Clone();
        }

        private void osuSelectButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog osuFileDialog = new OpenFileDialog();
            osuFileDialog.Filter = "osu! beatmap file|*.osu";

            if (osuFileDialog.ShowDialog() ?? true)
            {
                initialSettingsDialogue = new InitialSettingsWindow();
                if (initialSettingsDialogue.ShowDialog() ?? true)
                {
                    mapPath = osuFileDialog.FileName;
                    baseBeatmap = new Beatmap(mapPath);
                    songArtist.Content = baseBeatmap.Artist;
                    songTitle.Content = baseBeatmap.Title;
                    difficultyNameTextbox.Text = "generated " + baseBeatmap.Version;
                    applyBeatmapStats(baseBeatmap);
                }
            }
        }

        private void applyBeatmapStats(Beatmap baseMap)
        {
            beatmapStats.CS = baseBeatmap.CircleSize;
            beatmapStats.AR = baseBeatmap.ApproachRate;
            beatmapStats.OD = baseBeatmap.OverallDifficulty;
            beatmapStats.HP = baseBeatmap.HPDrainRate;
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

            try
            {
                PatternConfiguration config = new PatternConfiguration(difficultyNameTextbox.Text, beatmapStats, Patterns.ToList());
                initializeBeatmap();
                generator.mapContext = (MapContextAwareness)baseContext.Clone();
                Beatmap generatedMap = generator.generateBeatmap(config);

                if (!generatedMap.checkObjectsBounds())
                    MessageBox.Show("WARNING! Some objects were found to be outside of the playfield. Beware.");

                generatedMap.Version = difficultyNameTextbox.Text;
                generatedMap.regenerateFilename();
                generatedMap.Save(generatedMap.Filename);
                MessageBox.Show("Map saved");
            }
            catch (Exception ex)
            {
                MessageBox.Show("My appologies. Something went wrong while constructing the map. The description might help but most likely you chose large spacing for some pattern which the program wasn't able to handle. Please clear the patterns and try again.\n"+ex.ToString());
            }

            
        }

        private void extractMapContextFromWindow(InitialSettingsWindow window)
        {
            TimingPoint current = baseBeatmap.TimingPoints[0];
            //double generator_bpm = current.BpmDelay / 2;

            string tickDivisor = window.tickDivisorComboBox.Text;

            double bpm_multiplier;

            switch (tickDivisor)
            {
                case "1/1": bpm_multiplier = 2; break;
                case "1/2": bpm_multiplier = 1; break;
                case "1/3": bpm_multiplier = 2.0 / 3; break;
                case "1/4": bpm_multiplier = 0.5; break;
                default: throw new Exception("Unknown tick divisor"); 
            }

            double generator_beginOffset;

            if (window.firstTimingCheckbox.IsChecked.Value)
                generator_beginOffset = current.Time;
            else
            {
                double tmp;
                if (double.TryParse(window.beginOffsetTextbox.Text, out tmp))
                    generator_beginOffset = tmp;
                else
                {
                    MessageBox.Show("Unable to parse the begin offset. Please input a valid number or check the First timing point checkbox");
                    return;
                }
            }

            double generator_endOffset;

            if (window.lastObjectCheckbox.IsChecked.Value)
                generator_endOffset = findLastObjectTimingInMap(baseBeatmap);
            else
            {
                double tmp;
                if (double.TryParse(window.endOffsetTextbox.Text, out tmp))
                    generator_endOffset = tmp;
                else
                {
                    MessageBox.Show("Unable to parse the begin offset. Please input a valid number or check the Last object checkbox");
                    return;
                }
            }

            int generator_X;
            int generator_Y;

            if (window.overrideStartPointCheckBox.IsChecked ?? true)
            {
                double tmp;
                if (double.TryParse(window.XtextBox.Text, out tmp))
                    generator_X = (int)tmp;
                else
                {
                    MessageBox.Show("Unable to parse the X coordinate. Please input a valid number or uncheck the override starting position checkbox");
                    return;
                }

                if (double.TryParse(window.YtextBox.Text, out tmp))
                    generator_Y = (int)tmp;
                else
                {
                    MessageBox.Show("Unable to parse the Y coordinate. Please input a valid number or uncheck the override starting position checkbox");
                    return;
                }
            }
            else
            {
                generator_X = 200;
                generator_Y = 200;
            }

            generator.mapContext = new MapContextAwareness(bpm_multiplier, generator_beginOffset, generator_endOffset, generator_X, generator_Y, baseBeatmap.TimingPoints);

            if (window.keepOriginalPartCheckbox.IsChecked.Value)
            {
                PatternGenerator.copyMap(baseBeatmap, generator.generatedMap, generator.mapContext.Offset, generator.mapContext.endOffset);
            }
        }

        private double findLastObjectTimingInMap(Beatmap baseBeatmap)
        {
            return baseBeatmap.HitObjects.Last().StartTime;
        }

        private void addPolygonsButton_Click(object sender, RoutedEventArgs e)
        {
            AddPolygonDialogue dialog = new AddPolygonDialogue();
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                Patterns.Add(dialog.pattern);
            }
        }

        private void addStreamsButton_Click(object sender, RoutedEventArgs e)
        {
            AddStreamDialogue dialog = new AddStreamDialogue();
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                Patterns.Add(dialog.pattern);
            }
        }

        private void addJumpsButton_Click(object sender, RoutedEventArgs e)
        {
            AddRandomJumpsDialogue dialog = new AddRandomJumpsDialogue();
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                Patterns.Add(dialog.pattern);
            }
        }

        private void addBreak_Click(object sender, RoutedEventArgs e)
        {
            AddBreakDialogue dialog = new AddBreakDialogue();
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                Patterns.Add(dialog.pattern);
            }
        }

        private void deletePatternButton_Click(object sender, RoutedEventArgs e)
        {
            var items = configuredPatterns.SelectedItems;
            if (items.Count > 1)
            {
                for (int i = items.Count - 1; i >= 0; i--)
                    Patterns.Remove((ConfiguredPattern)items[i]);
                configuredPatterns.SelectedIndex = -1;
                return;
            }

            int index = configuredPatterns.SelectedIndex;
            if (index == -1)
                MessageBox.Show("Please select a pattern");
            else
            {
                Patterns.RemoveAt(index);

                if (Patterns.Count > 0)
                {
                    if (index < Patterns.Count)
                        configuredPatterns.SelectedIndex = index;
                    else
                        configuredPatterns.SelectedIndex = index - 1;
                    ListViewItem item = configuredPatterns.ItemContainerGenerator.ContainerFromIndex(configuredPatterns.SelectedIndex) as ListViewItem;
                    item.Focus();
                }

            }
        }

        private void addDoubleJumpsButton_Click(object sender, RoutedEventArgs e)
        {
            AddDoubleJumpsDialogue dialog = new AddDoubleJumpsDialogue();
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                Patterns.Add(dialog.pattern);
            }
        }

        private void loadConfigButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog selectConfigDialogue = new OpenFileDialog();
            selectConfigDialogue.Filter = "osu!oABC|*.xml";
            selectConfigDialogue.Multiselect = false;

            if (selectConfigDialogue.ShowDialog() ?? true)
            {
                var obj = ConfigStorage.readFromFile(selectConfigDialogue.FileName);

                SelectConfig selectConfig = new SelectConfig(obj);

                if (selectConfig.ShowDialog() ?? true)
                {
                    foreach (var p in selectConfig.selected.patterns)
                        Patterns.Add(p);
                    difficultyNameTextbox.Text = selectConfig.selected.name;
                    beatmapStats = selectConfig.selected.beatmapStats;
                }
            }
        }
        
        private void saveConfigButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialogue = new SaveFileDialog();
            saveFileDialogue.Filter = "osu!oABC config|*.xml";

            if (saveFileDialogue.ShowDialog() ?? true)
            {
                var storage = new ConfigStorage();
                storage.configs.Add(new PatternConfiguration(difficultyNameTextbox.Text, beatmapStats, Patterns.ToList()));
                ConfigStorage.saveToFile(saveFileDialogue.FileName,storage);
                MessageBox.Show("Configuration saved!");
            }
        }

        private void savePlusConfigButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog selectConfigDialogue = new OpenFileDialog();
            selectConfigDialogue.Filter = "osu!oABC|*.xml";
            selectConfigDialogue.Multiselect = false;

            if (selectConfigDialogue.ShowDialog() ?? true)
            {
                var storage = ConfigStorage.readFromFile(selectConfigDialogue.FileName);
                var existingConfig = storage.configs.FindIndex((x) => x.name == difficultyNameTextbox.Text);
                if (existingConfig != -1)
                {
                    MessageBoxResult dialogResult = MessageBox.Show("There is already a config with the name " + difficultyNameTextbox.Text + "\nDo you want to override it?", "Override warning!", MessageBoxButton.YesNo);
                    if (dialogResult == MessageBoxResult.Yes)
                    {
                        storage.configs[existingConfig] = new PatternConfiguration(difficultyNameTextbox.Text, beatmapStats,Patterns.ToList());
                    }
                    else if (dialogResult == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                else
                {
                    storage.configs.Add(new PatternConfiguration(difficultyNameTextbox.Text, beatmapStats, Patterns.ToList()));
                }
                ConfigStorage.saveToFile(selectConfigDialogue.FileName, storage);
                MessageBox.Show("Configuration saved!");
            }
        }

        private void generateFullMapsetFromConfigButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog selectConfigDialogue = new OpenFileDialog();
            selectConfigDialogue.Filter = "osu!oABC|*.xml";
            selectConfigDialogue.Multiselect = false;

            if (selectConfigDialogue.ShowDialog() ?? true)
            {
                var obj = ConfigStorage.readFromFile(selectConfigDialogue.FileName);

                foreach (var config in obj.configs)
                {
                    initializeBeatmap();
                    generator.mapContext = (MapContextAwareness)baseContext.Clone();
                    Beatmap generatedMap = generator.generateBeatmap(config);
                    generatedMap.Version = config.name;
                    generatedMap.regenerateFilename();
                    generatedMap.Save(generatedMap.Filename);
                }
                MessageBox.Show("Maps saved!");
            }
        }

        private void clearListButton_Click(object sender, RoutedEventArgs e)
        {
            Patterns.Clear();
        }

        private void changeBeatmapStatsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!mapSelected())
            {
                MessageBox.Show("You must select .osu file first!");
                return;
            }
            OverrideBeatmapStatsWindow overrideBeatmapStatsWindow = new OverrideBeatmapStatsWindow(beatmapStats);
            overrideBeatmapStatsWindow.ShowDialog();
        }

        private void randomConfigButton_Click(object sender, RoutedEventArgs e)
        {
            ChooseLevelDialogue chooseLevelDialogue = new ChooseLevelDialogue();
            if (chooseLevelDialogue.ShowDialog() ?? true)
            {
                int level = chooseLevelDialogue.level;
                int count = chooseLevelDialogue.count;
                int phase = chooseLevelDialogue.phase;
                bool toEnd = chooseLevelDialogue.toEndCheckBox.IsChecked ?? true;

                for (int i = 0; i < count; ++i)
                {
                    for (int j = 0; j < phase; ++j)
                    {
                        int patternType = Utils.rng.Next(4);
                        ConfiguredPattern pattern = null;
                        switch (patternType)
                        {
                            case 0: pattern = ConfiguredDoubleJumps.randomPattern(level); break;
                            case 1: pattern = ConfiguredPolygons.randomPattern(level); break;
                            case 2: pattern = ConfiguredRandomJumps.randomPattern(level); break;
                            case 3: pattern = ConfiguredStreams.randomPattern(level); break;
                            default: pattern = ConfiguredPolygons.randomPattern(level); break;
                        }
                        Patterns.Add(pattern);

                        if (i == count - 1 && j == phase - 1 && toEnd)
                            pattern.end = true;
                    }

                    ConfiguredBreak breakP = ConfiguredBreak.randomPattern(level);
                    Patterns.Add(breakP);
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                deletePatternButton_Click(sender, e);
                return;
            }

            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.KeyboardDevice.IsKeyDown(Key.C))
            {
                copySelectedPatterns();
            }

            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.KeyboardDevice.IsKeyDown(Key.V))
            {
                pastePatterns();
            }
        }

        private void pastePatterns()
        {
            if (patternBuffer != null)
            {
                foreach(var p in patternBuffer)
                {
                    this.Patterns.Add(p);
                }
            }
        }

        private void copySelectedPatterns()
        {
            var items = configuredPatterns.SelectedItems;
            List<ConfiguredPattern> a = new List<ConfiguredPattern>();
            for (int i = items.Count - 1; i >= 0; i--)
            {
                a.Add(this.Patterns[i]);
            }
            this.patternBuffer = a;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string help = "Please refer to https://github.com/firedigger/osu-AutoBeatmapConstructor!";
            MessageBox.Show(help,"Help");
        }
    }
}
