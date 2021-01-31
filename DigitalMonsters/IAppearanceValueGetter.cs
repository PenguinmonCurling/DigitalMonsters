namespace DigitalMonsters
{
    public interface IAppearanceValueGetter
    {
        float ApperanceValue(string appearanceName);

        string AppearanceName(float appearanceValue);
    }
}