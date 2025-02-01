using Fire_Emblem.Common.TypeCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.Models
{
    public class Race
    {
        public RacialType RacialType { get; set; }
        public List<StatType>? HumanStatChoices { get; set; }
        public GrowthRate RacialGrowth => GetGrowthRate(RacialType);
        public UnitType UnitType => GetUnitType(RacialType);

        public GrowthRate GetGrowthRate(RacialType race)
        {
            var growth = new GrowthRate();
            switch (race)
            {
                case RacialType.Human:
                    if (HumanStatChoices == null)
                    {
                        break;
                    }
                    foreach (var choice in HumanStatChoices)
                    {
                        switch (choice)
                        {
                            case StatType.HP:
                                growth.HP = 10;
                                break;
                            case StatType.Str:
                                growth.Str = 10;
                                break;
                            case StatType.Mag:
                                growth.Mag = 10;
                                break;
                            case StatType.Skl:
                                growth.Skl = 10;
                                break;
                            case StatType.Spd:
                                growth.Spd = 10;
                                break;
                            case StatType.Lck:
                                growth.Lck = 10;
                                break;
                            case StatType.Def:
                                growth.Def = 10;
                                break;
                            case StatType.Res:
                                growth.Res = 10;
                                break;
                        }
                    }
                    break;
                case RacialType.Kitsune:
                    growth.Str = 10;
                    growth.Mag = -10;
                    growth.Spd = 10;
                    growth.Lck = 20;
                    growth.Def = -10;
                    growth.Res = 10;
                    break;
                case RacialType.Manakete:
                    growth.HP = 20;
                    growth.Skl = -10;
                    growth.Spd = -10;
                    growth.Lck = 20;
                    growth.Def = 10;
                    break;
                case RacialType.Taguel:
                    growth.HP = 15;
                    growth.Str = 10;
                    growth.Mag = -10;
                    growth.Skl = 10;
                    growth.Spd = 10;
                    growth.Res = -10;
                    break;
                case RacialType.Wolfskin:
                    growth.HP = 25;
                    growth.Str = 20;
                    growth.Mag = -20;
                    growth.Skl = -5;
                    growth.Lck = 25;
                    growth.Def = 10;
                    growth.Res = -5;
                    break;                
            }            
            return growth;
        }

        public UnitType GetUnitType(RacialType race)
        {
            if (race != RacialType.Human)
            {
                var type = new UnitType();
                switch (race)
                {
                    case RacialType.Manakete:
                        type = TypeCodes.UnitType.Dragon;
                        break;
                    case RacialType.Kitsune:
                    case RacialType.Taguel:
                    case RacialType.Wolfskin:
                        type = TypeCodes.UnitType.Beast;
                        break;
                }
                return type;
            }
            return 0;
        }
    }
}
