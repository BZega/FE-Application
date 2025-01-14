using Fire_Emblem.Common.TypeCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.Models
{
    public class Flaw
    {
        public StatType FlawChoice { get; set; }
        public Stats MaxStatBonus => GetMaxStatBonus(FlawChoice);

        public Stats GetMaxStatBonus(StatType flawChoice)
        {
            Stats maxStatBonus = new Stats();
            switch (flawChoice)
            {
                case StatType.HP:
                    maxStatBonus.Str = -1;
                    maxStatBonus.Mag = -1;
                    maxStatBonus.Lck = -1;
                    maxStatBonus.Def = -1;
                    maxStatBonus.Res = -1;
                    break;
                case StatType.Str:
                    maxStatBonus.Str = -3;
                    maxStatBonus.Skl = -1;
                    maxStatBonus.Def = -1;
                    break;
                case StatType.Mag:
                    maxStatBonus.Mag = -3;
                    maxStatBonus.Spd = -1;
                    maxStatBonus.Res = -1;
                    break;
                case StatType.Skl:
                    maxStatBonus.Str = -1;
                    maxStatBonus.Skl = -3;
                    maxStatBonus.Def = -1;
                    break;
                case StatType.Spd:
                    maxStatBonus.Skl = -1;
                    maxStatBonus.Spd = -3;
                    maxStatBonus.Lck = -1;
                    break;
                case StatType.Lck:
                    maxStatBonus.Str = -1;
                    maxStatBonus.Mag = -1;
                    maxStatBonus.Lck = -3;
                    break;
                case StatType.Def:
                    maxStatBonus.Lck = -1;
                    maxStatBonus.Def = -3;
                    maxStatBonus.Res = -1;
                    break;
                case StatType.Res:
                    maxStatBonus.Mag = -1;
                    maxStatBonus.Lck = -1;
                    maxStatBonus.Res = -3;
                    break;

            }
            return maxStatBonus;
        }
    }
}
