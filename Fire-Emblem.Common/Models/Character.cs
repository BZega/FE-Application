using Fire_Emblem.Common.TypeCodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Fire_Emblem.Common.Models
{
    public class Character
    {
        public int Id { get; set; }
        public int Level { get; set; } = 1;
        public int InternalLevel { get; set; } = 1;
        public bool IsInCombat { get; set; } = false;
        public bool IsAttacking { get; set; } = false;
        public int Exp { get; set; } = 0;
        public int Gold { get; set; } = 1000;
        public int Attack => GetAttack();
        public int Heal => GetHeal();
        public int Damage => GetDamage();
        public int Crit => GetCrit();
        public int Avoid => GetAvoid();
        public int Dodge => GetDodge();
        public string StartingClass { get; set; }
        public string HeartSealClass { get; set; }
        public UnitClass CurrentClass { get; set; }
        public Flaw Flaw { get; set; }
        public Asset Asset { get; set; }
        public Condition Condition { get; set; }
        public Terrain Terrain { get; set; }
        public Biography Biography { get; set; }
        public PersonalAbility PersonalAbility { get; set; }
        public Inventory Inventory { get; set; }
        public Equipment? EquippedWeapon { get; set; }
        public Stats BaseStats => GetBaseStats();
        public Stats CurrentStats => GetCurrentStats();
        public Stats StatChangeAmount => GetStatChangeAmount();
        public Stats MaxStats => GetMaxStats();
        public List<Levelup>? LevelupStatIncreases { get; set; }
        public GrowthRate PersonalGrowthRate { get; set; }
        public GrowthRate TotalGrowthRate => GetTotalGrowthRate();
        public List<UnitType> UnitTypes => GetUnitTypes();
        public List<Weapon> WeaponRanks { get; set; }
        public List<Support>? Supports { get; set; }
        public List<Ability> AcquiredAbilities { get; set; }
        public List<Ability> EquippedAbilities { get; set; }

        public GrowthRate GetTotalGrowthRate()
        {
            var growthRate = new GrowthRate();
            growthRate.Add(CurrentClass.GrowthRate);
            growthRate.Add(PersonalGrowthRate);
            growthRate.Add(Biography.RaceChoice.RacialGrowth);
            foreach (var ability in EquippedAbilities)
            {
                if (ability.StatBonus?.Growths != null && ability.AbilityType == AbilityType.Passive)
                {
                    growthRate.Add(ability.StatBonus.Growths);
                }
            }
            return growthRate;
        }

        public Stats GetBaseStats()
        {
            var baseStats = new Stats();
            baseStats.Add(CurrentClass.BaseStats);
            if (LevelupStatIncreases != null && LevelupStatIncreases.Count > 0)
            {
                foreach (var levelup in LevelupStatIncreases)
                {
                    baseStats.Add(levelup.StatIncrease);
                }
                baseStats.MaximumCheck(MaxStats);
            }
            return baseStats;
        }

        public Stats GetMaxStats()
        {
            var maxStats = new Stats();
            maxStats.Add(CurrentClass.MaxStats);
            maxStats.Add(Asset.MaxStatBonus);
            maxStats.Add(Flaw.MaxStatBonus);
            return maxStats;
        }

        public Stats GetCurrentStats()
        {
            var currentStats = BaseStats;
            currentStats.Add(Condition.StatChange);
            if (Terrain.TerrainType != TerrainType.None)
            {
                int.TryParse(Terrain.DefBonus, out var defBonus);
                currentStats.Def += defBonus;
            }
            foreach (var ability in EquippedAbilities)
            {
                if (ability.StatBonus?.Stats != null && ability.AbilityType == AbilityType.Passive)
                {
                    currentStats.Add(ability.StatBonus.Stats);
                }
                else if (ability.StatBonus?.Stats != null && ability.AbilityType == AbilityType.Combat && IsInCombat)
                {
                    currentStats.Add(ability.StatBonus.Stats);
                }
            }
            if (Supports != null && Supports.Count > 0)
            {
                foreach (var support in Supports)
                {
                    if (support.IsPairedUp && support.PairedUpBonus?.Stats != null)
                    {
                        currentStats.Add(support.PairedUpBonus.Stats);
                    }
                }
            }
            currentStats.Add(Condition.StatChange);
            if (EquippedWeapon != null && EquippedWeapon.StatBonus?.Stats != null)
            {
                currentStats.Add(EquippedWeapon.StatBonus.Stats);
            }
            return currentStats;
        }

        public Stats GetStatChangeAmount()
        {
            var statChangeAmount = CurrentStats;
            var baseStats = BaseStats;
            statChangeAmount.Subtract(baseStats);
            return statChangeAmount;
        }

        public List<UnitType> GetUnitTypes()
        {
            List<UnitType> unitTypes = new List<UnitType>();
            foreach (var unitType in CurrentClass.UnitTypes)
            {
                if (!Biography.RaceChoice.UnitType.Equals(unitType))
                {
                    unitTypes.Add(unitType);
                }
            }
            unitTypes.Add(Biography.RaceChoice.UnitType);
            return unitTypes;
        }
        public int GetAttack()
        {
            var attack = 0;
            attack += (CurrentStats.Skl * 3 + CurrentStats.Lck) / 2;
            if (EquippedWeapon != null && EquippedWeapon.Hit != null)
            {
                attack += EquippedWeapon.Hit.Value;
                if (EquippedWeapon.StatBonus?.Attributes != null)
                {
                    attack += EquippedWeapon.StatBonus.Attributes.Hit;
                }
                if (WeaponRanks.Any(weapon => weapon.WeaponType == EquippedWeapon.WeaponType && !weapon.IsActive))
                {
                    attack -= 20; 
                }
                if (EquippedWeapon.WeaponType == WeaponType.DarkTome && WeaponRanks.Any(weapon => weapon.WeaponType == WeaponType.DarkTome && !weapon.IsActive) && WeaponRanks.Any(weapon => weapon.WeaponType == WeaponType.Tome && !weapon.IsActive))
                {
                    attack -= 20;
                }
                if (WeaponRanks.Any(weapon => weapon.WeaponType == EquippedWeapon.WeaponType && weapon.WeaponRank != EquippedWeapon.Rank && weapon.IsActive))
                {
                    var weaponRank = WeaponRanks.Where(weapon => weapon.WeaponType == EquippedWeapon.WeaponType).FirstOrDefault();
                    switch (weaponRank.WeaponRank)
                    {
                        case Rank.S:
                            break;
                        case Rank.A:
                            switch (EquippedWeapon.Rank)
                            {
                                case Rank.S:
                                    attack -= 10;
                                    break;
                            }
                            break;
                        case Rank.B:
                            switch (EquippedWeapon.Rank)
                            {
                                case Rank.S:
                                    attack -= 20;
                                    break;
                                case Rank.A:
                                    attack -= 10;
                                    break;
                            }
                            break;
                        case Rank.C:
                            switch (EquippedWeapon.Rank)
                            {
                                case Rank.S:
                                    attack -= 30;
                                    break;
                                case Rank.A:
                                    attack -= 20;
                                    break;
                                case Rank.B:
                                    attack -= 10;
                                    break;
                            }
                            break;
                        case Rank.D:
                            switch (EquippedWeapon.Rank)
                            {
                                case Rank.S:
                                    attack -= 40;
                                    break;
                                case Rank.A:
                                    attack -= 30;
                                    break;
                                case Rank.B:
                                    attack -= 20;
                                    break;
                                case Rank.C:
                                    attack -= 10;
                                    break;
                            }
                            break;
                        case Rank.E:
                            switch (EquippedWeapon.Rank)
                            {
                                case Rank.S:
                                    attack -= 50;
                                    break;
                                case Rank.A:
                                    attack -= 40;
                                    break;
                                case Rank.B:
                                    attack -= 30;
                                    break;
                                case Rank.C:
                                    attack -= 20;
                                    break;
                                case Rank.D:
                                    attack -= 10;
                                    break;
                            }
                            break;
                    }
                } 
                else if (WeaponRanks.Any(weapon => weapon.WeaponType == EquippedWeapon.WeaponType && weapon.WeaponRank != EquippedWeapon.Rank && !weapon.IsActive))
                {
                    var weaponRank = WeaponRanks.Where(weapon => weapon.WeaponType == EquippedWeapon.WeaponType).FirstOrDefault();
                    weaponRank.WeaponExperience = weaponRank.WeaponExperience / 2;
                    switch (weaponRank.WeaponRank)
                    {
                        case Rank.S:
                            break;
                        case Rank.A:
                            switch (EquippedWeapon.Rank)
                            {
                                case Rank.S:
                                    attack -= 10;
                                    break;
                            }
                            break;
                        case Rank.B:
                            switch (EquippedWeapon.Rank)
                            {
                                case Rank.S:
                                    attack -= 20;
                                    break;
                                case Rank.A:
                                    attack -= 10;
                                    break;
                            }
                            break;
                        case Rank.C:
                            switch (EquippedWeapon.Rank)
                            {
                                case Rank.S:
                                    attack -= 30;
                                    break;
                                case Rank.A:
                                    attack -= 20;
                                    break;
                                case Rank.B:
                                    attack -= 10;
                                    break;
                            }
                            break;
                        case Rank.D:
                            switch (EquippedWeapon.Rank)
                            {
                                case Rank.S:
                                    attack -= 40;
                                    break;
                                case Rank.A:
                                    attack -= 30;
                                    break;
                                case Rank.B:
                                    attack -= 20;
                                    break;
                                case Rank.C:
                                    attack -= 10;
                                    break;
                            }
                            break;
                        case Rank.E:
                            switch (EquippedWeapon.Rank)
                            {
                                case Rank.S:
                                    attack -= 50;
                                    break;
                                case Rank.A:
                                    attack -= 40;
                                    break;
                                case Rank.B:
                                    attack -= 30;
                                    break;
                                case Rank.C:
                                    attack -= 20;
                                    break;
                                case Rank.D:
                                    attack -= 10;
                                    break;
                            }
                            break;
                    }
                    weaponRank.WeaponExperience = weaponRank.WeaponExperience * 2;
                }
            }
            if (EquippedAbilities != null)
            {
                foreach (var ability in EquippedAbilities)
                {
                    if (ability.AbilityType == AbilityType.Passive && ability.StatBonus?.Attributes != null)
                    {
                        attack += ability.StatBonus.Attributes.Hit;
                    }
                    if (ability.AbilityType == AbilityType.Combat && ability.StatBonus?.Attributes != null && IsInCombat)
                    {
                        if (!ability.NeedsToInitiateCombat)
                        {
                            attack += ability.StatBonus.Attributes.Hit;
                        }
                        if (ability.NeedsToInitiateCombat && IsAttacking)
                        {
                            attack += ability.StatBonus.Attributes.Hit;
                        }
                    }
                }
            }
            if (PersonalAbility.StatBonus?.Attributes != null)
            {
                attack += PersonalAbility.StatBonus.Attributes.Hit;
            }
            if (CurrentClass.InnateBonus?.Attributes != null)
            {
                attack += CurrentClass.InnateBonus.Attributes.Hit;
            }
            return attack;
        }
        public int GetHeal()
        {
            var heal = 0;
            if (EquippedWeapon != null && EquippedWeapon.BaseHP != null && EquippedWeapon.WeaponType == WeaponType.Staff)
            {
                heal += EquippedWeapon.BaseHP.Value;
                foreach (var ability in EquippedAbilities)
                {
                    if (ability.StatBonus?.Attributes != null && ability.AbilityType == AbilityType.Passive)
                    {
                        heal += ability.StatBonus.Attributes.Heal;
                    }
                }
                if (WeaponRanks.Any(weaponRank => weaponRank.WeaponType == WeaponType.Staff && weaponRank.WeaponRankBonus?.Attributes != null))
                {
                    heal += WeaponRanks.Where(weaponRank => weaponRank.WeaponType == WeaponType.Staff).FirstOrDefault().WeaponRankBonus.Attributes.Heal;
                }
            }
            return heal;
        }
        public int GetDamage()
        {
            var damage = 0;
            if (EquippedWeapon != null)
            {
                if (EquippedWeapon.IsMagical)
                {
                    damage += CurrentStats.Mag;

                }
                else 
                {
                    damage += CurrentStats.Str;                  
                }
                if (EquippedWeapon.Might != null)
                {
                    damage += EquippedWeapon.Might.Value;
                }
                if (EquippedWeapon.StatBonus?.Attributes != null)
                {
                    damage += EquippedWeapon.StatBonus.Attributes.Damage;
                }
                if (WeaponRanks.Any(weapon => weapon.WeaponType == EquippedWeapon.WeaponType && !weapon.IsActive))
                {
                    damage -= (int)(damage * .2);
                }
                if (EquippedWeapon.WeaponType == WeaponType.DarkTome && WeaponRanks.Any(weapon => weapon.WeaponType == WeaponType.DarkTome && !weapon.IsActive) && WeaponRanks.Any(weapon => weapon.WeaponType == WeaponType.Tome && !weapon.IsActive))
                {
                    damage -= (int)(damage * .2);
                }
            }
            if (EquippedAbilities != null)
            {
                foreach (var ability in EquippedAbilities)
                {
                    if (ability.AbilityType == AbilityType.Passive && ability.StatBonus?.Attributes != null)
                    {
                        damage += ability.StatBonus.Attributes.Damage;
                    }
                    if (ability.AbilityType == AbilityType.Combat && ability.StatBonus?.Attributes != null && IsInCombat)
                    {
                        if (!ability.NeedsToInitiateCombat)
                        {
                            damage += ability.StatBonus.Attributes.Damage;
                        }
                        if (ability.NeedsToInitiateCombat && IsAttacking)
                        {
                            damage += ability.StatBonus.Attributes.Damage;
                        }
                    }
                }
            }

            return damage;
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
            if (EquippedAbilities != null)
            {
                foreach (var ability in EquippedAbilities)
                {
                    if (ability.AbilityType == AbilityType.Passive && ability.StatBonus?.Attributes != null)
                    {
                        crit += ability.StatBonus.Attributes.Crit;
                    }
                    if (ability.AbilityType == AbilityType.Combat && ability.StatBonus?.Attributes != null && IsInCombat)
                    {
                        if (!ability.NeedsToInitiateCombat)
                        {
                            crit += ability.StatBonus.Attributes.Crit;
                        }
                        if (ability.NeedsToInitiateCombat && IsAttacking)
                        {
                            crit += ability.StatBonus.Attributes.Crit;
                        }
                    }
                }
            }
            if (PersonalAbility.StatBonus?.Attributes != null)
            {
                crit += PersonalAbility.StatBonus.Attributes.Crit;
            }              
            if (CurrentClass.InnateBonus?.Attributes != null)
            {
                crit += CurrentClass.InnateBonus.Attributes.Crit;
            }
            return crit;
        }
        public int GetAvoid()
        {
            var avoid = 0;
            avoid += (CurrentStats.Spd * 3 + CurrentStats.Lck) / 2;
            if (EquippedWeapon != null && EquippedWeapon.StatBonus?.Attributes != null)
            {
                avoid += EquippedWeapon.StatBonus.Attributes.Avoid;
            }
            if (EquippedAbilities != null)
            {
                foreach (var ability in EquippedAbilities)
                {
                    if (ability.AbilityType == AbilityType.Passive && ability.StatBonus?.Attributes != null)
                    {
                        avoid += ability.StatBonus.Attributes.Avoid;
                    }
                    if (ability.AbilityType == AbilityType.Combat && ability.StatBonus?.Attributes != null && IsInCombat)
                    {
                        if (!ability.NeedsToInitiateCombat)
                        {
                            avoid += ability.StatBonus.Attributes.Avoid;
                        }
                        if (ability.NeedsToInitiateCombat && IsAttacking)
                        {
                            avoid += ability.StatBonus.Attributes.Avoid;
                        }
                    }
                }
            }
            if (PersonalAbility.StatBonus?.Attributes != null)
            {
                avoid += PersonalAbility.StatBonus.Attributes.Avoid;
            }
            if (CurrentClass.InnateBonus?.Attributes != null)
            {
                avoid += CurrentClass.InnateBonus.Attributes.Avoid;
            }
            return avoid;
        }
        public int GetDodge()
        {
            var dodge = 0;
            dodge += CurrentStats.Lck;
            if (EquippedWeapon != null && EquippedWeapon.StatBonus?.Attributes != null)
            {
                dodge += EquippedWeapon.StatBonus.Attributes.Dodge;
            }
            if (EquippedAbilities != null)
            {
                foreach (var ability in EquippedAbilities)
                {
                    if (ability.AbilityType == AbilityType.Passive && ability.StatBonus?.Attributes != null)
                    {
                        dodge += ability.StatBonus.Attributes.Dodge;
                    }
                    if (ability.AbilityType == AbilityType.Combat && ability.StatBonus?.Attributes != null && IsInCombat)
                    {
                        if (!ability.NeedsToInitiateCombat)
                        {
                            dodge += ability.StatBonus.Attributes.Dodge;
                        }
                        if (ability.NeedsToInitiateCombat && IsAttacking)
                        {
                            dodge += ability.StatBonus.Attributes.Dodge;
                        }
                    }
                }
            }
            if (PersonalAbility.StatBonus?.Attributes != null)
            {
                dodge += PersonalAbility.StatBonus.Attributes.Dodge;
            }
            if (CurrentClass.InnateBonus?.Attributes != null)
            {
                dodge += CurrentClass.InnateBonus.Attributes.Dodge;
            }
            return dodge;
        }
    }

    public class Condition()
    {
        public ConditionType CurrentCondition { get; set; }
        public Stats StatChange => GetStatChangeBasedOnCondtion();

        public Stats GetStatChangeBasedOnCondtion()
        {
            var statChange = new Stats();
            switch (CurrentCondition)
            {
                case ConditionType.Normal:
                    break;
                case ConditionType.Serious:
                    statChange.HP = -3;
                    statChange.Str = -3;
                    statChange.Mag = -3;
                    statChange.Skl = -3;
                    statChange.Spd = -3;
                    statChange.Lck = -3;
                    statChange.Def = -3;
                    statChange.Res = -3;
                    break;
                case ConditionType.Critical:
                    statChange.HP = -6;
                    statChange.Str = -6;
                    statChange.Mag = -6;
                    statChange.Skl = -6;
                    statChange.Spd = -6;
                    statChange.Lck = -6;
                    statChange.Def = -6;
                    statChange.Res = -6;
                    break;
                case ConditionType.DEAD:
                    statChange.HP = -99;
                    statChange.Str = -99;
                    statChange.Mag = -99;
                    statChange.Skl = -99;
                    statChange.Spd = -99;
                    statChange.Lck = -99;
                    statChange.Def = -99;
                    statChange.Res = -99;
                    break;
            }
            return statChange;
        }
    }
}
