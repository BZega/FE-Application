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
import { Ability } from '../../core/models/Ability';

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
  @ViewChild('biographyDialog') biographyDialog!: TemplateRef<any>;
  @ViewChild('equipAbilitiesDialog') equipAbilitiesDialog!: TemplateRef<any>;
  
  character$: Observable<Character | null>;
  currentCharacter: Character | null = null;
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

  // Expanded item/ability tracking
  expandedItemIndex: number | null = null;
  expandedAbilityIndex: number | null = null;
  expandedPersonalAbility: boolean = false;
  expandedSkillIndex: number | null = null;

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
      
      // Subscribe to character changes to keep currentCharacter updated
      this.character$.subscribe(char => {
        this.currentCharacter = char;
      });
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

  openBiographyDialog(): void {
    this.dialog.open(this.biographyDialog, {
      width: '600px',
      maxWidth: '90vw',
      panelClass: 'character-biography-dialog'
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
    this.dialog.open(this.equipAbilitiesDialog, {
      width: '700px',
      maxWidth: '90vw',
      panelClass: 'equip-abilities-dialog'
    });
  }

  // Check if there are unequipped acquired abilities
  hasUnequippedAbilities(character: Character | null): boolean {
    if (!character?.acquiredAbilities || character.acquiredAbilities.length === 0) {
      return false;
    }

    const equippedAbilityIds = (character.equippedAbilities || []).map(a => a.id);
    const unequippedAbilities = character.acquiredAbilities.filter(
      ability => !equippedAbilityIds.includes(ability.id)
    );

    return unequippedAbilities.length > 0;
  }

  // Get list of unequipped acquired abilities
  getUnequippedAbilities(character: Character | null): any[] {
    if (!character?.acquiredAbilities || character.acquiredAbilities.length === 0) {
      return [];
    }

    const equippedAbilityIds = (character.equippedAbilities || []).map(a => a.id);
    return character.acquiredAbilities.filter(
      ability => !equippedAbilityIds.includes(ability.id)
    );
  }

  // Check if character has available ability slots
  hasAvailableAbilitySlots(character: Character | null): boolean {
    if (!character) return false;
    const equippedCount = character.equippedAbilities?.length || 0;
    return equippedCount < 5;
  }

  // Equip an ability
  equipAbility(ability: any): void {
    if (!this.currentCharacter) return;

    const equippedCount = this.currentCharacter.equippedAbilities?.length || 0;
    if (equippedCount >= 5) {
      console.log('Cannot equip more than 5 abilities');
      return;
    }

    console.log(`Equipping ability: ${ability.name}`);

    this.characterService.updateEquipedAbilities(this.currentCharacter.id, ability.abilityOid, 'EQUIP').subscribe({
      next: () => {
        // Refresh character data after successful equip
        this.refreshCharacterData();
      },
      error: (err) => {
        console.error('Error equipping ability:', err);
      }
    });
  }

  // Unequip an ability
  unequipAbility(ability: any, event?: Event): void {
    if (event) {
      event.stopPropagation(); // Prevent tooltip from showing when clicking unequip
    }

    if (!this.currentCharacter?.equippedAbilities) return;

    console.log(`Unequipping ability: ${ability.name}`);
    
    this.characterService.updateEquipedAbilities(this.currentCharacter.id, ability.abilityOid, 'UNEQUIP').subscribe({
      next: () => {
        // Refresh character data after successful unequip
        this.refreshCharacterData();
      },
      error: (err) => {
        console.error('Error unequipping ability:', err);
      }
    });
  }

  // Refresh character data from server
  private refreshCharacterData(): void {
    this.character$ = this.characterService.getCharacterById(this.characterId);
    this.character$.subscribe(char => {
      this.currentCharacter = char;
    });
  }

  // Check if an ability is already equipped
  isAbilityEquipped(ability: any, character: Character | null): boolean {
    if (!character?.equippedAbilities) return false;
    return character.equippedAbilities.some(a => a.id === ability.id);
  }

  // Check if this is the first empty slot (for showing equip button)
  isFirstEmptySlot(character: Character | null, index: number): boolean {
    if (!character) return false;
    const slots = this.getAbilitySlots(character);
    // Find the index of the first empty slot
    const firstEmptyIndex = slots.findIndex(slot => slot === null);
    return index === firstEmptyIndex;
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

  hasSpecialProperties(item: any): boolean {
    return item.isMagical || item.isBrave || item.doesEffectiveDamage;
  }

  // Item expand/collapse methods
  toggleItemExpansion(index: number): void {
    if (this.expandedItemIndex === index) {
      this.expandedItemIndex = null; // Collapse if already expanded
    } else {
      this.expandedItemIndex = index; // Expand this item
    }
  }

  isItemExpanded(index: number): boolean {
    return this.expandedItemIndex === index;
  }

  // Ability expand/collapse methods
  toggleAbilityExpansion(index: number): void {
    if (this.expandedAbilityIndex === index) {
      this.expandedAbilityIndex = null; // Collapse if already expanded
    } else {
      this.expandedAbilityIndex = index; // Expand this ability
      this.expandedPersonalAbility = false; // Collapse personal ability if open
    }
  }

  togglePersonalAbilityExpansion(): void {
    this.expandedPersonalAbility = !this.expandedPersonalAbility;
    if (this.expandedPersonalAbility) {
      this.expandedAbilityIndex = null; // Collapse any expanded equipped ability
    }
  }

  isAbilityExpanded(index: number): boolean {
    return this.expandedAbilityIndex === index;
  }

  toggleSkillExpansion(index: number): void {
    this.expandedSkillIndex = this.expandedSkillIndex === index ? null : index;
  }

  isSkillExpanded(index: number): boolean {
    return this.expandedSkillIndex === index;
  }

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
}
