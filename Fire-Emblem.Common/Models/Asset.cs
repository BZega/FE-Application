using Fire_Emblem.Common.TypeCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.Models
{
    public class Asset
    {
        public StatType AssetChoice { get; set; }
        public Stats MaxStatBonus => GetMaxStatBonus(AssetChoice);

        public Stats GetMaxStatBonus(StatType assetChoice)
        {
            Stats maxStatBonus = new Stats();
            switch (assetChoice)
            {
                case StatType.HP:
                    maxStatBonus.Str = 1;
                    maxStatBonus.Mag = 1;
                    maxStatBonus.Lck = 2;
                    maxStatBonus.Def = 2;
                    maxStatBonus.Res = 2;
                    break;
                case StatType.Str:
                    maxStatBonus.Str = 4;
                    maxStatBonus.Skl = 2;
                    maxStatBonus.Def = 2;
                    break;
                case StatType.Mag:
                    maxStatBonus.Mag = 4;
                    maxStatBonus.Spd = 2;
                    maxStatBonus.Res = 2;                    
                    break;
                case StatType.Skl:
                    maxStatBonus.Str = 2;
                    maxStatBonus.Skl = 4;
                    maxStatBonus.Def = 2;
                    break;
                case StatType.Spd:
                    maxStatBonus.Skl = 2;
                    maxStatBonus.Spd = 4;
                    maxStatBonus.Lck = 2;
                    break;
                case StatType.Lck:
                    maxStatBonus.Str = 2;
                    maxStatBonus.Mag = 2;
                    maxStatBonus.Lck = 4;
                    break;
                case StatType.Def:
                    maxStatBonus.Lck = 2;
                    maxStatBonus.Def = 4;
                    maxStatBonus.Res = 2;
                    break;
                case StatType.Res:
                    maxStatBonus.Mag = 2;
                    maxStatBonus.Lck = 2;
                    maxStatBonus.Res = 4;   
                    break;
            }
            return maxStatBonus;
        }
    }
}
