import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { LevelUp } from '../../core/models/LevelUp';

export interface UpdateResourceDialogData {
  type: 'gold' | 'exp';
  currentGold: number;
  currentExp: number;
  characterLevel?: number;
  isSpecialClass?: boolean;
  classAbilities?: any[];
  equippedAbilityCount?: number;
}

@Component({
  selector: 'app-update-resource-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatButtonModule,
    MatCheckboxModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatTooltipModule
  ],
  templateUrl: './update-resource-dialog.component.html',
  styleUrls: ['./update-resource-dialog.component.scss']
})
export class UpdateResourceDialogComponent {
  amountToAdd: number = 0;
  manualLevelUp: boolean = false;
  
  pendingLevelUps: { level: number, stats: { [key: string]: boolean } }[] = [];
  readonly statKeys = ['hp', 'str', 'mag', 'skl', 'spd', 'lck', 'def', 'res'];
  
  availableAbilities: any[] = [];
  selectedAbilityId: number | null = null;
  equipAbility: boolean = false;

  constructor(
    public dialogRef: MatDialogRef<UpdateResourceDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: UpdateResourceDialogData
  ) {
    this.checkForAbilityAcquisition();
  }

  get currentValue(): number {
    return this.data.type === 'gold' ? this.data.currentGold : this.data.currentExp;
  }

  get newTotal(): number {
    return (this.currentValue || 0) + (this.amountToAdd || 0);
  }

  get maxAllowedExperience(): number {
    if (this.data.type !== 'exp') return Infinity;
    
    const currentLevel = this.data.characterLevel || 1;
    const maxLevel = this.data.isSpecialClass ? 40 : 20;
    const currentExp = this.data.currentExp || 0;
    
    // Calculate total experience to reach max level with 0 exp (e.g., level 20 exp 0)
    const maxTotalExp = (maxLevel - currentLevel) * 100;
    
    // Return the maximum amount we can add
    return maxTotalExp - currentExp;
  }

  onAmountChange() {
    if (this.data.type === 'exp') {
      // Cap the experience to prevent overflow
      if (this.amountToAdd > this.maxAllowedExperience) {
        this.amountToAdd = this.maxAllowedExperience;
      }
      this.calculateLevelUps();
      this.checkForAbilityAcquisition();
    }
  }
  
  checkForAbilityAcquisition() {
    if (this.data.type !== 'exp' || !this.data.classAbilities) {
      this.availableAbilities = [];
      return;
    }
    
    const currentLevel = this.data.characterLevel || 1;
    const currentExp = this.data.currentExp || 0;
    const totalExp = currentExp + (this.amountToAdd || 0);
    const levelsGained = Math.floor(totalExp / 100);
    
    if (levelsGained === 0) {
      this.availableAbilities = [];
      return;
    }
    
    // Find abilities that match any level threshold crossed (excluding level 1)
    const abilitiesArray = Array.isArray(this.data.classAbilities) 
      ? this.data.classAbilities 
      : [this.data.classAbilities];
    
    // Only get abilities for levels we're gaining (not current level)
    const minLevel = currentLevel + 1;
    const maxLevel = currentLevel + levelsGained;
    
    this.availableAbilities = abilitiesArray.filter((ability: any) => 
      ability.levelGained > 1 && 
      ability.levelGained >= minLevel && 
      ability.levelGained <= maxLevel
    );
    
    // Auto-select if only one ability
    if (this.availableAbilities.length === 1) {
      this.selectedAbilityId = this.availableAbilities[0].id;
    } else {
      this.selectedAbilityId = null;
    }
    
    // Reset equip checkbox
    this.equipAbility = false;
  }
  
  get canEquipAbility(): boolean {
    return (this.data.equippedAbilityCount || 0) < 5;
  }
  
  get hasAbilityToAcquire(): boolean {
    return this.availableAbilities.length > 0;
  }

  onManualLevelUpChange() {
    this.calculateLevelUps();
  }

  calculateLevelUps() {
    if (!this.manualLevelUp || this.data.type !== 'exp') {
      this.pendingLevelUps = [];
      return;
    }

    const currentExp = this.data.currentExp;
    const addedExp = this.amountToAdd;
    const totalExp = currentExp + addedExp;
    const currentLevel = this.data.characterLevel || 1;

    // Calculate levels gained. 100 exp per level.
    const levelsGained = Math.floor(totalExp / 100);
    
    const newPendingLevelUps = [];
    for (let i = 0; i < levelsGained; i++) {
      const level = currentLevel + i + 1;
      
      const existing = this.pendingLevelUps.find(p => p.level === level);
      if (existing) {
        newPendingLevelUps.push(existing);
      } else {
        newPendingLevelUps.push({
          level: level,
          stats: {
            hp: false, str: false, mag: false, skl: false, 
            spd: false, lck: false, def: false, res: false
          }
        });
      }
    }
    this.pendingLevelUps = newPendingLevelUps;
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onConfirm(): void {
    let manualLevelUps = null;

    if (this.data.type === 'exp' && this.manualLevelUp) {
      manualLevelUps = this.pendingLevelUps.map(p => ({
        level: p.level,
        levelUpType: 'Manual',
        statIncrease: {
          hp: p.stats['hp'] ? 1 : 0,
          str: p.stats['str'] ? 1 : 0,
          mag: p.stats['mag'] ? 1 : 0,
          skl: p.stats['skl'] ? 1 : 0,
          spd: p.stats['spd'] ? 1 : 0,
          lck: p.stats['lck'] ? 1 : 0,
          def: p.stats['def'] ? 1 : 0,
          res: p.stats['res'] ? 1 : 0,
          move: 0
        }
      }));
    }

    const payload: any = {
      gold: this.data.type === 'gold' ? this.amountToAdd : 0,
      experiencePoints: this.data.type === 'exp' ? this.amountToAdd : 0,
      manualLevelUps: manualLevelUps,
      abilityToAcquire: this.selectedAbilityId,
      equipAcquiredAbility: this.equipAbility
    };
    
    this.dialogRef.close(payload);
  }
}
