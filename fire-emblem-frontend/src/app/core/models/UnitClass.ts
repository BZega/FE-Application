import { Ability } from "./Ability";
import { ClassWeapon } from "./ClassWeapon";
import { GrowthRate } from "./Growthrate";
import { StatBonus } from "./StatBonus";
import { Stats } from "./Stats";

export class UnitClass {
    id: number;
    name: string;
    isPromoted: boolean;
    isSpecialClass: boolean;
    baseStats: Stats;
    maxStats: Stats;
    growthRate: GrowthRate;
    innateBonus?: StatBonus;
    unitTypes: string;
    abilities: Ability;
    usableWeapons: ClassWeapon[];
    classPromotions?: string[];
    reclassOptions?: string[];
    skillTypeOptions: string[];
}