using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BMAPI.v1.HitObjects;
using BMAPI;

namespace osu_AutoBeatmapConstructor
{
    [Serializable]
    public class ConfiguredRandomJumps : ConfiguredPattern
    {
        public int numberOfNotes;
        public int spacing;
        public int maxStack;
        public bool useOnly;

        public ConfiguredRandomJumps(int numberOfNotes, int spacing, bool end, int maxStack, bool useOnly) : base(PatternType.RandomJumps, end)
        {
            this.numberOfNotes = numberOfNotes;
            this.spacing = spacing;
            this.maxStack = maxStack;
            this.useOnly = useOnly;
        }

        public ConfiguredRandomJumps()
        {

        }

        public List<CircleObject> generateRandomJumps(MapContextAwareness mapContext, int numberOfNotes, int spacing)
        {
            var result = new List<CircleObject>();

            int c = 0;

            for (int i = 0; i < numberOfNotes && mapContext.Offset < mapContext.endOffset; ++i)
            {
                Point2 next;
                if (i > 0)
                    next = PatternGenerator.findNextPosition(mapContext.X, mapContext.Y, spacing);
                else
                    next = PatternGenerator.findNextPosition(mapContext.X, mapContext.Y, 0);

                int stackNumber;

                if (useOnly)
                    stackNumber = maxStack;
                else
                    stackNumber = Utils.rng.Next(1,maxStack + 1);

                for (int j = 0; j < stackNumber; ++j)
                {
                    CircleObject note = new CircleObject();

                    if (j == 0)
                        note.Type |= BMAPI.v1.HitObjectType.NewCombo;

                    note.Location = next;
                    result.Add(note);

                    mapContext.X = (int)next.X;
                    mapContext.Y = (int)next.Y;

                    note.StartTime = (int)mapContext.Offset;

                    if (j < stackNumber - 1)
                        mapContext.Offset += mapContext.bpm / 2;
                }
                if (stackNumber % 2 == 0)
                    ++c;

                mapContext.Offset += mapContext.bpm;
            }

            if (c % 2 == 1)
                mapContext.Offset += mapContext.bpm / 2;

            return result;
        }

        public override List<CircleObject> generatePattern(MapContextAwareness context)
        {
            var notes = new List<CircleObject>();

            if (end)
            {
                numberOfNotes = (int)1e6;
            }

            notes.AddRange(generateRandomJumps(context, numberOfNotes, spacing));

            return notes;
        }

        public override string ToString()
        {
            string description = maxStack + "-notes stacks";

            if (end)
            {
                description += " till end";
            }
            else
            {
                description = numberOfNotes + " " + description;
            }

            return description;
        }

        public static ConfiguredRandomJumps randomPattern(int level)
        {
            int number = Utils.rng.Next(10,20);
            int spacing = (level - 1) * 100 + Utils.rng.Next(10, 100);
            int maxStack = (level - 1) * 2 + Utils.rng.Next(2);
            bool useOnly = Utils.rng.Next(1, 5) > level;

            ConfiguredRandomJumps p = new ConfiguredRandomJumps(number, spacing, false, maxStack, useOnly);
            return p;
        }
    }
}
