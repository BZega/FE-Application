using Fire_Emblem.API.Business.Repository.Abilities;
using Fire_Emblem.API.Business.Repository.PersonalAbilities;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.API.Business.Context.PersonalAbilities
{
    public class PersonalAbilitiesContext : IPersonalAbilitiesContext
    {
        private readonly IPersonalAbilitiesRepository _personalAbilitiesRepository;
        public PersonalAbilitiesContext(IPersonalAbilitiesRepository personalAbilitiesRepository)
        {
            _personalAbilitiesRepository = personalAbilitiesRepository;
        }

        public async Task<List<PersonalAbility>> GetAllPersonalAbilities()
        {
            try
            {
                List<PersonalAbility> personalAbilities = await _personalAbilitiesRepository.GetAllPersonalAbilities();
                return personalAbilities;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<PersonalAbility> GetPersonalAbility(int? id = null, string name = null)
        {
            try
            {
                PersonalAbility result = null;
                if (id == null)
                {
                    result = await _personalAbilitiesRepository.GetPersonalAbilityByName(name);
                }
                else if (string.IsNullOrEmpty(name))
                {
                    result = await _personalAbilitiesRepository.GetPersonalAbilityById(id.Value);
                }
                else
                {
                    result = null;
                }
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> AddNewPersonalAbility(StatBonus statBonus, string name, string description)
        {
            try
            {
                var personalAbilities = await GetAllPersonalAbilities();
                var maxId = 0;
                if (personalAbilities != null && personalAbilities.Count > 0)
                {
                    maxId = personalAbilities.Max(id => id.Id);
                }
                PersonalAbility personalAbility = new PersonalAbility()
                {
                    Id = maxId + 1,
                    Name = name,
                    Description = description,
                    StatBonus = statBonus
                };
                if (personalAbility != null)
                {
                    var result = _personalAbilitiesRepository.AddNewPersonalAbility(personalAbility);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemovePersonalAbilityById(int id)
        {
            try
            {
                var result = await _personalAbilitiesRepository.RemovePersonalAbilityById(id);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
