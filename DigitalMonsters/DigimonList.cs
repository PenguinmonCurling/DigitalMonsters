using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DigitalMonsters
{
    public class DigimonList
    {
        private List<Digimon> _DigimonList;
        private List<Digimon> _DigimonFilteredList;
        private List<string> _InvalidMonsters;

        public IDigimonLoader DigimonLoader;

        public List<Digimon> DigimonCollection
        {
            get { return _DigimonFilteredList; }
        }

        public List<Digimon> UnfilteredDigimonCollection
        {
            get { return _DigimonList; }
        }

        public DigimonList()
        {
            _DigimonList = new List<Digimon>();
            _DigimonFilteredList = new List<Digimon>();
            _InvalidMonsters = new List<string>
            {
"Kodokugumon",
"Ohakadamon",
"Earthdramon",
"Huntermon",
"Punch Narabimon",
"Generamon",
"Holy Digitamamon",
"Technodramon",
"Cheer Galmon",
"Jiko Chuumon",
"Kyukanmon",
"Kita Kitsunemon",
"Urashimamon",
"Amon",
"Umon",
"EtemonChaos",
"Etemon Chaos",
"Black Guardian",
"Blue Guardian",
"Chaos Lord",
"Chaos Greymon",
"GAIA",
"Neo Crimson",
"Silver Guardian",
"VR-Apocaly",
"VR-BWarGrey",
"VR-CDuke",
"VR-Duke",
"VR-Imperial",
"VR-Omega",
"VR-SaintGalgo",
"VR-Sakuya",
"VR-Venom",
"VR-WarGrey",
"Venom Vamdemon Undead",
"Hermmon",
"Lykamon",
"Panimon",
"Kohagurumon",
"Minidekachimon",
"Citramon",
"Dukemon: Chaos Mode",
"Gaiamon",
"Gigantic Numemon"
            };
            DigimonLoader = new DigimonFileLoader(_InvalidMonsters);
        }

        public void LoadDigimon()
        {
            _DigimonList = DigimonLoader.Load();
            _DigimonFilteredList = _DigimonList.Select(x => x).ToList();
        }

        public Digimon GetDigimon(int count)
        {
            return _DigimonList.OrderBy(x => x.DebutYear).ThenBy(y => y.DebutedIn).Skip(count).Take(1).FirstOrDefault();
        }

        public void FilterList(DigimonFilter digimonFilter)
        {
            _DigimonFilteredList = _DigimonList.Where(
                x => digimonFilter.AntiFilter ? !FilterList(digimonFilter, x) : FilterList(digimonFilter, x))
                .ToList();
            DigimonLoader.SetNumbers(_DigimonFilteredList);
        }

        private bool FilterList(DigimonFilter digimonFilter, Digimon x)
        {
            return (string.IsNullOrWhiteSpace(digimonFilter.AppearanceFilter)
                                || x.Appearances.Any(
                                    y => (y.AppearanceCategory == digimonFilter.AppearanceTypeFilter
                                        || digimonFilter.AppearanceTypeFilter == Appearance.AppearanceType.Any)
                                            && Regex.IsMatch(y.AppearanceNameConvert(), digimonFilter.AppearanceFilter, RegexOptions.IgnoreCase)))
                            && (string.IsNullOrWhiteSpace(digimonFilter.LevelFilter) || Regex.IsMatch(x.DubLevel ?? string.Empty, digimonFilter.LevelFilter, RegexOptions.IgnoreCase))
                            && (string.IsNullOrWhiteSpace(digimonFilter.NameFilter) || Regex.IsMatch(x.Name, digimonFilter.NameFilter, RegexOptions.IgnoreCase) || Regex.IsMatch(x.DisplayName, digimonFilter.NameFilter, RegexOptions.IgnoreCase))
                            && (string.IsNullOrWhiteSpace(digimonFilter.TypeFilter) || Regex.IsMatch(x.Type ?? string.Empty, digimonFilter.TypeFilter, RegexOptions.IgnoreCase))
                            && (digimonFilter.NumberFilter <= 0 || x.Number <= digimonFilter.NumberFilter)
                            && (digimonFilter.YearFilter <= 0 || x.DebutYear >= digimonFilter.YearFilter)
                            && (digimonFilter.YearEndFilter <= 0 || x.DebutYear <= digimonFilter.YearEndFilter);
        }

        public bool TryAddDigimon(Digimon digimon)
        {
            if (!DigimonAlreadyExists(digimon.Name))
            {
                var webloader = new DigimonWebLoader(new List<string>());
                webloader.LoadImage(digimon);
                if (!string.IsNullOrEmpty(digimon.ImageUrl))
                {
                    DigimonCollection.Add(digimon);
                    UnfilteredDigimonCollection.Add(digimon);
                    return true;
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool DigimonAlreadyExists(string name)
        {
            return DigimonCollection.Any(x => x.NameCheck(name));
        }

        public bool IsInvalidMonster(string name)
        {
            return _InvalidMonsters.Contains(name);
        }
    }
}
