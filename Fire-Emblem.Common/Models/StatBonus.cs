using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.Models
{
    public class StatBonus
    {
        public GrowthRate? Growths { get; set; }
        public Attributes? Attributes { get; set; }
        public Stats? Stats { get; set; }
        
    }

    public class Attributes
    {
        public int Hit { get; set; } = 0;
        public int Crit { get; set; } = 0;
        public int Avoid { get; set; } = 0;
        public int Dodge { get; set; } = 0;
        public int Heal { get; set; } = 0;
        public int Damage { get; set; } = 0;
        public int DamageReceived { get; set; } = 0;
        public int AttackSpeed { get; set; } = 0;
    }
}
