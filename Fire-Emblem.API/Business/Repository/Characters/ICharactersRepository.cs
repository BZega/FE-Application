using Fire_Emblem.Common.Models;

namespace Fire_Emblem.API.Business.Repository.Characters
{
    public interface ICharactersRepository
    {
        Task<bool> AddNewCharacter(Character character);
        Task<List<Character>> GetAllCharacters();
        Task<Character> GetCharacter(string id);
        Task<bool> RemoveCharacterById(string id, bool shouldDeleteConvoy);
        Task<bool> UpdateCharacter(Character character);
        Task<bool> AddNewConvoy(Convoy convoy); 
        Task<Inventory> GetConvoyInventory(string convoyId);
        Task<List<Convoy>> GetAllConvoys();
        Task<Convoy> GetConvoyById(string convoyId);
        Task<bool> UpdateConvoy(Convoy convoy);
        Task<bool> AddNewSupport(Support support);
        Task<List<Support>> GetAllSupports();
        Task<Support> GetSupportById(string id);
        Task<bool> UpdateSupport(Support support);
        Task<Enemy> AddNewEnemy(Enemy enemy);
        Task<List<Enemy>> GetAllEnemies();
        Task<Enemy> GetEnemyById(int id);
        Task<bool> UpdateEnemy(Enemy enemy);
        Task<bool> RemoveEnemy(int id);
        Task<Tuple<bool, int>> ConvertCharacterFileToIndividualFiles();
    }
}
