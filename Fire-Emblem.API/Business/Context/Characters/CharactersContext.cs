using Fire_Emblem.API.Business.Context.Abilities;
using Fire_Emblem.API.Business.Context.Equips;
using Fire_Emblem.API.Business.Context.PersonalAbilities;
using Fire_Emblem.API.Business.Context.UnitClasses;
using Fire_Emblem.API.Business.Helper.FileReader;
using Fire_Emblem.API.Business.Repository.Characters;
using Fire_Emblem.API.Models.Character;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.API.Business.Context.Characters
{
    public class CharactersContext : ICharactersContext
    {
        private readonly IAbilitiesContext _abilitiesContext;
        private readonly IPersonalAbilitiesContext _personalAbilitiesContext;
        private readonly IUnitClassesContext _unitClassesContext;
        private readonly IEquipmentContext _equipmentContext;
        private readonly ICharactersRepository _charactersRepository;

        public CharactersContext(IAbilitiesContext abilitiesContext, 
                                 IPersonalAbilitiesContext personalAbilitiesContext, 
                                 IUnitClassesContext unitClassesContext, 
                                 IEquipmentContext equipmentContext,
                                 ICharactersRepository charactersRepository)
        {
            _abilitiesContext = abilitiesContext;
            _personalAbilitiesContext = personalAbilitiesContext;
            _unitClassesContext = unitClassesContext;
            _equipmentContext = equipmentContext;
            _charactersRepository = charactersRepository;
        }

        public async Task<List<Character>> GetAllCharacters()
        {
            try
            {
                List<Character> characters = await _charactersRepository.GetAllCharacters();
                return characters;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Character> GetCharacter(int id)
        {
            try
            {
                var result = await _charactersRepository.GetCharacter(id);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Tuple<bool, string>> AddNewCharacter(NewCharacterDto newCharacter, string name, StatType humanChoice1, StatType humanChoice2)
        {
            try
            {
                var characters = await GetAllCharacters();
                var convoys = await GetAllConvoys();
                var maxId = 0;
                var gold = 1000;
                List<StatType> humanChoices = [humanChoice1, humanChoice2];
                List<Equipment> equipment = new List<Equipment>();
                Biography biography = new Biography()
                {
                    Name = name,
                    Gender = newCharacter.Gender,
                    Background = newCharacter.Biography,
                    IsNoble = newCharacter.IsNoble,
                    RaceChoice = new Race()
                    {
                        RacialType = newCharacter.RaceChoice,
                        HumanStatChoices = humanChoices
                    }

                };
                if (characters != null && characters.Count > 0)
                {
                    maxId = characters.Max(id => id.Id);
                }
                if (newCharacter.IsNoble && (newCharacter.StartingClass == "Sword Lord"|| newCharacter.StartingClass == "Axe Lord" || newCharacter.StartingClass == "Lance Lord"))
                {
                    gold += 1000;
                }
                if (newCharacter.StartingWeapons != null && (((newCharacter.StartingClass == "Taguel" ||  newCharacter.StartingClass == "Wolfskin" || newCharacter.StartingClass == "Kitsune") && newCharacter.StartingWeapons.Contains("Beaststone")) ||
                      newCharacter.StartingClass == "Manakete" && newCharacter.StartingWeapons.Contains("Dragonstone"))) 
                { 
                    gold += 1000; 
                }
                if (newCharacter.StartingWeapons != null && newCharacter.StartingWeapons.Count > 0)
                {
                    foreach (var weapon in newCharacter.StartingWeapons)
                    {
                        var newWeapon = await _equipmentContext.GetEquipment(null, weapon);
                        if (newWeapon != null)
                        {
                            newWeapon.EquipOid = Guid.NewGuid().ToString();
                            _ = int.TryParse(newWeapon.Worth, out int worth);
                            gold -= worth;
                            equipment.Add(newWeapon);
                        }
                    }
                }
                if (newCharacter.StartingStaves != null && newCharacter.StartingStaves.Count > 0)
                {
                    foreach (var staff in newCharacter.StartingStaves)
                    {
                        var newStaff = await _equipmentContext.GetEquipment(null, staff);
                        if (newStaff != null)
                        {
                            newStaff.EquipOid = Guid.NewGuid().ToString();
                            _ = int.TryParse(newStaff.Worth, out int worth);
                            gold -= worth;
                            equipment.Add(newStaff);
                        }
                    }
                }
                if (newCharacter.StartingItems != null && newCharacter.StartingItems.Count > 0)
                {
                    foreach (var item in newCharacter.StartingItems)
                    {
                        var newItem = await _equipmentContext.GetEquipment(null, item);
                        if (newItem != null)
                        {
                            newItem.EquipOid = Guid.NewGuid().ToString();
                            _ = int.TryParse(newItem.Worth, out int worth);
                            gold -= worth;
                            equipment.Add(newItem);
                        }
                    }
                }
                if (equipment.Count > 5)
                {
                    return new Tuple<bool, string>(false, "Too many items");
                }
                if (gold < 0)
                {
                    return new Tuple<bool, string>(false, "Not enough gold");
                }
                if ((newCharacter.StartingClass == "Taguel" && newCharacter.RaceChoice != RacialType.Taguel) || 
                    (newCharacter.StartingClass == "Kitsune" && newCharacter.RaceChoice != RacialType.Kitsune) ||
                    (newCharacter.StartingClass == "Wolfskin" && newCharacter.RaceChoice != RacialType.Wolfskin) ||
                    (newCharacter.StartingClass == "Manakete" && newCharacter.RaceChoice != RacialType.Manakete) ||
                    ((newCharacter.StartingClass == "Sword Lord" || newCharacter.StartingClass == "Axe Lord" || newCharacter.StartingClass == "Lance Lord") && (!newCharacter.IsNoble || newCharacter.RaceChoice != RacialType.Human)) ||
                    (newCharacter.StartingClass == "Manakete Heir" && (!newCharacter.IsNoble || newCharacter.RaceChoice != RacialType.Manakete)))
                {
                    return new Tuple<bool, string>(false, "Class choice does not match Racial Choice/Nobility");
                }
                if (!NewCharacterDto.ValidateGrowthRate(newCharacter.PersonalGrowthRate))
                {
                    return new Tuple<bool, string>(false, "Personal Growth Rates are not equal to 330");
                }
                Character character = new Character()
                {
                    Id = maxId + 1,
                    Biography = biography,
                    Gold = gold,
                    StartingClass = newCharacter.StartingClass,
                    HeartSealClass = newCharacter.HeartSealClass,
                    CurrentClass = await _unitClassesContext.GetClass(null, newCharacter.StartingClass),
                    Flaw = new Flaw() { FlawChoice = newCharacter.FlawChoice },
                    Asset = new Asset() { AssetChoice = newCharacter.AssetChoice },
                    Condition = new Condition() { CurrentCondition = ConditionType.Normal },
                    Terrain = new Terrain(),
                    AcquiredAbilities = new List<Ability>(),
                    EquippedAbilities = new List<Ability>(),
                    PersonalAbility = await _personalAbilitiesContext.GetPersonalAbility(null, newCharacter.PersonalAbility),
                    Inventory = new Inventory()
                    {
                        Equipment = equipment
                    },
                    PersonalGrowthRate = newCharacter.PersonalGrowthRate,
                    WeaponRanks = new List<Weapon>(),
                    ConvoyId = Guid.NewGuid().ToString(),
                };
                var ability = await _abilitiesContext.GetAbility(null, newCharacter.FirstAquiredAbility);
                ability.AbilityOid = Guid.NewGuid().ToString();
                character.AcquiredAbilities.Add(ability);
                if (newCharacter.IsAquiredAbilityEquipped)
                {
                    character.EquippedAbilities.Add(ability);
                }
                List<WeaponType> weapons = [WeaponType.Axe, WeaponType.Bow, WeaponType.Dagger, WeaponType.Lance, WeaponType.Staff, WeaponType.Sword, WeaponType.Tome, WeaponType.DarkTome];
                if ((newCharacter.StartingClass == "Taguel" && newCharacter.RaceChoice == RacialType.Taguel) ||
                    (newCharacter.StartingClass == "Kitsune" && newCharacter.RaceChoice == RacialType.Kitsune) ||
                    (newCharacter.StartingClass == "Wolfskin" && newCharacter.RaceChoice == RacialType.Wolfskin) ||
                    (newCharacter.StartingClass == "Manakete" && newCharacter.RaceChoice == RacialType.Manakete) ||
                    (newCharacter.StartingClass == "Manakete Heir" && !newCharacter.IsNoble && newCharacter.RaceChoice == RacialType.Manakete))
                {
                    weapons.Add(WeaponType.Stone);
                }
                foreach (var weapon in weapons)
                {
                    var weaponRank = new Weapon() { WeaponType = weapon };
                    if (character.CurrentClass.UsableWeapons.Any(weaponType => weaponType.WeaponType == weapon)) 
                    { 
                        weaponRank.IsActive = true;
                    }
                    character.WeaponRanks.Add(weaponRank);
                }
                if (newCharacter.EquippedWeapon != null)
                {
                    var equip = character.Inventory.Equipment.Find(equipment => equipment.Name == newCharacter.EquippedWeapon);
                    if (equip != null && !(((newCharacter.RaceChoice == RacialType.Manakete || newCharacter.RaceChoice == RacialType.Human) && equip.Name == "Beaststone") || 
                       (newCharacter.RaceChoice != RacialType.Manakete && equip.Name == "Dragonstone")))
                    {
                        character.EquippedWeapon = equip;
                        character.EquippedWeapon.IsEquipped = true;
                    }
                }
                var convoy = new Convoy()
                {
                    Id = character.ConvoyId,
                    ConvoyItems = new Inventory()
                    {
                        Equipment = new List<Equipment>()
                    }
                };
                if (character.EquippedWeapon == null)
                {
                    character.EquippedWeapon = new Equipment()
                    {
                        Id = 0,
                        Name = "Unarmed",
                        IsMagical = false,
                        WeaponType = WeaponType.None,
                        Rank = Rank.E,
                        Might = 0,
                        Hit = 75,
                        Crit = 0,
                        Range = "1",
                        Uses = "Inf.",
                        Worth = "-",
                        IsEquipped = true,
                        WeaponExp = 0,
                        Description = ""
                    };
                    if (character != null)
                    {
                        var result = await _charactersRepository.AddNewCharacter(character);
                        if (!result)
                        {
                            return new Tuple<bool, string>(result, "Character was not Created");
                        }
                        var newConvoy = _charactersRepository.AddNewConvoy(convoy);
                        return new Tuple<bool, string>(result, "Character Created, but no weapon was equipped");
                    }
                }

                if (character != null)
                {
                    var result = await _charactersRepository.AddNewCharacter(character);
                    if (!result)
                    {
                        return new Tuple<bool, string>(result, "Character was not Created");
                    }
                    var newConvoy = _charactersRepository.AddNewConvoy(convoy);
                    return new Tuple<bool, string>(result, "Character Created"); 
                }
                else
                {
                    return new Tuple<bool, string>(false, "Error Creating a Character");
                }
            }
            catch (Exception)
            {
                return new Tuple<bool, string>(false, "Error Creating a Character");
            }
        }

        public async Task<Tuple<bool, LevelUp>> LevelUpCharacterManually(int id, Stats statIncrease)
        {
            try
            {
                var character = await GetCharacter(id);
                if (character.LevelupStatIncreases == null)
                {
                    character.LevelupStatIncreases = new List<LevelUp>();
                }
                if (character.CurrentClass.IsSpecialClass && character.Level < 40 || !character.CurrentClass.IsSpecialClass && character.Level < 20)
                {
                    character.Level += 1;
                    character.InternalLevel += 1;
                    LevelUp level = new LevelUp()
                    {
                        Level = character.InternalLevel,
                        LevelUpType = "MANUAL LEVEL UP",
                        StatIncrease = statIncrease
                    };           
                    level.StatIncrease.MaxLevelCheck(character.BaseStats, character.MaxStats);
                    var result = await _charactersRepository.UpdateCharacter(character);
                    return new Tuple<bool, LevelUp>(result, level);
                }
                else
                {
                    return new Tuple<bool, LevelUp>(false, null);
                }
            }
            catch (Exception)
            {
                return new Tuple<bool, LevelUp>(false, null);
            }
        }

        public async Task<Tuple<bool,LevelUp>> LevelUpCharacterRandomly(int id)
        {
            try
            {
                var character = await GetCharacter(id);
                if (character.LevelupStatIncreases == null)
                {
                    character.LevelupStatIncreases = new List<LevelUp>();
                }
                if (character.CurrentClass.IsSpecialClass && character.Level < 40 || !character.CurrentClass.IsSpecialClass && character.Level < 20)
                {
                    character.Level += 1;
                    character.InternalLevel += 1;
                    LevelUp level = new LevelUp()
                    {
                        Level = character.InternalLevel,
                        LevelUpType = "RANDOM LEVEL UP",
                        StatIncrease = new Stats()
                    };
                    level.StatIncrease.RandomizeStatIncrease(character.TotalGrowthRate);
                    level.StatIncrease.MaxLevelCheck(character.BaseStats, character.MaxStats);
                    character.LevelupStatIncreases.Add(level);
                    var result = await _charactersRepository.UpdateCharacter(character);
                    return new Tuple<bool, LevelUp>(result, level);
                }
                else
                {
                    return new Tuple<bool, LevelUp>(false, null);
                }
            }
            catch (Exception)
            {
                return new Tuple<bool, LevelUp>(false, null);
            }
        }

        public async Task<List<Convoy>> GetAllConvoys()
        {
            try
            {
                List<Convoy> convoys = await _charactersRepository.GetAllConvoys();
                return convoys;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Inventory> GetConvoyInventory(string convoyId)
        {
            try
            {
                var result = await _charactersRepository.GetConvoyInventory(convoyId);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Convoy> GetConvoyById(string convoyId)
        {
            try
            {
                var result = await _charactersRepository.GetConvoyById(convoyId);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateConvoyItems(int characterId, string updateType, string location = null, string equipOid = null, int equipId = 0,  int sellPrice = 0, string unitChoice = null)
        {
            try
            {
                var character = await GetCharacter(characterId);
                var convoy = await GetConvoyById(character.ConvoyId);
                var convoyItems = await GetConvoyInventory(character.ConvoyId);
                var result = false;
                var equipment = character.Inventory.Equipment.Find(equip => equip.EquipOid == equipOid);
                if (equipment == null)
                {
                    equipment = convoyItems.Equipment.Find(equip => equip.EquipOid == equipOid);
                    if (equipment == null && equipId > 0)
                    {
                        equipment = await _equipmentContext.GetEquipment(equipId);
                        equipment.EquipOid = Guid.NewGuid().ToString();
                    }
                }
                if (equipment != null)
                {
                    int.TryParse(equipment.Worth, out int cost);
                    var hasUses = int.TryParse(equipment.Uses, out int usesLeft);
                    if (location == LocationTypeCode.INVENTORY)
                    {
                        if (updateType == UpdateTypeCode.BUY)
                        {
                            equipment.EquipOid = Guid.NewGuid().ToString();
                            character.Gold -= cost;
                            if (cost > 0 && character.Gold > 0)
                            {
                                if (character.Inventory.Equipment.Count < 4)
                                {
                                    character.Inventory.Equipment.Add(equipment);
                                    result = await _charactersRepository.UpdateCharacter(character);
                                }
                                else if (convoy.ConvoyItems.Equipment.Count < 501)
                                {
                                    convoy.ConvoyItems.Equipment.Add(equipment);
                                    result = await _charactersRepository.UpdateConvoy(convoy);
                                }
                            }
                        }
                        else if (updateType == UpdateTypeCode.SELL)
                        {
                            character.Gold += sellPrice;
                            if (character.EquippedWeapon.Equals(equipment))
                            {
                                character.EquippedWeapon = new Equipment()
                                {
                                    Id = 0,
                                    Name = "Unarmed",
                                    IsMagical = false,
                                    WeaponType = WeaponType.None,
                                    Rank = Rank.E,
                                    Might = 0,
                                    Hit = 75,
                                    Crit = 0,
                                    Range = "1",
                                    Uses = "Inf.",
                                    Worth = "-",
                                    WeaponExp = 0,
                                    IsEquipped = true,
                                    Description = ""
                                };
                            }
                            character.Inventory.Equipment.Remove(equipment);
                            result = await _charactersRepository.UpdateCharacter(character);
                        }
                        else if (updateType == UpdateTypeCode.USE)
                        {
                            if (hasUses)
                            {
                                character.Inventory.Equipment.Remove(equipment);
                                if (unitChoice != null)
                                {
                                    var unitClass = await _unitClassesContext.GetClass(null, unitChoice);
                                    character.CurrentClass = unitClass;
                                }
                                usesLeft -= 1;
                                if (usesLeft > 0)
                                {
                                    equipment.Uses = usesLeft.ToString();
                                    character.Inventory.Equipment.Add(equipment);
                                }
                                result = await _charactersRepository.UpdateCharacter(character);
                            }
                        }
                        else if (updateType == UpdateTypeCode.ACQUIRE)
                        {
                            equipment.EquipOid = Guid.NewGuid().ToString();
                            if (character.Inventory.Equipment.Count < 4)
                            {
                                character.Inventory.Equipment.Add(equipment);
                                result = await _charactersRepository.UpdateCharacter(character);
                            }
                            else if (convoy.ConvoyItems.Equipment.Count < 501)
                            {
                                convoy.ConvoyItems.Equipment.Add(equipment);
                                result = await _charactersRepository.UpdateConvoy(convoy);
                            }
                        }
                    }
                    else
                    {
                        if (updateType == UpdateTypeCode.WITHDRAW)
                        {
                            if (character.Inventory.Equipment.Count < 4)
                            {
                                character.Inventory.Equipment.Add(equipment);
                                convoy.ConvoyItems.Equipment.Remove(equipment);
                                result = await _charactersRepository.UpdateCharacter(character);
                                if (result)
                                {
                                    result = await _charactersRepository.UpdateConvoy(convoy);
                                }
                            }
                        }
                        else if (updateType == UpdateTypeCode.DEPOSIT)
                        {
                            if (convoy.ConvoyItems.Equipment.Count < 501)
                            {
                                if (character.EquippedWeapon.EquipOid == equipment.EquipOid)
                                {
                                    character.EquippedWeapon = new Equipment()
                                    {
                                        Id = 0,
                                        Name = "Unarmed",
                                        WeaponType = WeaponType.None,
                                        Rank = Rank.E,
                                        Might = 0,
                                        Hit = 75,
                                        Crit = 0,
                                        Range = "1",
                                        Uses = "Inf.",
                                        Worth = "-",
                                        WeaponExp = 0,
                                        IsEquipped = true,
                                        Description = ""
                                    };
                                }
                                character.Inventory.Equipment.Remove(equipment);
                                convoy.ConvoyItems.Equipment.Add(equipment);
                                result = await _charactersRepository.UpdateCharacter(character);
                                if (result)
                                {
                                    result = await _charactersRepository.UpdateConvoy(convoy);
                                }
                            }
                        }
                        else if (updateType == UpdateTypeCode.EQUIP)
                        {
                            character.EquippedWeapon = equipment;
                            character.EquippedWeapon.IsEquipped = true;
                            result = await _charactersRepository.UpdateCharacter(character);
                        }
                        else if (cost > 0 && character.Gold > 0)
                        {
                            if (convoy.ConvoyItems.Equipment.Count < 501)
                            {
                                convoy.ConvoyItems.Equipment.Add(equipment);
                                result = await _charactersRepository.UpdateConvoy(convoy);
                            }
                        }
                        else if (updateType == UpdateTypeCode.SELL)
                        {
                            convoy.ConvoyItems.Equipment.Remove(equipment);
                            result = await _charactersRepository.UpdateConvoy(convoy);
                        }
                        else if (updateType == UpdateTypeCode.USE)
                        {
                            if (hasUses)
                            {
                                convoy.ConvoyItems.Equipment.Remove(equipment);
                                if (unitChoice != null)
                                {
                                    var unitClass = await _unitClassesContext.GetClass(null, unitChoice);
                                    character.CurrentClass = unitClass;
                                }
                                usesLeft -= 1;
                                if (usesLeft > 0)
                                {
                                    equipment.Uses = usesLeft.ToString();
                                    convoy.ConvoyItems.Equipment.Add(equipment);
                                }
                                result = await _charactersRepository.UpdateConvoy(convoy);
                            }
                        }
                        else if (updateType == UpdateTypeCode.ACQUIRE)
                        {
                            equipment.EquipOid = Guid.NewGuid().ToString();
                            if (character.Inventory.Equipment.Count < 4)
                            {
                                character.Inventory.Equipment.Add(equipment);
                                result = await _charactersRepository.UpdateCharacter(character);
                            }
                            else if (convoy.ConvoyItems.Equipment.Count < 501)
                            {
                                convoy.ConvoyItems.Equipment.Add(equipment);
                                result = await _charactersRepository.UpdateConvoy(convoy);
                            }
                        }
                    }
                }
                else if (updateType == UpdateTypeCode.UNEQUIP)
                {
                    if (character.Inventory.Equipment.Any(equip => equip.EquipOid == character.EquippedWeapon.EquipOid))
                    {
                        character.Inventory.Equipment.Find(equip => equip.EquipOid == character.EquippedWeapon.EquipOid).IsEquipped = false;

                        character.EquippedWeapon = new Equipment()
                        {
                            Id = 0,
                            Name = "Unarmed",
                            WeaponType = WeaponType.None,
                            Rank = Rank.E,
                            IsMagical = false,
                            Might = 0,
                            Hit = 75,
                            Crit = 0,
                            Range = "1",
                            Uses = "Inf.",
                            Worth = "-",
                            WeaponExp = 0,
                            IsEquipped = true,
                            Description = ""
                        };
                        result = await _charactersRepository.UpdateCharacter(character);
                    }
                }
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAbilities(int characterId, string updateType, string abilityOid = null, int abilityId = 0, bool equip = false)
        {
            try
            {
                var character = await GetCharacter(characterId);
                var ability = await _abilitiesContext.GetAbility(abilityId);
                if (ability == null)
                {
                    ability = character.AcquiredAbilities.Find(ability => ability.AbilityOid == abilityOid);
                    if (ability == null)
                    {
                        throw new Exception("Ability cannot be null");
                    }
                }
                var result = false;
                if (updateType == UpdateTypeCode.ACQUIRE)
                {
                    ability.AbilityOid = Guid.NewGuid().ToString();
                    character.AcquiredAbilities.Add(ability);
                    if (equip && character.EquippedAbilities.Count < 5)
                    {
                        character.EquippedAbilities.Add(ability);
                    }
                    result = await _charactersRepository.UpdateCharacter(character);
                }
                else if (updateType == UpdateTypeCode.EQUIP)
                {
                    if (character.EquippedAbilities.Count < 5)
                    {
                        character.EquippedAbilities.Add(ability);
                    }
                    result = await _charactersRepository.UpdateCharacter(character);
                }
                else if (updateType == UpdateTypeCode.UNEQUIP)
                {
                    character.EquippedAbilities.Remove(ability);
                    result = await _charactersRepository.UpdateCharacter(character);
                }
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SwitchTerrain(int characterId, TerrainType type)
        {
            try
            {
                var character = await GetCharacter(characterId);
                switch (type)
                {

                    case TerrainType.Cliff:
                    case TerrainType.Thicket:
                    case TerrainType.Lava:
                        if (!character.UnitTypes.Any(unitType => unitType == UnitType.Flying))
                        {
                            return false;
                        }
                        break;
                    case TerrainType.Mountain:
                        if (!character.UnitTypes.Any(unitType => unitType == UnitType.Infantry) && !character.UnitTypes.Any(unitType => unitType == UnitType.Flying))
                        {
                            return false;
                        }
                        break;
                    case TerrainType.Peak:
                    case TerrainType.Sea:
                        if (!character.UnitTypes.Any(unitType => unitType == UnitType.Flying) && character.CurrentClass.Name != "Fighter" && character.CurrentClass.Name != "Berserker")
                        {
                            return false;
                        }
                        break;
                }
                    character.Terrain.TerrainType = type;
                var result = await _charactersRepository.UpdateCharacter(character); 
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<bool> RemoveCharacterById(int id)
        {
            try
            {
                var result = await _charactersRepository.RemoveCharacterById(id);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
