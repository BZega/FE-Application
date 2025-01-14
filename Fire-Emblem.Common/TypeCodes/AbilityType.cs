using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.TypeCodes
{
    public enum AbilityType
    {
        [EnumMember(Value = "Passive")]
        Passive = 0,
        [EnumMember(Value = "Command")]
        Command = 1,
        [EnumMember(Value = "Combat")]
        Combat = 2
    }
}
