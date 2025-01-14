using Fire_Emblem.Common.Models;

namespace Fire_Emblem.API.Business.Repository.Abilities
{
    public interface IAbilitiesRepository
    {
        Task<bool> AddNewAbility(Ability ability);
        Task<List<Ability>> GetAllAbilities();
        Task<Ability> GetAbilityById(int id);
        Task<Ability> GetAbilityByName(string name);
        Task<bool> RemoveAbilityById(int id);
    }
}
