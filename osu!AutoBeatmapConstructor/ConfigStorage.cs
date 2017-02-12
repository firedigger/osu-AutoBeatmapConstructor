using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace osu_AutoBeatmapConstructor
{
    [Serializable]
    public class ConfigStorage
    {
        [XmlArray("configs"),XmlArrayItem("config")]
        public List<PatternConfiguration> configs = new List<PatternConfiguration>();

        public static ConfigStorage readFromFile(string fileName)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ConfigStorage));

            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                ConfigStorage obj = (ConfigStorage)formatter.Deserialize(fs);

                return obj;
            }
        }

        public static void saveToFile(string fileName, ConfigStorage storage)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ConfigStorage));

            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, storage);
            }
        }
    }
}
