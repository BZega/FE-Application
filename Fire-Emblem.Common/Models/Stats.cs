using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.Models
{
    public class Stats
    {
        public int HP { get; set; } = 0;
        public int Str { get; set; } = 0;
        public int Mag { get; set; } = 0;
        public int Skl { get; set; } = 0;
        public int Spd { get; set; } = 0;
        public int Lck { get; set; } = 0;
        public int Def { get; set; } = 0;
        public int Res { get; set; } = 0;
        public int Mov { get; set; } = 0;

        public void Add(Stats statBlock)
        {
            HP += statBlock.HP;
            Str += statBlock.Str;
            Mag += statBlock.Mag;
            Skl += statBlock.Skl;
            Spd += statBlock.Spd;
            Lck += statBlock.Lck;
            Def += statBlock.Def;
            Res += statBlock.Res;
            Mov += statBlock.Mov;
        }

        public void Subtract(Stats statBlock)
        {
            HP -= statBlock.HP;
            Str -= statBlock.Str;
            Mag -= statBlock.Mag;
            Skl -= statBlock.Skl;
            Spd -= statBlock.Spd;
            Lck -= statBlock.Lck;
            Def -= statBlock.Def;
            Res -= statBlock.Res;
            Mov -= statBlock.Mov;
        }

        public void MaximumCheck(Stats statBlock)
        {
            if (HP > statBlock.HP)
            {
                HP = statBlock.HP;
            }
            if (Str > statBlock.Str)
            {
                Str = statBlock.Str;
            }
            if (Mag > statBlock.Mag)
            {
                Mag = statBlock.Mag;
            }
            if (Skl > statBlock.Skl)
            {
                Skl = statBlock.Skl;
            }
            if (Spd > statBlock.Spd)
            {
                Spd = statBlock.Spd;
            }
            if (Lck > statBlock.Lck)
            {
                Lck = statBlock.Lck;
            }
            if (Def > statBlock.Def)
            {
                Def = statBlock.Def;
            }
            if (Res > statBlock.Res)
            {
                Res = statBlock.Res;
            }
            if (Mov > statBlock.Mov)
            {
                Mov = statBlock.Mov;
            }
        }

        public void RandomizeStatIncrease(GrowthRate rate)
        {
            Random random = new Random();
            int randomNumber = 0;

            randomNumber = random.Next(0, 101);
            HP += randomNumber < rate.HP ? 1 : 0;
            if (rate.HP > 100) {
                rate.HP -= 100;
                randomNumber = random.Next(0, 101);
                HP += randomNumber < rate.HP ? 1 : 0;
                rate.HP += 100;
            }

            randomNumber = random.Next(0, 101);
            Str += randomNumber < rate.Str ? 1 : 0;
            if (rate.Str > 100)
            {
                rate.Str -= 100;
                randomNumber = random.Next(0, 101);
                Str += randomNumber < rate.Str ? 1 : 0;
                rate.Str += 100;
            }

            randomNumber = random.Next(0, 101);
            Mag += randomNumber < rate.Mag ? 1 : 0;
            if (rate.Mag > 100)
            {
                rate.Mag -= 100;
                randomNumber = random.Next(0, 101);
                Mag += randomNumber < rate.Mag ? 1 : 0;
                rate.Mag += 100;
            }

            randomNumber = random.Next(0, 101);
            Skl += randomNumber < rate.Skl ? 1 : 0;
            if (rate.Skl > 100)
            {
                rate.Skl -= 100;
                randomNumber = random.Next(0, 101);
                Skl += randomNumber < rate.Skl ? 1 : 0;
                rate.Skl += 100;
            }

            randomNumber = random.Next(0, 101);
            Spd += randomNumber < rate.Spd ? 1 : 0;
            if (rate.Spd > 100)
            {
                rate.Spd -= 100;
                randomNumber = random.Next(0, 101);
                Spd += randomNumber < rate.Spd ? 1 : 0;
                rate.Spd += 100;
            }

            randomNumber = random.Next(0, 101);
            Lck += randomNumber < rate.Lck ? 1 : 0;
            if (rate.Lck > 100)
            {
                rate.Lck -= 100;
                randomNumber = random.Next(0, 101);
                Lck += randomNumber < rate.Lck ? 1 : 0;
                rate.Lck += 100;
            }

            randomNumber = random.Next(0, 101);
            Def += randomNumber < rate.Def ? 1 : 0;
            if (rate.Def > 100)
            {
                rate.Def -= 100;
                randomNumber = random.Next(0, 101);
                Def += randomNumber < rate.Def ? 1 : 0;
                rate.Def += 100;
            }

            randomNumber = random.Next(0, 101);
            Res += randomNumber < rate.Res ? 1 : 0;
            if (rate.Res > 100)
            {
                rate.Res -= 100;
                randomNumber = random.Next(0, 101);
                Res += randomNumber < rate.Res ? 1 : 0;
                rate.Res += 100;
            }
        }

        public void MaxLevelCheck(Stats baseStats, Stats maxStats)
        {
            if (baseStats.HP + HP > maxStats.HP)
            {
                HP = 0;
            }
            if (baseStats.Str + Str > maxStats.Str)
            {
                Str = 0;
            }
            if (baseStats.Mag + Mag > maxStats.Mag)
            {
                Mag = 0;
            }
            if (baseStats.Skl + Skl > maxStats.Skl)
            {
                Skl = 0;
            }
            if (baseStats.Spd + Spd > maxStats.Spd)
            {
                Spd = 0;
            }
            if (baseStats.Lck + Lck > maxStats.Lck)
            {
                Lck = 0;
            }
            if (baseStats.Def + Def > maxStats.Def)
            {
                Def = 0;
            }
            if (baseStats.Res + Res > maxStats.Res)
            {
                Res = 0;
            }
        }
    }
}
