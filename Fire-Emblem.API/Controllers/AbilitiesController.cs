using Fire_Emblem.API.Business.Context.Abilities;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Fire_Emblem.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AbilitiesController : ControllerBase
    {
        private readonly IAbilitiesContext _abilitiesContext;
        public AbilitiesController(IAbilitiesContext abilitiesContext)
        {
            _abilitiesContext = abilitiesContext;
        }

        [HttpGet]
        [Route("get-all-abilities")]
        public async Task<ActionResult<List<Ability>>> GetAllAbilities()
        {
            try
            {
                List<Ability> result = await _abilitiesContext.GetAllAbilities();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-ability-by-id/{id}")]
        public async Task<ActionResult<Ability>> GetAbilityById(int id)
        {
            try
            {
                Ability result = await _abilitiesContext.GetAbility(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-ability-by-name/{name}")]
        public async Task<ActionResult<Ability>> GetAbilityByName(string name)
        {
            try
            {
                Ability result = await _abilitiesContext.GetAbility(null, name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("add-new-ability/{name}")]
        public async Task<ActionResult<bool>> AddNewAbility(string name, string description, int levelAcquired, AbilityType type, bool combatCheck, StatBonus bonus = null)
        {
            try
            {
                var result = await _abilitiesContext.AddNewAbility(bonus, name, description, levelAcquired, type, combatCheck);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("remove-ability/{id}")]
        public async Task<ActionResult<bool>> RemoveAbilityById(int id)
        {
            try
            {
                var result = await _abilitiesContext.RemoveAbilityById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
