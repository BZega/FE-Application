import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { CharacterService } from '../../core/services/character.service';
import { Equipment } from '../../core/models/Equipment';
import { Convoy } from '../../core/models/Convoy';
import { Character } from '../../core/models/Character';
import { Subject, takeUntil } from 'rxjs';
import { EquipmentStoreDialogComponent, PurchasedEquipmentResult } from '../equipment-store-dialog/equipment-store-dialog.component';

export interface ConvoyDialogData {
  character: Character;
}

@Component({
  selector: 'app-convoy-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule],
  templateUrl: './convoy-dialog.component.html',
  styleUrl: './convoy-dialog.component.scss'
})
export class ConvoyDialogComponent implements OnInit, OnDestroy {
  private dialogRef = inject(MatDialogRef<ConvoyDialogComponent>);
  private data = inject<ConvoyDialogData>(MAT_DIALOG_DATA);
  private characterService = inject(CharacterService);
  private dialog = inject(MatDialog);
  private destroy$ = new Subject<void>();

  character: Character;
  convoy: Convoy | null = null;
  loading = true;
  expandedItemId: string | null = null;

  // Tabs for weapon types
  selectedTab: string = 'All';
  weaponTypes = ['All', 'Axe', 'Bow', 'Dagger', 'Lance', 'Staff', 'Stone', 'Sword', 'Tome', 'DarkTome', 'Consumable'];
  filteredItems: Equipment[] = [];

  // For deposit modal
  showDepositDialog = false;
  depositItems: Equipment[] = [];

  ngOnInit() {
    this.character = this.data.character;
    console.log(this.character);
    this.loadConvoy();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadConvoy() {
    if (!this.character.convoyId) {
      console.error('No convoy ID on character');
      this.loading = false;
      return;
    }

    this.characterService.getConvoy(this.character.convoyId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (convoy) => {
          this.convoy = convoy;
          this.filterItems();
          this.loading = false;
        },
        error: (err) => {
          console.error('Error loading convoy:', err);
          this.loading = false;
        }
      });
  }

  selectTab(tab: string) {
    this.selectedTab = tab;
    this.filterItems();
    this.expandedItemId = null;
  }

  filterItems() {
    if (!this.convoy?.convoyItems?.equipment) {
      this.filteredItems = [];
      return;
    }

    if (this.selectedTab === 'All') {
      this.filteredItems = this.convoy.convoyItems.equipment;
    } else {
      this.filteredItems = this.convoy.convoyItems.equipment.filter(
        item => item.weaponType === this.selectedTab
      );
    }
  }

  getTabCount(type: string): number {
    if (!this.convoy?.convoyItems.equipment || this.convoy.convoyItems.equipment.length === 0) return 0;
    
    if (type === 'All') {
      return this.convoy.convoyItems.equipment.length;
    }
    return this.convoy.convoyItems.equipment.filter(item => item.weaponType === type).length;
  }

  toggleItemExpansion(itemOid: string) {
    this.expandedItemId = this.expandedItemId === itemOid ? null : itemOid;
  }

  isItemExpanded(itemOid: string): boolean {
    return this.expandedItemId === itemOid;
  }

  getItemIcon(item: Equipment): string {
    const weaponType = item.weaponType?.toLowerCase() || '';
    if (weaponType.includes('sword')) return '⚔️';
    if (weaponType.includes('axe')) return '🪓';
    if (weaponType.includes('lance') || weaponType.includes('spear')) return '🔱';
    if (weaponType.includes('bow')) return '🏹';
    if (weaponType.includes('tome') || weaponType.includes('magic')) return '📖';
    if (weaponType.includes('staff')) return '🪄';
    if (weaponType.includes('stone')) return '💎';
    if (weaponType.includes('consumable')) return '🧪';
    return '⚔️';
  }

  // Open deposit dialog
  openDepositDialog() {
    if (!this.character?.inventory?.equipment || this.character.inventory.equipment.length === 0) {
      console.log('No items in inventory to deposit');
      return;
    }
    
    this.depositItems = this.character.inventory.equipment;
    this.showDepositDialog = true;
  }

  depositItem(item: Equipment) {
    if (!item.equipOid) {
      console.error('Item missing equipOid');
      return;
    }

    this.characterService.updateConvoyItems(
      this.character.id,
      'DEPOSIT',
      'NONE',
      item.equipOid,
      null,
      null,
      null
    ).pipe(takeUntil(this.destroy$))
    .subscribe({
      next: () => {
        console.log('Item deposited successfully');
        this.closeDepositDialog();
        // Reload convoy and character data
        this.loadConvoy();
        this.reloadCharacter();
      },
      error: (err) => {
        console.error('Error depositing item:', err);
      }
    });
  }

  closeDepositDialog() {
    this.showDepositDialog = false;
    this.depositItems = [];
  }

  // Withdraw item from convoy to inventory
  withdrawItem(item: Equipment) {
    if (!item.equipOid) {
      console.error('Item missing equipOid');
      return;
    }

    // Check if inventory is full (max 5)
    if (this.character.inventory?.equipment?.length >= 5) {
      console.log('Inventory is full');
      return;
    }

    this.characterService.updateConvoyItems(
      this.character.id,
      'WITHDRAW',
      'NONE',
      item.equipOid,
      null,
      null,
      null
    ).pipe(takeUntil(this.destroy$))
    .subscribe({
      next: () => {
        console.log('Item withdrawn successfully');
        // Reload convoy and character data
        this.loadConvoy();
        this.reloadCharacter();
      },
      error: (err) => {
        console.error('Error withdrawing item:', err);
      }
    });
  }

  // Open equipment store
  openStore() {
    const dialogRef = this.dialog.open(EquipmentStoreDialogComponent, {
      width: '900px',
      maxWidth: '95vw',
      data: {
        initialGold: this.character.gold,
        currentEquipment: this.character.inventory?.equipment || [],
      },
      disableClose: false
    });

    dialogRef.afterClosed().subscribe((result: PurchasedEquipmentResult) => {
      if (result && result.equipment && result.equipment.length > 0) {
        // Purchase items using the service
        this.purchaseItems(result.equipment);
      }
    });
  }

  purchaseItems(equipment: Equipment[]) {
    // For each purchased item, call the BUY endpoint
    const purchasePromises = equipment.map(item => 
      this.characterService.updateConvoyItems(
        this.character.id,
        'BUY',
        'CONVOY',
        null,
        item.id,
        null,
        null
      ).toPromise()
    );

    Promise.all(purchasePromises).then(() => {
      console.log('All items purchased successfully');
      this.loadConvoy();
      this.reloadCharacter();
    }).catch(err => {
      console.error('Error purchasing items:', err);
    });
  }

  reloadCharacter() {
    this.characterService.getCharacterById(this.character.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (character) => {
          this.character = character;
          this.dialogRef.close({ character: this.character });
        },
        error: (err) => {
          console.error('Error reloading character:', err);
        }
      });
  }

  close() {
    this.dialogRef.close();
  }
}
