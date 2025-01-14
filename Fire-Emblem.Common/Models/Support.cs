using Fire_Emblem.Common.TypeCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.Models
{
    public class Support
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public int Level { get; set; }
        public int InternalLevel { get; set; }
        public string StartingClass { get; set; }
        public bool IsPairedUp { get; set; }
        public StatBonus? PairedUpBonus { get; set; }
        public Rank SupportRank { get; set; }
        public int SupportExp { get; set; }
    }
}
