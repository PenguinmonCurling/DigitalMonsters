using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace DigitalMonsters
{
    public class DigimonWebLoader : IDigimonLoader
    {
        private List<string> _InvalidMonsters;

        public DigimonWebLoader(List<string> invalidMonsters)
        {
            _InvalidMonsters = invalidMonsters;
        }

        public List<Digimon> Load()
        {
            var directory = Directory.GetCurrentDirectory();
            var listFilePath = Path.Combine(directory, "DigimonList.txt");
            return LoadFromFile(listFilePath);
        }

        public void LoadImage(Digimon digimon)
        {
            MakeWebRequest(digimon);
        }

        public void SaveDigimon(List<Digimon> digimonList, string fileName = "DigimonJsonList.txt")
        {
            var json = new JavaScriptSerializer().Serialize(digimonList);
            var directory = Directory.GetCurrentDirectory();
            var listFilePath = Path.Combine(directory, fileName);
            File.WriteAllText(listFilePath, json);
        }

        private List<Digimon> LoadFromFile(string listFilePath)
        {
            var digimonList = new List<Digimon>();
            var digimon = new Digimon();
            foreach (var line in File.ReadAllLines(listFilePath))
            {
                if (line.StartsWith("|n="))
                {
                    digimon.Name = line.Replace("|n=", string.Empty);
                }
                if (line.StartsWith("|yr=") && int.TryParse(line.Replace("|yr=", string.Empty), out int debutYear))
                {
                    digimon.DebutYear = debutYear;
                }
                if (line.StartsWith("|d="))
                {
                    digimon.DebutedIn = line.Replace("|d=", string.Empty);
                }
                if (line.StartsWith("}}"))
                {
                    if(digimon.DebutYear == 0)
                    {
                        digimon.DebutYear = 2099;
                    }
                    digimonList.Add(digimon);
                    digimon = new Digimon();
                }
            }
            return digimonList.Where(x => IsValidDigimon(x)).ToList();
        }

        private bool IsValidDigimon(Digimon digimon)
        {
            return digimon.DebutYear > 2005
                && !string.Equals("[[C'mon Digimon]]", digimon.DebutedIn)
                && !_InvalidMonsters.Contains(digimon.Name, StringComparer.OrdinalIgnoreCase);
        }

        private void MakeWebRequest(Digimon digimon, bool retry = false)
        {
            try
            {
                var urlToLoad = string.Format("https://wikimon.net/index.php?title={0}&action=edit", digimon.Name.Replace(" ", "_"));
                var client = new WebClient();
                var downloadString = client.DownloadString(urlToLoad);
                if (Regex.IsMatch(downloadString, "\\|dub=[A-z]{1,}\\n"))
                {
                    digimon.DubName = Regex.Match(downloadString, "\\|dub=[A-z]{1,}\\n").Value.Replace("|dub=", string.Empty).Replace("\n", string.Empty);
                }
                if (Regex.IsMatch(downloadString, "\\|l1=[A-z\\s]{1,}\\n"))
                {
                    digimon.Level = Regex.Match(downloadString, "\\|l1=[A-z\\s]{1,}\\n").Value.Replace("|l1=", string.Empty).Replace("\n", string.Empty);
                }
                if (Regex.IsMatch(downloadString, "\\|t1=[A-z\\s]{1,}\\n"))
                {
                    digimon.Type = Regex.Match(downloadString, "\\|t1=[A-z\\s]{1,}\\n").Value.Replace("|t1=", string.Empty).Replace("\n", string.Empty);
                }
                if (Regex.IsMatch(downloadString, "\\|a1=[A-z\\s]{1,}\\n"))
                {
                    digimon.Attribute = Regex.Match(downloadString, "\\|a1=[A-z\\s]{1,}\\n").Value.Replace("|a1=", string.Empty).Replace("\n", string.Empty);
                }
                //if (Regex.IsMatch(downloadString, "\\|pe=[A-z\\s]{1,}\\n"))
                //{
                //    digimon.Description = Regex.Match(downloadString, "\\|pe=[A-z\\s]{1,}\\n").Value.Replace("|pe=", string.Empty).Replace("\n", string.Empty);
                //}

                GetEvolutions(digimon, downloadString);
                GetAnimeAppearances(digimon, downloadString);
                GetVideoGameAppearances(digimon, downloadString);
                GetVirutalPetAppearances(digimon, downloadString);

                if (Regex.IsMatch(downloadString, "\\|image=.{1,}\\n"))
                {
                    var imageDownloadString = client.DownloadString(string.Concat("https://wikimon.net/File:",
                        Regex.Match(downloadString, "\\|image=.{1,}\\n").Value.Replace("|image=", string.Empty).Replace("\n", string.Empty).Replace(" ", "_")));
                    var imageUrl = string.Concat("https://wikimon.net",
                        Regex.Match(Regex.Match(imageDownloadString, "<img.{1,}src=\"[^ \"]{1,}\"").Value,
                            "src=\"[^ \"]{1,}\"").Value.Replace("src=", string.Empty).Replace("\"", string.Empty));
                    digimon.ImageUrl = imageUrl;
                }
                else if (Regex.IsMatch(downloadString, "#REDIRECT\\s\\[\\[.{1,}]]") && !retry)
                {
                    var newName = Regex.Match(downloadString, "#REDIRECT\\s\\[\\[.{1,}]]").Value.Replace("#REDIRECT [[", string.Empty).Replace("]]", string.Empty);
                    digimon.Name = newName;
                    MakeWebRequest(digimon, true);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private static void GetVirutalPetAppearances(Digimon digimon, string downloadString)
        {
            if (Regex.IsMatch(downloadString, "==Virtual Pets==(.*?)[^=]==[^=]", RegexOptions.Singleline))
            {
                var appearances = new List<Appearance>();
                var appearancesText = Regex.Match(downloadString, "==Virtual Pets==(.*?)[^=]==[^=]", RegexOptions.Singleline).Value.Replace("==Evolves To==", string.Empty).Replace("=Appearances", string.Empty);
                foreach (var match in Regex.Matches(appearancesText, "===(.*?)===", RegexOptions.Singleline))
                {
                    appearances.Add(new Appearance
                    {
                        Name = match.ToString().Replace("=", string.Empty).Replace("{", string.Empty).Replace("hdr|", string.Empty).Replace("}", string.Empty),
                        AppearanceCategory = Appearance.AppearanceType.VirtualPet
                    });
                }
                if (digimon.Appearances == null)
                {
                    digimon.Appearances = appearances;
                }
                else
                {
                    digimon.Appearances.AddRange(appearances);
                }
            }
        }

        private static void GetVideoGameAppearances(Digimon digimon, string downloadString)
        {
            if (Regex.IsMatch(downloadString, "==Video Games==(.*?)[^=]==[^=]", RegexOptions.Singleline))
            {
                var appearances = new List<Appearance>();
                var appearancesText = Regex.Match(downloadString, "==Video Games==(.*?)[^=]==[^=]", RegexOptions.Singleline).Value;
                foreach (var match in Regex.Matches(appearancesText, "===(.*?)===", RegexOptions.Singleline))
                {
                    appearances.Add(new Appearance
                    {
                        Name = match.ToString().Replace("=", string.Empty).Replace("{", string.Empty).Replace("hdr|", string.Empty).Replace("}", string.Empty),
                        AppearanceCategory = Appearance.AppearanceType.Game
                    });
                }
                if (digimon.Appearances == null)
                {
                    digimon.Appearances = appearances;
                }
                else
                {
                    digimon.Appearances.AddRange(appearances);
                }
            }
        }

        private static void GetAnimeAppearances(Digimon digimon, string downloadString)
        {
            if (Regex.IsMatch(downloadString, "=Appearances=(.{1,})==Anime==(.*?)[^=]==[^=]", RegexOptions.Singleline))
            {
                var appearances = new List<Appearance>();
                var appearancesText = Regex.Match(downloadString, "=Appearances=(.{1,})==Anime==(.*?)[^=]==[^=]", RegexOptions.Singleline).Value;
                foreach (var match in Regex.Matches(appearancesText, "===(.*?)===", RegexOptions.Singleline))
                {
                    appearances.Add(new Appearance
                    {
                        Name = match.ToString().Replace("=", string.Empty).Replace("{", string.Empty).Replace("hdr|", string.Empty).Replace("}", string.Empty),
                        AppearanceCategory = Appearance.AppearanceType.Anime
                    });
                }
                digimon.Appearances = appearances;
            }
        }

        private static void GetEvolutions(Digimon digimon, string downloadString)
        {
            if (Regex.IsMatch(downloadString, "==Evolves To==(.{1,})=Appearances", RegexOptions.Singleline))
            {
                var possibleDigivolutions = new List<string>();
                var digivolutionText = Regex.Match(downloadString, "==Evolves To==(.{1,})=Appearances", RegexOptions.Singleline).Value.Replace("==Evolves To==", string.Empty).Replace("=Appearances", string.Empty);
                foreach (var match in Regex.Matches(digivolutionText, "\\*([^\\]]{1,})]", RegexOptions.Singleline))
                {
                    possibleDigivolutions.Add(match.ToString().Replace("* ", string.Empty).Replace("'''", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty));
                }
                digimon.Digivolutions = possibleDigivolutions;
            }
        }

        public void SetNumbers(List<Digimon> digimonList)
        {
            throw new NotImplementedException();
        }
    }
}
