using Fire_Emblem.API.Models.Character;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;
using static Fire_Emblem.API.Models.Character.BattleResultDto;

namespace Fire_Emblem.API.Business.Helper.Combat
{
    public class CombatHelper
    {
        public static void CheckWeaponTriangle(Character character, Enemy enemy)
        {
            var weaponAdvantageMap = new Dictionary<WeaponType, WeaponType[]>
            {
                { WeaponType.Sword, new[] { WeaponType.Axe, WeaponType.Bow } },
                { WeaponType.Axe, new[] { WeaponType.Lance, WeaponType.Dagger } },
                { WeaponType.Lance, new[] { WeaponType.Sword, WeaponType.Tome, WeaponType.DarkTome } }
            };

            if (weaponAdvantageMap.TryGetValue(character.EquippedWeapon.WeaponType, out var disadvantagedWeapons) &&
                disadvantagedWeapons.Contains(enemy.EquippedWeapon.WeaponType))
            {
                character.IsWeaponTriangleAdvantage = true;
                enemy.IsWeaponTriangleDisadvantage = true;
            }
            else if (weaponAdvantageMap.TryGetValue(enemy.EquippedWeapon.WeaponType, out var disadvantagedByEnemy) &&
                     disadvantagedByEnemy.Contains(character.EquippedWeapon.WeaponType))
            {
                enemy.IsWeaponTriangleAdvantage = true;
                character.IsWeaponTriangleDisadvantage = true;
            }
        }

        public static void ApplyWeaponTriangleDisadvantage(Character character, Enemy enemy, ref int charHitDecrease, ref int charDamageDecrease, ref int enemyHitDecrease, ref int enemyDamageDecrease)
        {
            var disadvantage = new StatBonus() { Attributes = new Attributes() };
            if (character.IsWeaponTriangleAdvantage && character.EquippedWeapon.Rank != null)
            {
                disadvantage = enemy.GetHitWeaponTriangleDisadvantage(character.WeaponRanks.FirstOrDefault(w => w.WeaponType == character.EquippedWeapon.WeaponType).WeaponRank);
                if (disadvantage.Attributes != null)
                {
                    enemyHitDecrease = disadvantage.Attributes.Hit;
                    enemyDamageDecrease = disadvantage.Attributes.Damage;
                }
            }

            if (enemy.IsWeaponTriangleAdvantage && enemy.EquippedWeapon.Rank != null)
            {
                disadvantage = character.GetHitWeaponTriangleDisadvantage(enemy.WeaponRank.WeaponRank);
                if (disadvantage.Attributes != null)
                {
                    charHitDecrease = disadvantage.Attributes.Hit;
                    charDamageDecrease = disadvantage.Attributes.Damage;
                }
            }
        }
    }
}
