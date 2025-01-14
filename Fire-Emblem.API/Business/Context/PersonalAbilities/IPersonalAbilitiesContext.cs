using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.API.Business.Context.PersonalAbilities
{
    public interface IPersonalAbilitiesContext
    {
        Task<List<PersonalAbility>> GetAllPersonalAbilities();
        Task<PersonalAbility> GetPersonalAbility(int? id = null, string name = null);
        Task<bool> AddNewPersonalAbility(StatBonus statBonus, string name, string description);
        Task<bool> RemovePersonalAbilityById(int id);
    }
}
