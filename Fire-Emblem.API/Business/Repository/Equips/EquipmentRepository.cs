using Fire_Emblem.API.Business.Helper.FileReader;
using Fire_Emblem.Common.Models;
using System.Text.Json;

namespace Fire_Emblem.API.Business.Repository.Equips
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "DataStore/equipment.txt");
        public EquipmentRepository() { }

        public async Task<bool> AddNewEquipment(Equipment equipment)
        {
            try
            {
                if (equipment == null)
                {
                    return false;
                }
                else
                {
                    FileHelper.WriteToFile(equipment, _filePath);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Equipment>> GetAllEquipment()
        {
            try
            {
                var equipmentFile = FileHelper.ReadFromFile<Equipment>(_filePath);
                var equipment = JsonSerializer.Deserialize<List<Equipment>>(equipmentFile);
                return equipment;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Equipment> GetEquipmentById(int id)
        {
            try
            {
                var equips = await GetAllEquipment();
                var equipment = equips.Find(equipment => equipment.Id == id);
                if (equipment != null)
                {
                    return equipment;
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

        public async Task<Equipment> GetEquipmentByName(string name)
        {
            try
            {
                var equips = await GetAllEquipment();
                var equipment = equips.Find(equipemnt => equipemnt.Name == name);
                if (equipment != null)
                {
                    return equipment;
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

        public async Task<bool> RemoveEquipmentById(int id)
        {
            try
            {
                var result = FileHelper.DeleteFromFile<Ability>(id, _filePath);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
