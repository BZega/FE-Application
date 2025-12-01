using Fire_Emblem.API.Business.Context.Abilities;
using Fire_Emblem.API.Business.Context.Equips;
using Fire_Emblem.API.Business.Context.PersonalAbilities;
using Fire_Emblem.API.Business.Context.UnitClasses;
using Fire_Emblem.API.Business.Helper.Combat;
using Fire_Emblem.API.Business.Repository.Characters;
using Fire_Emblem.API.Models.Character;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;
using System.Text.RegularExpressions;
using static Fire_Emblem.API.Models.Character.BattleResultDto;

namespace Fire_Emblem.API.Business.Context.Characters
{
  public class CharactersContext : ICharactersContext
  {
    private readonly IAbilitiesContext _abilitiesContext;
    private readonly IPersonalAbilitiesContext _personalAbilitiesContext;
    private readonly IUnitClassesContext _unitClassesContext;
    private readonly IEquipmentContext _equipmentContext;
    private readonly ICharactersRepository _charactersRepository;
    private readonly ILogger<CharactersContext> _logger;
    private static readonly Random _random = new Random();

    public CharactersContext(IAbilitiesContext abilitiesContext,
                             IPersonalAbilitiesContext personalAbilitiesContext,
                             IUnitClassesContext unitClassesContext,
                             IEquipmentContext equipmentContext,
                             ICharactersRepository charactersRepository,
                             ILogger<CharactersContext> logger)
    {
      _abilitiesContext = abilitiesContext;
      _personalAbilitiesContext = personalAbilitiesContext;
      _unitClassesContext = unitClassesContext;
      _equipmentContext = equipmentContext;
      _charactersRepository = charactersRepository;
      _logger = logger;
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

    public async Task<Character> GetCharacter(string id)
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
        var characterOid = GenerateCharacterOid(characters);
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
            HumanStatChoices = newCharacter.RaceChoice == RacialType.Human ? humanChoices : null
          }

        };
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
              newCharacter.StartingClass == ClassTypeCode.Manakete && newCharacter.StartingWeapons.Contains("Dragonstone (manakete)")))
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
        if ((newCharacter.StartingClass == ClassTypeCode.Taguel && !(newCharacter.RaceChoice == RacialType.Taguel || newCharacter.RaceChoice == RacialType.HalfHumanTaguel)) ||
            (newCharacter.StartingClass == ClassTypeCode.Kitsune && !(newCharacter.RaceChoice == RacialType.Kitsune || newCharacter.RaceChoice == RacialType.HalfHumanKitsune)) ||
            (newCharacter.StartingClass == ClassTypeCode.Wolfskin && !(newCharacter.RaceChoice == RacialType.Wolfskin || newCharacter.RaceChoice == RacialType.HalfHumanWolfskin)) ||
            (newCharacter.StartingClass == ClassTypeCode.Manakete && !(newCharacter.RaceChoice == RacialType.Manakete || newCharacter.RaceChoice == RacialType.HalfHumanManakete)) ||
            ((newCharacter.StartingClass == ClassTypeCode.SwordLord || newCharacter.StartingClass == ClassTypeCode.AxeLord || newCharacter.StartingClass == ClassTypeCode.LanceLord) && (!newCharacter.IsNoble || newCharacter.RaceChoice != RacialType.Human)) ||
            (newCharacter.StartingClass == ClassTypeCode.ManaketeHeir && (!newCharacter.IsNoble || !(newCharacter.RaceChoice == RacialType.Manakete || newCharacter.RaceChoice == RacialType.HalfHumanManakete))))
        {
          return new Tuple<bool, string>(false, "Class choice does not match Racial Choice/Nobility");
        }
        if (!NewCharacterDto.ValidateGrowthRate(newCharacter.PersonalGrowthRate))
        {
          return new Tuple<bool, string>(false, "Personal Growth Rates are not equal to 330");
        }
        Character character = new Character()
        {
          Id = characterOid,
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
          PermanentReclassOptions = new List<string>(),
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
        character.CurrentHP = character.CurrentStats.HP;
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
              skill.IsProficient = true;
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
        character.PermanentReclassOptions.Add(newCharacter.StartingClass);
        if ((newCharacter.RaceChoice == RacialType.Taguel || newCharacter.RaceChoice == RacialType.HalfHumanTaguel) && !character.PermanentReclassOptions.Contains(ClassTypeCode.Taguel))
        {
          character.PermanentReclassOptions.Add(ClassTypeCode.Taguel);
        }
        else if ((newCharacter.RaceChoice == RacialType.Kitsune || newCharacter.RaceChoice == RacialType.HalfHumanKitsune) && !character.PermanentReclassOptions.Contains(ClassTypeCode.Kitsune))
        {
          character.PermanentReclassOptions.Add(ClassTypeCode.Kitsune);
        }
        else if ((newCharacter.RaceChoice == RacialType.Wolfskin || newCharacter.RaceChoice == RacialType.HalfHumanWolfskin) && !character.PermanentReclassOptions.Contains(ClassTypeCode.Wolfskin))
        {
          character.PermanentReclassOptions.Add(ClassTypeCode.Wolfskin);
        }
        else if (newCharacter.RaceChoice == RacialType.Manakete || newCharacter.RaceChoice == RacialType.HalfHumanManakete)
        {
          if (!character.PermanentReclassOptions.Contains(ClassTypeCode.Manakete))
          {
            character.PermanentReclassOptions.Add(ClassTypeCode.Manakete);
          }
          if (newCharacter.IsNoble && !character.PermanentReclassOptions.Contains(ClassTypeCode.ManaketeHeir))
          {
            character.PermanentReclassOptions.Add(ClassTypeCode.ManaketeHeir);
          }
        }
        else if (newCharacter.RaceChoice == RacialType.Human && newCharacter.IsNoble)
        {
          if (newCharacter.StartingClass == ClassTypeCode.SwordLord && !character.PermanentReclassOptions.Contains(ClassTypeCode.SwordLord))
          {
            character.PermanentReclassOptions.Add(ClassTypeCode.SwordLord);
          }
          else if (newCharacter.StartingClass != ClassTypeCode.AxeLord && !character.PermanentReclassOptions.Contains(ClassTypeCode.AxeLord))
          {
            character.PermanentReclassOptions.Add(ClassTypeCode.AxeLord);
          }
          else if (newCharacter.StartingClass == ClassTypeCode.LanceLord && !character.PermanentReclassOptions.Contains(ClassTypeCode.LanceLord))
          {
            character.PermanentReclassOptions.Add(ClassTypeCode.LanceLord);
          }
          else
          {
            character.PermanentReclassOptions.Add(ClassTypeCode.SwordLord);
            character.PermanentReclassOptions.Add(ClassTypeCode.AxeLord);
            character.PermanentReclassOptions.Add(ClassTypeCode.LanceLord);
          }
        }
        if (character.CurrentClass.ReclassOptions != null && character.CurrentClass.ReclassOptions.Count > 0)
        {
          foreach (var reclassOption in character.CurrentClass.ReclassOptions)
          {
            if (!character.PermanentReclassOptions.Contains(reclassOption))
            {
              character.PermanentReclassOptions.Add(reclassOption);
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

    public async Task<Tuple<bool, LevelUp>> LevelUpCharacterManually(string id, Stats statIncrease)
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
            character.PermanentReclassOptions.Add(ClassTypeCode.Tactician);
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

    private static string GenerateCharacterOid(List<Character> existing)
    {
      const string prefix = "CHA";
      const int width = 8;

      var maxNum = 0;
      if (existing != null)
      {
        foreach (var c in existing)
        {
          if (string.IsNullOrWhiteSpace(c?.Id))
            continue;

          var m = Regex.Match(c.Id, @"^CHA0*(\d+)$", RegexOptions.IgnoreCase);
          if (m.Success && int.TryParse(m.Groups[1].Value, out var n))
          {
            if (n > maxNum) maxNum = n;
          }
        }
      }

      var next = maxNum + 1;
      return $"{prefix}{next.ToString().PadLeft(width, '0')}";
    }

    public async Task<Tuple<bool, LevelUp>> LevelUpCharacterRandomly(string id)
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
          character.Skills = UpdateSkills(character.Skills, character.CurrentStats, character.InternalLevel);
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

    public async Task<bool> CreateSupportCharacter(string characterId, string name, SupportCharacterDto support)
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
          Id = Guid.NewGuid().ToString(),
          Name = name,
          Gender = support.Gender,
          Level = support.Level,
          InternalLevel = support.InternalLevel,
          CurrentClassType = support.CurrentClass,
          CurrentClass = await _unitClassesContext.GetClass(null, ClassTypeCode.ClassTypeConverter(support.CurrentClass)),
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

    public async Task<bool> UpdateSupport(string characterId, string supportId, int supportPoints = 0, Stats levelUpStats = null, ClassType currentClass = ClassType.None, int level = 0, int internalLevel = 0, string equippedWeapon = null)
    {
      try
      {
        var result = false;
        var character = await GetCharacter(characterId);
        var supportCharacter = await GetSupportById(supportId);
        var unitClass = await _unitClassesContext.GetClass(null, ClassTypeCode.ClassTypeConverter(currentClass));
        if (character.Supports != null)
        {
          var supportToDelete = character.Supports.FirstOrDefault(support => support.Id == supportId);
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

    public async Task<bool> TogglePairedAndCloseToggle(string characterId, string supportId, bool isPaired, bool isClose)
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
          var supportToDelete = character.Supports.FirstOrDefault(support => support.Id == supportId);
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

    public async Task<bool> UpdateConvoyItems(string characterId, string updateType, string location = null, string equipOid = null, int equipId = 0, int sellPrice = 0, string unitChoice = null)
    {
      try
      {
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
            ConsumableTypeCode.NagasTear
        };
        List<string> temporaryItems = new List<string>
        {
            ConsumableTypeCode.HPTonic,
            ConsumableTypeCode.LuckTonic,
            ConsumableTypeCode.MagicTonic,
            ConsumableTypeCode.ResistanceTonic,
            ConsumableTypeCode.SkillTonic,
            ConsumableTypeCode.SpeedTonic,
            ConsumableTypeCode.DefenseTonic,
            ConsumableTypeCode.StrengthTonic,
            ConsumableTypeCode.PureWater,
            ConsumableTypeCode.RainbowTonic
        };
        List<string> healingItems = new List<string>
        {
            ConsumableTypeCode.Vulnerary,
            ConsumableTypeCode.Concoction,
            ConsumableTypeCode.Elixir,
            ConsumableTypeCode.SweetTincture
        };
        List<string> promotionItems = new List<string>
        {
            ConsumableTypeCode.MasterSeal,
            ConsumableTypeCode.SecondSeal,
            ConsumableTypeCode.HeartSeal,
            ConsumableTypeCode.FriendshipSeal,
            ConsumableTypeCode.PartnerSeal,
            ConsumableTypeCode.DreadScroll,
            ConsumableTypeCode.HerosBrand,
            ConsumableTypeCode.SightingLens,
            ConsumableTypeCode.VanguardBrand,
            ConsumableTypeCode.WitchsMark
        };
        var character = await GetCharacter(characterId);
        var convoy = await GetConvoyById(character.ConvoyId);
        var convoyItems = await GetConvoyInventory(character.ConvoyId);
        var result = false;

        var equipment = character.Inventory.Equipment?.FirstOrDefault(equip => equip.EquipOid == equipOid)
            ?? convoyItems.Equipment?.FirstOrDefault(equip => equip.EquipOid == equipOid);

        if (equipment == null && equipId > 0)
        {
          equipment = await _equipmentContext.GetEquipment(equipId);
          equipment.EquipOid = Guid.NewGuid().ToString();
        }

        if (equipment == null)
          return false;

        int.TryParse(equipment.Worth, out int cost);
        var hasUses = int.TryParse(equipment.Uses, out int usesLeft);

        // Helper functions for item type checks
        bool IsPermanent(string name) => permanentItems.Contains(name);
        bool IsTemporary(string name) => temporaryItems.Contains(name);
        bool IsHealing(string name) => healingItems.Contains(name);
        bool IsPromotion(string name) => promotionItems.Contains(name);

        switch (location)
        {
          case LocationTypeCode.INVENTORY:
            result = await HandleInventoryUpdate();
            break;
          case LocationTypeCode.CONVOY:
            result = await HandleConvoyUpdate();
            break;
          default:
            result = await HandleOtherUpdate();
            break;
        }
        return result;

        // Local functions for each location type
        async Task<bool> HandleInventoryUpdate()
        {
          switch (updateType)
          {
            case UpdateTypeCode.BUY:
              equipment.EquipOid = Guid.NewGuid().ToString();
              character.Gold -= cost;
              if (cost > 0 && character.Gold > 0)
              {
                if (character.Inventory.Equipment.Count < 4)
                  character.Inventory.Equipment.Add(equipment);
                else if (convoy.ConvoyItems.Equipment.Count < 501)
                  convoy.ConvoyItems.Equipment.Add(equipment);
                else
                  return false;
                return await _charactersRepository.UpdateCharacter(character);
              }
              break;

            case UpdateTypeCode.SELL:
              character.Gold += sellPrice;
              if (character.EquippedWeapon.Equals(equipment))
                character.EquippedWeapon = GetUnarmedEquipment();
              character.Inventory.Equipment.Remove(equipment);
              return await _charactersRepository.UpdateCharacter(character);

            case UpdateTypeCode.USE:
              if (!hasUses) return false;
              character.Inventory.Equipment.Remove(equipment);
              await ApplyItemEffects(character, equipment, unitChoice, usesLeft, temporaryItems, permanentItems, healingItems);
              return await _charactersRepository.UpdateCharacter(character);

            case UpdateTypeCode.ACQUIRE:
              equipment.EquipOid = Guid.NewGuid().ToString();
              if (character.Inventory.Equipment.Count < 4)
                character.Inventory.Equipment.Add(equipment);
              else if (convoy.ConvoyItems.Equipment.Count < 501)
                convoy.ConvoyItems.Equipment.Add(equipment);
              else
                return false;
              return await _charactersRepository.UpdateCharacter(character);
          }
          return false;
        }

        async Task<bool> HandleConvoyUpdate()
        {
          switch (updateType)
          {
            case UpdateTypeCode.BUY:
              if (cost > 0 && character.Gold > 0 && convoy.ConvoyItems.Equipment.Count < 501)
              {
                convoy.ConvoyItems.Equipment.Add(equipment);
                return await _charactersRepository.UpdateConvoy(convoy);
              }
              break;

            case UpdateTypeCode.SELL:
              convoy.ConvoyItems.Equipment.RemoveAll(e => e.EquipOid == equipment.EquipOid);
              character.Gold += sellPrice;
              var convoyResult = await _charactersRepository.UpdateConvoy(convoy);
              if (convoyResult)
                return await _charactersRepository.UpdateCharacter(character);
              break;

            case UpdateTypeCode.USE:
              if (!hasUses) return false;
              convoy.ConvoyItems.Equipment.RemoveAll(e => e.EquipOid == equipment.EquipOid);
              await ApplyItemEffects(character, equipment, unitChoice, usesLeft, temporaryItems, permanentItems, healingItems);
              var useResult = await _charactersRepository.UpdateConvoy(convoy);
              if (useResult)
                return await _charactersRepository.UpdateCharacter(character);
              break;

            case UpdateTypeCode.ACQUIRE:
              equipment.EquipOid = Guid.NewGuid().ToString();
              if (convoy.ConvoyItems.Equipment.Count < 501)
              {
                convoy.ConvoyItems.Equipment.Add(equipment);
                return await _charactersRepository.UpdateConvoy(convoy);
              }
              break;
          }
          return false;
        }

        async Task<bool> HandleOtherUpdate()
        {
          switch (updateType)
          {
            case UpdateTypeCode.WITHDRAW:
              if (character.Inventory.Equipment.Count < 4)
              {
                character.Inventory.Equipment.Add(equipment);
                convoy.ConvoyItems.Equipment.RemoveAll(e => e.EquipOid == equipment.EquipOid);
                var withdrawResult = await _charactersRepository.UpdateCharacter(character);
                if (withdrawResult)
                  return await _charactersRepository.UpdateConvoy(convoy);
              }
              break;

            case UpdateTypeCode.DEPOSIT:
              if (convoy.ConvoyItems.Equipment.Count < 501)
              {
                if (character.EquippedWeapon.EquipOid == equipment.EquipOid)
                  character.EquippedWeapon = GetUnarmedEquipment();
                character.Inventory.Equipment.Remove(equipment);
                convoy.ConvoyItems.Equipment.Add(equipment);
                var depositResult = await _charactersRepository.UpdateCharacter(character);
                if (depositResult)
                  return await _charactersRepository.UpdateConvoy(convoy);
              }
              break;

            case UpdateTypeCode.EQUIP:
              character.EquippedWeapon = equipment;
              character.EquippedWeapon.IsEquipped = true;
              return await _charactersRepository.UpdateCharacter(character);

            case UpdateTypeCode.UNEQUIP:
              var equipped = character.Inventory.Equipment.FirstOrDefault(equip => equip.EquipOid == character.EquippedWeapon.EquipOid);
              if (equipped != null)
                equipped.IsEquipped = false;
              character.EquippedWeapon = GetUnarmedEquipment();
              return await _charactersRepository.UpdateCharacter(character);
          }
          return false;
        }

        Equipment GetUnarmedEquipment() => new Equipment
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

        async Task ApplyItemEffects(Character character, Equipment equipment, string unitChoice, int usesLeft, List<string> temporaryItems, List<string> permanentItems, List<string> healingItems)
        {
          if (unitChoice != null)
          {
            var unitClass = await _unitClassesContext.GetClass(null, unitChoice);
            character.CurrentClass = unitClass;
          }
          if (equipment.StatBonus?.Stats != null)
          {
            var statIncrease = equipment.StatBonus.Stats;
            var levelUpType = "NONE";
            if (temporaryItems.Contains(equipment.Name)) levelUpType = "TEMPORARY";
            else if (permanentItems.Contains(equipment.Name)) levelUpType = "STAT ITEM";
            if (levelUpType != "NONE")
            {
              character.LevelupStatIncreases ??= new List<LevelUp>();
              character.LevelupStatIncreases.Add(new LevelUp
              {
                Level = character.Level,
                LevelUpType = levelUpType,
                StatIncrease = statIncrease
              });
              character.Skills = UpdateSkills(character.Skills, character.CurrentStats, character.InternalLevel);
            }
          }
          else if (equipment.Name == "Arms Scroll")
          {
            foreach (var weapon in character.WeaponRanks)
              weapon.WeaponExperience = Weapon.IncreaseRank(weapon);
          }
          else if (healingItems.Contains(equipment.Name))
          {
            var healingAmount = equipment.Name switch
            {
              ConsumableTypeCode.Vulnerary => 10,
              ConsumableTypeCode.Concoction => 20,
              ConsumableTypeCode.Elixir => 40,
              ConsumableTypeCode.SweetTincture => 5,
              _ => 0
            };
            var healthRecovered = Math.Min(healingAmount, character.CurrentStats.HP - character.CurrentHP);
            if (healthRecovered > 0)
              character.CurrentHP += healthRecovered;
          }
          usesLeft -= 1;
          if (usesLeft > 0)
          {
            equipment.Uses = usesLeft.ToString();
            if (location == LocationTypeCode.INVENTORY)
              character.Inventory.Equipment.Add(equipment);
            else if (location == LocationTypeCode.CONVOY)
              convoy.ConvoyItems.Equipment.Add(equipment);
          }
        }
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<bool> UpdateAbilities(string characterId, string updateType, string abilityOid = null, int abilityId = 0, bool equip = false)
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
            if (!character.EquippedAbilities.Any(a => a.AbilityOid == ability.AbilityOid))
            {
              character.EquippedAbilities.Add(ability);
            }
          }
          result = await _charactersRepository.UpdateCharacter(character);
        }
        else if (updateType == UpdateTypeCode.UNEQUIP)
        {
          var abilityToRemove = character.EquippedAbilities.FirstOrDefault(a => a.AbilityOid == ability.AbilityOid);
          if (abilityToRemove != null)
          {
            character.EquippedAbilities.Remove(abilityToRemove);
          }
          result = await _charactersRepository.UpdateCharacter(character);
        }
        return result;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<bool> SwitchTerrain(string characterId, TerrainType type)
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

    public async Task<bool> RemoveCharacterById(string id, bool shouldDeteleConvoy)
    {
      try
      {
        var result = await _charactersRepository.RemoveCharacterById(id, shouldDeteleConvoy);
        return result;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<Stats> GetTotalLevelUpStats(string characterId)
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

    public async Task<bool> ChangeCondition(string characterId, int trackerChange)
    {
      try
      {
        var character = await GetCharacter(characterId);
        if (character == null)
        {
          return false;
        }
        switch (character.Condition.CurrentCondition)
        {
          case ConditionType.Normal:
            if (trackerChange == 0)
            {
              return false;
            }
            else if (trackerChange == 1)
            {
              character.Condition.CurrentCondition = ConditionType.Serious;
            }
            else if (trackerChange == 2)
            {
              character.Condition.CurrentCondition = ConditionType.Critical;
            }
            break;
          case ConditionType.Serious:
            if (trackerChange == 0)
            {
              character.Condition.CurrentCondition = ConditionType.Normal;
            }
            else if (trackerChange == 1)
            {
              character.Condition.CurrentCondition = ConditionType.Critical;
            }
            else if (trackerChange == 2)
            {
              character.Condition.CurrentCondition = ConditionType.DEAD;
            }
            break;
          case ConditionType.Critical:
            if (trackerChange == 0)
            {
              character.Condition.CurrentCondition = ConditionType.Serious;
            }
            else if (trackerChange > 0)
            {
              character.Condition.CurrentCondition = ConditionType.DEAD;
            }
            break;
          case ConditionType.DEAD:
            break;
        }
        character.CurrentHP = character.CurrentStats.HP;
        var result = await _charactersRepository.UpdateCharacter(character);
        return result;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public async Task<bool> ReviveFallenCharacter(string characterId)
    {
      try
      {
        var result = false;
        var character = await GetCharacter(characterId);
        if (character == null)
        {
          return false;
        }
        if (character.Condition.CurrentCondition == ConditionType.DEAD)
        {
          character.Condition.CurrentCondition = ConditionType.Normal;
          result = await _charactersRepository.UpdateCharacter(character);
        }
        return result;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    private List<Skill> UpdateSkills(List<Skill> skills, Stats currentStats, int level)
    {
      try
      {
        var updatedSkills = new List<Skill>();
        foreach (var skill in skills)
        {
          switch (skill.StatType)
          {
            case StatType.Str:
              skill.Attribute = currentStats.Str;
              break;
            case StatType.Mag:
              skill.Attribute = currentStats.Mag;
              break;
            case StatType.Skl:
              skill.Attribute = currentStats.Skl;
              break;
            case StatType.Spd:
              skill.Attribute = currentStats.Spd;
              break;
            case StatType.Def:
              skill.Attribute = currentStats.Def;
              break;
            case StatType.Res:
              skill.Attribute = currentStats.Res;
              break;
          }
          skill.Bonus = skill.GetBonus(level);
          skill.Score = skill.GetScore(currentStats.Lck);
          updatedSkills.Add(skill);
        }
        return updatedSkills;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<Character> GainWeaponExp(string characterId, bool isDoubleAttack)
    {
      try
      {
        var character = await GetCharacter(characterId);
        if (character == null)
        {
          return null;
        }
        if (character.EquippedWeapon.WeaponType == WeaponType.None)
        {
          return character;
        }
        if (character.WeaponRanks.FirstOrDefault(weapon => weapon.WeaponType == character.EquippedWeapon.WeaponType) != null)
        {
          var bonus = character.EquippedWeapon.WeaponExp != null ? character.EquippedWeapon.WeaponExp.Value : 0;
          if (bonus > 0)
          {
            if (character.EquippedAbilities?.Any(ability => ability.Name == "Discipline") == true)
            {
              bonus *= 2;
            }
            character.WeaponRanks.FirstOrDefault(weapon => weapon.WeaponType == character.EquippedWeapon.WeaponType).WeaponExperience += bonus;
            _logger.LogInformation($"Character: {character.Biography.Name} gained {bonus} weapon exp");
            if (isDoubleAttack)
            {
              character.WeaponRanks.FirstOrDefault(weapon => weapon.WeaponType == character.EquippedWeapon.WeaponType).WeaponExperience += bonus;
              _logger.LogInformation($"Character: {character.Biography.Name} gained {bonus} weapon exp");
            }
          }
        }
        var result = await _charactersRepository.UpdateCharacter(character);
        _logger.LogInformation($"Character update result: {result}");
        return character;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<Enemy> CreateNewEnemy(EnemyDto enemyDto)
    {
      try
      {
        var maxId = 0;
        var enemies = await GetAllEnemies();
        if (enemies != null && enemies.Count > 0)
        {
          maxId = enemies.Max(id => id.Id);
        }
        var enemy = new Enemy()
        {
          Id = maxId + 1,
          Level = enemyDto.Level,
          InternalLevel = enemyDto.Level,
          CurrentClass = await _unitClassesContext.GetClass(null, enemyDto.CurrentClass),
          EquippedWeapon = await _equipmentContext.GetEquipment(null, enemyDto.EquippedWeapon),
          DifficultySetting = enemyDto.Difficulty,
          ManualStatGrowth = enemyDto.ManualStatGrowth ?? null,
          Terrain = new Terrain() { TerrainType = TerrainType.None },
          UnitTypes = new List<UnitType>(),
          EquippedAbilities = new List<Ability>(),
          WeaponRank = new Weapon()
        };

        foreach (var abilityName in enemyDto.EquippedAbilites)
        {
          var ability = await _abilitiesContext.GetAbility(null, abilityName);
          if (ability == null) continue;
          enemy.EquippedAbilities.Add(ability);
        }
        var race = new Race()
        {
          RacialType = enemyDto.Race
        };

        foreach (var unitType in enemy.CurrentClass.UnitTypes)
        {
          if (!race.UnitType.Equals(unitType))
          {
            enemy.UnitTypes.Add(unitType);
          }
        }
        enemy.UnitTypes.Add(race.UnitType);
        enemy.WeaponRank.WeaponType = enemy.EquippedWeapon.WeaponType;
        switch (enemy.EquippedWeapon.Rank)
        {
          case Rank.D:
            enemy.WeaponRank.WeaponExperience = 31;
            break;
          case Rank.C:
            enemy.WeaponRank.WeaponExperience = 71;
            break;
          case Rank.B:
            enemy.WeaponRank.WeaponExperience = 121;
            break;
          case Rank.A:
            enemy.WeaponRank.WeaponExperience = 180;
            break;
          case Rank.S:
            enemy.WeaponRank.WeaponExperience = 250;
            break;
        }
        enemy.CurrentHP = enemy.CurrentStats.HP;
        var result = await _charactersRepository.AddNewEnemy(enemy);
        return result;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<List<Enemy>> GetAllEnemies()
    {
      try
      {
        var result = await _charactersRepository.GetAllEnemies();
        return result;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<Enemy> GetEnemy(int enemyId)
    {
      try
      {
        var enemies = await _charactersRepository.GetAllEnemies();
        var result = enemies.FirstOrDefault(enemy => enemy.Id == enemyId);
        return result;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<BattleResultDto> AutomaticBattleSimulator(string characterId, int enemyId, bool canOpponentCounter, bool isCharacterAttacking, bool gainExp)
    {
      try
      {
        #region DATA PREP
        var character = await GetCharacter(characterId);
        var enemy = await GetEnemy(enemyId);
        var charHitDecrease = 0;
        var charDamageDecrease = 0;
        var enemyHitDecrease = 0;
        var enemyDamageDecrease = 0;
        CombatHelper.CheckWeaponTriangle(character, enemy);
        _logger.LogInformation($"character weapon triangle advantage is {character.IsWeaponTriangleAdvantage} and enemy weapon triangle advantage is {enemy.IsWeaponTriangleAdvantage}");
        _logger.LogInformation($"character weapon triangle disadvantage is {character.IsWeaponTriangleDisadvantage} and enemy weapon triangle disadvantage is {enemy.IsWeaponTriangleDisadvantage}");
        CombatHelper.ApplyWeaponTriangleDisadvantage(character, enemy, ref charHitDecrease, ref charDamageDecrease, ref enemyHitDecrease, ref enemyDamageDecrease);
        if (character.DealsEffectiveDamage)
        {
          bool appliedMultiplier = false;
          foreach (var effectiveUnit in character.EffectiveDamageUnitTypes)
          {
            if (enemy.UnitTypes.Contains(effectiveUnit) && !appliedMultiplier)
            {
              character.EquippedWeapon.Might *= 3;
              _logger.LogInformation($"character deals effective damage to {effectiveUnit}");
            }
          }
        }
        if (enemy.DealsEffectiveDamage)
        {
          bool appliedMultiplier = false;
          foreach (var effectiveUnit in enemy.EffectiveDamageUnitTypes)
          {
            if (character.UnitTypes.Contains(effectiveUnit) && !appliedMultiplier)
            {
              enemy.EquippedWeapon.Might *= 3;
              _logger.LogInformation($"enemy deals effective damage to {effectiveUnit}");
            }
          }
        }
        BattleResultDto battleResults = new BattleResultDto()
        {
          CharacterBaseHitChance = character.Attack - charHitDecrease,
          EnemyAvoid = enemy.Avoid,
          CharacterBaseDamage = character.Damage - charDamageDecrease,
          CharacterBaseCritChance = character.Crit,
          EnemyDodge = enemy.Dodge,
          CharacterBaseeAttackSpeed = character.CurrentStats.Spd,
          EnemyBaseHitChance = enemy.Attack - enemyHitDecrease,
          CharacterAvoid = character.Avoid,
          EnemyBaseDamage = enemy.Damage - enemyDamageDecrease,
          EnemyBaseCritChance = enemy.Crit,
          CharacterDodge = enemy.Dodge,
          EnemyBaseAttackSpeed = enemy.CurrentStats.Spd,
          IsCharacterWeaponTriangleAdvantage = character.IsWeaponTriangleAdvantage,
          IsEnemyWeaponTriangleAdvantage = enemy.IsWeaponTriangleAdvantage,
          AttackRollResults = new List<AttackRoll>()
        };
        AttackRoll characterAttacker = new AttackRoll() { Attacker = CombatTypeCode.Character };
        AttackRoll enemyAttacker = new AttackRoll() { Attacker = CombatTypeCode.Enemy };
        AttackRoll supportAttacker = new AttackRoll();

        character.IsInCombat = true;
        enemy.IsInCombat = true;

        var charHitChance = Math.Max(character.Attack + charHitDecrease - enemy.Avoid, 0);
        battleResults.CharacterHitCHance = charHitChance;

        var enemyHitChance = Math.Max(enemy.Attack + enemyHitDecrease - character.Avoid, 0);
        battleResults.EnemyHitChance = enemyHitChance;

        var charCritChance = Math.Max(character.Crit - enemy.Dodge, 0);
        battleResults.CharacterCritChance = charCritChance;

        var enemyCritChance = Math.Max(enemy.Crit - character.Dodge, 0);
        battleResults.EnemyCritChance = enemyCritChance;

        var charDamage = Math.Max(character.EquippedWeapon.IsMagical ? character.Damage + charDamageDecrease + enemy.DamageReceived - enemy.CurrentStats.Res : character.Damage + charDamageDecrease + enemy.DamageReceived - enemy.CurrentStats.Def, 0);
        battleResults.CharacterDamageOutput = charDamage;
        battleResults.EnemyDamageNegation = Math.Max(enemy.EquippedWeapon.IsMagical ? -enemy.DamageReceived + enemy.CurrentStats.Res : -enemy.DamageReceived + enemy.CurrentStats.Def, 0);

        var enemyDamage = Math.Max(enemy.EquippedWeapon.IsMagical ? enemy.Damage + enemyDamageDecrease + character.DamageReceived - character.CurrentStats.Res : enemy.Damage + enemyDamageDecrease + character.DamageReceived - character.CurrentStats.Def, 0);
        battleResults.EnemyDamageOutput = enemyDamage;
        battleResults.CharacterDamageNegation = Math.Max(character.EquippedWeapon.IsMagical ? -character.DamageReceived + character.CurrentStats.Res : -character.DamageReceived + character.CurrentStats.Def, 0);

        var charAttackSpeed = character.CurrentStats.Spd + character.AttackSpeed;
        var enemyAttackSpeed = enemy.CurrentStats.Spd + enemy.AttackSpeed;
        var pairedUpSupport = character.Supports?.FirstOrDefault(support => support.IsPairedUp);
        var adjacentSupports = character.Supports?.FindAll(support => support.IsClose);
        Support highestRankSupport = null;
        if (adjacentSupports != null)
        {
          highestRankSupport = adjacentSupports
                          .Where(support => support.SupportRank != Rank.None)
                          .OrderByDescending(support => support.SupportRank)
                          .FirstOrDefault();
        }

        var characterAttackCount = character.EquippedWeapon.IsBrave ? 2 : 1;
        var enemyAttackCount = enemy.EquippedWeapon.IsBrave ? 2 : 1;

        var characterAttackSpeedAdvCount = character.EquippedWeapon.IsBrave ? 2 : 1;
        var enemyAttackSpeedAdvCount = enemy.EquippedWeapon.IsBrave ? 2 : 1;

        bool characterAttackSpeedAdv = charAttackSpeed >= enemyAttackSpeed + 5;
        battleResults.CharacterAttackSpeed = charAttackSpeed;

        bool enemyAttackSpeedAdv = enemyAttackSpeed >= charAttackSpeed + 5;
        battleResults.EnemyAttackSpeed = enemyAttackSpeed;

        var characterDamageDealt = 0;
        var enemyDamageDealt = 0;
        var maxId = 1;
        #endregion

        if (isCharacterAttacking)
        {
          character.IsAttacking = true;
          enemy.IsAttacking = false;
          for (int i = 0; i < characterAttackCount; i++)
          {
            characterAttacker = RollAttackRoll(characterAttacker.Attacker, character, enemy, charHitChance, charDamage, charCritChance, battleResults.EnemyDamageNegation - enemy.DamageReceived);
            characterAttacker.Id = maxId;
            maxId++;
            enemy.CurrentHP -= Math.Min(characterAttacker.DamageDealt, enemy.CurrentHP);
            _logger.LogInformation($"Character dealt {characterAttacker.DamageDealt} to enemy");

            if (characterAttacker.DamageHealed > 0)
            {
              character.CurrentHP += character.CurrentHP + characterAttacker.DamageHealed <= character.CurrentStats.HP
                  ? characterAttacker.DamageHealed
                  : character.CurrentStats.HP - character.CurrentHP;
              _logger.LogInformation($"Character healed {characterAttacker.DamageHealed}");
            }

            characterDamageDealt += characterAttacker.DamageDealt;
            battleResults.AttackRollResults.Add(characterAttacker);
            character = await GainWeaponExp(characterId, false);

            if (enemy.CurrentHP == 0)
            {
              battleResults.EnemyDamageTaken = characterDamageDealt;
              await HandleEnemyDefeat(character, enemy);
              return battleResults;
            }

            if (pairedUpSupport != null)
            {
              var supportAttackCount = pairedUpSupport.EquippedWeapon.IsBrave ? 2 : 1;
              character.DualStrike = character.GetDualStrikeRate(pairedUpSupport);
              var randomNumber = _random.Next(0, 101);
              var supportAttack = randomNumber < character.DualStrike ? true : false;
              _logger.LogInformation($"Pair up Dual Strike chance: {character.DualStrike}, rol: {randomNumber}, action: {supportAttack}");
              if (supportAttack)
              {
                for (int s = 0; s < supportAttackCount; s++)
                {
                  supportAttacker = HandleSupportAttack(character, enemy, pairedUpSupport);
                  supportAttacker.Id = maxId;
                  maxId++;
                  enemy.CurrentHP -= Math.Min(characterAttacker.DamageDealt, enemy.CurrentHP);

                  characterDamageDealt += supportAttacker.DamageDealt;
                  battleResults.AttackRollResults.Add(supportAttacker);

                  if (enemy.CurrentHP == 0)
                  {
                    battleResults.EnemyDamageTaken += characterDamageDealt;
                    await HandleEnemyDefeat(character, enemy);
                    return battleResults;
                  }
                }
              }

            }
            else if (highestRankSupport != null)
            {
              var supportAttackCount = highestRankSupport.EquippedWeapon.IsBrave ? 2 : 1;
              character.DualStrike = character.GetDualStrikeRate(highestRankSupport);
              var randomNumber = _random.Next(0, 101);
              var supportAttack = randomNumber < character.DualStrike ? true : false;
              _logger.LogInformation($"Adjacent Support Dual Strike chance: {character.DualStrike}, rol: {randomNumber}, action: {supportAttack}");
              if (supportAttack)
              {
                for (int s = 0; s < supportAttackCount; s++)
                {
                  supportAttacker = HandleSupportAttack(character, enemy, highestRankSupport);
                  supportAttacker.Id = maxId;
                  maxId++;
                  enemy.CurrentHP -= Math.Min(characterAttacker.DamageDealt, enemy.CurrentHP);
                  if (enemy.CurrentHP == 0)
                  {
                    battleResults.EnemyDamageTaken = characterDamageDealt;
                    await HandleEnemyDefeat(character, enemy);
                    return battleResults;
                  }
                }
              }
            }
          }
          if (canOpponentCounter)
          {
            if (enemyAttackSpeedAdv)
            {
              enemyAttackCount *= 2;
            }
            for (int i = 0; i < enemyAttackCount; i++)
            {
              enemyAttacker = RollAttackRoll(enemyAttacker.Attacker, character, enemy, enemyHitChance, enemyDamage, enemyCritChance, battleResults.CharacterDamageNegation - character.DamageReceived);
              enemyAttacker.Id = maxId;
              maxId++;
              character.CurrentHP -= Math.Min(enemyAttacker.DamageDealt, character.CurrentHP);
              if (enemyAttacker.DamageHealed > 0)
              {
                enemy.CurrentHP += enemy.CurrentHP + enemyAttacker.DamageHealed <= enemy.CurrentStats.HP
                    ? enemyAttacker.DamageHealed
                    : enemy.CurrentStats.HP - enemy.CurrentHP;
              }
              enemyDamageDealt += enemyAttacker.DamageDealt;
              battleResults.AttackRollResults.Add(enemyAttacker);
              if (character.CurrentHP == 0)
              {
                battleResults.EnemyDamageTaken = characterDamageDealt;
                battleResults.CharacterDamageTaken = enemyDamageDealt;
                await HandleCharacterDefeat(character, enemy, enemyAttacker);
                return battleResults;
              }
            }
          }
          if (characterAttackSpeedAdv)
          {
            for (int i = 0; i < characterAttackSpeedAdvCount; i++)
            {
              characterAttacker = RollAttackRoll(characterAttacker.Attacker, character, enemy, charHitChance, charDamage, charCritChance, battleResults.EnemyDamageNegation - enemy.DamageReceived);
              characterAttacker.Id = maxId;
              maxId++;
              enemy.CurrentHP -= Math.Min(characterAttacker.DamageDealt, enemy.CurrentHP);

              if (characterAttacker.AbilityCheck != null)
              {
                character.CurrentHP += character.CurrentHP + characterAttacker.DamageHealed <= character.CurrentStats.HP
                    ? characterAttacker.DamageHealed
                    : character.CurrentStats.HP - character.CurrentHP;
              }

              characterDamageDealt += characterAttacker.DamageDealt;
              battleResults.AttackRollResults.Add(characterAttacker);
              character = await GainWeaponExp(characterId, false);
              if (enemy.CurrentHP == 0)
              {
                battleResults.EnemyDamageTaken = characterDamageDealt;
                battleResults.CharacterDamageTaken = enemyDamageDealt;
                await HandleEnemyDefeat(character, enemy);
                return battleResults;
              }

              if (pairedUpSupport != null)
              {
                var supportAttackCount = pairedUpSupport.EquippedWeapon.IsBrave ? 2 : 1;
                character.DualStrike = character.GetDualStrikeRate(pairedUpSupport);
                var randomNumber = _random.Next(0, 101);
                var supportAttack = randomNumber < character.DualStrike ? true : false;
                _logger.LogInformation($"Pair up Dual Strike chance: {character.DualStrike}, rol: {randomNumber}, action: {supportAttack}");
                if (supportAttack)
                {
                  for (int s = 0; s < supportAttackCount; s++)
                  {
                    supportAttacker = HandleSupportAttack(character, enemy, pairedUpSupport);
                    supportAttacker.Id = maxId;
                    maxId++;
                    enemy.CurrentHP -= Math.Min(characterAttacker.DamageDealt, enemy.CurrentHP);

                    characterDamageDealt += supportAttacker.DamageDealt;
                    battleResults.AttackRollResults.Add(supportAttacker);

                    if (enemy.CurrentHP == 0)
                    {
                      battleResults.EnemyDamageTaken += characterDamageDealt;
                      battleResults.CharacterDamageTaken = enemyDamageDealt;
                      await HandleEnemyDefeat(character, enemy);
                      return battleResults;
                    }
                  }
                }

              }
              else if (highestRankSupport != null)
              {
                var supportAttackCount = highestRankSupport.EquippedWeapon.IsBrave ? 2 : 1;
                character.DualStrike = character.GetDualStrikeRate(highestRankSupport);
                var randomNumber = _random.Next(0, 101);
                var supportAttack = randomNumber < character.DualStrike ? true : false;
                _logger.LogInformation($"Adjacent Support Dual Strike chance: {character.DualStrike}, rol: {randomNumber}, action: {supportAttack}");
                if (supportAttack)
                {
                  for (int s = 0; s < supportAttackCount; s++)
                  {
                    supportAttacker = HandleSupportAttack(character, enemy, highestRankSupport);
                    supportAttacker.Id = maxId;
                    maxId++;
                    enemy.CurrentHP -= Math.Min(characterAttacker.DamageDealt, enemy.CurrentHP);
                    if (enemy.CurrentHP == 0)
                    {
                      battleResults.EnemyDamageTaken = characterDamageDealt;
                      battleResults.CharacterDamageTaken = enemyDamageDealt;
                      await HandleEnemyDefeat(character, enemy);
                      return battleResults;
                    }
                  }
                }
              }
            }
          }
        }
        else
        {
          enemy.IsAttacking = true;
          character.IsAttacking = false;
          for (int i = 0; i < enemyAttackCount; i++)
          {
            enemyAttacker = RollAttackRoll(enemyAttacker.Attacker, character, enemy, enemyHitChance, enemyDamage, enemyCritChance, battleResults.CharacterDamageNegation - character.DamageReceived);
            enemyAttacker.Id = maxId;
            maxId++;
            character.CurrentHP -= Math.Min(enemyAttacker.DamageDealt, character.CurrentHP);
            if (enemyAttacker.DamageHealed > 0)
            {
              enemy.CurrentHP += enemy.CurrentHP + enemyAttacker.DamageHealed <= enemy.CurrentStats.HP
                  ? enemyAttacker.DamageHealed
                  : enemy.CurrentStats.HP - enemy.CurrentHP;
            }
            enemyDamageDealt += enemyAttacker.DamageDealt;
            battleResults.AttackRollResults.Add(enemyAttacker);
            if (character.CurrentHP == 0)
            {
              battleResults.CharacterDamageTaken = enemyDamageDealt;
              await HandleCharacterDefeat(character, enemy, enemyAttacker);
              return battleResults;
            }
          }
          if (canOpponentCounter)
          {
            if (characterAttackSpeedAdv)
            {
              characterAttackCount *= 2;
            }
            for (int i = 0; i < characterAttackCount; i++)
            {
              characterAttacker = RollAttackRoll(characterAttacker.Attacker, character, enemy, charHitChance, charDamage, charCritChance, battleResults.EnemyDamageNegation - enemy.DamageReceived);
              characterAttacker.Id = maxId;
              maxId++;
              enemy.CurrentHP -= Math.Min(characterAttacker.DamageDealt, enemy.CurrentHP);

              if (characterAttacker.DamageHealed > 0)
              {
                character.CurrentHP += character.CurrentHP + characterAttacker.DamageHealed <= character.CurrentStats.HP
                    ? characterAttacker.DamageHealed
                    : character.CurrentStats.HP - character.CurrentHP;
              }

              characterDamageDealt += characterAttacker.DamageDealt;
              battleResults.AttackRollResults.Add(characterAttacker);
              character = await GainWeaponExp(characterId, false);
              if (enemy.CurrentHP == 0)
              {
                battleResults.EnemyDamageTaken = characterDamageDealt;
                battleResults.CharacterDamageTaken = enemyDamageDealt;
                await HandleEnemyDefeat(character, enemy);
                return battleResults;
              }

              if (pairedUpSupport != null)
              {
                var supportAttackCount = pairedUpSupport.EquippedWeapon.IsBrave ? 2 : 1;
                character.DualStrike = character.GetDualStrikeRate(pairedUpSupport);
                var randomNumber = _random.Next(0, 101);
                var supportAttack = randomNumber < character.DualStrike ? true : false;
                _logger.LogInformation($"Pair up Dual Strike chance: {character.DualStrike}, rol: {randomNumber}, action: {supportAttack}");
                if (supportAttack)
                {
                  for (int s = 0; s < supportAttackCount; s++)
                  {
                    supportAttacker = HandleSupportAttack(character, enemy, pairedUpSupport);
                    supportAttacker.Id = maxId;
                    maxId++;
                    enemy.CurrentHP -= Math.Min(characterAttacker.DamageDealt, enemy.CurrentHP);

                    characterDamageDealt += supportAttacker.DamageDealt;
                    battleResults.AttackRollResults.Add(supportAttacker);

                    if (enemy.CurrentHP == 0)
                    {
                      battleResults.EnemyDamageTaken += characterDamageDealt;
                      battleResults.CharacterDamageTaken = enemyDamageDealt;
                      await HandleEnemyDefeat(character, enemy);
                      return battleResults;
                    }
                  }
                }

              }
              else if (highestRankSupport != null)
              {
                var supportAttackCount = highestRankSupport.EquippedWeapon.IsBrave ? 2 : 1;
                character.DualStrike = character.GetDualStrikeRate(highestRankSupport);
                var randomNumber = _random.Next(0, 101);
                var supportAttack = randomNumber < character.DualStrike ? true : false;
                _logger.LogInformation($"Adjacent Support Dual Strike chance: {character.DualStrike}, rol: {randomNumber}, action: {supportAttack}");
                if (supportAttack)
                {
                  for (int s = 0; s < supportAttackCount; s++)
                  {
                    supportAttacker = HandleSupportAttack(character, enemy, highestRankSupport);
                    supportAttacker.Id = maxId;
                    maxId++;
                    enemy.CurrentHP -= Math.Min(characterAttacker.DamageDealt, enemy.CurrentHP);
                    if (enemy.CurrentHP == 0)
                    {
                      battleResults.EnemyDamageTaken = characterDamageDealt;
                      battleResults.CharacterDamageTaken = enemyDamageDealt;
                      await HandleEnemyDefeat(character, enemy);
                      return battleResults;
                    }
                  }
                }
              }
            }
          }
          if (enemyAttackSpeedAdv)
          {
            for (int i = 0; i < enemyAttackSpeedAdvCount; i++)
            {
              enemyAttacker = RollAttackRoll(enemyAttacker.Attacker, character, enemy, enemyHitChance, enemyDamage, enemyCritChance, battleResults.CharacterDamageNegation - character.DamageReceived);
              enemyAttacker.Id = maxId;
              maxId++;
              character.CurrentHP -= Math.Min(enemyAttacker.DamageDealt, character.CurrentHP);
              if (enemyAttacker.DamageHealed > 0)
              {
                enemy.CurrentHP += enemy.CurrentHP + enemyAttacker.DamageHealed <= enemy.CurrentStats.HP
                    ? enemyAttacker.DamageHealed
                    : enemy.CurrentStats.HP - enemy.CurrentHP;
              }
              battleResults.AttackRollResults.Add(enemyAttacker);
              if (character.CurrentHP == 0)
              {
                battleResults.EnemyDamageTaken = characterDamageDealt;
                battleResults.CharacterDamageTaken = enemyDamageDealt;
                await HandleCharacterDefeat(character, enemy, enemyAttacker);
                return battleResults;
              }
            }
          }
        }
        character.IsInCombat = character.IsAttacking = character.IsWeaponTriangleAdvantage = character.IsWeaponTriangleDisadvantage = false;
        enemy.IsInCombat = enemy.IsAttacking = enemy.IsWeaponTriangleAdvantage = enemy.IsWeaponTriangleDisadvantage = false;
        _ = await _charactersRepository.UpdateCharacter(character);
        _ = await _charactersRepository.UpdateEnemy(enemy);
        battleResults.EnemyDamageTaken = characterDamageDealt;
        battleResults.CharacterDamageTaken = enemyDamageDealt;
        return battleResults;
      }
      catch (Exception)
      {
        return null;
      }
    }

    private static AttackRoll RollAttackRoll(string attacker, Character character, Enemy enemy, int hitChance, int damage, int critChance, int damageNegation)
    {
      AttackRoll currentAttacker = new AttackRoll() { Attacker = attacker };
      AbilityCheck abilityCheck = new AbilityCheck();
      List<string> combatAbilities = [CombatTypeCode.Lethality, CombatTypeCode.Aether, CombatTypeCode.Astra, CombatTypeCode.DragonFang, CombatTypeCode.Sol, CombatTypeCode.Luna, CombatTypeCode.Ignis, CombatTypeCode.RendHeaven, CombatTypeCode.Vengeance];
      List<string> equippedAbilities = attacker == CombatTypeCode.Character
          ? character.EquippedAbilities
          .Where(ability => combatAbilities.Contains(ability.Name))
          .Select(ability => ability.Name)
          .ToList()
          : enemy.EquippedAbilities
          .Where(ability => combatAbilities.Contains(ability.Name))
          .Select(ability => ability.Name)
          .ToList();
      var randomNumber = 0;

      var hit = false;
      var crit = false;
      var lethality = false;
      var aether = false;
      var astra = false;
      var dragonFang = false;
      var sol = false;
      var luna = false;
      var ignis = false;
      var rendHeaven = false;
      var vengeance = false;
      int lethalityChance = attacker == CombatTypeCode.Character ? character.CurrentStats.Skl / 4 : enemy.CurrentStats.Skl / 4;
      int aetherChance = attacker == CombatTypeCode.Character ? character.CurrentStats.Skl / 2 : enemy.CurrentStats.Skl / 2;
      int astraChance = attacker == CombatTypeCode.Character ? character.CurrentStats.Skl / 2 : enemy.CurrentStats.Skl / 2;
      int dragonFangChance = attacker == CombatTypeCode.Character ? (int)(character.CurrentStats.Skl * .75) : (int)(enemy.CurrentStats.Skl * .75);
      int solChance = attacker == CombatTypeCode.Character ? character.CurrentStats.Skl : enemy.CurrentStats.Skl;
      int lunaChance = attacker == CombatTypeCode.Character ? character.CurrentStats.Skl : enemy.CurrentStats.Skl;
      int ignisChance = attacker == CombatTypeCode.Character ? character.CurrentStats.Skl : enemy.CurrentStats.Skl;
      int rendHeavenChance = attacker == CombatTypeCode.Character ? (int)(character.CurrentStats.Skl * 1.5) : (int)(enemy.CurrentStats.Skl * 1.5);
      int vengeanceChance = attacker == CombatTypeCode.Character ? character.CurrentStats.Skl * 2 : enemy.CurrentStats.Skl * 2;

      randomNumber = _random.Next(0, 101);
      hit = randomNumber < hitChance ? true : false;
      currentAttacker.AttackRollResult = randomNumber;
      currentAttacker.AttackHit = hit;
      if (hit)
      {
        if (equippedAbilities.Contains(CombatTypeCode.Lethality))
        {
          randomNumber = _random.Next(0, 101);
          lethality = randomNumber < lethalityChance ? true : false;
          var lethalityRoll = randomNumber;
          if (lethality)
          {
            damage = enemy.CurrentHP;
          }
          currentAttacker.DamageDealt = damage;
          abilityCheck.AddAbilityCheck(CombatTypeCode.Lethality, lethalityChance, lethalityRoll, lethality);
          currentAttacker.AbilityCheck = abilityCheck;
        }
        if (equippedAbilities.Contains(CombatTypeCode.Aether) && !lethality)
        {
          var damageDealt = damage;
          randomNumber = _random.Next(0, 101);
          aether = randomNumber < aetherChance ? true : false;
          var aetherRoll = randomNumber;
          if (aether)
          {
            randomNumber = _random.Next(0, 101);
            crit = randomNumber < critChance ? true : false;
            currentAttacker.CritRollResult = randomNumber;
            currentAttacker.CritHit = crit;
            if (crit)
            {
              damage *= 3;
              damageDealt = damage;
            }
            currentAttacker.DamageDealt = damage;
            currentAttacker.DamageHealed = damage / 2;
            if (crit)
            {
              damage /= 3;
            }
            var aetherAttacks = new List<AttackRoll>();
            var aetherAttack = new AttackRoll();
            aetherAttack.Id = 1;
            randomNumber = _random.Next(0, 101);
            hit = randomNumber < hitChance ? true : false;
            aetherAttack.AttackRollResult = randomNumber;
            aetherAttack.AttackHit = hit;
            if (hit)
            {
              randomNumber = _random.Next(0, 101);
              crit = randomNumber < critChance ? true : false;
              aetherAttack.CritRollResult = randomNumber;
              aetherAttack.CritHit = crit;
              damage += damageNegation / 2;
              if (crit)
              {
                damage *= 3;
              }
              damageDealt += damage;
              aetherAttack.DamageDealt += damage;
            }
            aetherAttacks.Add(aetherAttack);
            abilityCheck.AddAbilityCheck(CombatTypeCode.Aether, aetherChance, aetherRoll, aether, currentAttacker.DamageHealed, aetherAttacks);
            currentAttacker.AbilityCheck = abilityCheck;
          }
          currentAttacker.DamageDealt = damageDealt;
        }
        if (equippedAbilities.Contains(CombatTypeCode.Astra) && !(aether || lethality))
        {
          var damageDealt = damage;
          randomNumber = _random.Next(0, 101);
          astra = randomNumber < astraChance ? true : false;
          var astraRoll = randomNumber;
          if (astra)
          {
            damage /= 2;
            damageDealt = damage;

            randomNumber = _random.Next(0, 101);
            crit = randomNumber < critChance ? true : false;
            currentAttacker.CritRollResult = randomNumber;
            currentAttacker.CritHit = crit;
            int critDamage = crit ? damage * 3 : damage;
            damageDealt = critDamage;

            var astraAttacks = new List<AttackRoll>();
            var astraAttackTimes = 1;

            while (astraAttackTimes < 4)
            {
              var astraAttack = new AttackRoll();
              randomNumber = _random.Next(0, 101);
              hit = randomNumber < hitChance ? true : false;
              astraAttack.AttackRollResult = randomNumber;
              astraAttack.AttackHit = hit;
              if (hit)
              {
                randomNumber = _random.Next(0, 101);
                crit = randomNumber < critChance ? true : false;
                astraAttack.CritRollResult = randomNumber;
                astraAttack.CritHit = crit;
                int finalDamage = crit ? damage * 3 : damage;

                damageDealt += finalDamage;
                astraAttack.DamageDealt += finalDamage;
              }
              astraAttack.Id = astraAttackTimes;
              astraAttacks.Add(astraAttack);
              astraAttackTimes++;
            }
            abilityCheck.AddAbilityCheck(CombatTypeCode.Astra, astraChance, astraRoll, astra, 0, astraAttacks);
            currentAttacker.AbilityCheck = abilityCheck;
          }
          currentAttacker.DamageDealt = damageDealt;
        }
        if (equippedAbilities.Contains(CombatTypeCode.DragonFang) && !(lethality || aether || astra))
        {
          randomNumber = _random.Next(0, 101);
          dragonFang = randomNumber < dragonFangChance ? true : false;
          var dragonFangRoll = randomNumber;
          if (dragonFang)
          {
            var bonusDamage = character.CurrentStats.Str / 2;
            randomNumber = _random.Next(0, 101);
            crit = randomNumber < critChance ? true : false;
            currentAttacker.CritRollResult = randomNumber;
            currentAttacker.CritHit = crit;
            damage += bonusDamage;
            if (crit)
            {
              damage *= 3;
            }
          }
          currentAttacker.DamageDealt = damage;
          abilityCheck.AddAbilityCheck(CombatTypeCode.DragonFang, dragonFangChance, dragonFangRoll, dragonFang);
          currentAttacker.AbilityCheck = abilityCheck;
        }
        if (equippedAbilities.Contains(CombatTypeCode.Sol) && !(lethality || aether || astra || dragonFang))
        {
          randomNumber = _random.Next(0, 101);
          sol = randomNumber < solChance ? true : false;
          var solRoll = randomNumber;
          if (sol)
          {
            randomNumber = _random.Next(0, 101);
            crit = randomNumber < critChance ? true : false;
            currentAttacker.CritRollResult = randomNumber;
            currentAttacker.CritHit = crit;
            if (crit)
            {
              damage *= 3;
            }
            currentAttacker.DamageHealed = damage / 2;
          }
          currentAttacker.DamageDealt = damage;
          abilityCheck.AddAbilityCheck(CombatTypeCode.Sol, solChance, solRoll, sol, currentAttacker.DamageHealed);
          currentAttacker.AbilityCheck = abilityCheck;
        }
        if (equippedAbilities.Contains(CombatTypeCode.Luna) && !(lethality || aether || astra || dragonFang || sol))
        {
          randomNumber = _random.Next(0, 101);
          luna = randomNumber < lunaChance ? true : false;
          var lunaRoll = randomNumber;
          if (luna)
          {
            damage += damageNegation / 2;
            randomNumber = _random.Next(0, 101);
            crit = randomNumber < critChance ? true : false;
            currentAttacker.CritRollResult = randomNumber;
            currentAttacker.CritHit = crit;
            if (crit)
            {
              damage *= 3;
            }
          }
          currentAttacker.DamageDealt = damage;
          abilityCheck.AddAbilityCheck(CombatTypeCode.Luna, lunaChance, lunaRoll, luna);
          currentAttacker.AbilityCheck = abilityCheck;
        }
        if (equippedAbilities.Contains(CombatTypeCode.Ignis) && !(lethality || aether || astra || dragonFang || sol || luna))
        {
          randomNumber = _random.Next(0, 101);
          ignis = randomNumber < ignisChance ? true : false;
          var ignisRoll = randomNumber;
          if (ignis)
          {
            if (character.EquippedWeapon.IsMagical)
            {
              damage += character.CurrentStats.Str / 2;
            }
            else
            {
              damage += character.CurrentStats.Mag / 2;
            }
            randomNumber = _random.Next(0, 101);
            crit = randomNumber < critChance ? true : false;
            currentAttacker.CritRollResult = randomNumber;
            currentAttacker.CritHit = crit;
            if (crit)
            {
              damage *= 3;
            }
          }
          currentAttacker.DamageDealt = damage;
          abilityCheck.AddAbilityCheck(CombatTypeCode.Ignis, ignisChance, ignisRoll, ignis);
          currentAttacker.AbilityCheck = abilityCheck;
        }
        if (equippedAbilities.Contains(CombatTypeCode.RendHeaven) && !(lethality || aether || astra || dragonFang || sol || luna || ignis))
        {
          randomNumber = _random.Next(0, 101);
          rendHeaven = randomNumber < rendHeavenChance ? true : false;
          var rendHeavenRoll = randomNumber;
          if (rendHeaven)
          {
            if (character.EquippedWeapon.IsMagical)
            {
              damage += enemy.CurrentStats.Mag / 2;
            }
            else
            {
              damage += enemy.CurrentStats.Str / 2;
            }
            randomNumber = _random.Next(0, 101);
            crit = randomNumber < critChance ? true : false;
            currentAttacker.CritRollResult = randomNumber;
            currentAttacker.CritHit = crit;
            if (crit)
            {
              damage *= 3;
            }
          }
          currentAttacker.DamageDealt = damage;
          abilityCheck.AddAbilityCheck(CombatTypeCode.RendHeaven, rendHeavenChance, rendHeavenRoll, rendHeaven);
          currentAttacker.AbilityCheck = abilityCheck;
        }
        if (equippedAbilities.Contains(CombatTypeCode.Vengeance) && !(lethality || aether || astra || dragonFang || sol || luna || ignis || rendHeaven))
        {
          randomNumber = _random.Next(0, 101);
          vengeance = randomNumber < vengeanceChance ? true : false;
          var vengeanceRoll = randomNumber;
          if (vengeance)
          {
            var bonusDamage = character.CurrentStats.HP - character.CurrentHP;
            if (bonusDamage > 0)
            {
              damage += bonusDamage;
            }
            randomNumber = _random.Next(0, 101);
            crit = randomNumber < critChance ? true : false;
            currentAttacker.CritRollResult = randomNumber;
            currentAttacker.CritHit = crit;
            if (crit)
            {
              damage *= 3;
            }
          }
          currentAttacker.DamageDealt = damage;
          abilityCheck.AddAbilityCheck(CombatTypeCode.Vengeance, vengeanceChance, vengeanceRoll, vengeance);
          currentAttacker.AbilityCheck = abilityCheck;
        }
        if (!(lethality || aether || astra || dragonFang || sol || luna || ignis || rendHeaven || vengeance))
        {
          randomNumber = _random.Next(0, 101);
          crit = randomNumber < critChance ? true : false;
          currentAttacker.CritRollResult = randomNumber;
          currentAttacker.CritHit = crit;
          if (crit)
          {
            damage *= 3;
          }
          currentAttacker.DamageDealt = damage;
        }
      }
      if (currentAttacker.DamageDealt < 0)
      {
        currentAttacker.DamageDealt = 0;
      }
      return currentAttacker;
    }

    private static AttackRoll HandleSupportAttack(Character character, Enemy enemy, Support support)
    {
      AttackRoll supportAttacker = new AttackRoll() { Attacker = CombatTypeCode.Support };
      supportAttacker.AttackHit = true;
      supportAttacker.AttackRollResult = 1;
      var randomNumber = 0;
      var crit = false;
      var critChance = Math.Max(support.Crit - enemy.Dodge, 0);
      var damage = support.EquippedWeapon.IsMagical ? Math.Max((support.CurrentStats.Mag + support.EquippedWeapon.Might.Value) - enemy.CurrentStats.Res, 0) : Math.Max((support.CurrentStats.Str + support.EquippedWeapon.Might.Value) - enemy.CurrentStats.Def, 0);
      randomNumber = _random.Next(0, 101);
      crit = randomNumber < critChance ? true : false;
      supportAttacker.CritHit = crit;
      supportAttacker.CritRollResult = randomNumber;
      if (crit)
      {
        damage *= 3;
      }
      supportAttacker.DamageDealt += damage;
      return supportAttacker;
    }

    private async Task HandleEnemyDefeat(Character character, Enemy enemy)
    {
      _logger.LogInformation("Enemy was defeated");
      character.IsInCombat = character.IsAttacking = character.IsWeaponTriangleAdvantage = character.IsWeaponTriangleDisadvantage = false;
      _ = await _charactersRepository.UpdateCharacter(character);
      _ = await _charactersRepository.RemoveEnemy(enemy.Id);
    }

    private async Task HandleCharacterDefeat(Character character, Enemy enemy, AttackRoll enemyAttacker)
    {
      _logger.LogInformation("Character was defeated ");
      character.IsInCombat = character.IsAttacking = character.IsWeaponTriangleAdvantage = character.IsWeaponTriangleDisadvantage = false;
      enemy.IsInCombat = enemy.IsAttacking = enemy.IsWeaponTriangleAdvantage = enemy.IsWeaponTriangleDisadvantage = false;
      _ = await _charactersRepository.UpdateCharacter(character);
      _ = await _charactersRepository.UpdateEnemy(enemy);
      _ = await ChangeCondition(character.Id, enemyAttacker.CritHit ? 2 : 1);
    }

    public async Task<Tuple<bool, int>> ConvertCharacterFileToIndividualFiles()
    {
      try
      {
        var result = await _charactersRepository.ConvertCharacterFileToIndividualFiles();
        return result;
      }
      catch (Exception)
      {
        return new Tuple<bool, int>(false, 0);
      }
    }
  }
}
