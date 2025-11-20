using Fire_Emblem.Common.TypeCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.Models
{
    public class Skill
    {
        public SkillType SkillType { get; set; }
        public StatType StatType { get; set; }
        public bool IsProficient { get; set; } = false;
        public int Attribute { get; set; } = 0;
        public int Bonus { get; set; } = 0;
        public int Score {  get; set; } = 0;

        public int GetScore(int luck)
        {
            var score = 0;
            score += (int)Math.Ceiling((double)((Attribute / 5) + (luck / 10)));
            if (IsProficient)
            {
                score += Bonus;
            }
            return score;
        }

        public int GetBonus(int level)
        {
            var score = 0;
            if (IsProficient)
            {
                score = (int)Math.Ceiling((double)(level / 5));
                if (score < 1)
                {
                    score = 1;
                }
                if (score > 8)
                {
                    score = 8;
                }
            }
            return score;
        }
    }
}
