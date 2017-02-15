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
    /// Логика взаимодействия для OverrideBeatmapStatsWindow.xaml
    /// </summary>
    public partial class OverrideBeatmapStatsWindow : Window
    {
        BeatmapStats beatmapStats;

        public OverrideBeatmapStatsWindow(BeatmapStats beatmapStats)
        {
            this.beatmapStats = beatmapStats;
            InitializeComponent();

            LoadBeatmapStats();
        }

        private void LoadBeatmapStats()
        {
            CSTextBox.Text = beatmapStats.CS.ToString();
            ARTextBox.Text = beatmapStats.AR.ToString();
            ODTextBox.Text = beatmapStats.OD.ToString();
            HPTextBox.Text = beatmapStats.HP.ToString();
        }

        private void SaveBeatmapStats()
        {
            beatmapStats.CS = float.Parse(CSTextBox.Text);
            beatmapStats.AR = float.Parse(ARTextBox.Text);
            beatmapStats.OD = float.Parse(ODTextBox.Text);
            beatmapStats.HP = float.Parse(HPTextBox.Text);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            SaveBeatmapStats();
            Close();
        }
    }
}
