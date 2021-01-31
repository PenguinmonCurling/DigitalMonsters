using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace DigitalMonsters
{
    public class DigimonArmourLoader
    {
        private List<string> ChosenDigis = new List<string>() { "Armadimon", "Patamon", "Tailmon", "Hawkmon", "V-mon", "Wormmon" };

        public void GenerateArmourData()
        {
            var digimentals = new List<string>();
            var armourChart = new List<ArmourChart>();
            var directory = Directory.GetCurrentDirectory();
            var listFilePath = Path.Combine(directory, "ArmourEvolutionChart.txt");

            var armourChartSize = 120;
            var currentArmourChart = new ArmourChart();
            foreach (var line in File.ReadAllLines(listFilePath))
            {
                if(Regex.IsMatch(line, "Digimental_\\w{1,}.png"))
                {
                    digimentals.Add(Regex.Match(line, "Digimental_\\w{1,}.png").Value.Replace("Digimental_", string.Empty).Replace(".png", string.Empty));
                }

                var foundChosen = false;
                foreach (var chosen in ChosenDigis)
                {
                    if (line.IndexOf(string.Format("[[{0}]]", chosen)) >= 0)
                    {
                        currentArmourChart = new ArmourChart { Digimon = chosen, DigimentalDigivolution = new SerialisableDictionary<string, string>() };
                        armourChartSize = 0;
                        foundChosen = true;
                    }
                }

                if(armourChartSize < digimentals.Count && !foundChosen)
                {
                    currentArmourChart.DigimentalDigivolution.Add(digimentals[armourChartSize],
                        Regex.Match(line, "\\[\\[\\w{1,}]]").Value.Replace("[[", string.Empty).Replace("]]", string.Empty));
                    armourChartSize++;
                }
                else if(currentArmourChart.DigimentalDigivolution?.Count > 0)
                {
                    armourChart.Add(currentArmourChart);
                    currentArmourChart = new ArmourChart();
                }
            }

            SaveArmourEvolution(armourChart);
        }

        private void SaveArmourEvolution(List<ArmourChart> armourList)
        {
            var fileStream = GetFileStream(FileMode.Create);
            using (fileStream)
            {
                new XmlSerializer(typeof(List<ArmourChart>)).Serialize(fileStream, armourList);
            }
        }

        public List<ArmourChart> LoadArmourData()
        {
            var fileStream = GetFileStream(FileMode.OpenOrCreate);
            using (fileStream)
            {
                var armourChart = new XmlSerializer(typeof(List<ArmourChart>)).Deserialize(fileStream) as List<ArmourChart>;
                SetDictionariesCaseInsensitive(armourChart);
                return armourChart;
            }
        }

        private void SetDictionariesCaseInsensitive(List<ArmourChart> armourCharts)
        {
            foreach(var armourChart in armourCharts)
            {
                var dictionary = new SerialisableDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var digivolution in armourChart.DigimentalDigivolution)
                {
                    dictionary.Add(digivolution.Key, digivolution.Value);
                }
                armourChart.DigimentalDigivolution = dictionary;
            }
        }

        private FileStream GetFileStream(FileMode fileMode)
        {
            var directory = Directory.GetCurrentDirectory();
            var listFilePath = Path.Combine(directory, "ArmourChart.xml");
            var fileStream = new FileStream(listFilePath, fileMode);
            return fileStream;
        }
    }
}
