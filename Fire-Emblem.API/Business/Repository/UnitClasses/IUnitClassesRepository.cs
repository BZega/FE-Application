using Fire_Emblem.Common.Models;

namespace Fire_Emblem.API.Business.Repository.UnitClasses
{
    public interface IUnitClassesRepository
    {
        Task<bool> AddNewClass(UnitClass UnitClass);
        Task<List<UnitClass>> GetAllClasses();
        Task<UnitClass> GetClassById(int id);
        Task<UnitClass> GetClassByName(string name);
        Task<bool> RemoveClassById(int id);
    }
}
