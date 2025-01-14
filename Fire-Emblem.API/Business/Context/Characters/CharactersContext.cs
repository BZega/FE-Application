using Fire_Emblem.API.Business.Context.Abilities;
using Fire_Emblem.API.Business.Context.Equips;
using Fire_Emblem.API.Business.Context.PersonalAbilities;
using Fire_Emblem.API.Business.Context.UnitClasses;
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
                var maxId = 0;
                var gold = 1000;
                List<StatType> humanChoices = [humanChoice1, humanChoice2];
                List<Equipment> equipment = new List<Equipment>();
                Biography biography = new Biography()
                {
                    Name = name,
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
                            newWeapon.EquuipOid = Guid.NewGuid().ToString();
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
                            newStaff.EquuipOid = Guid.NewGuid().ToString();
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
                            newItem.EquuipOid = Guid.NewGuid().ToString();
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
                    WeaponRanks = new List<Weapon>()
                };
                var ability = await _abilitiesContext.GetAbility(null, newCharacter.FirstAquiredAbility);
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
                    }
                }
                if (character.EquippedWeapon == null)
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
                        Range = 1,
                        Uses = "Inf.",
                        Worth = "-",
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

        public async Task<Tuple<bool, Levelup>> LevelUpCharacterManually(int id, Stats statIncrease)
        {
            try
            {
                var character = await GetCharacter(id);
                if (character.LevelupStatIncreases == null)
                {
                    character.LevelupStatIncreases = new List<Levelup>();
                }
                if (character.CurrentClass.IsSpecialClass && character.Level < 40 || !character.CurrentClass.IsSpecialClass && character.Level < 20)
                {
                    character.Level += 1;
                    character.InternalLevel += 1;
                    Levelup level = new Levelup()
                    {
                        Level = character.Level,
                        StatIncrease = statIncrease
                    };           
                    level.StatIncrease.MaxLevelCheck(character.BaseStats, character.MaxStats);
                    var result = await _charactersRepository.UpdateCharacter(character);
                    return new Tuple<bool, Levelup>(result, level);
                }
                else
                {
                    return new Tuple<bool, Levelup>(false, null);
                }
            }
            catch (Exception)
            {
                return new Tuple<bool, Levelup>(false, null);
            }
        }

        public async Task<Tuple<bool,Levelup>> LevelUpCharacterRandomly(int id)
        {
            try
            {
                var character = await GetCharacter(id);
                if (character.LevelupStatIncreases == null)
                {
                    character.LevelupStatIncreases = new List<Levelup>();
                }
                if (character.CurrentClass.IsSpecialClass && character.Level < 40 || !character.CurrentClass.IsSpecialClass && character.Level < 20)
                {
                    character.Level += 1;
                    character.InternalLevel += 1;
                    Levelup level = new Levelup()
                    {
                        Level = character.Level,
                        StatIncrease = new Stats()
                    };
                    level.StatIncrease.RandomizeStatIncrease(character.TotalGrowthRate);
                    level.StatIncrease.MaxLevelCheck(character.BaseStats, character.MaxStats);
                    character.LevelupStatIncreases.Add(level);
                    var result = await _charactersRepository.UpdateCharacter(character);
                    return new Tuple<bool, Levelup>(result, level);
                }
                else
                {
                    return new Tuple<bool, Levelup>(false, null);
                }
            }
            catch (Exception)
            {
                return new Tuple<bool, Levelup>(false, null);
            }
        }

        //public async Task<bool> 

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
