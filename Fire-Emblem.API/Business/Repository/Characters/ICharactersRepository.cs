using Fire_Emblem.Common.Models;

namespace Fire_Emblem.API.Business.Repository.Characters
{
    public interface ICharactersRepository
    {
        Task<bool> AddNewCharacter(Character character);
        Task<List<Character>> GetAllCharacters();
        Task<Character> GetCharacter(int id);
        Task<bool> RemoveCharacterById(int id);
        Task<bool> UpdateCharacter(Character character);
    }
}
