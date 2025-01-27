using Fire_Emblem.Common.TypeCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.Models
{
    public class Equipment
    {
        public int Id { get; set; }
        public string? EquipOid { get; set; }
        public string Name { get; set; }
        public Rank? Rank { get; set; }
        public WeaponType WeaponType { get; set; }
        public bool IsMagical { get; set; } = false;
        public int? Might { get; set; }
        public int? Hit { get; set; }
        public int? Crit { get; set; }
        public string? Range { get; set; }
        public string Uses { get; set; }
        public string Worth { get; set; }
        public int? WeaponExp { get; set; }
        public int? BaseHP { get; set; }
        public int? Experience { get; set; }
        public bool IsEquipped { get; set; } = false;
        public string Description { get; set; }
        public StatBonus? StatBonus { get; set; }
    }
}
