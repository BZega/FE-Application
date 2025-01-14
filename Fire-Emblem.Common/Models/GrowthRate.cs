using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.Models
{
    public class GrowthRate
    {
        public int HP { get; set; } = 0;
        public int Str { get; set; } = 0;
        public int Mag { get; set; } = 0;
        public int Skl { get; set; } = 0;
        public int Spd { get; set; } = 0;
        public int Lck { get; set; } = 0;
        public int Def { get; set; } = 0;
        public int Res { get; set; } = 0;

        public void Add(GrowthRate growthRate) 
        { 
            HP += growthRate.HP;
            Str += growthRate.Str;
            Mag += growthRate.Mag;
            Skl += growthRate.Skl;
            Spd += growthRate.Spd;
            Lck += growthRate.Lck;
            Def += growthRate.Def;
            Res += growthRate.Res;
        }
    }
}
