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

        public ConfiguredRandomJumps(int numberOfNotes, int spacing, bool end) : base(PatternType.RandomJumps, end)
        {
            this.numberOfNotes = numberOfNotes;
            this.spacing = spacing;
        }

        public ConfiguredRandomJumps()
        {

        }

        public List<CircleObject> generateRandomJumps(MapContextAwareness mapContext, int numberOfNotes, int spacing)
        {
            if (end)
            {
                double endOffset = mapContext.endOffset;
                double currOffset = mapContext.Offset;

                int n = (int)Math.Floor((endOffset - currOffset) / mapContext.bpm) - 1;

                numberOfNotes = n;
            }

            var result = new List<CircleObject>();

            for (int i = 0; i < numberOfNotes; ++i)
            {
                Point2 next;
                if (i > 0)
                    next = PatternGenerator.findNextPosition(mapContext.X, mapContext.Y, spacing);
                else
                    next = PatternGenerator.findNextPosition(mapContext.X, mapContext.Y, 0);
                CircleObject note = new CircleObject();

                if (i % 4 == 0)
                    note.Type |= BMAPI.v1.HitObjectType.NewCombo;

                note.Location = next;
                result.Add(note);

                mapContext.X = (int)next.X;
                mapContext.Y = (int)next.Y;
            }

            foreach (var obj in result)
            {
                obj.StartTime = (int)mapContext.Offset;
                mapContext.Offset += mapContext.bpm;
            }

            return result;
        }

        public override List<CircleObject> generatePattern(MapContextAwareness context)
        {
            var notes = new List<CircleObject>();

            if (end)
            {
                double endOffset = context.endOffset;
                double currOffset = context.Offset;

                int n = (int)Math.Floor((endOffset - currOffset) / context.bpm) - 1;

                numberOfNotes = n;

            }

            notes.AddRange(generateRandomJumps(context, numberOfNotes, spacing));

            return notes;
        }

        public override string ToString()
        {
            string description;

            if (end)
            {
                description = "Jumps till end";
            }
            else
            {
                description = numberOfNotes + "-notes jumps";
            }

            return description;
        }
    }
}
