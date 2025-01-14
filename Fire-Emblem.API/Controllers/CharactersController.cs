using Fire_Emblem.API.Business.Context.Abilities;
using Fire_Emblem.API.Business.Context.Characters;
using Fire_Emblem.API.Business.Context.Equips;
using Fire_Emblem.API.Business.Context.PersonalAbilities;
using Fire_Emblem.API.Business.Context.UnitClasses;
using Fire_Emblem.API.Models.Character;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;
using Microsoft.AspNetCore.Mvc;

namespace Fire_Emblem.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CharactersController : ControllerBase
    {
        private readonly ICharactersContext _charactersContext;

        public CharactersController(ICharactersContext charactersContext)
        {
            _charactersContext = charactersContext;
        }

        [HttpGet]
        [Route("get-all-characters")]
        public async Task<ActionResult<List<Character>>> GetAllCharacters()
        {
            try
            {
                List<Character> result = await _charactersContext.GetAllCharacters();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-character/{id}")]
        public async Task<ActionResult<Character>> GetCharacterById(int id)
        {
            try
            {
                Character result = await _charactersContext.GetCharacter(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("add-new-character/{name}")]
        public async Task<ActionResult<Tuple<bool, string>>> AddNewCharacter([FromBody] NewCharacterDto character, string name, StatType humanStat1 = StatType.HP, StatType humanStat2 = StatType.HP)
        {
            try
            {
                var result = await _charactersContext.AddNewCharacter(character, name, humanStat1, humanStat2);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("level-up-character/{characterId}/{manual}")]
        public async Task<ActionResult> LevelUpCharacter(int characterId, bool manual, Stats manualStats = null)
        {
            try
            {
                var result = new Tuple<bool, Levelup>(false, null);
                if (manual)
                {
                    result = await _charactersContext.LevelUpCharacterManually(characterId, manualStats);
                }
                else
                {
                    result = await _charactersContext.LevelUpCharacterRandomly(characterId);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpPost]
        //[Route("equip-new-weapon/{characterId}/{equipOid}")]
        //public async Task<ActionResult<bool>> EquipNewWeapon(int characterId, string equipOid)
        //{
        //    try
        //    {
        //        var result = await _charactersContext.EquipNewWeapon(equipOid);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("unequip-weapon/{characterId}")]
        //public async Task<ActionResult<bool>> UnequipWeapon(int characterId)
        //{
        //    try
        //    {
        //        var result = await _charactersContext.UnequipWeapon();
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("switch-terrain")]
        //public async Task<ActionResult<bool>> SwitchTerrain(TerrainType type)
        //{
        //    try
        //    {
        //        var result = await _charactersContext.SwitchTerrain(type);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("use-promotion-item")]
        //public async Task<ActionResult<bool>> UsePromotionItem(string promotionItem, string unitChoice)
        //{
        //    try
        //    {
        //        var result = await _charactersContext.HandleSealUse(promotionItem, unitChoice);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("buy-equipment/{id}")]
        //public async Task<ActionResult<bool>> BuyEquipment(int id)
        //{
        //    try
        //    {
        //        var result = await _charactersContext.BuyEquipment(id);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("sell-equipment/{equipOid}")]
        //public async Task<ActionResult<bool>> SellEquipment(string equipOid)
        //{
        //    try
        //    {
        //        var result = await _charactersContext.SellEquipment(equipOid);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPost]
        [Route("remove-character/{id}")]
        public async Task<ActionResult<bool>> RemoveCharacterById(int id)
        {
            try
            {
                var result = await _charactersContext.RemoveCharacterById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
