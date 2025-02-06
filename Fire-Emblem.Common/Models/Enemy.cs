using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.Common.Models
{
    public class Enemy
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public int InternalLevel { get; set; }
        public int CurrentHP { get; set; }
        public DifficultyType DifficultySetting { get; set; }
        public Stats? ManualStatGrowth { get; set; }
        public Stats CurrentStats => GetCurrentEnemyStats();
        public Equipment EquippedWeapon { get; set; }
        public UnitClass CurrentClass { get; set; }
        public Weapon WeaponRank { get; set; }
        public List<Ability> EquippedAbilities { get; set; }
        public Terrain Terrain { get; set; }
        public List<UnitType> UnitTypes { get; set; }
        public bool IsAttacking { get; set; } = false;
        public bool IsInCombat { get; set; } = false;
        public bool IsWeaponTriangleAdvantage { get; set; } = false;
        public bool IsWeaponTriangleDisadvantage { get; set; } = false;
        public bool DealsEffectiveDamage => CheckForEffectiveDamage();
        public List<UnitType> EffectiveDamageUnitTypes => CheckEffectiveType();
        public int Attack => GetAttack();
        public int Damage => GetDamage();
        public int Crit => GetCrit();
        public int Avoid => GetAvoid();
        public int Dodge => GetDodge();
        public int AttackSpeed => GetAttackSpeed();
        public int DamageReceived => GetDamageReceived();

        public Stats GetCurrentEnemyStats()
        {
            var stats = new Stats();
            stats.Add(CurrentClass.BaseStats);
            if (ManualStatGrowth != null)
            {
                stats.Add(ManualStatGrowth);
            }
            if (EquippedAbilities != null)
            {
                foreach (var ability in EquippedAbilities)
                {
                    if (ability.StatBonus?.Stats != null)
                    {
                        stats.Add(ability.StatBonus.Stats);
                    }
                }
            }
            if (EquippedWeapon.StatBonus?.Stats != null)
            {
                stats.Add(EquippedWeapon.StatBonus.Stats);
            }
            return stats;
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
            if (CurrentClass.InnateBonus?.Attributes != null)
            {
                attack += CurrentClass.InnateBonus.Attributes.Hit;
            }
            if (IsWeaponTriangleAdvantage)
            {
                var weapon = WeaponRank;
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
                var weapon = WeaponRank;
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
            if (CurrentClass.InnateBonus?.Attributes != null)
            {
                dodge += CurrentClass.InnateBonus.Attributes.Dodge;
            }
            return dodge;
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
                    hit -= 5;
                    break;
                case Rank.C:
                case Rank.B:
                    hit -= 10;
                    break;
                case Rank.A:
                case Rank.S:
                    hit -= 15;
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
}