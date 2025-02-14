using Fire_Emblem.Common.TypeCodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Fire_Emblem.Common.Models
{
    public class Character
    {
        public int Id { get; set; }
        public Biography Biography { get; set; }
        public int CurrentHP { get; set; } = 0;
        public PersonalAbility PersonalAbility { get; set; }
        public Stats CurrentStats => GetCurrentStats();
        public GrowthRate TotalGrowthRate => GetTotalGrowthRate();
        public Equipment EquippedWeapon { get; set; }
        public List<Ability> EquippedAbilities { get; set; }
        public UnitClass CurrentClass { get; set; }
        public int Exp { get; set; } = 0;
        public int Level { get; set; } = 1;
        public List<UnitType> UnitTypes => GetUnitTypes();
        public int Gold { get; set; } = 1000;
        public int Attack => GetAttack();
        public int Heal => GetHeal();
        public int Damage => GetDamage();
        public int Crit => GetCrit();
        public int Avoid => GetAvoid();
        public int Dodge => GetDodge();
        public int DamageReceived => GetDamageReceived();
        public int AttackSpeed => GetAttackSpeed();
        public int DualStrike { get; set; } = 0;
        public int DualGuard { get; set; } = 0;
        public string StartingClass { get; set; }
        public string HeartSealClass { get; set; }        
        public int InternalLevel { get; set; } = 1;
        public bool IsInCombat { get; set; } = false;
        public bool IsAttacking { get; set; } = false; 
        public bool IsWeaponTriangleAdvantage { get; set; } = false;
        public bool IsWeaponTriangleDisadvantage { get; set; } = false;
        public bool DealsEffectiveDamage => CheckForEffectiveDamage();
        public List<UnitType> EffectiveDamageUnitTypes => CheckEffectiveType(); 
        public Asset Asset { get; set; }
        public Flaw Flaw { get; set; }
        public Condition Condition { get; set; }
        public Terrain Terrain { get; set; }        
        public Inventory Inventory { get; set; }
        public Stats BaseStats => GetBaseStats();
        public Stats StatChangeAmount => GetStatChangeAmount();
        public Stats MaxStats => GetMaxStats();
        public GrowthRate PersonalGrowthRate { get; set; }    
        public List<string> ReclassOptions { get; set; }
        public List<Weapon> WeaponRanks { get; set; }
        public List<Support>? Supports { get; set; }
        public List<Ability> AcquiredAbilities { get; set; }
        public List<Skill> Skills { get; set; }
        public List<LevelUp>? LevelupStatIncreases { get; set; }
        public string ConvoyId { get; set; }

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
            baseStats.Add(Asset.BaseStatBonus);
            baseStats.Add(Flaw.BaseStatBonus);
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
                if (UnitTypes.Any(type => type == UnitType.Flying))
                {
                    currentStats.Def += defBonus;
                }
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
                    if (support.IsPairedUp && support.PairedUpBonus != null)
                    {
                        currentStats.Add(support.PairedUpBonus);
                    }
                }
            }
            currentStats.Add(Condition.StatChange);
            foreach (var item in Inventory.Equipment)
            {
                if (item.IsEquipped && item.StatBonus?.Stats != null)
                {
                    currentStats.Add(item.StatBonus.Stats);
                }
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
                    var weaponRank = WeaponRanks.FirstOrDefault(weapon => weapon.WeaponType == EquippedWeapon.WeaponType);
                    if (weaponRank != null)
                    {
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
                }
                else if (WeaponRanks.Any(weapon => weapon.WeaponType == EquippedWeapon.WeaponType && weapon.WeaponRank != EquippedWeapon.Rank && !weapon.IsActive))
                {
                    var weaponRank = WeaponRanks.FirstOrDefault(weapon => weapon.WeaponType == EquippedWeapon.WeaponType);
                    if (weaponRank != null)
                    {
                        var originalWeaponExp = weaponRank.WeaponExperience;
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
                        weaponRank.WeaponExperience = originalWeaponExp;
                    }
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
            if (Supports != null && Supports.Count > 0 && Supports.Any(support => support.IsPairedUp))
            {
                var supportPoints = 0;
                foreach (var support in Supports)
                {
                    if (support.IsPairedUp || support.IsClose)
                    {
                        switch (support.SupportRank)
                        {
                            case Rank.None:
                                supportPoints += 1;
                                break;
                            case Rank.C:
                                supportPoints += 2;
                                break;
                            case Rank.B:
                                supportPoints += 3;
                                break;
                            case Rank.A:
                                supportPoints += 4;
                                break;
                            case Rank.S:
                                supportPoints += 5;
                                break;
                        }
                    }
                }
                if (supportPoints > 0 && supportPoints < 5)
                {
                    attack += 10;
                }
                else if (supportPoints > 4 && supportPoints < 9)
                {
                    attack += 15;
                }
                else if (supportPoints > 8)
                {
                    attack += 20;
                }
            }
            if (IsWeaponTriangleAdvantage)
            {
                var weapon = WeaponRanks.FirstOrDefault(weapon => weapon.WeaponType == EquippedWeapon?.WeaponType);
                if (weapon != null)
                {
                    switch (weapon.WeaponRank)
                    {
                        case Rank.E:
                        case Rank.D: 
                            attack += 5; 
                            break;
                        case Rank.C:
                        case Rank.B:
                            attack += 10;
                            break;
                        case Rank.A:
                        case Rank.S:
                            attack += 15;
                            break;
                    }
                }
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
                    heal += WeaponRanks.FirstOrDefault(weaponRank => weaponRank.WeaponType == WeaponType.Staff).WeaponRankBonus.Attributes.Heal;
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
            if (IsWeaponTriangleAdvantage)
            {
                var weapon = WeaponRanks.FirstOrDefault(weapon => weapon.WeaponType == EquippedWeapon?.WeaponType);
                if (weapon != null)
                {
                    switch (weapon.WeaponRank)
                    {
                        case Rank.B:
                        case Rank.A:
                        case Rank.S:
                            damage += 1;
                            break;
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
            if (Supports != null && Supports.Count > 0)
            {
                var supportPoints = 0;
                foreach (var support in Supports)
                {
                    if (support.IsPairedUp || support.IsClose)
                    {
                        switch (support.SupportRank)
                        {
                            case Rank.None:
                                supportPoints += 1;
                                break;
                            case Rank.C:
                                supportPoints += 2;
                                break;
                            case Rank.B:
                                supportPoints += 3;
                                break;
                            case Rank.A:
                                supportPoints += 4;
                                break;
                            case Rank.S:
                                supportPoints += 5;
                                break;
                        }
                    }
                }
                if (supportPoints > 3 && supportPoints < 8)
                {
                    crit += 10;
                }
                else if (supportPoints > 7 && supportPoints < 12)
                {
                    crit += 15;
                }
                else if (supportPoints > 11)
                {
                    crit += 20;
                }
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
            if (Terrain.TerrainType != TerrainType.None)
            {
                var hasBonus = int.TryParse(Terrain.AvoidBonus, out var avoidBonus);
                if (hasBonus)
                {
                    if (UnitTypes.Any(type => type == UnitType.Flying))
                    {
                        avoid += avoidBonus;
                    }
                }
            }
            if (Supports != null && Supports.Count > 0)
            {
                var supportPoints = 0;
                foreach (var support in Supports)
                {
                    if (support.IsPairedUp || support.IsClose)
                    {
                        switch (support.SupportRank)
                        {
                            case Rank.None:
                                supportPoints += 1;
                                break;
                            case Rank.C:
                                supportPoints += 2;
                                break;
                            case Rank.B:
                                supportPoints += 3;
                                break;
                            case Rank.A:
                                supportPoints += 4;
                                break;
                            case Rank.S:
                                supportPoints += 5;
                                break;
                        }
                    }
                }
                if (supportPoints > 1 && supportPoints < 6)
                {
                    avoid += 10;
                }
                else if (supportPoints > 5 && supportPoints < 10)
                {
                    avoid += 15;
                }
                else if (supportPoints > 9)
                {
                    avoid += 20;
                }
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
            if (Supports != null && Supports.Count > 0)
            {
                var supportPoints = 0;
                foreach (var support in Supports)
                {
                    if (support.IsPairedUp || support.IsClose)
                    {
                        switch (support.SupportRank)
                        {
                            case Rank.None:
                                supportPoints += 1;
                                break;
                            case Rank.C:
                                supportPoints += 2;
                                break;
                            case Rank.B:
                                supportPoints += 3;
                                break;
                            case Rank.A:
                                supportPoints += 4;
                                break;
                            case Rank.S:
                                supportPoints += 5;
                                break;
                        }
                    }
                }
                if (supportPoints > 2 && supportPoints < 7)
                {
                    dodge += 10;
                }
                else if (supportPoints > 6 && supportPoints < 11)
                {
                    dodge += 15;
                }
                else if (supportPoints > 10)
                {
                    dodge += 20;
                }
            }
            return dodge;
        }

        public int GetDualStrikeRate(Support support)
        {
            var dualStrike = 0;
            dualStrike += (CurrentStats.Skl + support.CurrentStats.Skl) / 4;
            if (EquippedAbilities.Any(ability => ability.Name == "Dual Strike+"))
            {
                dualStrike += 10;
            }
            switch (support.SupportRank)
            {
                case Rank.None:
                    dualStrike += 20;
                    break;
                case Rank.C:
                    dualStrike += 30;
                    break;
                case Rank.B:
                    dualStrike += 40;
                    break;
                case Rank.A:
                    dualStrike += 50;
                    break;
                case Rank.S:
                    dualStrike += 60;
                    break;
            }
            return dualStrike;
        }

        public int GetDualGuardRate(string damageType)
        {
            var dualGuard = 0;
            if (Supports != null && Supports.Count > 0)
            {
                foreach (var support in Supports)
                {
                    if (support.IsPairedUp)
                    {
                        if (EquippedAbilities.Any(ability => ability.Name == "Dual Guard+"))
                        {
                            dualGuard += 10;
                        }
                        if (damageType == "Physical")
                        {
                            dualGuard += (CurrentStats.Def + support.CurrentStats.Def) / 4;
                        }
                        if (damageType == "Magical")
                        {
                            dualGuard += (CurrentStats.Res + support.CurrentStats.Res) / 4;
                        }
                        switch (support.SupportRank)
                        {
                            case Rank.None:
                                break;
                            case Rank.C:
                                dualGuard += 2;
                                break;
                            case Rank.B:
                                dualGuard += 5;
                                break;
                            case Rank.A:
                                dualGuard += 7;
                                break;
                            case Rank.S:
                                dualGuard += 10;
                                break;
                        }
                    }
                }
            }
            return dualGuard;
        }
        public int GetDamageReceived()
        {
            var damageReceived = 0;
            if (EquippedAbilities != null)
            {
                foreach (var ability in EquippedAbilities)
                {
                    if (ability.StatBonus != null && ability.StatBonus.Attributes != null && ability.StatBonus.Attributes.AttackSpeed != 0)
                    {
                        damageReceived += ability.StatBonus.Attributes.AttackSpeed;
                    }
                }
            }
            if (EquippedWeapon.StatBonus != null && EquippedWeapon.StatBonus.Attributes != null && EquippedWeapon.StatBonus.Attributes.AttackSpeed != 0)
            {
                damageReceived += EquippedWeapon.StatBonus.Attributes.AttackSpeed;
            }
            return damageReceived;
        }

        public int GetAttackSpeed()
        {
            var attackSpeed = 0;
            if (EquippedAbilities != null)
            {
                foreach (var ability in EquippedAbilities)
                {
                    if (ability.StatBonus != null && ability.StatBonus.Attributes != null && ability.StatBonus.Attributes.AttackSpeed != 0)
                    {
                        attackSpeed += ability.StatBonus.Attributes.AttackSpeed; 
                    }
                }
            }
            if (EquippedWeapon.StatBonus != null && EquippedWeapon.StatBonus.Attributes != null && EquippedWeapon.StatBonus.Attributes.AttackSpeed != 0)
            {
                attackSpeed += EquippedWeapon.StatBonus.Attributes.AttackSpeed;
            }
            return attackSpeed;
        }

        public StatBonus GetHitWeaponTriangleDisadvantage(Rank advantageWeaponRank)
        {
            var attribute = new StatBonus() { Attributes = new Attributes() };
            var hit = 0;
            var damage = 0;
            switch (advantageWeaponRank)
            {
                case Rank.E:
                case Rank.D:
                    hit += 5;
                    break;
                case Rank.C:
                case Rank.B:
                    hit += 10;
                    break;
                case Rank.A:
                case Rank.S:
                    hit += 15;
                    break;
            }
            switch (advantageWeaponRank)
            {
                case Rank.B:
                case Rank.A:
                case Rank.S:
                    damage -= 1;
                    break;
            }
            attribute.Attributes.Hit = hit;
            attribute.Attributes.Damage = damage;
            return attribute;
        }

        public bool CheckForEffectiveDamage()
        {
            var effective = false;
            if (EquippedAbilities != null && EquippedAbilities.Count > 0)
            {
                foreach (var ability in EquippedAbilities)
                {
                    if (ability.Name == "Beastbane" || ability.Name == "Wyrmsbane" || ability.Name == "Golembane")
                    {
                        effective = true;
                    }
                }
            }
            if (EquippedWeapon.DoesEffectiveDamage)
            {
                effective = true;
            }
            return effective;
        }

        public List<UnitType> CheckEffectiveType()
        {
            var units = new HashSet<UnitType>();
            if (EquippedAbilities != null && EquippedAbilities.Count > 0)
            {
                foreach (var ability in EquippedAbilities)
                {
                    switch (ability.Name)
                    {
                        case "Beastbane":
                            units.Add(UnitType.Beast);
                            break;

                        case "Wyrmsbane":
                            units.Add(UnitType.Dragon);
                            break;

                        case "Golembane":
                            units.Add(UnitType.Mechanists);
                            units.Add(UnitType.Puppets);
                            units.Add(UnitType.Golems);
                            break;
                    }
                }
            }
            if (EquippedWeapon != null && EquippedWeapon.DoesEffectiveDamage && EquippedWeapon.EffectiveUnitTypes.Any())
            {
                foreach (var unitType in EquippedWeapon.EffectiveUnitTypes)
                {
                    units.Add(unitType);
                }
            }
            return units.ToList();
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
