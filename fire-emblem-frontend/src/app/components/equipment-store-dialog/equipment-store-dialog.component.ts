import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { CharacterService } from '../../core/services/character.service';
import { Equipment } from '../../core/models/Equipment';

export interface EquipmentStoreData {
  initialGold: number;
  currentEquipment?: Equipment[];
}

export interface PurchasedEquipmentResult {
  equipment: Equipment[];
  remainingGold: number;
}

@Component({
  selector: 'app-equipment-store-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule],
  templateUrl: './equipment-store-dialog.component.html',
  styleUrl: './equipment-store-dialog.component.scss'
})
export class EquipmentStoreDialogComponent implements OnInit {
  private dialogRef = inject(MatDialogRef<EquipmentStoreDialogComponent>);
  private data = inject<EquipmentStoreData>(MAT_DIALOG_DATA);
  private characterService = inject(CharacterService);

  allEquipment: Equipment[] = [];
  filteredEquipment: Equipment[] = [];
  currentGold: number = 0;
  purchasedItems: Equipment[] = [];
  expandedItemId: number | null = null;
  loading = true;

  // Tabs
  selectedTab: string = 'All';
  weaponTypes = ['All', 'Axe', 'Bow', 'Dagger', 'Lance', 'Staff', 'Stone', 'Sword', 'Tome', 'DarkTome', 'Consumable'];

  // For confirmation dialog
  showConfirmDialog = false;
  confirmingItem: Equipment | null = null;
  confirmError = '';

  ngOnInit() {
    this.currentGold = this.data.initialGold;
    
    // If reopening store, restore previous purchases
    if (this.data.currentEquipment && this.data.currentEquipment.length > 0) {
      this.purchasedItems = this.data.currentEquipment.map(item => ({ ...item }));
      // Calculate gold spent
      const goldSpent = this.purchasedItems.reduce((sum, item) => 
        sum + this.parseWorth(item.worth), 0);
      this.currentGold = this.data.initialGold - goldSpent;
    }
    
    this.loadEquipment();
  }

  loadEquipment() {
    this.characterService.getAllEquipment().subscribe({
      next: (equipment) => {
        // Filter out items with worth "-" or empty
        this.allEquipment = equipment
          .filter(item => item.worth && item.worth !== '-' && item.worth !== '0' && item.worth.trim() !== '–')
          .sort((a, b) => {
            // Sort by weapon type, then by worth
            if (a.weaponType !== b.weaponType) {
              return a.weaponType.localeCompare(b.weaponType);
            }
            return this.parseWorth(a.worth) - this.parseWorth(b.worth);
          });
        this.filterEquipment();
        this.loading = false;
        console.log('Equipment loaded:', this.allEquipment);
      },
      error: (err) => {
        console.error('Error loading equipment:', err);
        this.loading = false;
      }
    });
  }

  filterEquipment() {
    if (this.selectedTab === 'All') {
      this.filteredEquipment = this.allEquipment;
    } else {
      this.filteredEquipment = this.allEquipment.filter(
        item => item.weaponType === this.selectedTab
      );
    }
  }

  selectTab(tab: string) {
    this.selectedTab = tab;
    this.filterEquipment();
    this.expandedItemId = null; // Collapse any expanded items when switching tabs
  }

  getTabCount(type: string): number {
    if (type === 'All') {
      return this.allEquipment.length;
    }
    return this.allEquipment.filter(item => item.weaponType === type).length;
  }

  parseWorth(worth: string): number {
    return parseInt(worth) || 0;
  }

  toggleItemExpansion(itemId: number) {
    this.expandedItemId = this.expandedItemId === itemId ? null : itemId;
  }

  isItemExpanded(itemId: number): boolean {
    return this.expandedItemId === itemId;
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

  initiratePurchase(item: Equipment) {
    const itemWorth = this.parseWorth(item.worth);
    
    if (itemWorth > this.currentGold) {
      this.confirmError = 'Not enough gold';
      this.confirmingItem = item;
      this.showConfirmDialog = true;
      setTimeout(() => {
        this.showConfirmDialog = false;
        this.confirmingItem = null;
        this.confirmError = '';
      }, 2000);
    } else {
      this.confirmingItem = item;
      this.confirmError = '';
      this.showConfirmDialog = true;
    }
  }

  confirmPurchase() {
    if (!this.confirmingItem || this.confirmError) return;

    const itemWorth = this.parseWorth(this.confirmingItem.worth);
    this.currentGold -= itemWorth;
    this.purchasedItems.push({ ...this.confirmingItem });
    
    this.showConfirmDialog = false;
    this.confirmingItem = null;
  }

  cancelPurchase() {
    this.showConfirmDialog = false;
    this.confirmingItem = null;
    this.confirmError = '';
  }

  isPurchased(item: Equipment): boolean {
    return this.purchasedItems.some(p => p.id === item.id);
  }

  getPurchaseCount(item: Equipment): number {
    return this.purchasedItems.filter(p => p.id === item.id).length;
  }

  removePurchasedItem(item: Equipment) {
    const index = this.purchasedItems.findIndex(p => p.id === item.id);
    if (index !== -1) {
      const removedItem = this.purchasedItems.splice(index, 1)[0];
      this.currentGold += this.parseWorth(removedItem.worth);
    }
  }

  finalizePurchases() {
    const result: PurchasedEquipmentResult = {
      equipment: this.purchasedItems.map(item => ({ ...item, isEquipped: item.isEquipped || false })),
      remainingGold: this.currentGold
    };

    this.dialogRef.close(result);
  }

  close() {
    this.dialogRef.close(null);
  }
}
