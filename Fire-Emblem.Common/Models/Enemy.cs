using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.Common.Models
{
    public class Enemy : CombatUnit
    {
        public int Id { get; set; }
        public DifficultyType DifficultySetting { get; set; }
        public Stats? ManualStatGrowth { get; set; }
        public override Stats CurrentStats => GetCurrentEnemyStats();
        public Weapon WeaponRank { get; set; }

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
            if (EquippedWeapon?.StatBonus?.Stats != null)
            {
                stats.Add(EquippedWeapon.StatBonus.Stats);
            }
            return stats;
        }

        protected override int GetAttack()
        {
            var attack = base.GetAttack();
            
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

        protected override int GetDamage()
        {
            var damage = base.GetDamage();
            
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
    }
}
