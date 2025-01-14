using Fire_Emblem.Common.TypeCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.Models
{
    public class Ability
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LevelGained { get; set; }
        public AbilityType AbilityType { get; set; }
        public bool NeedsToInitiateCombat { get; set; } = false;
        public string Description { get; set; }
        public StatBonus? StatBonus { get; set; }
    }
}
