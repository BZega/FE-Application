using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.API.Models.Equipment
{
    public class StaffDto
    {
        public string Name { get; set; }
        public Rank Rank { get; set; }
        public WeaponType WeaponType { get; set; }
        public bool IsMagical { get; set; } = true;
        public int BaseHP { get; set; }
        public int Range { get; set; }
        public string Uses { get; set; }
        public string Worth { get; set; }
        public int WeaponExp { get; set; }
        public int? Experience { get; set; }
        public string Description { get; set; }
        public StatBonus? StatBonus { get; set; }
    }
}
