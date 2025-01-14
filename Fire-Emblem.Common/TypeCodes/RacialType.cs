using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.TypeCodes
{
    public enum RacialType
    {
        [EnumMember(Value = "Human")]
        Human = 0,
        [EnumMember(Value = "Kitsune")]
        Kitsune = 1,
        [EnumMember(Value = "Manakete")]
        Manakete = 2,
        [EnumMember(Value = "Taguel")]
        Taguel = 3,
        [EnumMember(Value = "Wolfskin")]
        Wolfskin = 4,
        [EnumMember(Value = "Half-Human")]
        HalfHuman = 5,
    }
}
