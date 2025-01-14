using Fire_Emblem.Common.TypeCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.Models
{
    public class ClassWeapon
    {
        public WeaponType WeaponType { get; set; }
        public Rank MinRank { get; set; }
        public Rank MaxRank { get; set; }
    }
}
