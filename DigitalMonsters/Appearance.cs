using System;
using System.Collections.Generic;

namespace DigitalMonsters
{
    public class Appearance : IEquatable<Appearance>, IEqualityComparer<Appearance>
    {
        public string Name { get; set; }
        private IAppearanceValueGetter apperanceValueGetter;

        public Appearance()
        {
            apperanceValueGetter = new AppearanceValueGetter();
        }

        public AppearanceType AppearanceCategory { get; set; }

        public enum AppearanceType
        {
            Any,
            Anime,
            Game,
            VirtualPet,
            Card,
            Manga
        }

        public float AppearanceValue()
        {
            return apperanceValueGetter.ApperanceValue(Name);
        }

        public string AppearanceNameConvert()
        {
            return apperanceValueGetter.AppearanceName(AppearanceValue());
        }

        public bool Equals(Appearance other)
        {
            return string.Equals(other.Name, this.Name, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(Appearance x, Appearance y)
        {
            return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(Appearance obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}