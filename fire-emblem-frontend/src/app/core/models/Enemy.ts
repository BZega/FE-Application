import { Ability } from "./Ability";
import { Equipment } from "./Equipment";
import { Stats } from "./Stats";
import { Terrain } from "./Terrain";
import { UnitClass } from "./UnitClass";
import { Weapon } from "./Weapon";

export class Enemy {
    id: number;
    level: number;
    internalLevel: number;
    currentHP: number;
    difficultySetting: string;
    manualStatGrowth: Stats;
    currentStats: Stats;
    equippedWeapon: Equipment;
    currentClass: UnitClass;
    weaponRank: Weapon;
    equippedAbilities: Ability[];
    terrain: Terrain;
    unitTypes: string[];
    isAttacking: boolean;
    isWeaponTriangleAdvantage: boolean;
    isWeaponTriangleDisadvantage: boolean;
    dealEffectiveDamage: boolean;
    effectiveDamageTypes: string[];
    attack: number;
    damage: number;
    crit: number;
    avoid: number;
    dodge: number;
    attackSpeed: number;
    damageReceived: number;
}