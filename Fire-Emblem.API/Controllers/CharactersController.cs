﻿using Fire_Emblem.API.Business.Context.Abilities;
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

        [HttpGet]
        [Route("get-convoy/{id}")]
        public async Task<ActionResult<Convoy>> GetConvoyById(string id)
        {
            try
            {
                var result = await _charactersContext.GetConvoyById(id);
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
                var result = new Tuple<bool, LevelUp>(false, null);
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

        [HttpPost]
        [Route("modify-character-equipment/{characterId}/{updateType}")]
        public async Task<ActionResult<bool>> ModifyCharacterEquipment(int characterId, UpdateType updateType, LocationType location, string equipOid = null, int equipId = 0, int sellPrice = 0, string unitChoice = null)
        {
            try
            {
                var result = await _charactersContext.UpdateConvoyItems(characterId, updateType.ToString(), location.ToString(), equipOid, equipId, sellPrice, unitChoice);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("switch-terrain")]
        public async Task<ActionResult<bool>> SwitchTerrain(int characterId, TerrainType type)
        {
            try
            {
                var result = await _charactersContext.SwitchTerrain(characterId, type);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("acquire-ability/{abilityId}")]
        public async Task<ActionResult<bool>> AcquireAbility(int characterId, int abilityId, bool equip)
        {
            try
            {
                var result = await _charactersContext.UpdateAbilities(characterId, "ACQUIRE", null, abilityId, equip);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("equip-ability/{abilityOid}")]
        public async Task<ActionResult<bool>> EquipAbility(int characterId, string abilityOid)
        {
            try
            {
                var result = await _charactersContext.UpdateAbilities(characterId, "EQUIP", abilityOid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("unequip-ability/{abilityOid}")]
        public async Task<ActionResult<bool>> UnequipAbility(int characterId, string abilityOid)
        {
            try
            {
                var result = await _charactersContext.UpdateAbilities(characterId, "UNEQUIP", abilityOid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

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
