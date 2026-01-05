import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { LevelUp } from '../../core/models/LevelUp';

@Component({
  selector: 'app-level-up-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule],
  templateUrl: './level-up-dialog.component.html',
  styleUrls: ['./level-up-dialog.component.scss']
})
export class LevelUpDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<LevelUpDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { levelUps: LevelUp[] }
  ) {}

  getStatIncreases(levelUp: LevelUp): { stat: string, value: number }[] {
    const increases: { stat: string, value: number }[] = [];
    if (!levelUp.statIncrease) return increases;

    const stats = ['hp', 'str', 'mag', 'skl', 'spd', 'lck', 'def', 'res'];
    for (const stat of stats) {
      if (levelUp.statIncrease[stat as keyof typeof levelUp.statIncrease] > 0) {
        increases.push({ 
          stat: stat.toUpperCase(), 
          value: levelUp.statIncrease[stat as keyof typeof levelUp.statIncrease] 
        });
      }
    }
    return increases;
  }

  onClose(): void {
    this.dialogRef.close();
  }
}
