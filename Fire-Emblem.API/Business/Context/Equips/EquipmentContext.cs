using Fire_Emblem.API.Business.Repository.Equips;
using Fire_Emblem.API.Models.Equipment;
using Fire_Emblem.Common.Models;

namespace Fire_Emblem.API.Business.Context.Equips
{ 
    public class EquipmentContext : IEquipmentContext
    {
        private readonly IEquipmentRepository _equipmentRepository;
        public EquipmentContext(IEquipmentRepository equipmentRepository)
        {
            _equipmentRepository = equipmentRepository;
        }

        public async Task<List<Equipment>> GetAllEquipment()
        {
            try
            {
                List<Equipment> equipment = await _equipmentRepository.GetAllEquipment();
                return equipment;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Equipment> GetEquipment(int? id = null, string name = null)
        {
            try
            {
                Equipment result = null;
                if (id == null)
                {
                    result = await _equipmentRepository.GetEquipmentByName(name);
                }
                else if (string.IsNullOrEmpty(name))
                {
                    result = await _equipmentRepository.GetEquipmentById(id.Value);
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

        public async Task<bool> AddNewWeapon(WeaponDto weapon)
        {
            try
            {
                var equips = await GetAllEquipment();
                var maxId = 0;
                if (equips != null && equips.Count > 0)
                {
                    maxId = equips.Max(id => id.Id);
                }
                Equipment equipment = new Equipment()
                {
                    Id = maxId + 1,
                    Name = weapon.Name,
                    Rank = weapon.Rank,
                    WeaponType = weapon.WeaponType,
                    IsMagical = weapon.IsMagical,
                    Might = weapon.Might,
                    Hit = weapon.Hit,
                    Crit = weapon.Crit,
                    Range = weapon.Range,
                    Uses = weapon.Uses,
                    Worth = weapon.Worth,
                    WeaponExp = weapon.WeaponExp,
                    Description = weapon.Description,
                    StatBonus = weapon.StatBonus
                };
                if (equipment != null)
                {
                    var result = _equipmentRepository.AddNewEquipment(equipment);
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

        public async Task<bool> AddNewStaff(StaffDto staff)
        {
            try
            {
                var equips = await GetAllEquipment();
                var maxId = 0;
                if (equips != null && equips.Count > 0)
                {
                    maxId = equips.Max(id => id.Id);
                }
                Equipment equipment = new Equipment()
                {
                    Id = maxId + 1,
                    Name = staff.Name,
                    Rank = staff.Rank,
                    WeaponType = staff.WeaponType,
                    IsMagical = staff.IsMagical,
                    BaseHP = staff.BaseHP,
                    Range = staff.Range,
                    Uses = staff.Uses,
                    Worth = staff.Worth,
                    WeaponExp = staff.WeaponExp,
                    Description = staff.Description,
                    StatBonus = staff.StatBonus
                };
                if (equipment != null)
                {
                    var result = _equipmentRepository.AddNewEquipment(equipment);
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

        public async Task<bool> AddNewItem(ItemDto item)
        {
            try
            {
                var equips = await GetAllEquipment();
                var maxId = 0;
                if (equips != null && equips.Count > 0)
                {
                    maxId = equips.Max(id => id.Id);
                }
                Equipment equipment = new Equipment()
                {
                    Id = maxId + 1,
                    Name = item.Name,
                    WeaponType = item.WeaponType,
                    Uses = item.Uses,
                    Worth = item.Worth,
                    Description = item.Description,
                    StatBonus = item.StatBonus
                };
                if (equipment != null)
                {
                    var result = _equipmentRepository.AddNewEquipment(equipment);
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
        public async Task<bool> RemoveEquipmentById(int id)
        {
            try
            {
                var result = await _equipmentRepository.RemoveEquipmentById(id);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
