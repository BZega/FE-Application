using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.API.Models.Equipment
{
    public class ItemDto
    {
        public string Name { get; set; }
        public WeaponType WeaponType { get; set; }
        public string Uses { get; set; } 
        public string Worth { get; set; }
        public string Description { get; set; }
        public StatBonus? StatBonus { get; set; }
    }
}
