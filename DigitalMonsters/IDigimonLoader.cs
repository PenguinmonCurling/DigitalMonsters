using System.Collections.Generic;

namespace DigitalMonsters
{
    public interface IDigimonLoader
    {
        List<Digimon> Load();
        void SaveDigimon(List<Digimon> digimonList, string fileName = "DigimonList.xml");
        void SetNumbers(List<Digimon> digimonList);
    }
}