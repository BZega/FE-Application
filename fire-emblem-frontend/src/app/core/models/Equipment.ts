import { StatBonus } from "./StatBonus";

export class Equipment {
    id: number;
    equipOid?: string;
    name: string;
    rank?: string;
    weaponType: string;
    isMagical: boolean;
    isBrave: boolean;
    doesEffectiveDamage: boolean;
    effectiveUnitTypes: string[];
    might?: number;
    hit?: number;
    crit?: number;
    range?: string;
    uses: string;
    worth: string;
    weaponExp?: number;
    baseHP?: number;
    experience?: number;
    isEquipped: boolean;
    description: string;
    statBonus: StatBonus;
}