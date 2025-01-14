using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.API.Models.Equipment
{
    public class WeaponDto
    {
        public string Name { get; set; }
        public Rank? Rank { get; set; }
        public WeaponType WeaponType { get; set; }
        public bool IsMagical { get; set; }
        public int? Might { get; set; }
        public int? Hit { get; set; }
        public int? Crit { get; set; }
        public int? Range { get; set; }
        public string Uses { get; set; }
        public string Worth { get; set; }
        public int? WeaponExp { get; set; }
        public string Description { get; set; }
        public StatBonus? StatBonus { get; set; }
    }
}
