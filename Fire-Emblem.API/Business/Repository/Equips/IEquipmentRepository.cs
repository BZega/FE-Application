using Fire_Emblem.Common.Models;

namespace Fire_Emblem.API.Business.Repository.Equips
{
    public interface IEquipmentRepository
    {
        Task<bool> AddNewEquipment(Equipment equipment);
        Task<List<Equipment>> GetAllEquipment();
        Task<Equipment> GetEquipmentById(int id);
        Task<Equipment> GetEquipmentByName(string name);
        Task<bool> RemoveEquipmentById(int id);

    }
}
