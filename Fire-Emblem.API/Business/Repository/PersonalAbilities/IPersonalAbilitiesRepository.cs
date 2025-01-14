using Fire_Emblem.Common.Models;

namespace Fire_Emblem.API.Business.Repository.PersonalAbilities
{
    public interface IPersonalAbilitiesRepository
    {
        Task<bool> AddNewPersonalAbility(PersonalAbility personalAbility);
        Task<List<PersonalAbility>> GetAllPersonalAbilities();
        Task<PersonalAbility> GetPersonalAbilityById(int id);
        Task<PersonalAbility> GetPersonalAbilityByName(string name);
        Task<bool> RemovePersonalAbilityById(int id);
    }
}
