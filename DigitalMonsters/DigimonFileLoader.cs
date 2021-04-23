using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace DigitalMonsters
{
    public class DigimonFileLoader : IDigimonLoader
    {
        private List<string> _InvalidMonsters;

        public DigimonFileLoader(List<string> invalidMonsters)
        {
            _InvalidMonsters = invalidMonsters;
        }

        public List<Digimon> Load()
        {
            var directory = Directory.GetCurrentDirectory();
            var listFilePath = Path.Combine(directory, "DigimonList.xml");
            return LoadFromXmlFile(listFilePath);
        }

        public void SaveDigimon(List<Digimon> digimonList, string fileName = "DigimonList.xml")
        {
            var directory = Directory.GetCurrentDirectory();
            var listFilePath = Path.Combine(directory, fileName);
            var fileStream = new FileStream(listFilePath, FileMode.Create);
            using (fileStream)
            {
                new XmlSerializer(typeof(List<Digimon>)).Serialize(fileStream, digimonList);
            }
        }

        private List<Digimon> LoadFromXmlFile(string listFilePath)
        {
            var fileStream = new FileStream(listFilePath, FileMode.OpenOrCreate);
            using (fileStream)
            {
                var digimonList = new XmlSerializer(typeof(List<Digimon>)).Deserialize(fileStream) as List<Digimon>;
                var validDigimon = digimonList?.Where(x => IsValidDigimon(x)).ToList();
                SetNumbers(validDigimon);
                return validDigimon;
            }
        }

        private List<Digimon> LoadFromJsonFile(string listFilePath)
        {
            var digimonList = new List<Digimon>();
            var digimon = new Digimon();
            digimonList = new JavaScriptSerializer().Deserialize<List<Digimon>>(File.ReadAllText(listFilePath));
            var validDigimon = digimonList.Where(x => IsValidDigimon(x)).ToList();
            SetNumbers(validDigimon);
            return validDigimon;
        }

        private void WriteAllAppearances(List<Digimon> digimonList)
        {
            var appearances = digimonList.Where(x => x.Appearances != null).SelectMany(x => x.Appearances).Distinct(new Appearance()).ToList();
            var directory = Directory.GetCurrentDirectory();
            var listFilePath = Path.Combine(directory, "DigimonAppearances.xml");
            var fileStream = new FileStream(listFilePath, FileMode.Create);
            new XmlSerializer(typeof(List<Appearance>)).Serialize(fileStream, appearances);
        }

        public void SetNumbers(List<Digimon> digimonList)
        {
            SetNumbersCustom(digimonList);
        }

        private void SetNumbersCustom(List<Digimon> digimonList)
        {
            var orderedDigimon = digimonList
                .Where(x => x.Appearances != null)
                .OrderBy(y => y.Appearances.Any() ? y.Appearances.Min(x => x.AppearanceValue()) : 9999)
                .ThenBy(x => x.LevelNumber).ThenBy(z => z.DisplayName).ToList();
            foreach(var digimon in digimonList)
            {
                digimon.Number = orderedDigimon.IndexOf(digimon) + 1;
            }
        }

        private void SetNumbersFromFile(List<Digimon> digimonList)
        {
            var directory = Directory.GetCurrentDirectory();
            var listFilePath = Path.Combine(directory, "DigimonNumbered.txt");
            foreach (var line in File.ReadAllLines(listFilePath))
            {
                var row = line.Split(',');
                if (row.Length > 1)
                {
                    if (int.TryParse(row[0], out int number))
                    {
                        if (digimonList.Any(x => string.Equals(x.DisplayName, row[1], StringComparison.OrdinalIgnoreCase)))
                        {
                            digimonList.FirstOrDefault(x => string.Equals(x.DisplayName, row[1], StringComparison.OrdinalIgnoreCase)).Number = number;
                        }
                    }
                }
            }
        }

        private bool IsValidDigimon(Digimon digimon)
        {
            return !string.Equals("[[C'mon Digimon]]", digimon.DebutedIn)
                && !string.Equals("[[Digimon Universe Appli Monsters]]", digimon.DebutedIn)
                && digimon.Name.IndexOf("Mon", StringComparison.OrdinalIgnoreCase) >= 0
                && !digimon.Name.StartsWith("Bio", StringComparison.OrdinalIgnoreCase)
                && !_InvalidMonsters.Contains(digimon.Name, StringComparer.OrdinalIgnoreCase);
        }
    }
}
