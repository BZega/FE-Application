using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.API.Business.Context.Abilities
{
    public interface IAbilitiesContext
    {
        Task<List<Ability>> GetAllAbilities();
        Task<Ability> GetAbility(int? id = null, string name = null);
        Task<bool> AddNewAbility(StatBonus statBonus, string name, string description, int levelAcquired, AbilityType type, bool combatCheck);
        Task<bool> RemoveAbilityById(int id);
    }
}
