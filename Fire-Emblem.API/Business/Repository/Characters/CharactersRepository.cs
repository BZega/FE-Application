using Fire_Emblem.API.Business.Helper.FileReader;
using Fire_Emblem.API.Business.Repository.Equips;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;
using System.Text.Json;

namespace Fire_Emblem.API.Business.Repository.Characters
{
    public class CharactersRepository : ICharactersRepository
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "DataStore/character.txt");
        private readonly string _convoyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DataStore/convoy.txt");
        private readonly string _supportFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DataStore/support.txt");
        private readonly string _enemyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DataStore/enemy.txt");
        private readonly IEquipmentRepository _equipmentRepository;
        public CharactersRepository(IEquipmentRepository equipmentRepository)
        {
            _equipmentRepository = equipmentRepository;
        }

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
                var charactersFile = FileHelper.ReadFromFile<Character>(_filePath);
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
                var result = FileHelper.UpdateFile(character, character.Id.ToString(), _filePath);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveCharacterById(int id, bool shouldDeleteConvoy)
        {
            try
            {
                var character = await GetCharacter(id);
                var result = FileHelper.DeleteFromFile<Character>(id.ToString(), _filePath);
                if (result)
                {
                    if (shouldDeleteConvoy)
                    {
                        result = FileHelper.DeleteFromFile<Convoy>(character.ConvoyId, _convoyFilePath);
                    } 
                    if (character.Supports != null)
                    {
                        foreach (var support in character.Supports)
                        {
                            result = FileHelper.DeleteFromFile<Support>(support.Id, _supportFilePath);
                        }
                    }
                }
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddNewConvoy(Convoy convoy)
        {
            try
            {
                if (convoy == null)
                {
                    return false;
                }
                else
                {
                    FileHelper.WriteToFile(convoy, _convoyFilePath);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Convoy>> GetAllConvoys()
        {
            try
            {
                var convoyFile = FileHelper.ReadFromFile<Convoy>(_convoyFilePath);
                var convoys = JsonSerializer.Deserialize<List<Convoy>>(convoyFile);
                return convoys;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Inventory> GetConvoyInventory(string id)
        {
            try
            {
                var convoys = await GetAllConvoys();
                var convoy = convoys.Find(convoy => convoy.Id == id);
                if (convoy != null)
                {
                    return convoy.ConvoyItems;
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

        public async Task<Convoy> GetConvoyById(string id)
        {
            try
            {
                var convoys = await GetAllConvoys();
                var convoy = convoys.Find(convoy => convoy.Id == id);
                if (convoy != null)
                {
                    return convoy;
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

        public async Task<bool> UpdateConvoy(Convoy convoy)
        {
            try
            {
                var result = FileHelper.UpdateFile(convoy, convoy.Id.ToString(), _convoyFilePath);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddNewSupport(Support support)
        {
            try
            {
                if (support == null)
                {
                    return false;
                }
                else
                {
                    FileHelper.WriteToFile(support, _supportFilePath);
                    return true;
                }
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
                var supportFile = FileHelper.ReadFromFile<Support>(_supportFilePath);
                var supports = JsonSerializer.Deserialize<List<Support>>(supportFile);
                return supports;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Support> GetSupportById(string id)
        {
            try
            {
                var supports = await GetAllSupports();
                var support = supports.Find(support => support.Id == id);
                if (support != null)
                {
                    return support;
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

        public async Task<bool> UpdateSupport(Support support)
        {
            try
            {
                var result = FileHelper.UpdateFile(support, support.Id, _supportFilePath);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Enemy> AddNewEnemy(Enemy enemy)
        {
            try
            {
                if (enemy == null)
                {
                    return null;
                }
                else
                {
                    FileHelper.WriteToFile(enemy, _enemyFilePath);
                    return enemy;
                }
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
                var enemyFile = FileHelper.ReadFromFile<Enemy>(_enemyFilePath);
                var enemies = JsonSerializer.Deserialize<List<Enemy>>(enemyFile);
                return enemies;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Enemy> GetEnemyById(int id)
        {
            try
            {
                var enemies = await GetAllEnemies();
                var enemy = enemies.Find(enemy => enemy.Id == id);
                if (enemy != null)
                {
                    return enemy;
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

        public async Task<bool> UpdateEnemy(Enemy enemy)
        {
            try
            {
                var result = FileHelper.UpdateFile(enemy, enemy.Id.ToString(), _enemyFilePath);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveEnemy(int enemyId)
        {
            try
            {
                var result = FileHelper.DeleteFromFile<Enemy>(enemyId.ToString(), _enemyFilePath); 
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}