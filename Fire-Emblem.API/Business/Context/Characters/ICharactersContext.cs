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
        Task<Tuple<bool, Levelup>> LevelUpCharacterManually(int id, Stats statIncrease);
        Task<Tuple<bool, Levelup>> LevelUpCharacterRandomly(int id);
    }
}
