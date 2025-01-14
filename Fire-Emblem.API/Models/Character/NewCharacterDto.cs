using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.API.Models.Character
{
    public class NewCharacterDto
    {
        public string StartingClass { get; set; }
        public string HeartSealClass { get; set; }
        public string Biography { get; set; }
        public bool IsNoble { get; set; }
        public RacialType RaceChoice { get; set; }
        public StatType AssetChoice { get; set; }
        public StatType FlawChoice { get; set; }
        public string PersonalAbility { get; set; }
        public string? EquippedWeapon { get; set; }
        public string FirstAquiredAbility { get; set; }
        public bool IsAquiredAbilityEquipped { get; set; }
        public List<string>? StartingWeapons { get; set; }
        public List<string>? StartingStaves { get; set; }
        public List<string>? StartingItems { get; set; }
        public GrowthRate PersonalGrowthRate { get; set; }

        public static bool ValidateGrowthRate(GrowthRate growthRate)
        {
            int result = 0;
            result += growthRate.HP;
            result += growthRate.Str;
            result += growthRate.Mag;
            result += growthRate.Skl;
            result += growthRate.Spd;
            result += growthRate.Lck;
            result += growthRate.Def;
            result += growthRate.Res;
            if (result == 330)
            {
                return true;
            } 
            else
            {
                return false;
            }
        }
    }
}
