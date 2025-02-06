using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.API.Models.Character
{
    public class EnemyDto
    {
        public string CurrentClass { get; set; }
        public string EquippedWeapon { get; set; }
        public int Level { get; set; }
        public int InternalLevel { get; set; }
        public Stats? ManualStatGrowth { get; set; }
        public DifficultyType Difficulty { get; set; }
        public RacialType Race { get; set; }
        public List<string> EquippedAbilites { get; set; }
    }
}
