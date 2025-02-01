using Fire_Emblem.API.Business.Context.Abilities;
using Fire_Emblem.API.Business.Context.Equips;
using Fire_Emblem.API.Business.Context.PersonalAbilities;
using Fire_Emblem.API.Business.Context.UnitClasses;
using Fire_Emblem.API.Business.Helper.FileReader;
using Fire_Emblem.API.Business.Repository.Characters;
using Fire_Emblem.API.Models.Character;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;
using System.Text.RegularExpressions;
using System;
using System.Xml.Linq;
using System.Reflection.PortableExecutable;

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
                        HumanStatChoices = humanChoice1 != StatType.None && humanChoice2 != StatType.None ? humanChoices : null
                    }

                };
                if (characters != null && characters.Count > 0)
                {
                    maxId = characters.Max(id => id.Id);
                }
                if (newCharacter.IsNoble && 
                   (newCharacter.StartingClass == ClassTypeCode.SwordLord || 
                    newCharacter.StartingClass == ClassTypeCode.AxeLord || 
                    newCharacter.StartingClass == ClassTypeCode.LanceLord))
                {
                    gold += 1000;
                }
                if (newCharacter.StartingWeapons != null && 
                   (((newCharacter.StartingClass == ClassTypeCode.Taguel || 
                      newCharacter.StartingClass == ClassTypeCode.Wolfskin || 
                      newCharacter.StartingClass == ClassTypeCode.Kitsune) && 
                      newCharacter.StartingWeapons.Contains("Beaststone")) ||
                      newCharacter.StartingClass == ClassTypeCode.Manakete && newCharacter.StartingWeapons.Contains("Dragonstone")))
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
                if ((newCharacter.StartingClass == ClassTypeCode.Taguel && newCharacter.RaceChoice != RacialType.Taguel) ||
                    (newCharacter.StartingClass == ClassTypeCode.Kitsune && newCharacter.RaceChoice != RacialType.Kitsune) ||
                    (newCharacter.StartingClass == ClassTypeCode.Wolfskin && newCharacter.RaceChoice != RacialType.Wolfskin) ||
                    (newCharacter.StartingClass == ClassTypeCode.Manakete && newCharacter.RaceChoice != RacialType.Manakete) ||
                    ((newCharacter.StartingClass == ClassTypeCode.SwordLord || newCharacter.StartingClass == ClassTypeCode.AxeLord || newCharacter.StartingClass == ClassTypeCode.LanceLord) && (!newCharacter.IsNoble || newCharacter.RaceChoice != RacialType.Human)) ||
                    (newCharacter.StartingClass == ClassTypeCode.ManaketeHeir && (!newCharacter.IsNoble || newCharacter.RaceChoice != RacialType.Manakete)))
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
                    ReclassOptions = new List<string>(),
                    Skills = new List<Skill>(),
                    ConvoyId = Guid.NewGuid().ToString(),
                };
                var ability = await _abilitiesContext.GetAbility(null, newCharacter.FirstAquiredAbility);
                ability.AbilityOid = Guid.NewGuid().ToString();
                character.AcquiredAbilities.Add(ability);
                if (newCharacter.IsAquiredAbilityEquipped)
                {
                    character.EquippedAbilities.Add(ability);
                }
                List<WeaponType> weapons = [WeaponType.Axe, WeaponType.Bow, WeaponType.Dagger, WeaponType.Lance, WeaponType.Staff,
                                            WeaponType.Sword, WeaponType.Tome, WeaponType.DarkTome];
                List<SkillType> skillTypes = [SkillType.Athletics, SkillType.Arcana, SkillType.History, SkillType.Investigation,
                                              SkillType.Nature, SkillType.Religion, SkillType.Deception, SkillType.Intimidation,
                                              SkillType.Performance, SkillType.Persuasion,  SkillType.Acrobatics, SkillType.SleightofHand,
                                              SkillType.Stealth, SkillType.AnimalHandling, SkillType.Insight, SkillType.Medicine,
                                              SkillType.Perception, SkillType.Survival, SkillType.Strength, SkillType.Magic, SkillType.Skill,
                                              SkillType.Speed, SkillType.Defense, SkillType.Resistance];
                if ((newCharacter.StartingClass == ClassTypeCode.Taguel && newCharacter.RaceChoice == RacialType.Taguel) ||
                    (newCharacter.StartingClass == ClassTypeCode.Kitsune && newCharacter.RaceChoice == RacialType.Kitsune) ||
                    (newCharacter.StartingClass == ClassTypeCode.Wolfskin && newCharacter.RaceChoice == RacialType.Wolfskin) ||
                    (newCharacter.StartingClass == ClassTypeCode.Manakete && newCharacter.RaceChoice == RacialType.Manakete) ||
                    (newCharacter.StartingClass == ClassTypeCode.ManaketeHeir && newCharacter.IsNoble && newCharacter.RaceChoice == RacialType.Manakete))
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
                foreach (var skillType in skillTypes)
                {
                    var skill = new Skill() { SkillType = skillType };
                    foreach (var skillChoice in newCharacter.SkillTypeChoices)
                    {
                        if (skillType == skillChoice)
                        {
                            skill.IsProfecient = true;
                        }
                    }
                    switch (skillType)
                    {
                        case SkillType.Athletics:
                        case SkillType.Strength:
                            skill.StatType = StatType.Str;
                            break;
                        case SkillType.Arcana:
                        case SkillType.History:
                        case SkillType.Investigation:
                        case SkillType.Nature:
                        case SkillType.Religion:
                        case SkillType.Magic:
                            skill.StatType = StatType.Mag;
                            break;
                        case SkillType.Deception:
                        case SkillType.Intimidation:
                        case SkillType.Performance:
                        case SkillType.Persuasion:
                        case SkillType.Skill:
                            skill.StatType = StatType.Skl;
                            break;
                        case SkillType.Acrobatics:
                        case SkillType.SleightofHand:
                        case SkillType.Stealth:
                        case SkillType.Speed:
                            skill.StatType = StatType.Spd;
                            break;
                        case SkillType.Defense:
                            skill.StatType = StatType.Def;
                            break;
                        case SkillType.AnimalHandling:
                        case SkillType.Insight:
                        case SkillType.Medicine:
                        case SkillType.Perception:
                        case SkillType.Survival:
                        case SkillType.Resistance:
                            skill.StatType = StatType.Res;
                            break;
                    }
                    switch (skill.StatType)
                    {
                        case StatType.Str:
                            skill.Attribute = character.CurrentStats.Str;
                            break;
                        case StatType.Mag:
                            skill.Attribute = character.CurrentStats.Mag;
                            break;
                        case StatType.Skl:
                            skill.Attribute = character.CurrentStats.Skl;
                            break;
                        case StatType.Spd:
                            skill.Attribute = character.CurrentStats.Spd;
                            break;
                        case StatType.Def:
                            skill.Attribute = character.CurrentStats.Def;
                            break;
                        case StatType.Res:
                            skill.Attribute = character.CurrentStats.Res;
                            break;
                    }
                    skill.Bonus = skill.GetBonus(character.InternalLevel);
                    skill.Score = skill.GetScore(character.CurrentStats.Lck);
                    character.Skills.Add(skill);
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
                character.ReclassOptions.Add(newCharacter.StartingClass);
                if (newCharacter.RaceChoice == RacialType.Taguel && !character.ReclassOptions.Contains(ClassTypeCode.Taguel))
                {
                    character.ReclassOptions.Add(ClassTypeCode.Taguel);
                }
                else if (newCharacter.RaceChoice == RacialType.Kitsune && !character.ReclassOptions.Contains(ClassTypeCode.Kitsune))
                {
                    character.ReclassOptions.Add(ClassTypeCode.Kitsune);
                }
                else if (newCharacter.RaceChoice == RacialType.Wolfskin && !character.ReclassOptions.Contains(ClassTypeCode.Wolfskin))
                {
                    character.ReclassOptions.Add(ClassTypeCode.Wolfskin);
                }
                else if (newCharacter.RaceChoice == RacialType.Manakete && !character.ReclassOptions.Contains(ClassTypeCode.Manakete))
                {
                    character.ReclassOptions.Add(ClassTypeCode.Manakete);
                    if (newCharacter.IsNoble && !character.ReclassOptions.Contains(ClassTypeCode.ManaketeHeir))
                    {
                        character.ReclassOptions.Add(ClassTypeCode.ManaketeHeir);
                    }
                }
                else if (newCharacter.RaceChoice == RacialType.Human && newCharacter.IsNoble)
                {
                    if (newCharacter.StartingClass == ClassTypeCode.SwordLord && !character.ReclassOptions.Contains(ClassTypeCode.SwordLord))
                    {
                        character.ReclassOptions.Add(ClassTypeCode.SwordLord);
                    }
                    else if (newCharacter.StartingClass != ClassTypeCode.AxeLord && !character.ReclassOptions.Contains(ClassTypeCode.AxeLord))
                    {
                        character.ReclassOptions.Add(ClassTypeCode.AxeLord);
                    }
                    else if (newCharacter.StartingClass == ClassTypeCode.LanceLord && !character.ReclassOptions.Contains(ClassTypeCode.LanceLord)) 
                    {
                        character.ReclassOptions.Add(ClassTypeCode.LanceLord);
                    } 
                    else
                    {
                        character.ReclassOptions.Add(ClassTypeCode.SwordLord);
                        character.ReclassOptions.Add(ClassTypeCode.AxeLord);
                        character.ReclassOptions.Add(ClassTypeCode.LanceLord);
                    }
                }
                if (character.CurrentClass.ReclassOptions != null && character.CurrentClass.ReclassOptions.Count > 0)
                {
                    foreach (var reclassOption in character.CurrentClass.ReclassOptions) 
                    {
                        if (!character.ReclassOptions.Contains(reclassOption))
                        {
                            character.ReclassOptions.Add(reclassOption);
                        }
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
                    if (character.CurrentStats.Mag > 10 && character.WeaponRanks.Find(weapon => weapon.WeaponType == WeaponType.Sword).WeaponExperience > 70)
                    {
                        character.ReclassOptions.Add(ClassTypeCode.Tactician);
                    }
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

        public async Task<bool> CreateSupportCharacter(int characterId, string name, SupportCharacterDto support)
        {
            try
            {
                var result = false;
                var character = await GetCharacter(characterId);
                var className = ClassTypeCode.ClassTypeConverter(support.CurrentClass);
                var unitClass = await _unitClassesContext.GetClass(null, className);
                if (support.CurrentClass == ClassType.SwordLord || support.CurrentClass == ClassType.AxeLord || support.CurrentClass == ClassType.LanceLord)
                {
                    support.CurrentClass = ClassType.Lord;
                }
                else if (support.CurrentClass == ClassType.GreatSwordLordwithLance || support.CurrentClass == ClassType.GreatSwordLordwithAxe ||
                         support.CurrentClass == ClassType.GreatAxeLordwithSword || support.CurrentClass == ClassType.GreatAxeLordwithLance ||
                         support.CurrentClass == ClassType.GreatLanceLordwithSword || support.CurrentClass == ClassType.GreatLanceLordwithAxe)
                {
                    support.CurrentClass = ClassType.GreatLord;
                }
                else if (support.CurrentClass == ClassType.SwordThief || support.CurrentClass == ClassType.BowThief)
                {
                    support.CurrentClass = ClassType.Thief;
                }
                else if (support.CurrentClass == ClassType.SwordAssassin || support.CurrentClass == ClassType.BowAssassin)
                {
                    support.CurrentClass = ClassType.Assassin;
                }
                else if (support.CurrentClass == ClassType.SwordTrickster || support.CurrentClass == ClassType.BowTrickster)
                {
                    support.CurrentClass = ClassType.Trickster;
                }
                else if (support.CurrentClass == ClassType.SwordPerformer || support.CurrentClass == ClassType.LancePerformer)
                {
                    support.CurrentClass = ClassType.Performer;
                }
                else if (support.CurrentClass == ClassType.TomeDreadFighter || support.CurrentClass == ClassType.ShurikenDreadFighter)
                {
                    support.CurrentClass = ClassType.DreadFighter;
                }
                Support supportChar = new Support()
                {
                    SupportId = Guid.NewGuid().ToString(),
                    Name = name,
                    Gender = support.Gender,
                    Level = support.Level,
                    InternalLevel = support.InternalLevel,
                    CurrentClassType = support.CurrentClass,
                    CurrentClassBaseStats = unitClass.BaseStats,
                    CurrentClassMaxStats = unitClass.MaxStats,
                    EquippedWeapon = await _equipmentContext.GetEquipment(null, support.EquippedWeaponName),
                    LevelUpStats = support.LevelUpStats,
                    StartingClass = ClassTypeCode.ClassTypeConverter(support.StartingClass)
                };
                if (character.Supports == null)
                {
                    character.Supports = new List<Support>() { supportChar };
                }
                else
                {
                    character.Supports.Add(supportChar);
                }
                var newSupport = await _charactersRepository.AddNewSupport(supportChar);
                if (newSupport)
                {
                    result = await _charactersRepository.UpdateCharacter(character);
                }
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Support>> GetAllSupports()
        {
            try
            {
                List<Support> supports = await _charactersRepository.GetAllSupports();
                return supports;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Support> GetSupportById(string supportId)
        {
            try
            {
                var result = await _charactersRepository.GetSupportById(supportId);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateSupport(int characterId, string supportId, int supportPoints = 0, Stats levelUpStats = null, ClassType currentClass = ClassType.None, int level = 0, int internalLevel = 0, string equippedWeapon = null)
        {
            try
            {
                var result = false;
                var character = await GetCharacter(characterId);
                var supportCharacter = await GetSupportById(supportId);
                var unitClass = await _unitClassesContext.GetClass(null, ClassTypeCode.ClassTypeConverter(currentClass));
                if (character.Supports != null)
                {
                    var supportToDelete = character.Supports.FirstOrDefault(support => support.SupportId == supportId);
                    if (supportToDelete != null)
                    {
                        character.Supports.Remove(supportToDelete);
                    }
                }
                else
                {
                    return result;
                }
                if (supportPoints > 0)
                {                    
                    supportCharacter.SupportPoints += supportPoints;         
                }
                if (level > 0 && internalLevel > 0)
                {
                    supportCharacter.Level = level;
                    supportCharacter.InternalLevel = internalLevel;
                }
                if (levelUpStats != null)
                {
                    supportCharacter.LevelUpStats.Add(levelUpStats);
                }
                if (currentClass != ClassType.None)
                {
                    supportCharacter.CurrentClassBaseStats = unitClass.BaseStats;
                    supportCharacter.CurrentClassMaxStats = unitClass.MaxStats;
                    supportCharacter.CurrentClassType = currentClass;
                }
                if (equippedWeapon != null)
                {
                    supportCharacter.EquippedWeapon = await _equipmentContext.GetEquipment(null, equippedWeapon);
                }
                character.Supports.Add(supportCharacter);
                var supportUpdated = await _charactersRepository.UpdateSupport(supportCharacter);
                if (supportUpdated)
                {
                    result = await _charactersRepository.UpdateCharacter(character);
                }
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> TogglePairedAndCloseToggle(int characterId, string supportId, bool isPaired, bool isClose)
        {
            try
            {
                var result = false;
                var character = await GetCharacter(characterId);
                var supportCharacter = await GetSupportById(supportId);
                if (isPaired && isClose)
                {
                    return result;
                }
                if (character.Supports != null)
                {
                    var supportToDelete = character.Supports.FirstOrDefault(support => support.SupportId == supportId);
                    if (supportToDelete != null)
                    {
                        character.Supports.Remove(supportToDelete);
                    }
                }
                else
                {
                    return result;
                }
                if (isPaired)
                {
                    supportCharacter.IsPairedUp = true;
                    supportCharacter.IsClose = false;
                }
                else
                {
                    supportCharacter.IsPairedUp = false;
                }
                if (isClose)
                {
                    supportCharacter.IsClose = true;
                    supportCharacter.IsPairedUp = false;
                }
                else
                {
                    supportCharacter.IsClose = false;
                }
                character.Supports.Add(supportCharacter);
                var supportUpdated = await _charactersRepository.UpdateSupport(supportCharacter);
                if (supportUpdated)
                {
                    result = await _charactersRepository.UpdateCharacter(character);
                }
                return result;
            }
            catch (Exception)
            {
                return false;
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
                List<string> permanentItems = new List<string>
                {
                    ConsumableTypeCode.Boots,
                    ConsumableTypeCode.Dracoshield,
                    ConsumableTypeCode.EnergyDrop,
                    ConsumableTypeCode.GoddessIcon,
                    ConsumableTypeCode.SecretBook,
                    ConsumableTypeCode.SeraphRope,
                    ConsumableTypeCode.Speedwing,
                    ConsumableTypeCode.SpiritDust,
                    ConsumableTypeCode.Talisman,
                    ConsumableTypeCode.NagasTear,
                    ConsumableTypeCode.DefenseTonic
                };
                List<string> temporaryItems = new List<string>
                {
                    ConsumableTypeCode.HPTonic,
                    ConsumableTypeCode.LuckTonic,
                    ConsumableTypeCode.MagicTonic,
                    ConsumableTypeCode.ResistanceTonic,
                    ConsumableTypeCode.SkillTonic,
                    ConsumableTypeCode.SpeedTonic,
                    ConsumableTypeCode.StrengthTonic,
                    ConsumableTypeCode.PureWater,
                    ConsumableTypeCode.RainbowTonic
                };
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
                                if (equipment.StatBonus?.Stats != null)
                                {
                                    var statIncrease = equipment.StatBonus.Stats;
                                    LevelUp level = new LevelUp()
                                    {
                                        Level = character.Level,
                                        LevelUpType = "NONE",
                                        StatIncrease = statIncrease
                                    };
                                    if (temporaryItems.Contains(equipment.Name))
                                    {
                                        level.LevelUpType = "TEMPORARY";
                                    }
                                    else if (permanentItems.Contains(equipment.Name))
                                    {
                                        level.LevelUpType = "STAT ITEM";
                                    }
                                    if (level.LevelUpType != "NONE")
                                    {
                                        if (character.LevelupStatIncreases == null)
                                        {
                                            character.LevelupStatIncreases = new List<LevelUp>();
                                        }
                                        character.LevelupStatIncreases.Add(level);
                                    }
                                }
                                else if (equipment.Name == "Arms Scroll")
                                {
                                    foreach (var weapon in character.WeaponRanks)
                                    {
                                        weapon.WeaponExperience = Weapon.IncreaseRank(weapon);
                                    }
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
                    else if (location == LocationTypeCode.CONVOY)
                    {
                        if (updateType == UpdateTypeCode.BUY)
                        { 
                            if (cost > 0 && character.Gold > 0)
                            {
                                if (convoy.ConvoyItems.Equipment.Count < 501)
                                {
                                    convoy.ConvoyItems.Equipment.Add(equipment);
                                    result = await _charactersRepository.UpdateConvoy(convoy);
                                }
                            }
                        }
                        else if (updateType == UpdateTypeCode.SELL)
                        {
                            convoy.ConvoyItems.Equipment.Remove(equipment);
                            character.Gold += sellPrice;
                            result = await _charactersRepository.UpdateConvoy(convoy);
                            if (result)
                            {
                                result = await _charactersRepository.UpdateCharacter(character);
                            }
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
                                if (equipment.StatBonus?.Stats != null)
                                {
                                    var statIncrease = equipment.StatBonus.Stats;
                                    LevelUp level = new LevelUp()
                                    {
                                        Level = character.Level,
                                        LevelUpType = "NONE",
                                        StatIncrease = statIncrease
                                    };
                                    if (temporaryItems.Contains(equipment.Name))
                                    {
                                        level.LevelUpType = "TEMPORARY";
                                    }
                                    else if (permanentItems.Contains(equipment.Name))
                                    {
                                            level.LevelUpType = "STAT ITEM"; 
                                    }
                                    if (level.LevelUpType != "NONE")
                                    {
                                        if (character.LevelupStatIncreases == null)
                                        {
                                            character.LevelupStatIncreases = new List<LevelUp>();
                                        }
                                        character.LevelupStatIncreases.Add(level);
                                    }
                                }
                                else if (equipment.Name == "Arms Scroll")
                                {
                                    foreach (var weapon in character.WeaponRanks)
                                    {
                                        weapon.WeaponExperience = Weapon.IncreaseRank(weapon);
                                    }
                                }
                                usesLeft -= 1;
                                if (usesLeft > 0)
                                {
                                    equipment.Uses = usesLeft.ToString();
                                    convoy.ConvoyItems.Equipment.Add(equipment);
                                }
                                result = await _charactersRepository.UpdateConvoy(convoy);
                                if (result)
                                {
                                    result = await _charactersRepository.UpdateCharacter(character);
                                }
                            }
                        }
                        else if (updateType == UpdateTypeCode.ACQUIRE)
                        {
                            equipment.EquipOid = Guid.NewGuid().ToString();
                            if (convoy.ConvoyItems.Equipment.Count < 501)
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

        public async Task<Stats> GetTotalLevelUpStats(int characterId)
        {
            try
            {
                var character = await _charactersRepository.GetCharacter(characterId);
                Stats stats = new Stats();
                if (character.LevelupStatIncreases != null)
                {
                    foreach (var levelUp in character.LevelupStatIncreases)
                    {
                        stats.Add(levelUp.StatIncrease);
                    }
                }
                return stats;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
