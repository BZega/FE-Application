using Fire_Emblem.Common.TypeCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Fire_Emblem.Common.Models
{
    public class Support 
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public int Level { get; set; } = 1;
        public int InternalLevel { get; set; } = 1;
        public ClassType CurrentClassType { get; set; }
        public UnitClass CurrentClass { get; set; }
        public Equipment EquippedWeapon { get; set; }
        public string StartingClass { get; set; }
        public bool IsPairedUp { get; set; } = false;
        public bool IsClose { get; set; } = false;
        public Stats LevelUpStats { get; set; }
        public Stats CurrentStats => GetCurrentStats();
        public Stats PairedUpBonus => GetPairedUpBonus();
        public Rank SupportRank => GetSupportRank();
        public int Crit => GetCrit();
        public int SupportPoints { get; set; } = 0;
        public PersonalAbility PersonalAbility { get; set; }

        public Rank GetSupportRank()
        {
            if (SupportPoints > 17 && SupportPoints < 45)
            {
                return Rank.C;
            }
            else if (SupportPoints > 44 && SupportPoints < 81)
            {
                return Rank.B;
            }
            else if (SupportPoints > 80 && SupportPoints < 126)
            {
                return Rank.A;
            }
            else if (SupportPoints > 125)
            {
                return Rank.S;
            }
            else
            {
                return Rank.None;
            }
        }

        public Stats GetCurrentStats()
        {
            Stats currentStats = new Stats();
            currentStats.Add(CurrentClass.BaseStats);
            currentStats.Add(LevelUpStats);
            currentStats.MaximumCheck(CurrentClass.MaxStats);
            if (EquippedWeapon.StatBonus?.Stats != null)
            {
                currentStats.Add(EquippedWeapon.StatBonus.Stats);
            }
            return currentStats;
        }

        public Stats GetPairedUpBonus()
        {
            Stats statBonus = new Stats();
            if (CurrentStats.Str > 9)
            {
                statBonus.Str += 1;
            }
            if (CurrentStats.Str > 19)
            {
                statBonus.Str += 1;
            }
            if (CurrentStats.Str > 29)
            {
                statBonus.Str += 1;
            }
            if (CurrentStats.Mag > 9)
            {
                statBonus.Mag += 1;
            }
            if (CurrentStats.Mag > 19)
            {
                statBonus.Mag += 1;
            }
            if (CurrentStats.Mag > 29)
            {
                statBonus.Mag += 1;
            }
            if (CurrentStats.Skl > 9)
            {
                statBonus.Skl += 1;
            }
            if (CurrentStats.Skl > 19)
            {
                statBonus.Skl += 1;
            }
            if (CurrentStats.Skl > 29)
            {
                statBonus.Skl += 1;
            }
            if (CurrentStats.Spd > 9)
            {
                statBonus.Spd += 1;
            }
            if (CurrentStats.Spd > 19)
            {
                statBonus.Spd += 1;
            }
            if (CurrentStats.Spd > 29)
            {
                statBonus.Spd += 1;
            }
            if (CurrentStats.Lck > 9)
            {
                statBonus.Lck += 1;
            }
            if (CurrentStats.Lck > 19)
            {
                statBonus.Lck += 1;
            }
            if (CurrentStats.Lck > 29)
            {
                statBonus.Lck += 1;
            }
            if (CurrentStats.Def > 9)
            {
                statBonus.Def += 1;
            }
            if (CurrentStats.Def > 19)
            {
                statBonus.Def += 1;
            }
            if (CurrentStats.Def > 29)
            {
                statBonus.Def += 1;
            }
            if (CurrentStats.Res > 9)
            {
                statBonus.Res += 1;
            }
            if (CurrentStats.Res > 19)
            {
                statBonus.Res += 1;
            }
            if (CurrentStats.Res > 29)
            {
                statBonus.Res += 1;
            }

            switch (CurrentClassType)
            {
                case ClassType.Apothecary:
                case ClassType.SpearFighter:
                    statBonus.Str += 2;
                    statBonus.Skl += 2;
                    statBonus.Spd += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                    }
                    break;
                case ClassType.Archer:
                    statBonus.Str += 2;
                    statBonus.Skl += 2;
                    statBonus.Def += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Skl += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Skl += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.Assassin:
                    statBonus.Str += 2;
                    statBonus.Skl += 2;
                    statBonus.Spd += 4;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                    }
                    break;
                case ClassType.Ballistician:
                    statBonus.Str += 4;
                    statBonus.Skl += 3;
                    statBonus.Lck += 1;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Skl += 1;
                        statBonus.Lck += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Skl += 2;
                        statBonus.Lck += 2;
                    }
                    break;
                case ClassType.Basara:
                    statBonus.Str += 1;
                    statBonus.Mag += 1;
                    statBonus.Skl += 1;
                    statBonus.Spd += 1;
                    statBonus.Lck += 4;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Mag += 1;
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                        statBonus.Lck += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Mag += 2;
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                        statBonus.Lck += 2;
                    }
                    break;
                case ClassType.Berserker:
                    statBonus.Str += 5;
                    statBonus.Skl += 2;
                    statBonus.Spd += 1;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                    }
                    break;
                case ClassType.Blacksmith:
                case ClassType.SpearMaster:
                    statBonus.Str += 3;
                    statBonus.Skl += 3;
                    statBonus.Spd += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                    }
                    break;
                case ClassType.BowKnight:
                    statBonus.Skl += 3;
                    statBonus.Spd += 3;
                    statBonus.Mov += 1;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                    }
                    break;
                case ClassType.Cavalier:
                    statBonus.Str += 2;
                    statBonus.Skl += 1;
                    statBonus.Spd += 1;
                    statBonus.Def += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.Cleric:
                    statBonus.Mag += 2;
                    statBonus.Lck += 2;
                    statBonus.Res += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Mag += 1;
                        statBonus.Lck += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Mag += 2;
                        statBonus.Lck += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.DarkFlier:
                    statBonus.Mag += 3;
                    statBonus.Spd += 3;
                    statBonus.Res += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Mag += 1;
                        statBonus.Spd += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Mag += 2;
                        statBonus.Spd += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.DarkKnight:
                    statBonus.Mag += 2;
                    statBonus.Def += 3;
                    statBonus.Res += 1;
                    statBonus.Mov += 1;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Mag += 1;
                        statBonus.Def += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Mag += 2;
                        statBonus.Def += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.DarkMage:
                    statBonus.Mag += 3;
                    statBonus.Def += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Mag += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Mag += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.DawnNoble:
                    statBonus.Str += 3;
                    statBonus.Mag += 2;
                    statBonus.Def += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Mag += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Mag += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.DreadFighter:
                    statBonus.Str += 3;
                    statBonus.Mag += 1;
                    statBonus.Spd += 1;
                    statBonus.Res += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Mag += 1;
                        statBonus.Spd += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Mag += 2;
                        statBonus.Spd += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.DuskNoble:
                    statBonus.Str += 2;
                    statBonus.Mag += 3;
                    statBonus.Res += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Mag += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Mag += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.FalconKnight:
                    statBonus.Spd += 4;
                    statBonus.Res += 4;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Spd += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Spd += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.Fighter:
                    statBonus.Str += 3;
                    statBonus.Skl += 2;
                    statBonus.Spd += 1;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                    }
                    break;
                case ClassType.General:
                    statBonus.Str += 3;
                    statBonus.Def += 5;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.Grandmaster:
                    statBonus.Str += 2;
                    statBonus.Mag += 2;
                    statBonus.Skl += 2;
                    statBonus.Spd += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Mag += 1;
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Mag += 2;
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                    }
                    break;
                case ClassType.GreatKnight:
                    statBonus.Str += 3;
                    statBonus.Def += 3;
                    statBonus.Mov += 1;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.GreatLord:
                    statBonus.Spd += 3;
                    statBonus.Lck += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Spd += 1;
                        statBonus.Lck += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Spd += 2;
                        statBonus.Lck += 2;
                    }
                    break;
                case ClassType.GreatMaster:
                    statBonus.Str += 3;
                    statBonus.Spd += 3;
                    statBonus.Lck += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Spd += 1;
                        statBonus.Lck += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Spd += 2;
                        statBonus.Lck += 2;
                    }
                    break;
                case ClassType.GriffonRider:
                    statBonus.Str += 3;
                    statBonus.Lck += 1;
                    statBonus.Def += 2;
                    statBonus.Mov += 1;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Lck += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Lck += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.Hero:
                    statBonus.Skl += 3;
                    statBonus.Spd += 3;
                    statBonus.Def += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.KinshiKnight:
                    statBonus.Skl += 2;
                    statBonus.Spd += 2;
                    statBonus.Lck += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                        statBonus.Lck += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                        statBonus.Lck += 2;
                    }
                    break;
                case ClassType.Kitsune:
                    statBonus.Spd += 3;
                    statBonus.Res += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Spd += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Spd += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.Knight:
                    statBonus.Str += 2;
                    statBonus.Def += 4;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.Lodestar:
                    statBonus.Str += 2;
                    statBonus.Skl += 3;
                    statBonus.Spd += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                    }
                    break;
                case ClassType.Lord:
                    statBonus.Skl += 2;
                    statBonus.Spd += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                    }
                    break;
                case ClassType.Mage:
                    statBonus.Mag += 4;
                    statBonus.Skl += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Mag += 1;
                        statBonus.Skl += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Mag += 2;
                        statBonus.Skl += 2;
                    }
                    break;
                case ClassType.Maid:
                    statBonus.Str += 1;
                    statBonus.Mag += 1;
                    statBonus.Skl += 2;
                    statBonus.Spd += 2;
                    statBonus.Lck += 1;
                    statBonus.Res += 1;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Mag += 1;
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                        statBonus.Lck += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Mag += 2;
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                        statBonus.Lck += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.MaligKnight:
                    statBonus.Str += 2;
                    statBonus.Mag += 2;
                    statBonus.Res += 4;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Mag += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Mag += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.Manakete:
                    statBonus.Str += 2;
                    statBonus.Mag += 2;
                    statBonus.Def += 2;
                    statBonus.Res += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Mag += 1;
                        statBonus.Def += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Mag += 2;
                        statBonus.Def += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.ManaketeHeir:
                    statBonus.Str += 2;
                    statBonus.Mag += 1;
                    statBonus.Skl += 1;
                    statBonus.Spd += 1;
                    statBonus.Lck += 1;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Mag += 1;
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                        statBonus.Lck += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Mag += 2;
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                        statBonus.Lck += 2;
                    }
                    break;
                case ClassType.MasterNinja:
                    statBonus.Skl += 3;
                    statBonus.Spd += 3;
                    statBonus.Res += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.MasterOfArms:
                    statBonus.Str += 4;
                    statBonus.Skl += 2;
                    statBonus.Spd += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                    }
                    break;
                case ClassType.Mechanist:
                    statBonus.Skl += 3;
                    statBonus.Spd += 2;
                    statBonus.Res += 2;
                    statBonus.Mov += 1;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.Mercenary:
                    statBonus.Skl += 2;
                    statBonus.Spd += 3;
                    statBonus.Def += 1;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.Merchant:
                    statBonus.Str += 3;
                    statBonus.Skl += 2;
                    statBonus.Lck += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Skl += 1;
                        statBonus.Lck += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Skl += 2;
                        statBonus.Lck += 2;
                    }
                    break;
                case ClassType.Myrmidon:
                    statBonus.Spd += 4;
                    statBonus.Lck += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Spd += 1;
                        statBonus.Lck += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Spd += 2;
                        statBonus.Lck += 2;
                    }
                    break;
                case ClassType.NineTails:
                    statBonus.Spd += 4;
                    statBonus.Res += 4; 
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Spd += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Spd += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.Ninja:
                    statBonus.Skl += 3;
                    statBonus.Spd += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                    }
                    break;
                case ClassType.OniChieftain:
                    statBonus.Str += 3;
                    statBonus.Spd += 1;
                    statBonus.Def += 4;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Spd += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Spd += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.OniSavage:
                    statBonus.Str += 3;
                    statBonus.Def += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.Paladin:
                    statBonus.Str += 2;
                    statBonus.Skl += 2;
                    statBonus.Spd += 2;
                    statBonus.Def += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.PegasusKnight:
                    statBonus.Spd += 3;
                    statBonus.Res += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Spd += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Spd += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.Performer:
                    statBonus.Spd += 3;
                    statBonus.Lck += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Spd += 1;
                        statBonus.Lck += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Spd += 2;
                        statBonus.Lck += 2;
                    }
                    break;
                case ClassType.Sage:
                    statBonus.Mag += 4;
                    statBonus.Skl += 2;
                    statBonus.Res += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Mag += 1;
                        statBonus.Skl += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Mag += 2;
                        statBonus.Skl += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.Sniper:
                    statBonus.Str += 3;
                    statBonus.Skl += 3;
                    statBonus.Def += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Skl += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Skl += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.Sorcerer:
                    statBonus.Mag += 3;
                    statBonus.Def += 2;
                    statBonus.Res += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Mag += 1;
                        statBonus.Def += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Mag += 2;
                        statBonus.Def += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.Strategist:
                    statBonus.Str += 3;
                    statBonus.Spd += 2;
                    statBonus.Res += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Spd += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Spd += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.Swordmaster:
                    statBonus.Spd += 5;
                    statBonus.Lck += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Spd += 1;
                        statBonus.Lck += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Spd += 2;
                        statBonus.Lck += 2;
                    }
                    break;
                case ClassType.Tactician:
                    statBonus.Str += 1;
                    statBonus.Mag += 1;
                    statBonus.Skl += 2;
                    statBonus.Spd += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Mag += 1;
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Mag += 2;
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                    }
                    break;
                case ClassType.Taguel:
                    statBonus.Str += 3;
                    statBonus.Skl += 2;
                    statBonus.Spd += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                    }
                    break;
                case ClassType.Thief:
                    statBonus.Skl += 2;
                    statBonus.Spd += 2;
                    statBonus.Mov += 1;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                    }
                    break;
                case ClassType.Trickster:
                    statBonus.Mag += 2;
                    statBonus.Skl += 1;
                    statBonus.Spd += 3;
                    statBonus.Mov += 1;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Mag += 1;
                        statBonus.Skl += 1;
                        statBonus.Spd += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Mag += 2;
                        statBonus.Skl += 2;
                        statBonus.Spd += 2;
                    }
                    break;
                case ClassType.Troubadour:
                    statBonus.Mag += 2;
                    statBonus.Spd += 1;
                    statBonus.Res += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Mag += 1;
                        statBonus.Spd += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Mag += 2;
                        statBonus.Spd += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.Vanguard:
                    statBonus.Str += 4;
                    statBonus.Lck += 1;
                    statBonus.Def += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Lck += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Lck += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.Villager:
                    statBonus.Skl += 3;
                    statBonus.Lck += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Skl += 1;
                        statBonus.Lck += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Skl += 2;
                        statBonus.Lck += 2;
                    }
                    break;
                case ClassType.WarCleric:
                    statBonus.Str += 2;
                    statBonus.Mag += 2;
                    statBonus.Lck += 2;
                    statBonus.Res += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Mag += 1;
                        statBonus.Lck += 1;
                        statBonus.Res += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Mag += 2;
                        statBonus.Lck += 2;
                        statBonus.Res += 2;
                    }
                    break;
                case ClassType.Warrior:
                    statBonus.Str += 4;
                    statBonus.Skl += 4;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Skl += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Skl += 2;
                    }
                    break;
                case ClassType.Witch:
                    statBonus.Mag += 4;
                    statBonus.Spd += 4;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Mag += 1;
                        statBonus.Spd += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Mag += 2;
                        statBonus.Spd += 2;
                    }
                    break;
                case ClassType.Wolfskin:
                    statBonus.Str += 3;
                    statBonus.Spd += 2;
                    statBonus.Def += 1;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Spd += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Spd += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.Wolfssegner:
                    statBonus.Str += 3;
                    statBonus.Spd += 3;
                    statBonus.Def += 2;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Spd += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Spd += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.WyvernLord:
                    statBonus.Str += 4;
                    statBonus.Def += 4;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Def += 2;
                    }
                    break;
                case ClassType.WyvernRider:
                    statBonus.Str += 3;
                    statBonus.Def += 3;
                    if (SupportRank == Rank.C || SupportRank == Rank.B)
                    {
                        statBonus.Str += 1;
                        statBonus.Def += 1;
                    }
                    if (SupportRank == Rank.A || SupportRank == Rank.S)
                    {
                        statBonus.Str += 2;
                        statBonus.Def += 2;
                    }
                    break;
            }
            return statBonus;
        }

        public int GetCrit()
        {
            var crit = 0;
            crit += CurrentStats.Skl / 2;
            if (EquippedWeapon != null && EquippedWeapon.Crit != null)
            {
                crit += EquippedWeapon.Crit.Value;
                if (EquippedWeapon.StatBonus?.Attributes != null)
                {
                    crit += EquippedWeapon.StatBonus.Attributes.Crit;
                }
            }
            //if (PersonalAbility.StatBonus?.Attributes != null)
            //{
            //    crit += PersonalAbility.StatBonus.Attributes.Crit;
            //}
            //if (CurrentClass.InnateBonus?.Attributes != null)
            //{
            //    crit += CurrentClass.InnateBonus.Attributes.Crit;
            //}
            return crit;
        }
    }
}
