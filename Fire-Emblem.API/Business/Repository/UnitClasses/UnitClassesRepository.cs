using Fire_Emblem.API.Business.Helper.FileReader;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;
using System.Text.Json;

namespace Fire_Emblem.API.Business.Repository.UnitClasses
{
    public class UnitClassesRepository : IUnitClassesRepository
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "DataStore/class.txt");

        public UnitClassesRepository() { }

        public async Task<bool> AddNewClass(UnitClass UnitClass)
        {
            try
            {
                if (UnitClass == null)
                {
                    return false;
                }
                else
                {
                    FileHelper.WriteToFileAsync(UnitClass, _filePath);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<UnitClass>> GetAllClasses()
        {
            try
            {
                var classesFile = await FileHelper.ReadFromFileAsync<UnitClass>(_filePath);
                var classes = JsonSerializer.Deserialize<List<UnitClass>>(classesFile);
                return classes;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<UnitClass> GetClassById(int id)
        {
            try
            {
                var classes = await GetAllClasses();
                var unitClass = classes.Find(unitClass => unitClass.Id == id);
                if (unitClass != null)
                {
                    return unitClass;
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

        public async Task<UnitClass> GetClassByName(string name)
        {
            try
            {
                var Classes = await GetAllClasses();
                var UnitClass = Classes.Find(UnitClass => UnitClass.Name == name);
                if (UnitClass != null)
                {
                    return UnitClass;
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

        public async Task<bool> UpdateClassSkills(UnitClass unitClass)
        {
            try
            {
                var result = await FileHelper.UpdateFileAsync(unitClass, unitClass.Id.ToString(), _filePath);
                return result;
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
                var result = await FileHelper.DeleteFromFileAsync<UnitClass>(id.ToString(), _filePath);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}