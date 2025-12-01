import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../material/material.module';
import { CharacterService } from '../../core/services/character.service';

interface StatBlock {
  hp: number;
  str: number;
  mag: number;
  skl: number;
  spd: number;
  lck: number;
  def: number;
  res: number;
  mov: number;
}

interface GrowthBlock {
  hp: number;
  str: number;
  mag: number;
  skl: number;
  spd: number;
  lck: number;
  def: number;
  res: number;
}

interface InnateBonusStats {
  hp: number;
  str: number;
  mag: number;
  skl: number;
  spd: number;
  lck: number;
  def: number;
  res: number;
  mov: number;
}

interface InnateBonusGrowths {
  hp: number;
  str: number;
  mag: number;
  skl: number;
  spd: number;
  lck: number;
  def: number;
  res: number;
}

interface InnateBonusAttributes {
  hit: number;
  crit: number;
  avoid: number;
  dodge: number;
  heal: number;
  damage: number;
  damageReceived: number;
  attackSpeed: number;
}

interface UsableWeapon {
  weaponType: string;
  minRank: string;
  maxRank: string;
}

@Component({
  selector: 'app-class-creator',
  imports: [CommonModule, MaterialModule, FormsModule],
  templateUrl: './class-creator.component.html',
  styleUrl: './class-creator.component.scss'
})
export class ClassCreatorComponent implements OnInit {
  private dialogRef = inject(MatDialogRef<ClassCreatorComponent>);
  private characterService = inject(CharacterService);

  // Basic info
  className: string = '';
  isPromoted: boolean = true;
  isSpecialClass: boolean = false;

  // Stats
  baseStats: StatBlock = { hp: 0, str: 0, mag: 0, skl: 0, spd: 0, lck: 0, def: 0, res: 0, mov: 0 };
  maxStats: StatBlock = { hp: 0, str: 0, mag: 0, skl: 0, spd: 0, lck: 0, def: 0, res: 0, mov: 0 };
  growthRates: GrowthBlock = { hp: 0, str: 0, mag: 0, skl: 0, spd: 0, lck: 0, def: 0, res: 0 };

  // Innate Bonus
  hasInnateBonus: boolean = false;
  hasInnateBonusStats: boolean = false;
  hasInnateBonusGrowths: boolean = false;
  hasInnateBonusAttributes: boolean = false;
  
  innateBonusStats: InnateBonusStats = { hp: 0, str: 0, mag: 0, skl: 0, spd: 0, lck: 0, def: 0, res: 0, mov: 0 };
  innateBonusGrowths: InnateBonusGrowths = { hp: 0, str: 0, mag: 0, skl: 0, spd: 0, lck: 0, def: 0, res: 0 };
  innateBonusAttributes: InnateBonusAttributes = { hit: 0, crit: 0, avoid: 0, dodge: 0, heal: 0, damage: 0, damageReceived: 0, attackSpeed: 0 };

  // Unit Types
  unitTypes: string[] = [''];
  availableUnitTypes: string[] = [
    'Infantry', 'Armored', 'Cavalry', 'Flying', 'Dragon', 'Beast', 
    'Monster', 'Undead', 'Automaton', 'Mounted'
  ];

  // Abilities
  abilities: string[] = ['', ''];

  // Usable Weapons
  usableWeapons: UsableWeapon[] = [{ weaponType: '', minRank: 'E', maxRank: 'S' }];
  availableWeaponTypes: string[] = [
    'Sword', 'Lance', 'Axe', 'Bow', 'Tome', 'Staff', 
    'Knife', 'Shuriken', 'Stone', 'Dragonstone', 'Beaststone'
  ];
  availableRanks: string[] = ['E', 'D', 'C', 'B', 'A', 'S'];

  // Class Promotions and Reclass Options
  classPromotions: string = '';
  reclassOptions: string = '';

  // Skill Type Options
  skillTypeOptions: string[] = ['', '', '', '', '', '', '', ''];
  availableSkillTypes: string[] = [
    'Acrobatics', 'Animal Handling', 'Arcana', 'Athletics', 'Deception',
    'History', 'Insight', 'Intimidation', 'Investigation', 'Medicine',
    'Nature', 'Perception', 'Performance', 'Persuasion', 'Religion',
    'Sleight of Hand', 'Stealth', 'Survival'
  ];

  ngOnInit(): void {
    // Component initialization
  }

  // Stat increment/decrement methods
  incrementStat(statBlock: any, stat: string, amount: number = 5): void {
    statBlock[stat] = Math.max(0, statBlock[stat] + amount);
  }

  decrementStat(statBlock: any, stat: string, amount: number = 5): void {
    statBlock[stat] = Math.max(0, statBlock[stat] - amount);
  }

  // Unit Types methods
  addUnitType(): void {
    this.unitTypes.push('');
  }

  removeUnitType(index: number): void {
    if (this.unitTypes.length > 1) {
      this.unitTypes.splice(index, 1);
    }
  }

  // Abilities methods
  addAbility(): void {
    this.abilities.push('');
  }

  removeAbility(index: number): void {
    if (this.abilities.length > 2) {
      this.abilities.splice(index, 1);
    }
  }

  // Usable Weapons methods
  addWeapon(): void {
    this.usableWeapons.push({ weaponType: '', minRank: 'E', maxRank: 'S' });
  }

  removeWeapon(index: number): void {
    if (this.usableWeapons.length > 1) {
      this.usableWeapons.splice(index, 1);
    }
  }

  // Save class
  saveClass(): void {
    // Filter out empty values
    const filteredUnitTypes = this.unitTypes.filter(ut => ut.trim() !== '').join(', ');
    const filteredAbilities = this.abilities.filter(a => a.trim() !== '');
    const filteredUsableWeapons = this.usableWeapons.filter(w => w.weaponType.trim() !== '');
    const filteredSkillTypes = this.skillTypeOptions.filter(st => st.trim() !== '');

    // Parse class promotions and reclass options
    const classPromotionsArray = this.classPromotions.trim() 
      ? this.classPromotions.split(',').map(c => c.trim()).filter(c => c !== '')
      : null;
    const reclassOptionsArray = this.reclassOptions.trim()
      ? this.reclassOptions.split(',').map(c => c.trim()).filter(c => c !== '')
      : null;

    // Build innate bonus object
    let innateBonus = null;
    if (this.hasInnateBonus) {
      innateBonus = {
        stats: this.hasInnateBonusStats ? this.innateBonusStats : null,
        growths: this.hasInnateBonusGrowths ? this.innateBonusGrowths : null,
        attributes: this.hasInnateBonusAttributes ? this.innateBonusAttributes : null
      };
    }

    const classData: any = {
      name: this.className,
      isPromoted: this.isPromoted,
      isSpecialClass: this.isSpecialClass,
      baseStats: this.baseStats,
      maxStats: this.maxStats,
      growthRate: this.growthRates,
      innateBonus: innateBonus,
      unitTypes: filteredUnitTypes,
      abilities: filteredAbilities.length > 0 ? { name: filteredAbilities[0] } : null,
      usableWeapons: filteredUsableWeapons.map(w => ({
        weaponType: w.weaponType,
        minRank: w.minRank,
        maxRank: w.maxRank
      })),
      classPromotions: classPromotionsArray,
      reclassOptions: reclassOptionsArray,
      skillTypeOptions: filteredSkillTypes
    };

    this.characterService.addClass(classData).subscribe({
      next: (result) => {
        console.log('Class created successfully:', result);
        this.dialogRef.close(result);
      },
      error: (error) => {
        console.error('Error creating class:', error);
      }
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
