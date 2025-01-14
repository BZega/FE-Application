using Fire_Emblem.Common.TypeCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.Models
{
    public class Terrain
    {
        public TerrainType TerrainType { get; set; } = TerrainType.None;
        public string DefBonus => GetDefBonus();
        public string AvoidBonus => GetAvoidBonus();
        public string HealPercent => GetHealPercent();
        public string MoveCost => GetMoveCost();
        public string OtherData => GetOtherData();

        public string GetDefBonus()
        {
            var defBonus = string.Empty;
            switch (TerrainType)
            {
                case TerrainType.Forest:
                case TerrainType.Pillar:
                    defBonus = "1";
                    break;
                case TerrainType.Fort:
                case TerrainType.Mountain:
                    defBonus = "2"; 
                    break;
                case TerrainType.GateThone:
                case TerrainType.Peak:
                    defBonus = "3";
                    break;
                default:
                    defBonus = "-";
                    break;
            }
                
            return defBonus;
        }

        public string GetAvoidBonus()
        {
            var avoidBonus = string.Empty;
            switch (TerrainType)
            {
                case TerrainType.Forest:
                case TerrainType.Pillar:
                case TerrainType.Stairway:
                    avoidBonus = "10";
                    break;
                case TerrainType.Fort:
                case TerrainType.Mountain:
                case TerrainType.GateThone:
                    avoidBonus = "20";
                    break; 
                case TerrainType.Peak:
                    avoidBonus = "30";
                    break;
                default:
                    avoidBonus = "-";
                    break;
            }
            return avoidBonus;
        }

        public string GetHealPercent()
        {
            var healPercent = string.Empty;
            switch (TerrainType)
            {
                case TerrainType.Fort:
                case TerrainType.GateThone:
                    healPercent = "20";
                    break;
                default:
                    healPercent = "-";
                    break;
            }
                return healPercent;
        }

        public string GetMoveCost()
        {
            var moveCost = string.Empty;
            switch (TerrainType)
            {
                
                case TerrainType.Cliff:
                case TerrainType.Lava:
                case TerrainType.Thicket:
                case TerrainType.ClosedGate:
                    moveCost = "-";
                    break;
                case TerrainType.None:
                    moveCost = "0";
                    break;
                case TerrainType.Bridge:
                case TerrainType.Fort:
                case TerrainType.Pillar:
                case TerrainType.Stairway:
                    moveCost = "1";
                    break;
                case TerrainType.Dessert:
                case TerrainType.Forest:
                    moveCost = "2";
                    break;
                case TerrainType.Mountain:
                case TerrainType.Peak:
                    moveCost = "4";
                    break;
                case TerrainType.River:
                case TerrainType.Sea:
                    moveCost = "5";
                    break;
            }
            return moveCost;
        }

        public string GetOtherData()
        {
            var otherData = string.Empty;
            switch (TerrainType)
            {
                case TerrainType.Cliff:
                case TerrainType.Thicket:
                    otherData = "Only flying units can cross this terrain.";
                    break;
                case TerrainType.Dessert:
                    otherData = "Non-mounted magical units and thieves cross at normal speed.";
                    break;
                case TerrainType.Forest:
                    otherData = "Armored units and mounted units, it takes 4 movement.";
                    break;
                case TerrainType.GateThone:
                    otherData = "Gates cost 1 Movement if open, but are impassable if closed.";
                    break;
                case TerrainType.Lava:
                    otherData = "Only flying units can cross this terrain, but they cannot stop over it.";
                    break;
                case TerrainType.Mountain:
                    otherData = "Only infantry (non-mounted, non-armored) units can cross this.";
                    break;
                case TerrainType.Peak:
                case TerrainType.Sea:
                    otherData = "Only Fighters and Berserkers can cross this terrain.";
                    break;
                case TerrainType.Stairway:
                    otherData = "Units tend to spawn from these points";
                    break;
                default:
                    otherData = "-";
                    break;
            }
            return otherData;
        }
    }
}
