using Fire_Emblem.API.Business.Context.UnitClasses;
using Fire_Emblem.API.Models.UnitClass;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;
using Microsoft.AspNetCore.Mvc;

namespace Fire_Emblem.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UnitClassesController : ControllerBase
    {
        private readonly IUnitClassesContext _unitClassesContext;
        public UnitClassesController(IUnitClassesContext unitClassesContext)
        {
            _unitClassesContext = unitClassesContext;
        }

        [HttpGet]
        [Route("get-all-unit-classes")]
        public async Task<ActionResult<List<UnitClass>>> GetAllClasses()
        {
            try
            {
                List<UnitClass> result = await _unitClassesContext.GetAllClasses();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-class-by-id/{id}")]
        public async Task<ActionResult<UnitClass>> GetClassById(int id)
        {
            try
            {
                UnitClass result = await _unitClassesContext.GetClass(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-class-by-name/{name}")]
        public async Task<ActionResult<UnitClass>> GetClassByName(string name)
        {
            try
            {
                UnitClass result = await _unitClassesContext.GetClass(null, name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("add-new-class")]
        public async Task<ActionResult<bool>> AddNewClass([FromBody] UnitClassDto unitClass)
        {
            try
            {
                var result = await _unitClassesContext.AddNewClass(unitClass);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("update-class-skills")]
        public async Task<ActionResult<bool>> UpdateClassSkills(int classId, List<SkillType> skillTypes)
        {
            try
            {
                var result = await _unitClassesContext.UpdateClassSkills(classId, skillTypes);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("remove-class/{id}")]
        public async Task<ActionResult<bool>> RemoveAbilityById(int id)
        {
            try
            {
                var result = await _unitClassesContext.RemoveClassById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
