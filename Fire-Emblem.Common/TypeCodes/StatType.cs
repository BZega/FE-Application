using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.TypeCodes
{
    public enum StatType
    {
        [EnumMember(Value = "HP")]
        HP = 0,
        [EnumMember(Value = "Str")]
        Str = 1,
        [EnumMember(Value = "Mag")]
        Mag = 2,
        [EnumMember(Value = "Skl")]
        Skl = 3,
        [EnumMember(Value = "Spd")]
        Spd = 4,
        [EnumMember(Value = "Lck")]
        Lck = 5,
        [EnumMember(Value = "Def")]
        Def = 6,
        [EnumMember(Value = "Res")]
        Res = 7
    }
}
