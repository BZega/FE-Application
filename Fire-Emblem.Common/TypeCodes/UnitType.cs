using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.TypeCodes
{
    public enum UnitType
    {
        [EnumMember(Value = "Infantry")]
        Infantry = 0,
        [EnumMember(Value = "Armor")]
        Armor = 1,
        [EnumMember(Value = "Mounted")]
        Mounted = 2,
        [EnumMember(Value = "Flying")]
        Flying = 3,
        [EnumMember(Value = "Beast")]
        Beast = 4,
        [EnumMember(Value = "Dragon")]
        Dragon = 5
    }
}
