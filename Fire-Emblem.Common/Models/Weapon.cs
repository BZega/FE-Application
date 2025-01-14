using Fire_Emblem.Common.TypeCodes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.Models
{
    public class Weapon
    {
        public WeaponType WeaponType { get; set; }
        public Rank WeaponRank => GetWeaponLetterRank();
        public int WeaponExperience { get; set; } = 1;
        public bool IsActive { get; set; } = false;
        public StatBonus? WeaponRankBonus => GetWeaponRankBonus();

        public Rank GetWeaponLetterRank()
        {
            if (WeaponExperience > 30 && WeaponExperience < 70)
            {
                return Rank.D;
            }
            else if (WeaponExperience > 70 && WeaponExperience < 120)
            {
                return Rank.C;
            }
            else if (WeaponExperience > 120 &&  WeaponExperience < 180)
            {
                return Rank.B;
            }
            else if (WeaponExperience > 180 && WeaponExperience < 250)
            {
                return Rank.A;
            }
            else if (WeaponExperience > 250)
            {
                return Rank.S;
            }
            else
            {
                return Rank.E;
            }
        }

        public StatBonus GetWeaponRankBonus()
        {
            var attributes = new Attributes();
            if (WeaponRank == Rank.C)
            {
                switch (WeaponType)
                {
                    case WeaponType.Sword:
                    case WeaponType.Dagger:
                        attributes.Damage = 1;
                        break;
                    case WeaponType.Bow:
                    case WeaponType.Lance:
                    case WeaponType.Tome:
                    case WeaponType.Stone:
                        attributes.Damage = 1;
                        break;
                    case WeaponType.Axe:
                        attributes.Hit = 5;
                        break;
                    case WeaponType.Staff:
                        attributes.Heal = 1;
                        break;
                }
            }
            else if (WeaponRank == Rank.B)
            {
                switch (WeaponType)
                {
                    case WeaponType.Sword:
                    case WeaponType.Dagger:
                        attributes.Damage = 2;
                        break;
                    case WeaponType.Bow:
                    case WeaponType.Lance:
                    case WeaponType.Tome:
                    case WeaponType.Stone:
                        attributes.Damage = 1;
                        attributes.Hit = 5;
                        break;
                    case WeaponType.Axe:
                        attributes.Hit = 10;
                        break;
                    case WeaponType.Staff:
                        attributes.Heal = 1;
                        attributes.Hit = 5;
                        break;
                }
            }
            else if (WeaponRank == Rank.A)
            {
                switch (WeaponType)
                {
                    case WeaponType.Sword:
                    case WeaponType.Dagger:
                        attributes.Damage = 3;
                        break;
                    case WeaponType.Bow:
                    case WeaponType.Lance:
                    case WeaponType.Tome:
                    case WeaponType.Stone:
                        attributes.Damage = 2;
                        attributes.Hit = 5;
                        break;
                    case WeaponType.Axe:
                        attributes.Damage = 1;
                        attributes.Hit = 10;
                        break;
                    case WeaponType.Staff:
                        attributes.Heal = 2;
                        attributes.Hit = 5;
                        break;
                }
            } 
            else if (WeaponRank == Rank.S)
            {
                switch (WeaponType)
                {
                    case WeaponType.Sword:
                    case WeaponType.Dagger:
                        attributes.Damage = 4;
                        attributes.Hit = 5;
                        break;
                    case WeaponType.Bow:
                    case WeaponType.Lance:
                    case WeaponType.Tome:
                    case WeaponType.Stone:
                        attributes.Damage = 3;
                        attributes.Hit = 10;
                        break;
                    case WeaponType.Axe:
                        attributes.Damage = 2;
                        attributes.Hit = 15;
                        break;
                    case WeaponType.Staff:
                        attributes.Heal = 3;
                        attributes.Hit = 10;
                        break;
                }
            }
            else
            {
                attributes = null;
            }
            StatBonus statBonus = new StatBonus()
            {
                Attributes = attributes
            };

            return statBonus;
        }
    }
}
