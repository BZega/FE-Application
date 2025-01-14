using Fire_Emblem.API.Business.Context.Abilities;
using Fire_Emblem.API.Business.Context.PersonalAbilities;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;
using Microsoft.AspNetCore.Mvc;

namespace Fire_Emblem.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonalAbilitiesController : ControllerBase
    {
        private readonly IPersonalAbilitiesContext _personalAbilitiesContext;
        public PersonalAbilitiesController(IPersonalAbilitiesContext personalAbilitiesContext)
        {
            _personalAbilitiesContext = personalAbilitiesContext;
        }

        [HttpGet]
        [Route("get-all-personal-abilities")]
        public async Task<ActionResult<List<PersonalAbility>>> GetAllPersonalAbilities()
        {
            try
            {
                List<PersonalAbility> result = await _personalAbilitiesContext.GetAllPersonalAbilities();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-personal-ability-by-id/{id}")]
        public async Task<ActionResult<PersonalAbility>> GetPersonalAbilityById(int id)
        {
            try
            {
                PersonalAbility result = await _personalAbilitiesContext.GetPersonalAbility(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-personal-ability-by-name/{name}")]
        public async Task<ActionResult<PersonalAbility>> GetPersonalAbilityByName(string name)
        {
            try
            {
                PersonalAbility result = await _personalAbilitiesContext.GetPersonalAbility(null, name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("add-new-personal-ability/{name}")]
        public async Task<ActionResult<bool>> AddNewPersonalAbility(string name, string description, StatBonus bonus = null)
        {
            try
            {
                var result = await _personalAbilitiesContext.AddNewPersonalAbility(bonus, name, description);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("remove-personal-ability/{id}")]
        public async Task<ActionResult<bool>> RemovePersonalAbilityById(int id)
        {
            try
            {
                var result = await _personalAbilitiesContext.RemovePersonalAbilityById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
