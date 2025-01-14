using Fire_Emblem.API.Business.Helper.FileReader;
using Fire_Emblem.Common.Models;
using System.Text.Json;

namespace Fire_Emblem.API.Business.Repository.Characters
{
    public class CharactersRepository : ICharactersRepository
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "DataStore/character.txt");
        public CharactersRepository() { }

        public async Task<bool> AddNewCharacter(Character character)
        {
            try
            {
                if (character == null)
                {
                    return false;
                }
                else
                {
                    FileHelper.WriteToFile(character, _filePath);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Character>> GetAllCharacters()
        {
            try
            {
                var charactersFile = FileHelper.ReadFromFile<Ability>(_filePath);
                var characters = JsonSerializer.Deserialize<List<Character>>(charactersFile);
                return characters;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Character> GetCharacter(int id)
        {
            try
            {
                var characters = await GetAllCharacters();
                var character = characters.Find(character => character.Id == id);
                if (character != null)
                {
                    return character;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateCharacter(Character character)
        {
            try
            {
                var result = FileHelper.UpdateFile(character, character.Id, _filePath);
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
                var result = FileHelper.DeleteFromFile<Character>(id, _filePath);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
