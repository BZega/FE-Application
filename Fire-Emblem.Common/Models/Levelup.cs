﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.Models
{
    public class LevelUp
    {
        public int Level { get; set; }
        public string LevelUpType {  get; set; }
        public Stats StatIncrease { get; set; }
    }
}
