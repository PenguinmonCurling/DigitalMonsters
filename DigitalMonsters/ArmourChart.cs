using System;

namespace DigitalMonsters
{
    public class ArmourChart
    {
        public ArmourChart()
        {
            DigimentalDigivolution = new SerialisableDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Digimon { get; set; }
        public SerialisableDictionary<string, string> DigimentalDigivolution { get; set; }
    }
}
