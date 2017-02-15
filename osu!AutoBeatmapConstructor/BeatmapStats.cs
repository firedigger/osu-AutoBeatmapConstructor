using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_AutoBeatmapConstructor
{
    public class BeatmapStats
    {
        public float CS, AR, OD, HP;

        public BeatmapStats()
        {

        }

        public BeatmapStats(float CS, float AR, float OD, float HP)
        {
            this.CS = CS;
            this.AR = AR;
            this.OD = OD;
            this.HP = HP;
        }
    }
}
