using Fire_Emblem.Common.TypeCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.Models
{
    public class UnitClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsPromoted { get; set; }
        public bool IsSpecialClass { get; set; }
        public Stats BaseStats { get; set; }
        public Stats MaxStats { get; set; }
        public GrowthRate GrowthRate { get; set; }
        public StatBonus? InnateBonus { get; set; }
        public List<UnitType> UnitTypes { get; set; }
        public List<Ability> Abilities { get; set; }
        public List<ClassWeapon> UsableWeapons { get; set; }
        public List<string>? ClassPromotions { get; set; }
        public List<string>? ReclassOptions { get; set; }

    }
}
