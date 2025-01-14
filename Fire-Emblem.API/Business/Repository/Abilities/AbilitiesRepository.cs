using Fire_Emblem.API.Business.Helper.FileReader;
using Fire_Emblem.Common.Models;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Fire_Emblem.API.Business.Repository.Abilities
{
    public class AbilitiesRepository : IAbilitiesRepository
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "DataStore/ability.txt");
        public AbilitiesRepository() { }

        public async Task<bool> AddNewAbility(Ability ability)
        {
            try
            {
                if (ability == null)
                {
                    return false;
                }
                else
                {
                    FileHelper.WriteToFile<Ability>(ability, _filePath);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Ability>> GetAllAbilities()
        {
            try
            {
                var abilitiesFile = FileHelper.ReadFromFile<Ability>(_filePath);
                var abilities = JsonSerializer.Deserialize<List<Ability>>(abilitiesFile);
                return abilities;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Ability> GetAbilityById(int id)
        {
            try
            {
                var abilities = await GetAllAbilities();
                var ability = abilities.Find(ability => ability.Id == id);
                if (ability != null)
                {
                    return ability;
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

        public async Task<Ability> GetAbilityByName(string name)
        {
            try
            {
                var abilities = await GetAllAbilities();
                var ability = abilities.Find(ability => ability.Name == name);
                if (ability != null)
                {
                    return ability;
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

        public async Task<bool> RemoveAbilityById(int id)
        {
            try
            {
                var result = FileHelper.DeleteFromFile<Ability>(id, _filePath);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
