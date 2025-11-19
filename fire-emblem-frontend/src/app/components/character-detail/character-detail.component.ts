import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { Character } from '../../core/models/Character';
import { CharacterService } from '../../core/services/character.service';
import { MaterialModule } from '../../material/material.module';

@Component({
  selector: 'app-character-detail',
  imports: [CommonModule, MaterialModule, FormsModule],
  templateUrl: './character-detail.component.html',
  styleUrl: './character-detail.component.scss',
  animations: [
    trigger('fadeInOut', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(-10px)' }),
        animate('200ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
      ]),
      transition(':leave', [
        animate('150ms ease-in', style({ opacity: 0, transform: 'translateY(-10px)' }))
      ])
    ])
  ]
})
export class CharacterDetailComponent implements OnInit {
  @ViewChild('actionsDialog') actionsDialog!: TemplateRef<any>;
  
  character$: Observable<Character | null>;
  characterId: number;
  
  // Dialog form fields
  selectedTerrain: string = '';
  experienceToGain: number | null = null;
  terrainOptions: string[] = [
    'Normal',
    'Forest',
    'Mountain',
    'Desert',
    'Water',
    'Fort',
    'Throne',
    'Gate',
    'Village'
  ];

  // Tooltip properties
  hoveredItem: any = null;
  hoveredAbility: any = null;
  isPersonalAbility: boolean = false;
  tooltipPosition = { x: 0, y: 0 };
  private tooltipTimeout: any = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private characterService: CharacterService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.characterId = +params['id'];
      this.character$ = this.characterService.getCharacterById(this.characterId);
    });
  }

  goBack(): void {
    this.router.navigate(['/character-summary-container']);
  }

  openActionsDialog(): void {
    this.dialog.open(this.actionsDialog, {
      width: '500px',
      maxWidth: '90vw',
      panelClass: 'character-actions-dialog'
    });
  }

  gainExperience(): void {
    if (this.experienceToGain && this.experienceToGain > 0) {
      console.log(`Gaining ${this.experienceToGain} experience points`);
      // TODO: Implement experience gain logic here
      // You can call a service method to update the character's experience
      
      // Reset the input after gaining experience
      this.experienceToGain = null;
    }
  }

  openEquipAbilitiesModal(): void {
    console.log('Opening Equip Abilities modal');
    // TODO: Implement equip abilities modal
    // This will open another dialog or modal for managing abilities
  }

  // Inventory helper methods
  getInventorySlots(character: Character | null): (any | null)[] {
    if (!character?.inventory?.equipment) {
      return [null, null, null, null, null]; // 5 empty slots
    }

    const inventory = character.inventory.equipment;
    const slots: (any | null)[] = [];
    
    // Add up to 5 items from inventory
    for (let i = 0; i < 5; i++) {
      slots.push(inventory[i] || null);
    }
    
    return slots;
  }

  getInventoryCount(character: Character | null): number {
    if (!character?.inventory?.equipment) {
      return 0;
    }
    return Math.min(character.inventory.equipment.length, 5);
  }

  isEquipped(item: any, character: Character | null): boolean {
    if (!item || !character?.equippedWeapon) {
      return false;
    }
    // Check if this item matches the equipped weapon (by id or name)
    return item.id === character.equippedWeapon.id || 
           item.name === character.equippedWeapon.name;
  }

  getItemIcon(item: any): string {
    if (!item) return '📦';
    
    // Return icon based on weapon type
    const weaponType = item.weaponType?.toLowerCase() || '';
    
    // Check if it's an item (non-weapon)
    if (weaponType.includes('item') || weaponType.includes('consumable') || 
        weaponType.includes('vulnerary') || weaponType.includes('elixir') || 
        weaponType.includes('tonic') || weaponType.includes('stat booster') ||
        weaponType.includes('potion') || weaponType.includes('medicine')) {
      return '🎒'; // Bag for items
    }
    
    // Weapons
    if (weaponType.includes('sword')) return '⚔️';
    if (weaponType.includes('lance') || weaponType.includes('spear')) return '🔱';
    if (weaponType.includes('axe')) return '🪓';
    if (weaponType.includes('bow')) return '🏹';
    if (weaponType.includes('tome') || weaponType.includes('magic')) return '📖';
    if (weaponType.includes('staff') || weaponType.includes('heal')) return '🪄';
    if (weaponType.includes('dragonstone')) return '🐉';
    if (weaponType.includes('stone')) return '💎';
    if (weaponType.includes('knife') || weaponType.includes('dagger')) return '🗡️';
    if (weaponType.includes('shuriken')) return '⭐';
    
    // If weaponType suggests it's not a weapon, use bag icon
    if (!weaponType || weaponType === 'none' || weaponType === 'n/a') {
      return '🎒';
    }
    
    return '⚔️'; // Default weapon icon
  }

  // Tooltip methods
  showItemDetails(item: any, event: MouseEvent): void {
    // Clear any existing timeout
    if (this.tooltipTimeout) {
      clearTimeout(this.tooltipTimeout);
    }

    // Small delay before showing tooltip to avoid flickering
    this.tooltipTimeout = setTimeout(() => {
      this.hoveredItem = item;
      this.updateTooltipPosition(event);
    }, 300);
  }

  hideItemDetails(): void {
    // Clear timeout if user leaves before tooltip shows
    if (this.tooltipTimeout) {
      clearTimeout(this.tooltipTimeout);
      this.tooltipTimeout = null;
    }
    
    this.hoveredItem = null;
  }

  updateTooltipPosition(event: MouseEvent): void {
    const offset = 20; // Distance from cursor
    const tooltipWidth = 350; // Approximate tooltip width
    const tooltipHeight = 400; // Approximate max tooltip height
    
    let x = event.clientX + offset;
    let y = event.clientY + offset;

    // Adjust if tooltip would go off right edge
    if (x + tooltipWidth > window.innerWidth) {
      x = event.clientX - tooltipWidth - offset;
    }

    // Adjust if tooltip would go off bottom edge
    if (y + tooltipHeight > window.innerHeight) {
      y = event.clientY - tooltipHeight - offset;
    }

    // Ensure tooltip doesn't go off left edge
    if (x < 0) {
      x = offset;
    }

    // Ensure tooltip doesn't go off top edge
    if (y < 0) {
      y = offset;
    }

    this.tooltipPosition = { x, y };
  }

  hasSpecialProperties(item: any): boolean {
    return item.isMagical || item.isBrave || item.doesEffectiveDamage;
  }

  // Ability helper methods
  getAbilitySlots(character: Character | null): (any | null)[] {
    if (!character?.equippedAbilities) {
      return [null, null, null, null, null]; // 5 empty slots
    }

    const abilities = character.equippedAbilities;
    const slots: (any | null)[] = [];
    
    // Add up to 5 abilities from equipped abilities
    for (let i = 0; i < 5; i++) {
      slots.push(abilities[i] || null);
    }
    
    return slots;
  }

  getEquippedAbilitiesCount(character: Character | null): number {
    if (!character?.equippedAbilities) {
      return 0;
    }
    return Math.min(character.equippedAbilities.length, 5);
  }

  // Ability tooltip methods
  showAbilityDetails(ability: any, event: MouseEvent, isPersonal: boolean): void {
    // Clear any existing timeout
    if (this.tooltipTimeout) {
      clearTimeout(this.tooltipTimeout);
    }

    // Small delay before showing tooltip to avoid flickering
    this.tooltipTimeout = setTimeout(() => {
      this.hoveredAbility = ability;
      this.isPersonalAbility = isPersonal;
      this.updateTooltipPosition(event);
    }, 300);
  }

  hideAbilityDetails(): void {
    // Clear timeout if user leaves before tooltip shows
    if (this.tooltipTimeout) {
      clearTimeout(this.tooltipTimeout);
      this.tooltipTimeout = null;
    }
    
    this.hoveredAbility = null;
    this.isPersonalAbility = false;
  }
}
