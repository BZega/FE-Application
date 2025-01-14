using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.TypeCodes
{
    public enum Rank
    {
        [EnumMember(Value = "E")]
        E = 0,
        [EnumMember(Value = "D")]
        D = 1,
        [EnumMember(Value = "C")]
        C = 2,
        [EnumMember(Value = "B")]
        B = 3,
        [EnumMember(Value = "A")]
        A = 4,
        [EnumMember(Value = "S")]
        S = 5
    }
}
