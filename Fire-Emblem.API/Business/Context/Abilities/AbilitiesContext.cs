using Fire_Emblem.API.Business.Repository.Abilities;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.API.Business.Context.Abilities
{
    public class AbilitiesContext : IAbilitiesContext
    {
        private readonly IAbilitiesRepository _abilitiesRepository;
        public AbilitiesContext(IAbilitiesRepository abilitiesRepository)
        {
            _abilitiesRepository = abilitiesRepository;
        }

        public async Task<List<Ability>> GetAllAbilities()
        {
            try
            {
                List<Ability> abilities = await _abilitiesRepository.GetAllAbilities();
                return abilities;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Ability> GetAbility(int? id = null, string name = null)
        {
            try
            {
                Ability result = null;
                if (id == null)
                {
                    result = await _abilitiesRepository.GetAbilityByName(name);
                }
                else if (string.IsNullOrEmpty(name))
                {
                    result = await _abilitiesRepository.GetAbilityById(id.Value);
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

        public async Task<bool> AddNewAbility(StatBonus statBonus, string name, string description, int levelAcquired, AbilityType type, bool combatCheck)
        {
            try
            {
                var exists = await GetAbility(null, name);
                if(exists != null)
                {
                    return false;
                }
                var abilities = await GetAllAbilities();
                var maxId = 0;
                if (abilities != null && abilities.Count > 0)
                {
                    maxId = abilities.Max(id => id.Id);
                }
                Ability ability = new Ability()
                {
                    Id = maxId + 1,
                    Name = name,
                    Description = description,
                    LevelGained = levelAcquired,
                    AbilityType = type,
                    StatBonus = statBonus,
                    NeedsToInitiateCombat = combatCheck
                };
                
                if (ability != null)
                {
                    var result = _abilitiesRepository.AddNewAbility(ability);
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

        public async Task<bool> RemoveAbilityById(int id)
        {
            try
            {
                var result = await _abilitiesRepository.RemoveAbilityById(id);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
