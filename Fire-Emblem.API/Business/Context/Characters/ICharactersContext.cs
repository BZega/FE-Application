using Fire_Emblem.API.Models.Character;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace Fire_Emblem.API.Business.Context.Characters
{
  public interface ICharactersContext
  {
    Task<List<Character>> GetAllCharacters();
    Task<Character> GetCharacter(string id);
    Task<Tuple<bool, string>> AddNewCharacter(NewCharacterDto newCharacter, string name, StatType humanChoice1, StatType humanChoice2);
    Task<bool> RemoveCharacterById(string id, bool shouldDeleteConvoy);
    Task<Tuple<bool, List<LevelUp>>> UpdateCharacterById(string id, UpdateCharacterDto updateCharacter);
    Task<Tuple<bool, LevelUp>> LevelUpCharacterManually(string id, Stats statIncrease);
    Task<Tuple<bool, LevelUp>> LevelUpCharacterRandomly(string id);
    Task<List<Convoy>> GetAllConvoys();
    Task<Inventory> GetConvoyInventory(string convoyId);
    Task<Convoy> GetConvoyById(string convoyId);
    Task<bool> UpdateConvoyItems(string characterId, string updateType, string location = null, string equipOid = null, int equipId = 0, int sellPrice = 0, string unitChoice = null);
    Task<bool> UpdateAbilities(string characterId, string updateType, string equipOid = null, int equipId = 0, bool equip = false);
    Task<bool> SwitchTerrain(string characterId, TerrainType type);
    Task<bool> CreateSupportCharacter(string characterId, string name, SupportCharacterDto support);
    Task<List<Support>> GetAllSupports();
    Task<Support> GetSupportById(string supportId);
    Task<Stats> GetTotalLevelUpStats(string characterId);
    Task<bool> UpdateSupport(string characterId, string supportId, int supportPoints = 0, Stats levelUpStats = null, ClassType currentClass = ClassType.None, int level = 0, int internalLevel = 0, string equippedWeapon = null);
    Task<bool> TogglePairedAndCloseToggle(string characterId, string supportId, bool isPaired, bool isClose);
    Task<bool> ChangeCondition(string characterId, int trackerChange);
    Task<bool> ReviveFallenCharacter(string characterId);
    Task<Character> GainWeaponExp(string characterId, bool isDoubleAttack);
    Task<Enemy> CreateNewEnemy(EnemyDto enemyDto);
    Task<List<Enemy>> GetAllEnemies();
    Task<Enemy> GetEnemy(int enemyId);
    Task<BattleResultDto> AutomaticBattleSimulator(string characterId, int enemyId, bool canOppenentCounter, bool isCharacterAttacking, bool gainExp);
    Task<Tuple<bool, int>> ConvertCharacterFileToIndividualFiles();
  }
}
