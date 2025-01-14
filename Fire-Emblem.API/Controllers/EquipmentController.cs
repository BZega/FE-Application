using Fire_Emblem.API.Business.Context.Equips;
using Fire_Emblem.API.Models.Equipment;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;
using Microsoft.AspNetCore.Mvc;

namespace Fire_Emblem.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentContext _equipmentContext;
        public EquipmentController(IEquipmentContext equipmentContext)
        {
            _equipmentContext = equipmentContext;
        }

        [HttpGet]
        [Route("get-all-equipment")]
        public async Task<ActionResult<List<Equipment>>> GetAllEquipment()
        {
            try
            {
                List<Equipment> result = await _equipmentContext.GetAllEquipment();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-equipment-by-id/{id}")]
        public async Task<ActionResult<Equipment>> GetEquipmentById(int id)
        {
            try
            {
                Equipment result = await _equipmentContext.GetEquipment(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-equipment-by-name/{name}")]
        public async Task<ActionResult<Equipment>> GetEquipmentByName(string name)
        {
            try
            {
                Equipment result = await _equipmentContext.GetEquipment(null, name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("add-new-weapon")]
        public async Task<ActionResult<bool>> AddNewWeapon([FromBody] WeaponDto weapon)
        {
            try
            {
                var result = await _equipmentContext.AddNewWeapon(weapon);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("add-new-staff")]
        public async Task<ActionResult<bool>> AddNewStaff([FromBody] StaffDto staff)
        {
            try
            {
                var result = await _equipmentContext.AddNewStaff(staff);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("add-new-item")]
        public async Task<ActionResult<bool>> AddNewItem([FromBody] ItemDto item)
        {
            try
            {
                var result = await _equipmentContext.AddNewItem(item);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("remove-Equipment/{id}")]
        public async Task<ActionResult<bool>> RemoveEquipmentById(int id)
        {
            try
            {
                var result = await _equipmentContext.RemoveEquipmentById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
