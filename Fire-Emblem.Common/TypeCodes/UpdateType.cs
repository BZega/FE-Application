using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.TypeCodes
{
    public enum UpdateType
    {
        NONE = 0,
        EQUIP = 1,
        UNEQUIP = 2,
        BUY = 3,
        SELL = 4,
        WITHDRAW = 5,
        DEPOSIT = 6,
        USE = 7,
        ACQUIRE = 8
    }
}
