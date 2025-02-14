using Fire_Emblem.API.Business.Helper.FileReader;
using Fire_Emblem.Common.Models;
using System.Text.Json;

namespace Fire_Emblem.API.Business.Repository.PersonalAbilities
{
    public class PersonalAbilitiesRepository : IPersonalAbilitiesRepository
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "DataStore/personalAbility.txt");
        public PersonalAbilitiesRepository() { }

        public async Task<bool> AddNewPersonalAbility(PersonalAbility personalAbility)
        {
            try
            {
                if (personalAbility == null)
                {
                    return false;
                }
                else
                {
                    FileHelper.WriteToFileAsync(personalAbility, _filePath);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<PersonalAbility>> GetAllPersonalAbilities()
        {
            try
            {
                var personalAbilitiesFile = await FileHelper.ReadFromFileAsync<PersonalAbility>(_filePath);
                var personalAbilities = JsonSerializer.Deserialize<List<PersonalAbility>>(personalAbilitiesFile);
                return personalAbilities;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<PersonalAbility> GetPersonalAbilityById(int id)
        {
            try
            {
                var personalAbilities = await GetAllPersonalAbilities();
                var personalAbility = personalAbilities.Find(personalAbility => personalAbility.Id == id);
                if (personalAbility != null)
                {
                    return personalAbility;
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

        public async Task<PersonalAbility> GetPersonalAbilityByName(string name)
        {
            try
            {
                var personalAbilities = await GetAllPersonalAbilities();
                var personalAbility = personalAbilities.Find(personalAbility => personalAbility.Name == name);
                if (personalAbility != null)
                {
                    return personalAbility;
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

        public async Task<bool> RemovePersonalAbilityById(int id)
        {
            try
            {
                var result = await FileHelper.DeleteFromFileAsync<PersonalAbility>(id.ToString(), _filePath);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
