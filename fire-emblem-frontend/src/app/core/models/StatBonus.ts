import { GrowthRate } from "./Growthrate";
import { Stats } from "./Stats";

export class StatBonus {
    growths: GrowthRate;
    attributes: Attributes;
    stats: Stats;
}

export class Attributes {
    hit: number;
    crit: number;
    avoid: number;
    dodge: number;
    heal: number;
    damage: number;
    damageReceived: number;
    attackSpeed: number;
}