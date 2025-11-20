import { Component, inject, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { CharacterService } from '../../core/services/character.service';
import { UnitClass } from '../../core/models/UnitClass';
import { ThemeService } from '../../core/services/theme.service';
import { PersonalAbility } from '../../core/models/PersonalAbility';
import { Ability } from '../../core/models/Ability';
import { Equipment } from '../../core/models/Equipment';
import { EquipmentStoreDialogComponent } from '../equipment-store-dialog/equipment-store-dialog.component';
import { ConfirmationModalComponent, ConfirmationModalData } from './confirmation-modal.component';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-character-creator',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule, MatButtonModule],
  templateUrl: './character-creator.component.html',
  styleUrl: './character-creator.component.scss'
})
export class CharacterCreatorComponent {
  private characterService = inject(CharacterService);
  private dialog = inject(MatDialog);
  @Optional() private dialogRef = inject(MatDialogRef<CharacterCreatorComponent>, { optional: true });
  themeService = inject(ThemeService);

  // Class dropdown options
  availableClasses = [
    { name: 'Archer', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Apothecary', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Cavalier', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Cleric', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Dark Mage', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Fighter', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Kitsune', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: 'Kitsune' },
    { name: 'Knight', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Lord', hasWeaponChoice: true, weaponTypes: ['Sword', 'Axe', 'Lance'], requiresNoble: true, requiredRace: 'Human' },
    { name: 'Mage', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Manakete Heir', hasWeaponChoice: false, weaponTypes: [], requiresNoble: true, requiredRace: 'Manakete' },
    { name: 'Mercenary', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Myrmidon', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Ninja', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Pegasus Knight', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Oni Savage', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Spear Fighter', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Tactician', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Thief', hasWeaponChoice: true, weaponTypes: ['Sword', 'Bow'], requiresNoble: false, requiredRace: null },
    { name: 'Troubadour', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Villager', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Wolfskin', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: 'Wolfskin' },
    { name: 'Wyvern Rider', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: null },
    { name: 'Manakete', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: 'Manakete' },
    { name: 'Performer', hasWeaponChoice: true, weaponTypes: ['Sword', 'Lance'], requiresNoble: false, requiredRace: null },
    { name: 'Taguel', hasWeaponChoice: false, weaponTypes: [], requiresNoble: false, requiredRace: 'Taguel' }
  ];

  get filteredClasses() {
    return this.availableClasses.filter(c => {
      // Check noble requirement
      if (c.requiresNoble && !this.isNoble) return false;
      
      // Check race requirement
      if (c.requiredRace) {
        // Get the final race choice
        const effectiveRace = this.getEffectiveRace();
        
        // For Half-Human, check if they have the required race as their half choice
        if (this.selectedRace === 'Half-Human') {
          return this.halfRaceChoice === c.requiredRace;
        }
        
        // For pure races, direct match
        return effectiveRace === c.requiredRace;
      }
      
      return true;
    });
  }

  get filteredHeartSealClasses() {
    return this.availableClasses.filter(c => {
      // Can't be the same as starting class
      if (c.name === this.selectedClassBase) return false;
      
      // Check noble requirement
      if (c.requiresNoble && !this.isNoble) return false;
      
      // Check race requirement
      if (c.requiredRace) {
        const effectiveRace = this.getEffectiveRace();
        
        if (this.selectedRace === 'Half-Human') {
          return this.halfRaceChoice === c.requiredRace;
        }
        
        return effectiveRace === c.requiredRace;
      }
      
      return true;
    });
  }

  raceOptions = [
    { name: 'Human', hasSubChoice: false },
    { name: 'Kitsune', hasSubChoice: false },
    { name: 'Manakete', hasSubChoice: false },
    { name: 'Taguel', hasSubChoice: false },
    { name: 'Wolfskin', hasSubChoice: false },
    { name: 'Half-Human', hasSubChoice: true }
  ];

  halfRaceOptions = ['Kitsune', 'Manakete', 'Taguel', 'Wolfskin'];

  personalAbilityOptions = [
    'Aching Blood',
    'Bibliophile',
    'Bloodthirst',
    'Bushido',
    'Calm',
    'Capture',
    'Chivalry',
    'Competitive',
    'Countercurse',
    'Daydream',
    'Divine Retribution',
    'Draconic Heir',
    'Fancy Footwork',
    'Fearsome Blow',
    'Fierce Counter',
    'Fierce Mien',
    'Fierce Rival',
    'Fiery Blood',
    'Forager',
    'Fortunate One',
    'Gallant',
    'Goody Basket',
    'Guarded Bravery',
    'Haiku',
    'Healing Descant',
    'Highwayman',
    'Icy Blood',
    'In Extremis',
    'Lily\'s Poise',
    'Lucky Charm',
    'Make a Killing',
    'Miraculous Save',
    'Mischievous',
    'Misfortunate',
    'Morbid Celebration',
    'Noble Cause',
    'Opportunist',
    'Optimist',
    'Optimistic',
    'Peacebringer',
    'Perfect Pitch',
    'Perfectionist',
    'Perspicacious',
    'Playthings',
    'Pragmatic',
    'Pride',
    'Prodigy',
    'Puissance',
    'Pyrotechnics',
    'Quiet Strength',
    'Rallying Cry',
    'Reciprocity',
    'Rose\'s Thorns',
    'Shuriken Mastery',
    'Sisterhood/Brotherhood',
    'Supportive',
    'Sweet Tooth',
    'Triple Threat',
    'Unmask',
    'Vendetta',
    'Wind Disciple'
  ];

  humanStatChoiceOptions = ['HP', 'Str', 'Mag', 'Skl', 'Spd', 'Lck', 'Def', 'Res'];
  statChoiceOptions = ['None','HP', 'Str', 'Mag', 'Skl', 'Spd', 'Lck', 'Def', 'Res'];

  get availableHumanStat1Options(): string[] {
    if (!this.humanBonusStat2) return this.humanStatChoiceOptions;
    return this.humanStatChoiceOptions.filter(stat => stat !== this.humanBonusStat2);
  }

  get availableHumanStat2Options(): string[] {
    if (!this.humanBonusStat1) return this.humanStatChoiceOptions;
    return this.humanStatChoiceOptions.filter(stat => stat !== this.humanBonusStat1);
  }

  // Race growth rate modifiers
  raceGrowthModifiers: { [key: string]: { [stat: string]: number } } = {
    'Kitsune': { hp: 0, str: 10, mag: -15, skl: 0, spd: 10, lck: 20, def: -10, res: 10 },
    'Manakete': { hp: 20, str: 0, mag: 0, skl: -10, spd: -10, lck: 20, def: 10, res: 0 },
    'Taguel': { hp: 15, str: 10, mag: -10, skl: 10, spd: 10, lck: 0, def: 0, res: -10 },
    'Wolfskin': { hp: 25, str: 20, mag: -20, skl: -5, spd: 0, lck: 0, def: 10, res: -5 }
  };

  // Form fields
  name = '';
  selectedClassBase = '';
  selectedWeaponType = '';
  startingClass = '';
  selectedHeartSealClassBase = '';
  selectedHeartSealWeaponType = '';
  heartSealClass = '';
  biography = '';
  gender = '';
  isNoble = false;
  selectedRace = '';
  halfRaceChoice = '';
  raceChoice = '';
  assetChoice = '';
  flawChoice = '';
  humanBonusStat1 = '';
  humanBonusStat2 = '';
  personalAbility = '';
  firstAquiredAbility = '';
  isAquiredAbilityEquipped = false;

  // Class data
  selectedClassData: UnitClass | null = null;
  loadingClassData = false;
  selectedAbilityChoice = '';
  hasMultipleAbilityChoices = false;
  
  // Ability data
  personalAbilityData: PersonalAbility | null = null;
  loadingPersonalAbility = false;
  acquiredAbilityData: Ability | null = null;
  loadingAcquiredAbility = false;
  
  // Equipment array - stores Equipment objects
  purchasedEquipment: Equipment[] = [];
  skillTypeChoices: string[] = [];
  selectedSkillTypes: string[] = [];

  // Equipment store
  useEquipmentStore = false;
  stoneEquipment: Equipment | null = null;
  loadingStone = false;
  expandedItemId: number | null = null;
  
  personalGrowthRate = {
    hp: 0,
    str: 0,
    mag: 0,
    skl: 0,
    spd: 0,
    lck: 0,
    def: 0,
    res: 0
  };
  errorMsg = '';
  successMsg = '';

  get growthRateSum(): number {
    const g = this.personalGrowthRate;
    return g.hp + g.str + g.mag + g.skl + g.spd + g.lck + g.def + g.res;
  }

  getTotalGrowthRate(stat: 'hp' | 'str' | 'mag' | 'skl' | 'spd' | 'lck' | 'def' | 'res'): number {
    const classGrowth = this.selectedClassData?.growthRate?.[stat] || 0;
    const personalGrowth = this.personalGrowthRate[stat] || 0;
    const humanBonus = this.getHumanBonus(stat);
    const raceModifier = this.getRaceModifier(stat);
    return classGrowth + personalGrowth + humanBonus + raceModifier;
  }

  getHumanBonus(stat: string): number {
    if (this.selectedRace !== 'Human') return 0;
    let bonus = 0;
    if (this.humanBonusStat1 && this.humanBonusStat1.toLowerCase() === stat.toLowerCase()) {
      bonus += 10;
    }
    if (this.humanBonusStat2 && this.humanBonusStat2.toLowerCase() === stat.toLowerCase()) {
      bonus += 10;
    }
    return bonus;
  }

  getRaceModifier(stat: string): number {
    const effectiveRace = this.getEffectiveRace();
    if (!effectiveRace || effectiveRace === 'Human') return 0;
    return this.raceGrowthModifiers[effectiveRace]?.[stat.toLowerCase()] || 0;
  }

  getAssetBaseStatBonus(stat: string): number {
    if (!this.assetChoice || this.assetChoice === 'None') return 0;
    const statLower = stat.toLowerCase();
    const assetLower = this.assetChoice.toLowerCase();
    
    if (assetLower === statLower) {
      if (statLower === 'hp') return 5;
      if (statLower === 'lck') return 4;
      return 2;
    }
    return 0;
  }

  getFlawBaseStatPenalty(stat: string): number {
    if (!this.flawChoice || this.flawChoice === 'None') return 0;
    const statLower = stat.toLowerCase();
    const flawLower = this.flawChoice.toLowerCase();
    
    if (flawLower === statLower) {
      if (statLower === 'hp') return -3;
      if (statLower === 'lck') return -2;
      return -1;
    }
    return 0;
  }

  getModifiedBaseStat(stat: 'hp' | 'str' | 'mag' | 'skl' | 'spd' | 'lck' | 'def' | 'res'): number {
    const baseStat = this.selectedClassData?.baseStats?.[stat] || 0;
    const assetBonus = this.getAssetBaseStatBonus(stat);
    const flawPenalty = this.getFlawBaseStatPenalty(stat);
    let weaponBonus = 0;
    
    const equippedItem = this.purchasedEquipment.find(item => item.isEquipped);
    if (equippedItem?.statBonus?.stats) {
      weaponBonus = equippedItem.statBonus.stats[stat] || 0;
    }

    return baseStat + assetBonus + flawPenalty + weaponBonus;
  }

  onAssetChange() {
    // If asset and flaw are the same and not 'None', clear flaw
    if (this.assetChoice && this.flawChoice && 
        this.assetChoice === this.flawChoice && 
        this.assetChoice !== 'None') {
      this.flawChoice = '';
    }
  }

  onFlawChange() {
    // If flaw and asset are the same and not 'None', clear asset
    if (this.assetChoice && this.flawChoice && 
        this.assetChoice === this.flawChoice && 
        this.flawChoice !== 'None') {
      this.assetChoice = '';
    }
  }

  onHumanBonusStat1Change() {
    // If both human bonuses are the same, clear the second
    if (this.humanBonusStat1 && this.humanBonusStat2 && 
        this.humanBonusStat1 === this.humanBonusStat2) {
      this.humanBonusStat2 = '';
    }
  }

  onHumanBonusStat2Change() {
    // If both human bonuses are the same, clear the first
    if (this.humanBonusStat1 && this.humanBonusStat2 && 
        this.humanBonusStat1 === this.humanBonusStat2) {
      this.humanBonusStat1 = '';
    }
  }

  get totalGrowthRateSum(): number {
    if (!this.selectedClassData?.growthRate) return this.growthRateSum;
    
    let total = 0;
    const stats: Array<'hp' | 'str' | 'mag' | 'skl' | 'spd' | 'lck' | 'def' | 'res'> = 
      ['hp', 'str', 'mag', 'skl', 'spd', 'lck', 'def', 'res'];
    
    stats.forEach(stat => {
      total += this.getTotalGrowthRate(stat);
    });
    
    return total;
  }

  get canShowEquipmentButton(): boolean {
    return !!this.selectedRace && !!this.startingClass;
  }

  get isLordClass(): boolean {
    return this.startingClass.includes('Lord');
  }

  get initialGold(): number {
    return (this.isNoble && this.isLordClass) ? 2000 : 1000;
  }

  get isBeastClassCombo(): boolean {
    const combos = [
      { race: 'Manakete', class: 'Manakete' },
      { race: 'Taguel', class: 'Taguel' },
      { race: 'Kitsune', class: 'Kitsune' },
      { race: 'Wolfskin', class: 'Wolfskin' }
    ];
    return combos.some(c => this.selectedRace === c.race && this.startingClass === c.class);
  }

  isEquippableType(type: string): boolean {
    const equippableTypes = ['Axe', 'Bow', 'Dagger', 'Lance', 'Sword', 'Tome', 'DarkTome', 'Stone'];
    return equippableTypes.includes(type);
  }

  toggleEquipItem(item: Equipment) {
    // Unequip all other items
    this.purchasedEquipment.forEach(eq => {
      if (eq !== item) {
        eq.isEquipped = false;
      }
    });
    // Toggle this item
    item.isEquipped = !item.isEquipped;
  }

  toggleItemExpansion(itemId: number) {
    this.expandedItemId = this.expandedItemId === itemId ? null : itemId;
  }

  isItemExpanded(itemId: number): boolean {
    return this.expandedItemId === itemId;
  }

  openEquipmentStore() {
    // Check if beast class combo - auto-assign stone
    if (this.isBeastClassCombo) {
      this.loadBeastStone();
      return;
    }

    // Open equipment store dialog
    const dialogRef = this.dialog.open(EquipmentStoreDialogComponent, {
      width: '900px',
      maxWidth: '95vw',
      data: {
        initialGold: this.initialGold,
        currentEquipment: this.purchasedEquipment
      },
      disableClose: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.equipment) {
        this.purchasedEquipment = result.equipment.map((item: Equipment) => ({
          ...item,
          isEquipped: item.isEquipped || false
        }));
        this.useEquipmentStore = true;
      }
    });
  }

  loadBeastStone() {
    const stoneName = this.selectedRace === 'Manakete' ? 'Dragonstone (manakete)' : 'Beaststone';
    this.loadingStone = true;
    
    this.characterService.getEquipmentByName(stoneName).subscribe({
      next: (equipment) => {
        this.stoneEquipment = equipment;
        this.stoneEquipment.isEquipped = false;
        this.purchasedEquipment = [this.stoneEquipment];
        this.useEquipmentStore = true;
        this.loadingStone = false;
      },
      error: (err) => {
        console.error(`Error loading ${stoneName}:`, err);
        this.loadingStone = false;
      }
    });
  }

  onClassBaseChange() {
    // Reset weapon type selection
    this.selectedWeaponType = '';
    this.selectedClassData = null;
    this.selectedAbilityChoice = '';
    this.hasMultipleAbilityChoices = false;
    this.selectedSkillTypes = [];

    // Reset equipment when class changes
    this.resetEquipment();

    if (!this.selectedClassBase) {
      this.startingClass = '';
      return;
    }

    const classConfig = this.availableClasses.find(c => c.name === this.selectedClassBase);
    
    // If no weapon choice needed, immediately load class data
    if (!classConfig?.hasWeaponChoice) {
      this.startingClass = this.selectedClassBase;
      this.loadClassData(this.selectedClassBase);
    }

    // Reset heart seal class if it's the same as starting class
    if (this.selectedHeartSealClassBase === this.selectedClassBase) {
      this.selectedHeartSealClassBase = '';
      this.selectedHeartSealWeaponType = '';
      this.heartSealClass = '';
    }
  }

  onWeaponTypeChange() {
    // Reset equipment when weapon type changes
    this.resetEquipment();
    
    if (!this.selectedClassBase || !this.selectedWeaponType) {
      this.startingClass = '';
      return;
    }

    // Construct the class name with weapon type prefix
    this.startingClass = `${this.selectedWeaponType} ${this.selectedClassBase}`;
    this.loadClassData(this.startingClass);
  }

  loadClassData(className: string) {
    this.loadingClassData = true;
    this.characterService.getClass(className).subscribe({
      next: (classData) => {
        this.selectedClassData = classData;
        this.loadingClassData = false;

        // Populate skill type options
        if (classData.skillTypeOptions && classData.skillTypeOptions.length > 0) {
          this.skillTypeChoices = classData.skillTypeOptions;
          this.selectedSkillTypes = [];
        }

        // Handle abilities - check if there are multiple level 1 abilities
        // Note: Based on the model, abilities is a single Ability object, not an array
        // If the backend returns an array of abilities, this logic would need adjustment
        if (classData.abilities) {
          // Assuming abilities might be an array in actual API response
          const abilitiesArray = Array.isArray(classData.abilities) ? classData.abilities : [classData.abilities];
          const level1Abilities = abilitiesArray.filter((a: any) => a.levelGained === 1);
          
          if (level1Abilities.length > 1) {
            this.hasMultipleAbilityChoices = true;
            this.selectedAbilityChoice = '';
            this.acquiredAbilityData = null;
          } else if (level1Abilities.length === 1) {
            this.hasMultipleAbilityChoices = false;
            this.firstAquiredAbility = level1Abilities[0].name;
            // Load ability details automatically
            this.loadAcquiredAbilityData(level1Abilities[0].name);
          }
        }
      },
      error: (err) => {
        console.error('Error loading class data:', err);
        this.errorMsg = `Failed to load class data for ${className}`;
        this.loadingClassData = false;
      }
    });
  }

  onAbilityChoiceChange() {
    if (this.selectedAbilityChoice) {
      this.firstAquiredAbility = this.selectedAbilityChoice;
      this.loadAcquiredAbilityData(this.selectedAbilityChoice);
    } else {
      this.acquiredAbilityData = null;
    }
  }

  onPersonalAbilityChange() {
    if (this.personalAbility) {
      this.loadPersonalAbilityData(this.personalAbility);
    } else {
      this.personalAbilityData = null;
    }
  }

  loadPersonalAbilityData(abilityName: string) {
    this.loadingPersonalAbility = true;
    this.characterService.getPersonalAbility(abilityName).subscribe({
      next: (data) => {
        this.personalAbilityData = data;
        this.loadingPersonalAbility = false;
      },
      error: (err) => {
        console.error('Error loading personal ability:', err);
        this.loadingPersonalAbility = false;
        this.personalAbilityData = null;
      }
    });
  }

  loadAcquiredAbilityData(abilityName: string) {
    this.loadingAcquiredAbility = true;
    this.characterService.getAbility(abilityName).subscribe({
      next: (data) => {
        this.acquiredAbilityData = data;
        this.loadingAcquiredAbility = false;
      },
      error: (err) => {
        console.error('Error loading acquired ability:', err);
        this.loadingAcquiredAbility = false;
        this.acquiredAbilityData = null;
      }
    });
  }

  getSelectedClass() {
    return this.availableClasses.find(c => c.name === this.selectedClassBase);
  }

  getLevel1Abilities(): any[] {
    if (!this.selectedClassData?.abilities) return [];
    const abilitiesArray = Array.isArray(this.selectedClassData.abilities) 
      ? this.selectedClassData.abilities 
      : [this.selectedClassData.abilities];
    return abilitiesArray.filter((a: any) => a.levelGained === 1);
  }

  get maxSkillChoices(): number {
    const specialClasses = ['Manakete', 'Sword Performer', 'Lance Performer', 'Taguel'];
    return specialClasses.includes(this.startingClass) ? 4 : 3;
  }

  onSkillTypeToggle(skillType: string, event: any) {
    if (event.target.checked) {
      // Check if we've reached the maximum number of skill choices
      if (this.selectedSkillTypes.length >= this.maxSkillChoices) {
        event.target.checked = false;
        return;
      }
      if (!this.selectedSkillTypes.includes(skillType)) {
        this.selectedSkillTypes.push(skillType);
      }
    } else {
      this.selectedSkillTypes = this.selectedSkillTypes.filter(s => s !== skillType);
    }
  }

  validateGrowthRate(stat: 'hp' | 'str' | 'mag' | 'skl' | 'spd' | 'lck' | 'def' | 'res') {
    const maxGrowth = 80;
    if (this.personalGrowthRate[stat] > maxGrowth) {
      this.personalGrowthRate[stat] = maxGrowth;
    }
    if (this.personalGrowthRate[stat] < 0) {
      this.personalGrowthRate[stat] = 0;
    }
  }

  onNobleChange() {
    // If unchecking noble and current class requires noble, reset class selection
    if (!this.isNoble) {
      const currentClass = this.availableClasses.find(c => c.name === this.selectedClassBase);
      if (currentClass?.requiresNoble) {
        this.selectedClassBase = '';
        this.selectedWeaponType = '';
        this.startingClass = '';
        this.selectedClassData = null;
      }
    }
  }

  onRaceChange() {
    // Reset half-race choice if not Half-Human
    if (this.selectedRace !== 'Half-Human') {
      this.halfRaceChoice = '';
    }
    
    // Update the raceChoice field
    this.updateRaceChoice();
    
    // Check if current class is still valid
    this.validateClassAgainstRace();
    
    // Reset equipment when race changes
    this.resetEquipment();
  }

  onHalfRaceChange() {
    // Update the raceChoice field
    this.updateRaceChoice();
    
    // Check if current class is still valid
    this.validateClassAgainstRace();
    
    // Reset equipment when race changes
    this.resetEquipment();
  }

  updateRaceChoice() {
    if (this.selectedRace === 'Half-Human' && this.halfRaceChoice) {
      this.raceChoice = `HalfHuman${this.halfRaceChoice}`;
    } else if (this.selectedRace) {
      this.raceChoice = this.selectedRace;
    } else {
      this.raceChoice = '';
    }
  }

  getEffectiveRace(): string {
    if (this.selectedRace === 'Half-Human') {
      return this.halfRaceChoice;
    }
    return this.selectedRace;
  }

  validateClassAgainstRace() {
    const currentClass = this.availableClasses.find(c => c.name === this.selectedClassBase);
    if (currentClass?.requiredRace) {
      const effectiveRace = this.getEffectiveRace();
      
      // If class requires a race and user doesn't have it, reset class selection
      if (this.selectedRace === 'Half-Human') {
        if (this.halfRaceChoice !== currentClass.requiredRace) {
          this.resetClassSelection();
        }
      } else if (effectiveRace !== currentClass.requiredRace) {
        this.resetClassSelection();
      }
    }
  }

  resetClassSelection() {
    this.selectedClassBase = '';
    this.selectedWeaponType = '';
    this.startingClass = '';
    this.selectedClassData = null;
  }

  resetEquipment() {
    this.purchasedEquipment = [];
    this.useEquipmentStore = false;
    this.stoneEquipment = null;
    this.expandedItemId = null;
  }

  getSelectedRace() {
    return this.raceOptions.find(r => r.name === this.selectedRace);
  }

  getSelectedHeartSealClass() {
    return this.availableClasses.find(c => c.name === this.selectedHeartSealClassBase);
  }

  onHeartSealClassBaseChange() {
    // Reset weapon type selection
    this.selectedHeartSealWeaponType = '';

    if (!this.selectedHeartSealClassBase) {
      this.heartSealClass = '';
      return;
    }

    const classConfig = this.availableClasses.find(c => c.name === this.selectedHeartSealClassBase);
    
    // If no weapon choice needed, immediately set heart seal class
    if (!classConfig?.hasWeaponChoice) {
      this.heartSealClass = this.selectedHeartSealClassBase;
    }
  }

  onHeartSealWeaponTypeChange() {
    if (!this.selectedHeartSealClassBase || !this.selectedHeartSealWeaponType) {
      this.heartSealClass = '';
      return;
    }

    // Construct the class name with weapon type prefix
    this.heartSealClass = `${this.selectedHeartSealWeaponType} ${this.selectedHeartSealClassBase}`;
  }

  confirmCreateCharacter() {
    // Prepare character data for modal
    const equippedWeapon = this.purchasedEquipment.find(item => item.isEquipped)?.name || 'None';
    
    const modalData: ConfirmationModalData = {
      type: 'create',
      characterData: {
        name: this.name || 'Unnamed',
        race: this.selectedRace === 'Half-Human' ? `Half-Human (${this.halfRaceChoice})` : this.selectedRace,
        class: this.startingClass,
        gender: this.gender || 'Not specified',
        personalAbility: this.personalAbility || 'None',
        equippedWeapon: equippedWeapon,
        baseStats: {
          hp: this.getModifiedBaseStat('hp'),
          str: this.getModifiedBaseStat('str'),
          mag: this.getModifiedBaseStat('mag'),
          skl: this.getModifiedBaseStat('skl'),
          spd: this.getModifiedBaseStat('spd'),
          lck: this.getModifiedBaseStat('lck'),
          def: this.getModifiedBaseStat('def'),
          res: this.getModifiedBaseStat('res')
        }
      }
    };
    
    const dialogRef = this.dialog.open(ConfirmationModalComponent, {
      width: '600px',
      maxWidth: '95vw',
      data: modalData,
      disableClose: true
    });
    
    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.createCharacter();
      }
    });
  }

  createCharacter() {
    this.errorMsg = '';
    this.successMsg = '';
    
    if (!this.startingClass) {
      this.errorMsg = 'Please select a starting class';
      return;
    }

    if (this.hasMultipleAbilityChoices && !this.selectedAbilityChoice) {
      this.errorMsg = 'Please select a level 1 ability';
      return;
    }

    if (this.growthRateSum !== 330) {
      this.errorMsg = 'Personal Growth Rate must total 330.';
      return;
    }

    // Get equipped weapon and separate equipment by type
    const equippedWeapon = this.purchasedEquipment.find(item => item.isEquipped)?.name || '';
    const startingWeapons = this.purchasedEquipment
      .filter(item => this.isEquippableType(item.weaponType))
      .map(item => item.name);
    const startingStaves = this.purchasedEquipment
      .filter(item => item.weaponType === 'Staff')
      .map(item => item.name);
    const startingItems = this.purchasedEquipment
      .filter(item => item.weaponType === 'Consumable')
      .map(item => item.name);

    const payload = {
      startingClass: this.startingClass,
      heartSealClass: this.heartSealClass,
      biography: this.biography,
      gender: this.gender,
      isNoble: this.isNoble,
      raceChoice: this.raceChoice,
      assetChoice: this.assetChoice,
      flawChoice: this.flawChoice,
      personalAbility: this.personalAbility,
      equippedWeapon: equippedWeapon,
      firstAquiredAbility: this.firstAquiredAbility,
      isAquiredAbilityEquipped: this.isAquiredAbilityEquipped,
      startingWeapons: startingWeapons,
      startingStaves: startingStaves,
      startingItems: startingItems,
      skillTypeChoices: this.selectedSkillTypes,
      personalGrowthRate: { ...this.personalGrowthRate }
    };
    this.characterService.createCharacter(this.name, payload).subscribe({
      next: () => {
        this.successMsg = 'Character created successfully!';
        // Close the dialog if opened in one
        if (this.dialogRef) {
          setTimeout(() => {
            this.dialogRef.close('created');
          }, 1500);
        }
      },
      error: err => {
        this.errorMsg = 'Error creating character.';
      }
    });
  }

  cancelCreation() {
    const modalData: ConfirmationModalData = {
      type: 'cancel'
    };
    
    const dialogRef = this.dialog.open(ConfirmationModalComponent, {
      width: '500px',
      maxWidth: '95vw',
      data: modalData,
      disableClose: true
    });
    
    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed && this.dialogRef) {
        this.dialogRef.close(null);
      }
    });
  }

}
