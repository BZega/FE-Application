using Fire_Emblem.API.Models.Character;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.API.Business.Context.Characters
{
    public interface ICharactersContext
    {
        Task<List<Character>> GetAllCharacters();
        Task<Character> GetCharacter(int id);
        Task<Tuple<bool, string>> AddNewCharacter(NewCharacterDto newCharacter, string name, StatType humanChoice1, StatType humanChoice2);
        Task<bool> RemoveCharacterById(int id);
        Task<Tuple<bool, LevelUp>> LevelUpCharacterManually(int id, Stats statIncrease);
        Task<Tuple<bool, LevelUp>> LevelUpCharacterRandomly(int id);
        Task<List<Convoy>> GetAllConvoys();
        Task<Inventory> GetConvoyInventory(string convoyId);
        Task<Convoy> GetConvoyById(string convoyId);
        Task<bool> UpdateConvoyItems(int characterId, string updateType, string location = null, string equipOid = null, int equipId = 0, int sellPrice = 0, string unitChoice = null);
        Task<bool> UpdateAbilities(int characterId, string updateType, string equipOid = null, int equipId = 0, bool equip = false);
        Task<bool> SwitchTerrain(int characterId, TerrainType type);

    }
}
