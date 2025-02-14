import { Equipment } from "./Equipment";
import { PersonalAbility } from "./PersonalAbility";
import { Stats } from "./Stats";
import { UnitClass } from "./UnitClass";

export class Support {
    id: number;
    name: string;
    gender: string;
    level: number;
    internalLevel: number;
    currentClassType: string;
    currentClass: UnitClass;
    equippedWeapon: Equipment;
    startingClass: string;
    isPairedUp: boolean;
    isClose: boolean;
    levelUpStats: Stats;
    currentStats: Stats;
    pairedUpBonus: Stats;
    supportRank: string;
    crit: number;
    supportPoints: number;
    personalAbility: PersonalAbility;
}