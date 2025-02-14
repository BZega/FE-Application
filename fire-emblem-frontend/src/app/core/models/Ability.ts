import { StatBonus } from "./StatBonus";

export class Ability {
    id: number;
    abilityOid?: string;
    name: string;
    levelGained: number;
    abilityType: string;
    needsToInitiateCombat: boolean;
    description: string;
    statBonus?: StatBonus;
}