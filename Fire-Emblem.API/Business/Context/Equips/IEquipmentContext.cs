using Fire_Emblem.API.Models.Equipment;
using Fire_Emblem.Common.Models;

namespace Fire_Emblem.API.Business.Context.Equips
{
    public interface IEquipmentContext
    {
        Task<List<Equipment>> GetAllEquipment();
        Task<Equipment> GetEquipment(int? id = null, string name = null);
        Task<bool> AddNewWeapon(WeaponDto weapon);
        Task<bool> AddNewStaff(StaffDto weapon);
        Task<bool> AddNewItem(ItemDto weapon);
        Task<bool> RemoveEquipmentById(int id);
    }
}
