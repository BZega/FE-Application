using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.API.Models.UnitClass
{
    public class UnitClassDto
    {
        public string Name { get; set; }
        public bool IsPromoted { get; set; }
        public bool IsSpecialClass { get; set; }
        public Stats BaseStats { get; set; }
        public Stats MaxStats { get; set; }
        public GrowthRate GrowthRate { get; set; }
        public StatBonus? InnateBonus { get; set; }
        public List<UnitType> UnitTypes { get; set; }
        public List<string> Abilities { get; set; }
        public List<ClassWeapon> UsableWeapons { get; set; }
        public List<string>? ClassPromotions { get; set; }
        public List<string>? ReclassOptions { get; set; }
        public List<SkillType> SkillTypeOptions { get; set; }
    }
}
