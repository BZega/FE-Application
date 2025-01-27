using Fire_Emblem.Common.TypeCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.Models
{
    public class Biography
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Background { get; set; }
        public bool IsNoble { get; set; }
        public Race RaceChoice { get; set; }
    }
}
