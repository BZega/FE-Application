import { StatBonus } from "./StatBonus";

export class Weapon {
    weaponType: string;
    weaponRank: string;
    weaponExperience: number;
    isActive: boolean;
    weaponRankBonus: StatBonus;
}