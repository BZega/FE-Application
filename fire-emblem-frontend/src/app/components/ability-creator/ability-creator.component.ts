import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../material/material.module';
import { CharacterService } from '../../core/services/character.service';

interface StatBonusStats {
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

interface StatBonusGrowths {
  hp: number;
  str: number;
  mag: number;
  skl: number;
  spd: number;
  lck: number;
  def: number;
  res: number;
}

interface StatBonusAttributes {
  hit: number;
  crit: number;
  avoid: number;
  dodge: number;
  heal: number;
  damage: number;
  damageReceived: number;
  attackSpeed: number;
}

@Component({
  selector: 'app-ability-creator',
  imports: [CommonModule, MaterialModule, FormsModule],
  templateUrl: './ability-creator.component.html',
  styleUrl: './ability-creator.component.scss'
})
export class AbilityCreatorComponent implements OnInit {
  private dialogRef = inject(MatDialogRef<AbilityCreatorComponent>);
  private characterService = inject(CharacterService);

  // Basic info
  abilityName: string = '';
  description: string = '';
  levelAcquired: number = 1;
  type: string = '';
  combatCheck: boolean = false;

  // Stat Bonus
  hasStatBonus: boolean = false;
  hasStatBonusStats: boolean = false;
  hasStatBonusGrowths: boolean = false;
  hasStatBonusAttributes: boolean = false;
  
  statBonusStats: StatBonusStats = { hp: 0, str: 0, mag: 0, skl: 0, spd: 0, lck: 0, def: 0, res: 0, mov: 0 };
  statBonusGrowths: StatBonusGrowths = { hp: 0, str: 0, mag: 0, skl: 0, spd: 0, lck: 0, def: 0, res: 0 };
  statBonusAttributes: StatBonusAttributes = { hit: 0, crit: 0, avoid: 0, dodge: 0, heal: 0, damage: 0, damageReceived: 0, attackSpeed: 0 };

  // Available types
  availableTypes: string[] = ['Combat', 'Command', 'Passive'];

  ngOnInit(): void {
    // Component initialization
  }

  // No increment/decrement methods needed - using direct input like class creator

  // Save ability
  saveAbility(): void {
    let statBonusData: any = null;

    if (this.hasStatBonus) {
      statBonusData = {};
      if (this.hasStatBonusStats) {
        statBonusData.stats = { ...this.statBonusStats };
      }
      if (this.hasStatBonusGrowths) {
        statBonusData.growths = { ...this.statBonusGrowths };
      }
      if (this.hasStatBonusAttributes) {
        statBonusData.attributes = { ...this.statBonusAttributes };
      }
    }

    this.characterService.addAbility(
      this.abilityName,
      this.description,
      this.levelAcquired,
      this.type,
      this.combatCheck,
      statBonusData
    ).subscribe({
      next: (result) => {
        console.log('Ability created successfully:', result);
        this.dialogRef.close(result);
      },
      error: (error) => {
        console.error('Error creating ability:', error);
      }
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
