using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DigitalMonsters
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //CompareLists();
            //CompareMedals();
            //CompareNewList();
            //CompareReferenceList();

            Application.Run(new DigimonAnalyser());
        }

        private static void CompareReferenceList()
        {
            var digimonList = new DigimonList();
            digimonList.LoadDigimon();
            var directory = Directory.GetCurrentDirectory();
            var listFilePath = Path.Combine(directory, "InRefeBook.txt");
            var referenceList = new DigimonList();
            foreach (var line in File.ReadAllLines(listFilePath))
            {
                if (!String.IsNullOrWhiteSpace(line))
                {
                    referenceList.DigimonCollection.Add(new Digimon { Name = line.Trim() });
                }
            }
            var notInMyList = referenceList.DigimonCollection.Where(x => !digimonList.DigimonAlreadyExists(x.Name)
                && !x.Name.Contains("X-Antibody")
                && !x.Name.StartsWith("Bio")
                && !digimonList.IsInvalidMonster(x.Name));
            var notInRefList = digimonList.DigimonCollection.Where(x => !referenceList.DigimonAlreadyExists(x.Name)
                && !x.Name.Contains("X-Antibody")
                && !x.Name.StartsWith("Bio")
                && !digimonList.IsInvalidMonster(x.Name));
            var y = 5;
        }

        private static void CompareNewList()
        {
            var webloader = new DigimonWebLoader(new List<string>());
            var digimon = webloader.Load();
            var digimonList = new DigimonList();
            digimonList.LoadDigimon();
            var notInList = digimon.Where(x => !digimonList.DigimonAlreadyExists(x.Name)
                && !x.Name.Contains("X-Antibody")
                && !x.Name.StartsWith("Bio")
                && !digimonList.IsInvalidMonster(x.Name));
            var y = 5;
        }

        private static void DeleteAllEvolutions()
        {
            var digimonList = new DigimonList();
            digimonList.LoadDigimon();
            var fileLoader = new DigimonFileLoader(new List<string>());
            foreach (var digimon in digimonList.DigimonCollection)
            {
                digimon.Digivolutions.Clear();
            }
            fileLoader.SaveDigimon(digimonList.DigimonCollection);
        }

        private static void UpdateData()
        {
            var digimonList = new DigimonList();
            digimonList.LoadDigimon();
            var webloader = new DigimonWebLoader(new List<string>());
            var fileLoader = new DigimonFileLoader(new List<string>());
            foreach (var digimon in digimonList.DigimonCollection.Where(x => x.Number >= 100 && x.Number <= 457))
            {
                webloader.LoadImage(digimon);
            }
            fileLoader.SaveDigimon(digimonList.DigimonCollection);
        }

        private static void GetSaversDigimon()
        {
            var webloader = new DigimonWebLoader(new List<string>());
            var digimon = webloader.Load();
            LoadData(digimon);
        }

        private static void LoadData(IEnumerable<Digimon> digimons)
        {
            var digimonList = new DigimonList();
            digimonList.LoadDigimon();
            var webloader = new DigimonWebLoader(new List<string>());
            var fileLoader = new DigimonFileLoader(new List<string>());
            foreach (var digimon in digimons)
            {
                webloader.LoadImage(digimon);
                digimonList.DigimonCollection.Add(digimon);
            }
            fileLoader.SaveDigimon(digimonList.DigimonCollection);
        }

        private static void CompareLists()
        {
            var digimonList = new DigimonList();
            digimonList.LoadDigimon();

            var numberedDigimon = new List<string>();
            var directory = Directory.GetCurrentDirectory();
            var listFilePath = Path.Combine(directory, "DigimonNumbered.txt");
            foreach (var line in File.ReadAllLines(listFilePath))
            {
                var row = line.Split(',');
                if (row.Length > 1)
                {
                    if (int.TryParse(row[0], out int number) && number <= 351)
                    {
                        numberedDigimon.Add(row[1]);
                    }
                }
            }

            var unnumberedDigimon = new List<string>();
            foreach (var digimon in digimonList.DigimonCollection.Where(x => x.Number <= 361 && x.Name.IndexOf("X-Antibody", StringComparison.OrdinalIgnoreCase) < 0))
            {
                if (!numberedDigimon.Any(x =>
                    string.Equals(x, digimon.DisplayName, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(x, digimon.Name, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(x, digimon.DubName, StringComparison.OrdinalIgnoreCase)))
                {
                    unnumberedDigimon.Add(digimon.DisplayName);
                }
                else
                {
                    numberedDigimon.Remove(numberedDigimon.FirstOrDefault(x =>
                    string.Equals(x, digimon.DisplayName, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(x, digimon.Name, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(x, digimon.DubName, StringComparison.OrdinalIgnoreCase)));
                }
            }

            Console.WriteLine(string.Join(",", unnumberedDigimon));
            Console.WriteLine(string.Join(",", numberedDigimon));
        }

        private static void CompareCards()
        {
            var digimonList = new DigimonList();
            digimonList.LoadDigimon();

            var digimonInCards = new List<string>();
            var directory = Directory.GetCurrentDirectory();
            var listFilePath = Path.Combine(directory, "DigimonCards.txt");
            foreach (var line in File.ReadAllLines(listFilePath))
            {
                if (line.IndexOf("mon") > 0)
                {
                    digimonInCards.Add(line.Replace("||", string.Empty).Trim());
                }
            }
            digimonInCards = digimonInCards.Distinct().ToList();

            var unnumberedDigimon = new List<string>();
            foreach (var digimon in digimonList.DigimonCollection.Where(x => x.Number <= 458
                && x.Name.IndexOf("X-Antibody", StringComparison.OrdinalIgnoreCase) < 0))
            {
                if (!digimonInCards.Any(x =>
                    string.Equals(x, digimon.DisplayName, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(x, digimon.Name, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(x, digimon.DubName, StringComparison.OrdinalIgnoreCase)))
                {
                    unnumberedDigimon.Add(digimon.DisplayName);
                }
                else
                {
                    digimonInCards.Remove(digimonInCards.FirstOrDefault(x =>
                    string.Equals(x, digimon.DisplayName, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(x, digimon.Name, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(x, digimon.DubName, StringComparison.OrdinalIgnoreCase)));
                }
            }

            Console.WriteLine(string.Join(",", unnumberedDigimon));
            Console.WriteLine(string.Join(",", digimonInCards));
        }

        private static void UpdateLevel()
        {
            var digimonList = new DigimonList();
            digimonList.LoadDigimon();

            var armourLoader = new DigimonArmourLoader();
            var armourChart = armourLoader.LoadArmourData();

            foreach(var chart in armourChart)
            {
                foreach(var digimon in chart.DigimentalDigivolution.Select(x => x.Value))
                {
                    var digimonInList = digimonList.DigimonCollection.FirstOrDefault(x => 
                        string.Equals(x.Name, digimon, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(x.DubName, digimon, StringComparison.OrdinalIgnoreCase));
                    if (digimonInList != null)
                    {
                        digimonInList.Level = "Armour";
                    }
                }
            }

            digimonList.DigimonLoader.SaveDigimon(digimonList.DigimonCollection);
        }

        private static void RemoveDuplicates()
        {
            var digimonList = new DigimonList();
            digimonList.LoadDigimon();
            var fileLoader = new DigimonFileLoader(new List<string>());
            var digimonToRemove = new List<Digimon>();
            foreach(var digimon in digimonList.DigimonCollection)
            {
                if(digimonList.DigimonCollection.Any(x => string.Equals(x.Name, digimon.Name) && digimon.Number != x.Number && string.IsNullOrEmpty(digimon.Type)))
                {
                    digimonToRemove.Add(digimon);
                }
            }
            foreach (var digimon in digimonToRemove)
            {
                digimonList.DigimonCollection.Remove(digimon);
            }
            fileLoader.SaveDigimon(digimonList.DigimonCollection);
        }

        private static void CompareMedals()
        {
            var digimonList = new DigimonList();
            digimonList.LoadDigimon();
            var directory = Directory.GetCurrentDirectory();
            var listFilePath = Path.Combine(directory, "DigimonMedals.txt");
            var file = File.ReadAllText(listFilePath);
            var medallLess = new List<string>();

            foreach(var digimon in digimonList.DigimonCollection.OrderBy(x => x.Number))
            {
                if (!(file.IndexOf(digimon.Name, StringComparison.OrdinalIgnoreCase) >= 0) && !(file.Replace(" ", string.Empty).IndexOf(digimon.Name, StringComparison.OrdinalIgnoreCase) >= 0)  
                    && (string.IsNullOrWhiteSpace(digimon.DubName) 
                    || (!(file.IndexOf(digimon.DubName, StringComparison.OrdinalIgnoreCase) >= 0) && !(file.Replace(" ", string.Empty).IndexOf(digimon.DubName, StringComparison.OrdinalIgnoreCase) >= 0)
                    )))
                {
                    medallLess.Add(digimon.Name);
                }
            }
            Console.WriteLine(string.Join(",", medallLess));
        }
    }
}
