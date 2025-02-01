using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.API.Models.Character
{
    public class SupportCharacterDto
    {
        public string Gender { get; set; }
        public int Level { get; set; } = 1;
        public int InternalLevel { get; set; } = 1;
        public ClassType CurrentClass { get; set; }
        public ClassType StartingClass { get; set; }
        public string EquippedWeaponName { get; set; }
        public Stats LevelUpStats { get; set; }
    }
}
