import { GrowthRate } from "./Growthrate";

export class Race {
    racialType: string;
    humanStatChoices?: string[];
    racialGrowth: GrowthRate;
    unitType: string;
}