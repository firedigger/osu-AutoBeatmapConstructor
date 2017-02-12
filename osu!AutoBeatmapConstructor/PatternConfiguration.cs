using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace osu_AutoBeatmapConstructor
{
    public class PatternConfiguration
    {
        [XmlElement("name")]
        public string name;
        [XmlArray("patterns"), XmlArrayItem("pattern")]
        public List<ConfiguredPattern> patterns;

        public PatternConfiguration()
        {
            this.patterns = new List<ConfiguredPattern>();
        }

        public PatternConfiguration(string name, List<ConfiguredPattern> patterns)
        {
            this.name = name;
            this.patterns = patterns;
        }
        
        public override string ToString()
        {
            return name;
        }
    }
}