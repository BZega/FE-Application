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
        private readonly ILogger<CharactersController> _logger;

        public CharactersController(ICharactersContext charactersContext, ILogger<CharactersController> logger)
        {
            _charactersContext = charactersContext;
            _logger = logger;
        }

        [HttpGet]
        [Route("get-all-characters")]
        public async Task<ActionResult<List<Character>>> GetAllCharacters()
        {
            try
            {
                _logger.LogInformation("Attempting to get all characters");
                List<Character> result = await _charactersContext.GetAllCharacters();
                _logger.LogInformation($"{result.Count} characters");
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
        [Route("remove-character/{id}")]
        public async Task<ActionResult<bool>> RemoveCharacterById(int id, bool shouldDeleteConvoy)
        {
            try
            {
                var result = await _charactersContext.RemoveCharacterById(id, shouldDeleteConvoy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-all-convoys")]
        public async Task<ActionResult<List<Convoy>>> GetAllConvoys()
        {
            try
            {
                List<Convoy> result = await _charactersContext.GetAllConvoys();
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
        public async Task<ActionResult<Tuple<bool, string>>> AddNewCharacter([FromBody] NewCharacterDto character, string name, StatType humanStat1 = StatType.None, StatType humanStat2 = StatType.None)
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
        [Route("update-equipped-abilities/{abilityOid}")]
        public async Task<ActionResult<bool>> UpdateEquippedAbilities(int characterId, UpdateType updateType, string abilityOid)
        {
            try
            {
                var result = await _charactersContext.UpdateAbilities(characterId, updateType.ToString(), abilityOid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("create-support-character/{characterId}/{name}")]
        public async Task<ActionResult<bool>> CreateSupportCharacter(int characterId, string name, SupportCharacterDto support)
        {
            try
            {
                var result = await _charactersContext.CreateSupportCharacter(characterId, name, support);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-all-supports")]
        public async Task<ActionResult<List<Support>>> GetAllSupports()
        {
            try
            {
                List<Support> result = await _charactersContext.GetAllSupports();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-support/{id}")]
        public async Task<ActionResult<Support>> GetSupportById(string id)
        {
            try
            {
                var result = await _charactersContext.GetSupportById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("update-support-character{characterId}/{supportId}")]
        public async Task<ActionResult<bool>> UpdateSupportCharacter(int characterId, string supportId, int supportPoints = 0, Stats levelUpStats = null, ClassType currentClass = ClassType.None, int level = 0, int internalLevel = 0, string equippedWeapon = null)
        {
            try
            {
                var result = await _charactersContext.UpdateSupport(characterId, supportId, supportPoints, levelUpStats, currentClass, level, internalLevel, equippedWeapon);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("toggle-paired-and-close-status/{characterId}/{supportId}")]
        public async Task<ActionResult<bool>> TogglePairedAndCloseToggle(int characterId, string supportId, bool isPaired, bool isClose)
        {
            try
            {
                var result = await _charactersContext.TogglePairedAndCloseToggle(characterId, supportId, isPaired, isClose);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-total-level-up/{characterId}")]
        public async Task<ActionResult<Stats>> GetTotalLevelUpStats(int characterId)
        {
            try
            {
                var result = await _charactersContext.GetTotalLevelUpStats(characterId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("change-condition/{characterId}")]
        public async Task<ActionResult<bool>> ChangeCondition(int characterId, int trackerChange)
        {
            try
            {
                var result = await _charactersContext.ChangeCondition(characterId, trackerChange);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("revive-fallen-character")]
        public async Task<ActionResult<bool>> ReviveFallenCharacter(int characterId)
        {
            try
            {
                var result = await _charactersContext.ReviveFallenCharacter(characterId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("gain-weapon-exp/{characterId}")]
        public async Task<ActionResult<bool>> GainWeaponExp(int characterId, bool isDoubleAttack)
        {
            try
            {
                var result = await _charactersContext.GainWeaponExp(characterId, isDoubleAttack);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("create-enemy")]
        public async Task<ActionResult<Enemy>> CreateEnemy(EnemyDto enemy)
        {
            try
            {
                var result = await _charactersContext.CreateNewEnemy(enemy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-all-enemies")]
        public async Task<ActionResult<List<Enemy>>> GetAllEnemies()
        {
            try
            {
                var result = await _charactersContext.GetAllEnemies();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-enemy/{enemyId}")]
        public async Task<ActionResult<Enemy>> GetEnemy(int enemyId)
        {
            try
            {
                var result = await _charactersContext.GetEnemy(enemyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("auto-battle/{characterId}/{enemyId}")]
        public async Task<ActionResult<BattleResultDto>> AutomaticBattleOpponent(int characterId, int enemyId, bool canOpponentCounter, bool isCharacterAttacking, bool gainExp)
        {
            try
            {
                var result = await _charactersContext.AutomaticBattleSimulator(characterId, enemyId, canOpponentCounter, isCharacterAttacking, gainExp);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
