import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

export interface ConfirmationModalData {
  type: 'create' | 'cancel';
  characterData?: {
    name: string;
    race: string;
    class: string;
    gender: string;
    personalAbility: string;
    equippedWeapon: string;
    baseStats: {
      hp: number;
      str: number;
      mag: number;
      skl: number;
      spd: number;
      lck: number;
      def: number;
      res: number;
    };
  };
}

@Component({
  selector: 'app-confirmation-modal',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule],
  template: `
    <div class="confirmation-modal">
      <h2 mat-dialog-title class="modal-title">
        {{ data.type === 'create' ? 'Create Character?' : 'Cancel Creation?' }}
      </h2>
      
      <mat-dialog-content class="modal-content">
        <div *ngIf="data.type === 'cancel'" class="cancel-message">
          <p>Are you sure you want to leave? Your changes will not be saved.</p>
        </div>
        
        <div *ngIf="data.type === 'create' && data.characterData" class="character-overview">
          <p class="confirm-message">Are you sure you want to create this character?</p>
          
          <div class="overview-section">
            <h3>Character Overview</h3>
            <div class="overview-grid">
              <div class="overview-item">
                <span class="label">Name:</span>
                <span class="value">{{ data.characterData.name }}</span>
              </div>
              <div class="overview-item">
                <span class="label">Race:</span>
                <span class="value">{{ data.characterData.race }}</span>
              </div>
              <div class="overview-item">
                <span class="label">Class:</span>
                <span class="value">{{ data.characterData.class }}</span>
              </div>
              <div class="overview-item">
                <span class="label">Gender:</span>
                <span class="value">{{ data.characterData.gender }}</span>
              </div>
              <div class="overview-item">
                <span class="label">Personal Ability:</span>
                <span class="value">{{ data.characterData.personalAbility }}</span>
              </div>
              <div class="overview-item" *ngIf="data.characterData.equippedWeapon">
                <span class="label">Equipped Weapon:</span>
                <span class="value">{{ data.characterData.equippedWeapon }}</span>
              </div>
            </div>
            
            <h4>Base Stats</h4>
            <div class="stats-grid">
              <div class="stat-item">
                <span class="stat-label">HP</span>
                <span class="stat-value">{{ data.characterData.baseStats.hp }}</span>
              </div>
              <div class="stat-item">
                <span class="stat-label">Str</span>
                <span class="stat-value">{{ data.characterData.baseStats.str }}</span>
              </div>
              <div class="stat-item">
                <span class="stat-label">Mag</span>
                <span class="stat-value">{{ data.characterData.baseStats.mag }}</span>
              </div>
              <div class="stat-item">
                <span class="stat-label">Skl</span>
                <span class="stat-value">{{ data.characterData.baseStats.skl }}</span>
              </div>
              <div class="stat-item">
                <span class="stat-label">Spd</span>
                <span class="stat-value">{{ data.characterData.baseStats.spd }}</span>
              </div>
              <div class="stat-item">
                <span class="stat-label">Lck</span>
                <span class="stat-value">{{ data.characterData.baseStats.lck }}</span>
              </div>
              <div class="stat-item">
                <span class="stat-label">Def</span>
                <span class="stat-value">{{ data.characterData.baseStats.def }}</span>
              </div>
              <div class="stat-item">
                <span class="stat-label">Res</span>
                <span class="stat-value">{{ data.characterData.baseStats.res }}</span>
              </div>
            </div>
          </div>
        </div>
      </mat-dialog-content>
      
      <mat-dialog-actions class="modal-actions">
        <button mat-button class="modal-cancel-btn" (click)="onCancel()">
          {{ data.type === 'create' ? 'Back' : 'Stay' }}
        </button>
        <button mat-raised-button class="modal-confirm-btn" (click)="onConfirm()">
          {{ data.type === 'create' ? 'Create Character' : 'Leave' }}
        </button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [`
    .confirmation-modal {
      max-width: 600px;
    }
    
    .modal-title {
      color: var(--primary-color);
      font-size: 24px;
      font-weight: 700;
      margin: 0 0 16px 0;
      padding: 0;
    }
    
    .modal-content {
      padding: 0 24px !important;
      max-height: 70vh;
      overflow-y: auto;
    }
    
    .cancel-message {
      padding: 20px 0;
      font-size: 16px;
      color: var(--text-color);
    }
    
    .character-overview {
      padding: 10px 0;
    }
    
    .confirm-message {
      font-size: 16px;
      color: var(--text-color);
      margin-bottom: 20px;
    }
    
    .overview-section h3 {
      color: var(--primary-color);
      font-size: 18px;
      font-weight: 600;
      margin: 16px 0 12px 0;
      padding-bottom: 8px;
      border-bottom: 2px solid var(--input-border);
    }
    
    .overview-section h4 {
      color: var(--primary-color);
      font-size: 16px;
      font-weight: 600;
      margin: 20px 0 12px 0;
    }
    
    .overview-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 12px;
      margin-bottom: 12px;
    }
    
    .overview-item {
      display: flex;
      flex-direction: column;
      gap: 4px;
      padding: 8px;
      background: var(--input-bg);
      border-radius: 6px;
    }
    
    .overview-item .label {
      font-size: 12px;
      font-weight: 600;
      color: var(--primary-color);
      text-transform: uppercase;
    }
    
    .overview-item .value {
      font-size: 14px;
      font-weight: 500;
      color: var(--text-color);
    }
    
    .stats-grid {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
      gap: 8px;
    }
    
    .stat-item {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: 8px;
      background: var(--input-bg);
      border: 2px solid var(--input-border);
      border-radius: 6px;
    }
    
    .stat-label {
      font-size: 11px;
      font-weight: 700;
      color: var(--primary-color);
      text-transform: uppercase;
    }
    
    .stat-value {
      font-size: 16px;
      font-weight: 700;
      color: var(--text-color);
      margin-top: 4px;
    }
    
    .modal-actions {
      padding: 16px 24px !important;
      display: flex;
      justify-content: flex-end;
      gap: 12px;
      border-top: 1px solid var(--input-border);
    }
    
    .modal-cancel-btn {
      padding: 8px 24px;
      color: var(--text-color);
      border: 2px solid var(--input-border);
      border-radius: 6px;
      font-weight: 600;
      transition: all 0.3s ease;
    }
    
    .modal-cancel-btn:hover {
      background: var(--input-bg);
      border-color: var(--primary-color);
    }
    
    .modal-confirm-btn {
      padding: 8px 24px;
      background: linear-gradient(135deg, #3f51b5, #5c6bc0);
      color: white;
      border: none;
      border-radius: 6px;
      font-weight: 600;
      transition: all 0.3s ease;
    }
    
    .modal-confirm-btn:hover {
      background: linear-gradient(135deg, #5c6bc0, #3f51b5);
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(63, 81, 181, 0.3);
    }
  `]
})
export class ConfirmationModalComponent {
  dialogRef = inject(MatDialogRef<ConfirmationModalComponent>);
  data = inject<ConfirmationModalData>(MAT_DIALOG_DATA);

  onCancel() {
    this.dialogRef.close(false);
  }

  onConfirm() {
    this.dialogRef.close(true);
  }
}
