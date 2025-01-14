using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.TypeCodes
{
    public enum ConditionType
    {
        [EnumMember(Value = "Normal")]
        Normal = 0,
        [EnumMember(Value = "Serious")]
        Serious = 1,
        [EnumMember(Value = "Critical")]
        Critical = 2,
        [EnumMember(Value = "Dead")]
        DEAD = 3
    }
}
