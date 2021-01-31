using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DigitalMonsters
{
    [DebuggerDisplay("Name = {Name}")]
    public class Digimon
    {
        public string Name { get; set; }
        public int DebutYear { get; set; }
        public string DebutedIn { get; set; }
        public string Level { get; set; }
        public string DubName { get; set; }
        public string ImageUrl { get; set; }
        public int Number { get; set; }
        public List<string> Digivolutions { get; set; }
        public List<Appearance> Appearances { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Attribute { get; set; }

        public string DisplayName
        {
            get
            {
                //return (string.IsNullOrWhiteSpace(DubName) ? Name : DubName).Replace(" ", string.Empty);
                return Name;
            }
        }

        public string DubLevel
        {
            get
            {
                if(string.Equals(Level, "Baby", StringComparison.OrdinalIgnoreCase))
                {
                    return "In-Training";
                }
                else if (string.Equals(Level, "Baby I", StringComparison.OrdinalIgnoreCase))
                {
                    return "Fresh";
                }
                else if (string.Equals(Level, "Baby II", StringComparison.OrdinalIgnoreCase))
                {
                    return "In-Training";
                }
                else if (string.Equals(Level, "Child", StringComparison.OrdinalIgnoreCase))
                {
                    return "Rookie";
                }
                else if (string.Equals(Level, "Adult", StringComparison.OrdinalIgnoreCase))
                {
                    return "Champion";
                }
                else if (string.Equals(Level, "Perfect", StringComparison.OrdinalIgnoreCase))
                {
                    return "Ultimate";
                }
                else if (string.Equals(Level, "Ultimate", StringComparison.OrdinalIgnoreCase))
                {
                    return "Mega";
                }
                else if (string.Equals(Level, "Armor", StringComparison.OrdinalIgnoreCase))
                {
                    return "Armour";
                }
                else
                {
                    return Level;
                }
            }
        }

        public int LevelNumber
        {
            get
            {
                if (string.Equals(Level, "Baby", StringComparison.OrdinalIgnoreCase))
                {
                    return 2;
                }
                else if (string.Equals(Level, "Baby I", StringComparison.OrdinalIgnoreCase))
                {
                    return 1;
                }
                else if (string.Equals(Level, "Baby II", StringComparison.OrdinalIgnoreCase))
                {
                    return 2;
                }
                else if (string.Equals(Level, "Hybrid Child", StringComparison.OrdinalIgnoreCase))
                {
                    return 3;
                }
                else if (string.Equals(Level, "Child", StringComparison.OrdinalIgnoreCase))
                {
                    return 3;
                }
                else if (string.Equals(Level, "Adult", StringComparison.OrdinalIgnoreCase))
                {
                    return 5;
                }
                else if (string.Equals(Level, "Hybrid Human", StringComparison.OrdinalIgnoreCase))
                {
                    return 5;
                }
                else if (string.Equals(Level, "Hybrid Beast", StringComparison.OrdinalIgnoreCase))
                {
                    return 6;
                }
                else if (string.Equals(Level, "Perfect", StringComparison.OrdinalIgnoreCase))
                {
                    return 6;
                }
                else if (string.Equals(Level, "Hybrid Fusion", StringComparison.OrdinalIgnoreCase))
                {
                    return 7;
                }
                else if (string.Equals(Level, "Hybrid Unified", StringComparison.OrdinalIgnoreCase))
                {
                    return 7;
                }
                else if (string.Equals(Level, "Ultimate", StringComparison.OrdinalIgnoreCase))
                {
                    return 7;
                }
                else if (string.Equals(Level, "Hybrid", StringComparison.OrdinalIgnoreCase))
                {
                    return 4;
                }
                else if (string.Equals(Level, "Armor", StringComparison.OrdinalIgnoreCase))
                {
                    return 4;
                }
                else if (string.Equals(Level, "Armour", StringComparison.OrdinalIgnoreCase))
                {
                    return 4;
                }
                else
                {
                    return 0;
                }
            }
        }

        public string DisplayDebut
        {
            get
            {
                var displaydDebut = DebutedIn.Replace("[[", string.Empty).Replace("]]", string.Empty).Replace("{{", string.Empty).Replace("}}", string.Empty);
                if (DebutedIn.IndexOf('|') >= 0)
                {
                    displaydDebut = displaydDebut.Substring(displaydDebut.LastIndexOf('|') + 1);
                }
                return displaydDebut;
            }
        }

        public bool NameCheck(string digimonName)
        {
            return (string.Equals(DisplayName, digimonName, StringComparison.OrdinalIgnoreCase)
                         || (!string.IsNullOrWhiteSpace(DubName) && string.Equals(DubName, digimonName, StringComparison.OrdinalIgnoreCase))
                         || string.Equals(Name, digimonName, StringComparison.OrdinalIgnoreCase)
                         || (!string.IsNullOrWhiteSpace(DubName) && string.Equals(DubName.Replace(" ", string.Empty), digimonName, StringComparison.OrdinalIgnoreCase))
                         || string.Equals(Name.Replace(" ", string.Empty), digimonName, StringComparison.OrdinalIgnoreCase));
        }

        public bool XAntibodyNameCheck(string currentDigimonName)
        {
            return (string.Equals(Name, string.Format("{0} X-Antibody", currentDigimonName), StringComparison.OrdinalIgnoreCase))
                || (string.Equals(Name, string.Format("{0} (X-Antibody)", currentDigimonName), StringComparison.OrdinalIgnoreCase));
        }

        public bool DeXAntibodyNameCheck(string currentDigimonName)
        {
            return string.Equals(Name, currentDigimonName.Replace(" X-Antibody", string.Empty), StringComparison.OrdinalIgnoreCase)
                || string.Equals(Name, currentDigimonName.Replace(" (X-Antibody)", string.Empty), StringComparison.OrdinalIgnoreCase);
        }
    }
}
