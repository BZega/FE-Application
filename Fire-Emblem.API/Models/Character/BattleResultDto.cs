namespace Fire_Emblem.API.Models.Character
{
    public class BattleResultDto
    {
        public int CharacterHitCHance { get; set; }
        public int CharacterDamageOutput { get; set; }
        public int CharacterCritChance { get; set; }
        public int CharacterAttackSpeed { get; set; }
        public int EnemyHitChance { get; set; }
        public int EnemyDamageOutput { get; set; }
        public int EnemyCritChance { get; set; }
        public int EnemyAttackSpeed { get; set; }
        public int CharacterBaseHitChance { get; set; }
        public int EnemyAvoid { get; set; }
        public int CharacterBaseDamage { get; set; }
        public int EnemyDamageNegation { get; set; }
        public int CharacterBaseCritChance { get; set; }
        public int EnemyDodge { get; set; }
        public int CharacterBaseeAttackSpeed { get; set; }
        public int EnemyBaseHitChance { get; set; }
        public int CharacterAvoid { get; set; }
        public int EnemyBaseCritChance { get; set; }
        public int CharacterDodge { get; set; }
        public int EnemyBaseDamage { get; set; }
        public int CharacterDamageNegation { get; set; }
        public int EnemyBaseAttackSpeed { get; set; }
        public bool IsCharacterWeaponTriangleAdvantage { get; set; }
        public bool IsEnemyWeaponTriangleAdvantage { get; set; }
        public int CharacterDamageTaken { get; set; } = 0;
        public int EnemyDamageTaken { get; set; } = 0;
        public List<AttackRoll> AttackRollResults { get; set; }

        public class AttackRoll
        {
            public int Id { get; set; } = 0;
            public string Attacker { get; set; }
            public int AttackRollResult { get; set; } = 0;
            public bool AttackHit { get; set; } = false;
            public int CritRollResult { get; set; } = 0;
            public bool CritHit { get; set; } = false;
            public int DamageDealt { get; set; } = 0;
            public int DamageHealed { get; set; } = 0;

            public AbilityCheck? AbilityCheck { get; set; }
        }

        public class AbilityCheck
        {
            private readonly Dictionary<string, object> _abilityCheck = new();

            public IReadOnlyDictionary<string, object> AbilityData => _abilityCheck;

            public void AddAbilityCheck(string abilityName, int rollChance, int rollResult, bool isSuccess, int healed = 0, List<AttackRoll> attackRolls = null)
            {
                switch (abilityName)
                {
                    case "Miracle":
                        _abilityCheck["MiracleChance"] = rollChance;
                        _abilityCheck["MiracleRollResult"] = rollResult;
                        _abilityCheck["IsMiracleSuccess"] = isSuccess;
                        break;
                    case "Lethality":
                        _abilityCheck["LethalityChance"] = rollChance;
                        _abilityCheck["LethalityRollResult"] = rollResult;
                        _abilityCheck["IsLethalitySuccess"] = isSuccess;
                        break;
                    case "Aether":
                        _abilityCheck["AetherChance"] = rollChance;
                        _abilityCheck["AetherRollResult"] = rollResult;
                        _abilityCheck["IsAetherSuccess"] = isSuccess;
                        _abilityCheck["DamageHealed"] = healed;
                        if (attackRolls != null)
                            _abilityCheck["AetherAttackRolls"] = new List<AttackRoll>(attackRolls);
                        break;
                    case "Astra":
                        _abilityCheck["AstraChance"] = rollChance;
                        _abilityCheck["AstraRollResult"] = rollResult;
                        _abilityCheck["IsAstraSuccess"] = isSuccess;
                        if (attackRolls != null)
                            _abilityCheck["AstraAttackRolls"] = new List<AttackRoll>(attackRolls);
                        break;
                    case "Dragon Fang":
                        _abilityCheck["DragonFangChance"] = rollChance;
                        _abilityCheck["DragonFangRollResult"] = rollResult;
                        _abilityCheck["IsDragonFangSuccess"] = isSuccess;
                        break;
                    case "Sol":
                        _abilityCheck["SolChance"] = rollChance;
                        _abilityCheck["SolRollResult"] = rollResult;
                        _abilityCheck["IsSolSuccess"] = isSuccess;
                        _abilityCheck["DamageHealed"] = healed;
                        break;
                    case "Luna":
                        _abilityCheck["LunaChance"] = rollChance;
                        _abilityCheck["LunaRollResult"] = rollResult;
                        _abilityCheck["IsLunaSuccess"] = isSuccess;
                        break;
                    case "Ignis":
                        _abilityCheck["IgnisChance"] = rollChance;
                        _abilityCheck["IgnisRollResult"] = rollResult;
                        _abilityCheck["IsIgnisSuccess"] = isSuccess;
                        break;
                    case "Rend Heaven":
                        _abilityCheck["RendHeavenChance"] = rollChance;
                        _abilityCheck["RendHeavenRollResult"] = rollResult;
                        _abilityCheck["IsRendHeavenSuccess"] = isSuccess;
                        break;
                    case "Vengeance":
                        _abilityCheck["VengeanceChance"] = rollChance;
                        _abilityCheck["VengeanceRollResult"] = rollResult;
                        _abilityCheck["IsVengeanceSuccess"] = isSuccess;
                        break;
                    default:
                        throw new ArgumentException($"Unknown ability: {abilityName}");
                }
            }
        }
    }
}


//        public class AbilityCheck
//        {
//            public int MiracleRollResult { get; set; } = 0;
//            public bool IsMiracleSuccess { get; set; } = false;
//            public int LethalityRollResult { get; set; } = 0;
//            public bool IsLethalitySuccess { get; set; } = false;
//            public int AetherRollResult { get; set; } = 0;
//            public bool IsAetherSuccess { get; set; } = false;
//            public int AstraRollResult { get; set; } = 0;
//            public bool IsAstraSuccess { get; set; } = false;
//            public int DragonFangRollResult { get; set; } = 0;
//            public bool IsDragonFangSuccess { get; set; } = false;
//            public int SolRollResult { get; set; } = 0;
//            public bool IsSolSuccess { get; set; } = false;
//            public int LunaRollResult { get; set; } = 0;
//            public bool IsLunaSuccess { get; set; } = false;
//            public int IgnisRollResult { get; set; } = 0;
//            public bool IsIgnisSuccess { get; set; } = false;
//            public int RendHeavenRollResult { get; set; } = 0;
//            public bool IsRendHeavenSuccess { get; set; } = false;
//            public int VengeanceRollResult { get; set; } = 0;
//            public bool IsVengeanceSuccess { get; set; } = false;
//            public int DamageHealed { get; set; } = 0;
//            public List<AttackRoll>? AstraOrAetherAttackRolls { get; set; }
//        }
//    }
//}
