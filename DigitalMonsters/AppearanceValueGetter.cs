using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace DigitalMonsters
{
    public class AppearanceValueGetter : IAppearanceValueGetter
    {
        private static SerialisableDictionary<string, AppearanceOptions> AppearanceValues;
        private static SerialisableDictionary<float, string> AppearanceNames;
        private const int MaxValue = 9999;

        public AppearanceValueGetter()
        {
            if (AppearanceValues == null)
            {
                DeserialiseAppearanceList();
                SetAppearanceNames();
            }
        }

        private void AddAppearanceValue(string appearanceName, float value, bool isRegexMatch)
        {
            foreach(var appearanceValue in AppearanceValues.Where(x => x.Value.Value >= value))
            {
                appearanceValue.Value.Value++;
            }
            AppearanceValues.Add(appearanceName, new AppearanceOptions { Value = value, IsRegexMatch = isRegexMatch });
        }

        private void SetAppearanceNames()
        {
            var uniques = AppearanceValues.Where(x => string.Equals(x.Key, AppearanceValues.FirstOrDefault(y => y.Value.Value == x.Value.Value).Key, StringComparison.OrdinalIgnoreCase));
            AppearanceNames = new SerialisableDictionary<float, string>(uniques.ToDictionary(x => x.Value.Value, x => x.Key));
        }

        public float ApperanceValue(string appearanceName)
        {
            if (AppearanceValues.ContainsKey(appearanceName))
            {
                return AppearanceValues[appearanceName].Value;
            }
            else
            {
                var foundKey = GetOtherCaseAppearanceName(appearanceName);
                if (string.IsNullOrWhiteSpace(foundKey))
                {
                    foundKey = GetRegexAppearanceName(appearanceName);
                }
                if (!string.IsNullOrWhiteSpace(foundKey))
                {
                    return AppearanceValues[foundKey].Value;
                }
            }
            return MaxValue;
        }

        private static string GetRegexAppearanceName(string appearanceName)
        {
            return AppearanceValues.Where(x => x.Value.IsRegexMatch).Select(x => x.Key).FirstOrDefault(x => Regex.IsMatch(appearanceName, x, RegexOptions.IgnoreCase));
        }

        public string GetOtherCaseAppearanceName(string appearanceName)
        {
            return AppearanceValues.Select(x => x.Key).FirstOrDefault(y => string.Equals(y, appearanceName, StringComparison.OrdinalIgnoreCase));
        }

        public string AppearanceName(float appearanceValue)
        {
            if (AppearanceNames.ContainsKey(appearanceValue))
            {
                return AppearanceNames[appearanceValue];
            }
            else
            {
                return string.Empty;
            }
        }

        public void SerialiseAppearanceList()
        {
            var directory = Directory.GetCurrentDirectory();
            var listFilePath = Path.Combine(directory, "DigimonAppearances.xml");
            var fileStream = new FileStream(listFilePath, FileMode.Create);
            using (fileStream)
            {
                new XmlSerializer(typeof(SerialisableDictionary<string, AppearanceOptions>)).Serialize(fileStream, AppearanceValues);
            }
        }

        public void DeserialiseAppearanceList()
        {
            var directory = Directory.GetCurrentDirectory();
            var listFilePath = Path.Combine(directory, "DigimonAppearances.xml");
            var fileStream = new FileStream(listFilePath, FileMode.OpenOrCreate);
            using (fileStream)
            {
                AppearanceValues = new XmlSerializer(typeof(SerialisableDictionary<string, AppearanceOptions>)).Deserialize(fileStream) as SerialisableDictionary<string, AppearanceOptions>;
            }
        }

        private bool GetIsRegextMatch(string value)
        {
            var isRegex = false;
            if (value.IndexOf("Pendulum", StringComparison.OrdinalIgnoreCase) >= 0
                && value.IndexOf("Progress", StringComparison.OrdinalIgnoreCase) < 0
                && value.IndexOf("X", StringComparison.OrdinalIgnoreCase) < 0)
            {
                isRegex = true;
            }
            else if (value.IndexOf("Pendulum", StringComparison.OrdinalIgnoreCase) >= 0
                && value.IndexOf("Progress", StringComparison.OrdinalIgnoreCase) >= 0
                && value.IndexOf("X", StringComparison.OrdinalIgnoreCase) < 0)
            {
                isRegex = true;
            }
            else if (value.IndexOf("Pendulum", StringComparison.OrdinalIgnoreCase) >= 0
                && value.IndexOf("X", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                isRegex = true;
            }
            else if (value.IndexOf("D-Ark", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                isRegex = true;
            }
            else if (value.IndexOf("D-Scanner", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                isRegex = true;
            }
            else if (value.IndexOf("Digimon Twin", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                isRegex = true;
            }
            else if (value.IndexOf("Sunburst", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                isRegex = true;
            }
            else if (value.IndexOf("Moonlight", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                isRegex = true;
            }
            return isRegex;
        }
    }
}
