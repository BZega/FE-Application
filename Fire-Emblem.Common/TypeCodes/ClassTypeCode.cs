using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fire_Emblem.Common.TypeCodes
{
    public class ClassTypeCode
    {
        public const string Apothecary = "Apothecary";
        public const string Archer = "Archer";
        public const string SwordAssassin = "Sword Assassin";
        public const string BowAssassin = "Bow Assassin";
        public const string Ballistician = "Ballistician";
        public const string Basara = "Basara";
        public const string Berserker = "Berserker";
        public const string Blacksmith = "Blacksmith";
        public const string BowKnight = "Bow Knight";
        public const string Cavalier = "Cavalier";
        public const string Cleric = "Cleric";
        public const string DarkFlier = "Dark Flier";
        public const string DarkKnight = "Dark Knight";
        public const string DarkMage = "Dark Mage";
        public const string DawnNoble = "Dawn Noble";
        public const string TomeDreadFighter = "Tome Dread Fighter";
        public const string ShurikenDreadFighter = "Shuriken Dread Fighter";
        public const string DuskNoble = "Dusk Noble";
        public const string FalconKnight = "Falcon Knight";
        public const string Fighter = "Fighter";
        public const string General = "General";
        public const string Grandmaster = "Grandmaster";
        public const string GreatKnight = "Great Knight";
        public const string GreatSwordLordwithLance = "Great Sword Lord with Lance";
        public const string GreatSwordLordwithAxe = "Great Sword Lord with Axe";
        public const string GreatAxeLordwithSword = "Great Axe Lord with Sword";
        public const string GreatAxeLordwithLance = "Great Axe Lord with Lance";
        public const string GreatLanceLordwithSword = "Great Lance Lord with Sword";
        public const string GreatLanceLordwithAxe = "Great Lance Lord with Axe";
        public const string GreatMaster = "Great Master";
        public const string GriffonRider = "Griffon Rider";
        public const string Hero = "Hero";
        public const string KinshiKnight = "Kinshi Knight";
        public const string Kitsune = "Kitsune";
        public const string Knight = "Knight";
        public const string Lodestar = "Lodestar";
        public const string SwordLord = "Sword Lord";
        public const string AxeLord = "Axe Lord";
        public const string LanceLord = "Lance Lord";
        public const string Mage = "Mage";
        public const string Maid = "Maid";
        public const string MaligKnight = "Malig Knight";
        public const string Manakete = "Manakete";
        public const string ManaketeHeir = "Manakete Heir";
        public const string MasterNinja = "Master Ninja";
        public const string MasterOfArms = "Master of Arms";
        public const string Mechanist = "Mechanist";
        public const string Mercenary = "Mercenary";
        public const string Merchant = "Merchant";
        public const string Myrmidon = "Myrmidon";
        public const string NineTails = "Nine-Tails";
        public const string Ninja = "Ninja";
        public const string OniChieftain = "Oni Chieftain";
        public const string OniSavage = "Oni Savage";
        public const string Paladin = "Paladin";
        public const string PegasusKnight = "Pegasus Knight";
        public const string SwordPerformer = "Sword Performer";
        public const string LancePerformer = "Lance Performer";
        public const string Sage = "Sage";
        public const string Sniper = "Sniper";
        public const string Sorcerer = "Sorcerer";
        public const string SpearFighter = "Spear Fighter";
        public const string SpearMaster = "Spear Master";
        public const string Strategist = "Strategist";
        public const string Swordmaster = "Swordmaster";
        public const string Tactician = "Tactician";
        public const string Taguel = "Taguel";
        public const string SwordThief = "Sword Thief";
        public const string BowThief = "Bow Thief";
        public const string SwordTrickster = "Sword Trickster";
        public const string BowTrickster = "Bow Trickster";
        public const string Troubadour = "Troubadour";
        public const string Vanguard = "Vanguard";
        public const string Villager = "Villager";
        public const string WarCleric = "War Cleric";
        public const string Warrior = "Warrior";
        public const string Witch = "Witch";
        public const string Wolfskin = "Wolfskin";
        public const string Wolfssegner = "Wolfssegner";
        public const string WyvernLord = "Wyvern Lord";
        public const string WyvernRider = "Wyvern Rider";

        public static string ClassTypeConverter(ClassType type)
        {
            switch (type)
            {
                case ClassType.Apothecary:
                    return Apothecary;
                case ClassType.Archer:
                    return Archer;
                case ClassType.SwordAssassin:
                    return SwordAssassin;
                case ClassType.BowAssassin:
                    return BowAssassin;
                case ClassType.Ballistician:
                    return Ballistician;
                case ClassType.Basara:
                    return Basara;
                case ClassType.Berserker:
                    return Berserker;
                case ClassType.Blacksmith:
                    return Blacksmith;
                case ClassType.BowKnight:
                    return BowKnight;
                case ClassType.Cavalier:
                    return Cavalier;
                case ClassType.Cleric:
                    return Cleric;
                case ClassType.DarkFlier:
                    return DarkFlier;
                case ClassType.DarkKnight:
                    return DarkKnight;
                case ClassType.DarkMage:
                    return DarkMage;
                case ClassType.DawnNoble:
                    return DawnNoble;
                case ClassType.TomeDreadFighter:
                    return TomeDreadFighter;
                case ClassType.ShurikenDreadFighter:
                    return ShurikenDreadFighter;
                case ClassType.DuskNoble:
                    return DuskNoble;
                case ClassType.FalconKnight:
                    return FalconKnight;
                case ClassType.Fighter:
                    return Fighter;
                case ClassType.General:
                    return General;
                case ClassType.Grandmaster:
                    return Grandmaster;
                case ClassType.GreatKnight:
                    return GreatKnight;
                case ClassType.GreatSwordLordwithLance:
                    return GreatSwordLordwithLance;
                case ClassType.GreatSwordLordwithAxe:
                    return GreatSwordLordwithAxe;
                case ClassType.GreatAxeLordwithSword:
                    return GreatAxeLordwithSword;
                case ClassType.GreatAxeLordwithLance:
                    return GreatAxeLordwithLance;
                case ClassType.GreatLanceLordwithSword:
                    return GreatLanceLordwithSword;
                case ClassType.GreatLanceLordwithAxe:
                    return GreatLanceLordwithAxe;
                case ClassType.GreatMaster:
                    return GreatMaster;
                case ClassType.GriffonRider:
                    return GriffonRider;
                case ClassType.Hero:
                    return Hero;
                case ClassType.KinshiKnight:
                    return KinshiKnight;
                case ClassType.Kitsune:
                    return Kitsune;
                case ClassType.Knight:
                    return Knight;
                case ClassType.Lodestar:
                    return Lodestar;
                case ClassType.SwordLord:
                    return SwordLord;
                case ClassType.AxeLord:
                    return AxeLord;
                case ClassType.LanceLord:
                    return LanceLord;
                case ClassType.Mage:
                    return Mage;
                case ClassType.Maid:
                    return Maid;
                case ClassType.MaligKnight:
                    return MaligKnight;
                case ClassType.Manakete:
                    return Manakete;
                case ClassType.ManaketeHeir:
                    return ManaketeHeir;
                case ClassType.MasterNinja:
                    return MasterNinja;
                case ClassType.MasterOfArms:
                    return MasterOfArms;
                case ClassType.Mechanist:
                    return Mechanist;
                case ClassType.Mercenary:
                    return Mercenary;
                case ClassType.Merchant:
                    return Merchant;
                case ClassType.Myrmidon:
                    return Myrmidon;
                case ClassType.NineTails:
                    return NineTails;
                case ClassType.Ninja:
                    return Ninja;
                case ClassType.OniChieftain:
                    return OniChieftain;
                case ClassType.OniSavage:
                    return OniSavage;
                case ClassType.Paladin:
                    return Paladin;
                case ClassType.PegasusKnight:
                    return PegasusKnight;
                case ClassType.SwordPerformer:
                    return SwordPerformer;
                case ClassType.LancePerformer:
                    return LancePerformer;
                case ClassType.Sage:
                    return Sage;
                case ClassType.Sniper:
                    return Sniper;
                case ClassType.Sorcerer:
                    return Sorcerer;
                case ClassType.SpearFighter:
                    return SpearFighter;
                case ClassType.SpearMaster:
                    return SpearMaster;
                case ClassType.Strategist:
                    return Strategist;
                case ClassType.Swordmaster:
                    return Swordmaster;
                case ClassType.Tactician:
                    return Tactician;
                case ClassType.Taguel:
                    return Taguel;
                case ClassType.SwordThief:
                    return SwordThief;
                case ClassType.BowThief:
                    return BowThief;
                case ClassType.SwordTrickster:
                    return SwordTrickster;
                case ClassType.BowTrickster:
                    return BowTrickster;
                case ClassType.Troubadour:
                    return Troubadour;
                case ClassType.Vanguard:
                    return Vanguard;
                case ClassType.Villager:
                    return Villager;
                case ClassType.WarCleric:
                    return WarCleric;
                case ClassType.Warrior:
                    return Warrior;
                case ClassType.Witch:
                    return Witch;
                case ClassType.Wolfskin:
                    return Wolfskin;
                case ClassType.Wolfssegner:
                    return Wolfssegner;
                case ClassType.WyvernLord:
                    return WyvernLord;
                case ClassType.WyvernRider:
                    return WyvernRider;
                default:
                    return "NONE";
            }
        }
    }
}
