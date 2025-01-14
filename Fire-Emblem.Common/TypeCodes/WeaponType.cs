using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.TypeCodes
{
    public enum WeaponType
    {
        [EnumMember(Value = "Axe")]
        Axe = 0,
        [EnumMember(Value = "Bow")]
        Bow = 1,
        [EnumMember(Value = "Dagger")]
        Dagger = 2,
        [EnumMember(Value = "Lance")]
        Lance = 3,
        [EnumMember(Value = "Staff")]
        Staff = 4,
        [EnumMember(Value = "Stone")]
        Stone = 5,
        [EnumMember(Value = "Sword")]
        Sword = 6,
        [EnumMember(Value = "Tome")]
        Tome = 7,
        [EnumMember(Value = "Dark Tome")]
        DarkTome = 8,
        [EnumMember(Value = "Consumable")]
        Cosumable = 9,
        None = 10
    }
}
