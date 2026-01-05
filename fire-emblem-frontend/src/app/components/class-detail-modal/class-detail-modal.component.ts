import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../material/material.module';
import { UnitClass } from '../../core/models/UnitClass';
import { Stats } from '../../core/models/Stats';
import { Ability } from '../../core/models/Ability';

@Component({
  selector: 'app-class-detail-modal',
  imports: [CommonModule, MaterialModule],
  templateUrl: './class-detail-modal.component.html',
  styleUrl: './class-detail-modal.component.scss'
})
export class ClassDetailModalComponent {
  unitClass: UnitClass;
  classAbilities: Ability[] = [];

  constructor(
    public dialogRef: MatDialogRef<ClassDetailModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { unitClass: UnitClass }
  ) {
    this.unitClass = data.unitClass;
    
    // Handle abilities - might be single object or array
    if (this.unitClass.abilities) {
      this.classAbilities = Array.isArray(this.unitClass.abilities) 
        ? this.unitClass.abilities 
        : [this.unitClass.abilities];
    }
  }

  close(): void {
    this.dialogRef.close();
  }

  // Get stat percentage for progress bar
  getStatPercentage(base: number, max: number): number {
    if (!max || max === 0) return 0;
    if (base <= 0) return 0;
    return Math.min((base / max) * 100, 100);
  }

  // Check if base stat equals max stat (green indicator)
  isBaseStatMaxed(stat: keyof Stats): boolean {
    if (!this.unitClass?.baseStats || !this.unitClass?.maxStats) return false;
    const baseStat = this.unitClass.baseStats[stat] || 0;
    const maxStat = this.unitClass.maxStats[stat] || 40;
    return baseStat === maxStat;
  }
}
