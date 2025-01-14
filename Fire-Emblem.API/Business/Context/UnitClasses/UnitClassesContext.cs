using Fire_Emblem.API.Business.Context.Abilities;
using Fire_Emblem.API.Business.Repository.UnitClasses;
using Fire_Emblem.API.Models.UnitClass;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.API.Business.Context.UnitClasses
{
    public class UnitClassesContext : IUnitClassesContext
    {
        private readonly IUnitClassesRepository _unitClassesRepository;
        private readonly IAbilitiesContext _abilitiesContext;
        public UnitClassesContext(IUnitClassesRepository unitClassesRepository, IAbilitiesContext abilitiesContext)
        {
            _unitClassesRepository = unitClassesRepository;
            _abilitiesContext = abilitiesContext;
        }

        public async Task<List<UnitClass>> GetAllClasses()
        {
            try
            {
                List<UnitClass> classes = await _unitClassesRepository.GetAllClasses();
                return classes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<UnitClass> GetClass(int? id = null, string name = null)
        {
            try
            {
                UnitClass result = null;
                if (id == null)
                {
                    result = await _unitClassesRepository.GetClassByName(name);
                }
                else if (string.IsNullOrEmpty(name))
                {
                    result = await _unitClassesRepository.GetClassById(id.Value);
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

        public async Task<bool> AddNewClass(UnitClassDto unitClassDto)
        {
            try
            {
                var classes = await GetAllClasses();
                var maxId = 0;
                if (classes != null && classes.Count > 0)
                {
                    maxId = classes.Max(id => id.Id);
                }
                var unitClass = new UnitClass()
                {
                    Id = maxId + 1,
                    Name = unitClassDto.Name,
                    IsPromoted = unitClassDto.IsPromoted,
                    IsSpecialClass = unitClassDto.IsSpecialClass,
                    InnateBonus = unitClassDto.InnateBonus,
                    UnitTypes = unitClassDto.UnitTypes,
                    BaseStats = unitClassDto.BaseStats,
                    MaxStats = unitClassDto.MaxStats,
                    GrowthRate = unitClassDto.GrowthRate,
                    ClassPromotions = unitClassDto.ClassPromotions,
                    ReclassOptions = unitClassDto.ReclassOptions,
                    Abilities = new List<Ability>(),
                    UsableWeapons = new List<ClassWeapon>()
                };

                foreach (var usableWeapon in unitClassDto.UsableWeapons)
                {
                    var weapon = new ClassWeapon()
                    {
                        WeaponType = usableWeapon.WeaponType,
                        MinRank = usableWeapon.MinRank,
                        MaxRank = usableWeapon.MaxRank
                    };
                    unitClass.UsableWeapons.Add(weapon);
                }

                foreach (var ability in unitClassDto.Abilities)
                {
                    
                    var abilityResult = await _abilitiesContext.GetAbility(null, ability);
                    if (abilityResult != null)
                    {
                        unitClass.Abilities.Add(abilityResult);
                    }
                }

                if (unitClass != null)
                {
                    var result = _unitClassesRepository.AddNewClass(unitClass);
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

        public async Task<bool> RemoveClassById(int id)
        {
            try
            {
                var result = await _unitClassesRepository.RemoveClassById(id);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
