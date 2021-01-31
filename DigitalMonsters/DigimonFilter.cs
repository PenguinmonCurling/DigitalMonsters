namespace DigitalMonsters
{
    public class DigimonFilter
    {
        public string NameFilter { get; set; }
        public string AppearanceFilter { get; set; }
        public string LevelFilter { get; set; }
        public Appearance.AppearanceType AppearanceTypeFilter { get; set; }
        public bool AntiFilter { get; set; }
        public string TypeFilter { get; set; }
        public int NumberFilter { get; set; }
        public int YearFilter { get; set; }
        public int YearEndFilter { get; set; }
    }
}