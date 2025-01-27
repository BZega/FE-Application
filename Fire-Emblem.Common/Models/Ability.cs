using Fire_Emblem.Common.TypeCodes;
namespace Fire_Emblem.Common.Models
{
    public class Ability
    {
        public int Id { get; set; }
        public string? AbilityOid { get; set; }
        public string Name { get; set; }
        public int LevelGained { get; set; }
        public AbilityType AbilityType { get; set; }
        public bool NeedsToInitiateCombat { get; set; } = false;
        public string Description { get; set; }
        public StatBonus? StatBonus { get; set; }
    }
}
