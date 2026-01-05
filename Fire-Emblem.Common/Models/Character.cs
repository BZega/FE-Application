using Fire_Emblem.Common.TypeCodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Fire_Emblem.Common.Models
{
  public class Character : CombatUnit
  {
    public string Id { get; set; }
    public Biography Biography { get; set; }
    public PersonalAbility PersonalAbility { get; set; }
    public override Stats CurrentStats => GetCurrentStats();
    public GrowthRate TotalGrowthRate => GetTotalGrowthRate();
    public int Exp { get; set; } = 0;
    public int Gold { get; set; } = 1000;
    public int Heal => GetHeal();
    public int DualStrike { get; set; } = 0;
    public int DualGuard { get; set; } = 0;
    public string StartingClass { get; set; }
    public string HeartSealClass { get; set; }
    public List<string> PermanentReclassOptions { get; set; }
    public Asset Asset { get; set; }
    public Flaw Flaw { get; set; }
    public Condition Condition { get; set; }
    public Inventory Inventory { get; set; }
    public Stats BaseStats => GetBaseStats();
    public Stats StatChangeAmount => GetStatChangeAmount();
    public Stats MaxStats => GetMaxStats();
    public GrowthRate PersonalGrowthRate { get; set; }
    public List<string> ReclassOptions => GetReclassOptions();
    public List<Weapon> WeaponRanks { get; set; }
    public List<Support>? Supports { get; set; }
    public List<Ability> AcquiredAbilities { get; set; }
    public List<Skill> Skills { get; set; }
    public List<LevelUp>? LevelupStatIncreases { get; set; }
    public string ConvoyId { get; set; }

    // Override UnitTypes to use Character-specific logic
    public new List<UnitType> UnitTypes => GetUnitTypes();

    public GrowthRate GetTotalGrowthRate()
    {
      var growthRate = new GrowthRate();
      growthRate.Add(CurrentClass.GrowthRate);
      growthRate.Add(PersonalGrowthRate);
      growthRate.Add(Biography.RaceChoice.RacialGrowth);
      foreach (var ability in EquippedAbilities)
      {
        if (ability.StatBonus?.Growths != null && ability.AbilityType == AbilityType.Passive)
        {
          growthRate.Add(ability.StatBonus.Growths);
        }
      }
      return growthRate;
    }

    public Stats GetBaseStats()
    {
      var baseStats = new Stats();
      baseStats.Add(CurrentClass.BaseStats);
      baseStats.Add(Asset.BaseStatBonus);
      baseStats.Add(Flaw.BaseStatBonus);
      if (LevelupStatIncreases != null && LevelupStatIncreases.Count > 0)
      {
        foreach (var levelup in LevelupStatIncreases)
        {
          baseStats.Add(levelup.StatIncrease);
        }
        baseStats.MaximumCheck(MaxStats);
      }
      return baseStats;
    }

    public Stats GetMaxStats()
    {
      var maxStats = new Stats();
      maxStats.Add(CurrentClass.MaxStats);
      maxStats.Add(Asset.MaxStatBonus);
      maxStats.Add(Flaw.MaxStatBonus);
      return maxStats;
    }

    public Stats GetCurrentStats()
    {
      var currentStats = BaseStats;
      if (Terrain.TerrainType != TerrainType.None)
      {
        int.TryParse(Terrain.DefBonus, out var defBonus);
        if (UnitTypes.Any(type => type == UnitType.Flying))
        {
          currentStats.Def += defBonus;
        }
      }
      foreach (var ability in EquippedAbilities)
      {
        if (ability.StatBonus?.Stats != null && ability.AbilityType == AbilityType.Passive)
        {
          currentStats.Add(ability.StatBonus.Stats);
        }
        else if (ability.StatBonus?.Stats != null && ability.AbilityType == AbilityType.Combat && IsInCombat)
        {
          currentStats.Add(ability.StatBonus.Stats);
        }
      }
      if (Supports != null && Supports.Count > 0)
      {
        foreach (var support in Supports)
        {
          if (support.IsPairedUp && support.PairedUpBonus != null)
          {
            currentStats.Add(support.PairedUpBonus);
          }
        }
      }
      currentStats.Add(Condition.StatChange);
      foreach (var item in Inventory.Equipment)
      {
        if (item.IsEquipped && item.StatBonus?.Stats != null)
        {
          currentStats.Add(item.StatBonus.Stats);
        }
      }
      return currentStats;
    }

    public Stats GetStatChangeAmount()
    {
      var statChangeAmount = CurrentStats;
      var baseStats = BaseStats;
      statChangeAmount.Subtract(baseStats);
      return statChangeAmount;
    }

    public List<UnitType> GetUnitTypes()
    {
      List<UnitType> unitTypes = new List<UnitType>();
      foreach (var unitType in CurrentClass.UnitTypes)
      {
        if (!Biography.RaceChoice.UnitType.Equals(unitType))
        {
          unitTypes.Add(unitType);
        }
      }
      unitTypes.Add(Biography.RaceChoice.UnitType);
      return unitTypes;
    }

    // Override GetAttack with Character-specific logic
    protected override int GetAttack()
    {
      var attack = 0;
      attack += (int)Math.Ceiling((CurrentStats.Skl * 3 + CurrentStats.Res) / 2.0);
      if (EquippedWeapon != null && EquippedWeapon.Hit != null)
      {
        attack += EquippedWeapon.Hit.Value;
        if (EquippedWeapon.StatBonus?.Attributes != null)
        {
          attack += EquippedWeapon.StatBonus.Attributes.Hit;
        }
        if (WeaponRanks.Any(weapon => weapon.WeaponType == EquippedWeapon.WeaponType && !weapon.IsActive))
        {
          attack -= 20;
        }
        if (EquippedWeapon.WeaponType == WeaponType.DarkTome && WeaponRanks.Any(weapon => weapon.WeaponType == WeaponType.DarkTome && !weapon.IsActive) && WeaponRanks.Any(weapon => weapon.WeaponType == WeaponType.Tome && !weapon.IsActive))
        {
          attack -= 20;
        }
        if (WeaponRanks.Any(weapon => weapon.WeaponType == EquippedWeapon.WeaponType && weapon.WeaponRank != EquippedWeapon.Rank && weapon.IsActive))
        {
          var weaponRank = WeaponRanks.FirstOrDefault(weapon => weapon.WeaponType == EquippedWeapon.WeaponType);
          if (weaponRank != null)
          {
            switch (weaponRank.WeaponRank)
            {
              case Rank.S:
                break;
              case Rank.A:
                switch (EquippedWeapon.Rank)
                {
                  case Rank.S:
                    attack -= 10;
                    break;
                }
                break;
              case Rank.B:
                switch (EquippedWeapon.Rank)
                {
                  case Rank.S:
                    attack -= 20;
                    break;
                  case Rank.A:
                    attack -= 10;
                    break;
                }
                break;
              case Rank.C:
                switch (EquippedWeapon.Rank)
                {
                  case Rank.S:
                    attack -= 30;
                    break;
                  case Rank.A:
                    attack -= 20;
                    break;
                  case Rank.B:
                    attack -= 10;
                    break;
                }
                break;
              case Rank.D:
                switch (EquippedWeapon.Rank)
                {
                  case Rank.S:
                    attack -= 40;
                    break;
                  case Rank.A:
                    attack -= 30;
                    break;
                  case Rank.B:
                    attack -= 20;
                    break;
                  case Rank.C:
                    attack -= 10;
                    break;
                }
                break;
              case Rank.E:
                switch (EquippedWeapon.Rank)
                {
                  case Rank.S:
                    attack -= 50;
                    break;
                  case Rank.A:
                    attack -= 40;
                    break;
                  case Rank.B:
                    attack -= 30;
                    break;
                  case Rank.C:
                    attack -= 20;
                    break;
                  case Rank.D:
                    attack -= 10;
                    break;
                }
                break;
            }
          }
        }
        else if (WeaponRanks.Any(weapon => weapon.WeaponType == EquippedWeapon.WeaponType && weapon.WeaponRank != EquippedWeapon.Rank && !weapon.IsActive))
        {
          var weaponRank = WeaponRanks.FirstOrDefault(weapon => weapon.WeaponType == EquippedWeapon.WeaponType);
          if (weaponRank != null)
          {
            var originalWeaponExp = weaponRank.WeaponExperience;
            weaponRank.WeaponExperience = weaponRank.WeaponExperience / 2;
            switch (weaponRank.WeaponRank)
            {
              case Rank.S:
                break;
              case Rank.A:
                switch (EquippedWeapon.Rank)
                {
                  case Rank.S:
                    attack -= 10;
                    break;
                }
                break;
              case Rank.B:
                switch (EquippedWeapon.Rank)
                {
                  case Rank.S:
                    attack -= 20;
                    break;
                  case Rank.A:
                    attack -= 10;
                    break;
                }
                break;
              case Rank.C:
                switch (EquippedWeapon.Rank)
                {
                  case Rank.S:
                    attack -= 30;
                    break;
                  case Rank.A:
                    attack -= 20;
                    break;
                  case Rank.B:
                    attack -= 10;
                    break;
                }
                break;
              case Rank.D:
                switch (EquippedWeapon.Rank)
                {
                  case Rank.S:
                    attack -= 40;
                    break;
                  case Rank.A:
                    attack -= 30;
                    break;
                  case Rank.B:
                    attack -= 20;
                    break;
                  case Rank.C:
                    attack -= 10;
                    break;
                }
                break;
              case Rank.E:
                switch (EquippedWeapon.Rank)
                {
                  case Rank.S:
                    attack -= 50;
                    break;
                  case Rank.A:
                    attack -= 40;
                    break;
                  case Rank.B:
                    attack -= 30;
                    break;
                  case Rank.C:
                    attack -= 20;
                    break;
                  case Rank.D:
                    attack -= 10;
                    break;
                }
                break;
            }
            weaponRank.WeaponExperience = originalWeaponExp;
          }
        }
      }
      if (EquippedAbilities != null)
      {
        foreach (var ability in EquippedAbilities)
        {
          if (ability.AbilityType == AbilityType.Passive && ability.StatBonus?.Attributes != null)
          {
            attack += ability.StatBonus.Attributes.Hit;
          }
          if (ability.AbilityType == AbilityType.Combat && ability.StatBonus?.Attributes != null && IsInCombat)
          {
            if (!ability.NeedsToInitiateCombat)
            {
              attack += ability.StatBonus.Attributes.Hit;
            }
            if (ability.NeedsToInitiateCombat && IsAttacking)
            {
              attack += ability.StatBonus.Attributes.Hit;
            }
          }
        }
      }
      if (PersonalAbility.StatBonus?.Attributes != null)
      {
        attack += PersonalAbility.StatBonus.Attributes.Hit;
      }
      if (CurrentClass.InnateBonus?.Attributes != null)
      {
        attack += CurrentClass.InnateBonus.Attributes.Hit;
      }
      if (Supports != null && Supports.Count > 0 && Supports.Any(support => support.IsPairedUp))
      {
        var supportPoints = 0;
        foreach (var support in Supports)
        {
          if (support.IsPairedUp || support.IsClose)
          {
            switch (support.SupportRank)
            {
              case Rank.None:
                supportPoints += 1;
                break;
              case Rank.C:
                supportPoints += 2;
                break;
              case Rank.B:
                supportPoints += 3;
                break;
              case Rank.A:
                supportPoints += 4;
                break;
              case Rank.S:
                supportPoints += 5;
                break;
            }
          }
        }
        if (supportPoints > 0 && supportPoints < 5)
        {
          attack += 10;
        }
        else if (supportPoints > 4 && supportPoints < 9)
        {
          attack += 15;
        }
        else if (supportPoints > 8)
        {
          attack += 20;
        }
      }
      if (IsWeaponTriangleAdvantage)
      {
        var weapon = WeaponRanks.FirstOrDefault(weapon => weapon.WeaponType == EquippedWeapon?.WeaponType);
        if (weapon != null)
        {
          switch (weapon.WeaponRank)
          {
            case Rank.E:
            case Rank.D:
              attack += 5;
              break;
            case Rank.C:
            case Rank.B:
              attack += 10;
              break;
            case Rank.A:
            case Rank.S:
              attack += 15;
              break;
          }
        }
      }
      return attack;
    }
    public int GetHeal()
    {
      var heal = 0;
      if (EquippedWeapon != null && EquippedWeapon.BaseHP != null && EquippedWeapon.WeaponType == WeaponType.Staff)
      {
        heal += EquippedWeapon.BaseHP.Value;
        foreach (var ability in EquippedAbilities)
        {
          if (ability.StatBonus?.Attributes != null && ability.AbilityType == AbilityType.Passive)
          {
            heal += ability.StatBonus.Attributes.Heal;
          }
        }
        if (WeaponRanks.Any(weaponRank => weaponRank.WeaponType == WeaponType.Staff && weaponRank.WeaponRankBonus?.Attributes != null))
        {
          heal += WeaponRanks.FirstOrDefault(weaponRank => weaponRank.WeaponType == WeaponType.Staff).WeaponRankBonus.Attributes.Heal;
        }
      }
      return heal;
    }

    // Override GetDamage with Character-specific logic
    protected override int GetDamage()
    {
      var damage = 0;
      if (EquippedWeapon != null)
      {
        if (EquippedWeapon.IsMagical)
        {
          damage += CurrentStats.Mag;
        }
        else
        {
          damage += CurrentStats.Str;
        }
        if (EquippedWeapon.Might != null)
        {
          damage += EquippedWeapon.Might.Value;
        }
        if (EquippedWeapon.StatBonus?.Attributes != null)
        {
          damage += EquippedWeapon.StatBonus.Attributes.Damage;
        }
        if (WeaponRanks.Any(weapon => weapon.WeaponType == EquippedWeapon.WeaponType && !weapon.IsActive))
        {
          damage -= (int)(damage * .2);
        }
        if (EquippedWeapon.WeaponType == WeaponType.DarkTome && WeaponRanks.Any(weapon => weapon.WeaponType == WeaponType.DarkTome && !weapon.IsActive) && WeaponRanks.Any(weapon => weapon.WeaponType == WeaponType.Tome && !weapon.IsActive))
        {
          damage -= (int)(damage * .2);
        }
      }
      if (EquippedAbilities != null)
      {
        foreach (var ability in EquippedAbilities)
        {
          if (ability.AbilityType == AbilityType.Passive && ability.StatBonus?.Attributes != null)
          {
            damage += ability.StatBonus.Attributes.Damage;
          }
          if (ability.AbilityType == AbilityType.Combat && ability.StatBonus?.Attributes != null && IsInCombat)
          {
            if (!ability.NeedsToInitiateCombat)
            {
              damage += ability.StatBonus.Attributes.Damage;
            }
            if (ability.NeedsToInitiateCombat && IsAttacking)
            {
              damage += ability.StatBonus.Attributes.Damage;
            }
          }
        }
      }
      if (IsWeaponTriangleAdvantage)
      {
        var weapon = WeaponRanks.FirstOrDefault(weapon => weapon.WeaponType == EquippedWeapon?.WeaponType);
        if (weapon != null)
        {
          switch (weapon.WeaponRank)
          {
            case Rank.B:
            case Rank.A:
            case Rank.S:
              damage += 1;
              break;
          }
        }
      }
      return damage;
    }

    // Override GetCrit with Character-specific logic
    protected override int GetCrit()
    {
      var crit = 0;
      crit += (int)Math.Ceiling(CurrentStats.Skl / 2.0);
      if (EquippedWeapon != null && EquippedWeapon.Crit != null)
      {
        crit += EquippedWeapon.Crit.Value;
        if (EquippedWeapon.StatBonus?.Attributes != null)
        {
          crit += EquippedWeapon.StatBonus.Attributes.Crit;
        }
      }
      if (EquippedAbilities != null)
      {
        foreach (var ability in EquippedAbilities)
        {
          if (ability.AbilityType == AbilityType.Passive && ability.StatBonus?.Attributes != null)
          {
            crit += ability.StatBonus.Attributes.Crit;
          }
          if (ability.AbilityType == AbilityType.Combat && ability.StatBonus?.Attributes != null && IsInCombat)
          {
            if (!ability.NeedsToInitiateCombat)
            {
              crit += ability.StatBonus.Attributes.Crit;
            }
            if (ability.NeedsToInitiateCombat && IsAttacking)
            {
              crit += ability.StatBonus.Attributes.Crit;
            }
          }
        }
      }
      if (PersonalAbility.StatBonus?.Attributes != null)
      {
        crit += PersonalAbility.StatBonus.Attributes.Crit;
      }
      if (CurrentClass.InnateBonus?.Attributes != null)
      {
        crit += CurrentClass.InnateBonus.Attributes.Crit;
      }
      if (Supports != null && Supports.Count > 0)
      {
        var supportPoints = 0;
        foreach (var support in Supports)
        {
          if (support.IsPairedUp || support.IsClose)
          {
            switch (support.SupportRank)
            {
              case Rank.None:
                supportPoints += 1;
                break;
              case Rank.C:
                supportPoints += 2;
                break;
              case Rank.B:
                supportPoints += 3;
                break;
              case Rank.A:
                supportPoints += 4;
                break;
              case Rank.S:
                supportPoints += 5;
                break;
            }
          }
        }
        if (supportPoints > 3 && supportPoints < 8)
        {
          crit += 10;
        }
        else if (supportPoints > 7 && supportPoints < 12)
        {
          crit += 15;
        }
        else if (supportPoints > 11)
        {
          crit += 20;
        }
      }
      return crit;
    }

    // Override GetAvoid with Character-specific logic
    protected override int GetAvoid()
    {
      var avoid = 0;
      avoid += (int)Math.Ceiling((CurrentStats.Spd * 3 + CurrentStats.Lck) / 2.0);
      if (EquippedWeapon != null && EquippedWeapon.StatBonus?.Attributes != null)
      {
        avoid += EquippedWeapon.StatBonus.Attributes.Avoid;
      }
      if (EquippedAbilities != null)
      {
        foreach (var ability in EquippedAbilities)
        {
          if (ability.AbilityType == AbilityType.Passive && ability.StatBonus?.Attributes != null)
          {
            avoid += ability.StatBonus.Attributes.Avoid;
          }
          if (ability.AbilityType == AbilityType.Combat && ability.StatBonus?.Attributes != null && IsInCombat)
          {
            if (!ability.NeedsToInitiateCombat)
            {
              avoid += ability.StatBonus.Attributes.Avoid;
            }
            if (ability.NeedsToInitiateCombat && IsAttacking)
            {
              avoid += ability.StatBonus.Attributes.Avoid;
            }
          }
        }
      }
      if (PersonalAbility.StatBonus?.Attributes != null)
      {
        avoid += PersonalAbility.StatBonus.Attributes.Avoid;
      }
      if (CurrentClass.InnateBonus?.Attributes != null)
      {
        avoid += CurrentClass.InnateBonus.Attributes.Avoid;
      }
      if (Terrain.TerrainType != TerrainType.None)
      {
        var hasBonus = int.TryParse(Terrain.AvoidBonus, out var avoidBonus);
        if (hasBonus)
        {
          if (UnitTypes.Any(type => type == UnitType.Flying))
          {
            avoid += avoidBonus;
          }
        }
      }
      if (Supports != null && Supports.Count > 0)
      {
        var supportPoints = 0;
        foreach (var support in Supports)
        {
          if (support.IsPairedUp || support.IsClose)
          {
            switch (support.SupportRank)
            {
              case Rank.None:
                supportPoints += 1;
                break;
              case Rank.C:
                supportPoints += 2;
                break;
              case Rank.B:
                supportPoints += 3;
                break;
              case Rank.A:
                supportPoints += 4;
                break;
              case Rank.S:
                supportPoints += 5;
                break;
            }
          }
        }
        if (supportPoints > 1 && supportPoints < 6)
        {
          avoid += 10;
        }
        else if (supportPoints > 5 && supportPoints < 10)
        {
          avoid += 15;
        }
        else if (supportPoints > 9)
        {
          avoid += 20;
        }
      }
      return avoid;
    }

    // Override GetDodge with Character-specific logic
    protected override int GetDodge()
    {
      var dodge = 0;
      dodge += CurrentStats.Lck;
      if (EquippedWeapon != null && EquippedWeapon.StatBonus?.Attributes != null)
      {
        dodge += EquippedWeapon.StatBonus.Attributes.Dodge;
      }
      if (EquippedAbilities != null)
      {
        foreach (var ability in EquippedAbilities)
        {
          if (ability.AbilityType == AbilityType.Passive && ability.StatBonus?.Attributes != null)
          {
            dodge += ability.StatBonus.Attributes.Dodge;
          }
          if (ability.AbilityType == AbilityType.Combat && ability.StatBonus?.Attributes != null && IsInCombat)
          {
            if (!ability.NeedsToInitiateCombat)
            {
              dodge += ability.StatBonus.Attributes.Dodge;
            }
            if (ability.NeedsToInitiateCombat && IsAttacking)
            {
              dodge += ability.StatBonus.Attributes.Dodge;
            }
          }
        }
      }
      if (PersonalAbility.StatBonus?.Attributes != null)
      {
        dodge += PersonalAbility.StatBonus.Attributes.Dodge;
      }
      if (CurrentClass.InnateBonus?.Attributes != null)
      {
        dodge += CurrentClass.InnateBonus.Attributes.Dodge;
      }
      if (Supports != null && Supports.Count > 0)
      {
        var supportPoints = 0;
        foreach (var support in Supports)
        {
          if (support.IsPairedUp || support.IsClose)
          {
            switch (support.SupportRank)
            {
              case Rank.None:
                supportPoints += 1;
                break;
              case Rank.C:
                supportPoints += 2;
                break;
              case Rank.B:
                supportPoints += 3;
                break;
              case Rank.A:
                supportPoints += 4;
                break;
              case Rank.S:
                supportPoints += 5;
                break;
            }
          }
        }
        if (supportPoints > 2 && supportPoints < 7)
        {
          dodge += 10;
        }
        else if (supportPoints > 6 && supportPoints < 11)
        {
          dodge += 15;
        }
        else if (supportPoints > 10)
        {
          dodge += 20;
        }
      }
      return dodge;
    }

    public int GetDualStrikeRate(Support support)
    {
      var dualStrike = 0;
      dualStrike += (int)Math.Ceiling((CurrentStats.Skl + support.CurrentStats.Skl) / 4.0);
      if (EquippedAbilities.Any(ability => ability.Name == "Dual Strike+"))
      {
        dualStrike += 10;
      }
      switch (support.SupportRank)
      {
        case Rank.None:
          dualStrike += 20;
          break;
        case Rank.C:
          dualStrike += 30;
          break;
        case Rank.B:
          dualStrike += 40;
          break;
        case Rank.A:
          dualStrike += 50;
          break;
        case Rank.S:
          dualStrike += 60;
          break;
      }
      return dualStrike;
    }

    public int GetDualGuardRate(string damageType, Support support)
    {
      var dualGuard = 0;
      if (EquippedAbilities.Any(ability => ability.Name == "Dual Guard+"))
      {
        dualGuard += 10;
      }
      dualGuard += damageType == CombatTypeCode.Physical ? (int)Math.Ceiling((CurrentStats.Def + support.CurrentStats.Def) / 4.0) : (int)Math.Ceiling((CurrentStats.Res + support.CurrentStats.Res) / 4.0);

      switch (support.SupportRank)
      {
        case Rank.None:
          break;
        case Rank.C:
          dualGuard += 2;
          break;
        case Rank.B:
          dualGuard += 5;
          break;
        case Rank.A:
          dualGuard += 7;
          break;
        case Rank.S:
          dualGuard += 10;
          break;
      }
      return dualGuard;
    }

    // Override GetHitWeaponTriangleDisadvantage with Character-specific logic
    public override StatBonus GetHitWeaponTriangleDisadvantage(Rank advantageWeaponRank)
    {
      var attribute = new StatBonus() { Attributes = new Attributes() };
      var hit = 0;
      var damage = 0;
      switch (advantageWeaponRank)
      {
        case Rank.E:
        case Rank.D:
          hit += 5;
          break;
        case Rank.C:
        case Rank.B:
          hit += 10;
          break;
        case Rank.A:
        case Rank.S:
          hit += 15;
          break;
      }
      switch (advantageWeaponRank)
      {
        case Rank.B:
        case Rank.A:
        case Rank.S:
          damage -= 1;
          break;
      }
      attribute.Attributes.Hit = hit;
      attribute.Attributes.Damage = damage;
      return attribute;
    }

    public List<string> GetReclassOptions()
    {
      var reclassOptions = new List<string>();

      if (PermanentReclassOptions != null && PermanentReclassOptions.Count > 0)
      {
        reclassOptions.AddRange(PermanentReclassOptions);
      }

      //if (!string.IsNullOrEmpty(StartingClass) && !reclassOptions.Contains(StartingClass))
      //  reclassOptions.Add(StartingClass);

      //var race = Biography.RaceChoice.RacialType;

      //// Taguel
      //if ((race == RacialType.Taguel || race == RacialType.HalfHumanTaguel) && !reclassOptions.Contains(ClassTypeCode.Taguel))
      //{
      //  reclassOptions.Add(ClassTypeCode.Taguel);
      //}
      //// Kitsune
      //if ((race == RacialType.Kitsune || race == RacialType.HalfHumanKitsune) && !reclassOptions.Contains(ClassTypeCode.Kitsune))
      //{
      //  reclassOptions.Add(ClassTypeCode.Kitsune);
      //}
      //// Wolfskin
      //if ((race == RacialType.Wolfskin || race == RacialType.HalfHumanWolfskin) && !reclassOptions.Contains(ClassTypeCode.Wolfskin))
      //{
      //  reclassOptions.Add(ClassTypeCode.Wolfskin);
      //}
      //// Manakete
      //if (race == RacialType.Manakete || race == RacialType.HalfHumanManakete)
      //{
      //  if (!reclassOptions.Contains(ClassTypeCode.Manakete))
      //    reclassOptions.Add(ClassTypeCode.Manakete);
      //  if (Biography.IsNoble && !reclassOptions.Contains(ClassTypeCode.ManaketeHeir))
      //    reclassOptions.Add(ClassTypeCode.ManaketeHeir);
      //}
      // Human Nobles
      //if (race == RacialType.Human && Biography.IsNoble)
      //{
      //  if (StartingClass == ClassTypeCode.SwordLord && !reclassOptions.Contains(ClassTypeCode.SwordLord))
      //    reclassOptions.Add(ClassTypeCode.SwordLord);
      //  if (StartingClass == ClassTypeCode.AxeLord && !reclassOptions.Contains(ClassTypeCode.AxeLord))
      //    reclassOptions.Add(ClassTypeCode.AxeLord);
      //  if (StartingClass == ClassTypeCode.LanceLord && !reclassOptions.Contains(ClassTypeCode.LanceLord))
      //    reclassOptions.Add(ClassTypeCode.LanceLord);
      //  // If one of the above already exists I don't want to add all three
      //  var hasLordClass = reclassOptions.Contains(ClassTypeCode.SwordLord) ||
      //                    reclassOptions.Contains(ClassTypeCode.AxeLord) ||
      //                    reclassOptions.Contains(ClassTypeCode.LanceLord);

      //  // Only add lord options if no lord class exists yet
      //  if (!hasLordClass)
      //  {
      //    reclassOptions.Add(ClassTypeCode.SwordLord);
      //    reclassOptions.Add(ClassTypeCode.AxeLord);
      //    reclassOptions.Add(ClassTypeCode.LanceLord);
      //  }
      //}

      // Add class-specific reclass options
      //if (CurrentClass.ReclassOptions != null)
      //{
      //  foreach (var option in CurrentClass.ReclassOptions)
      //  {
      //    if (!reclassOptions.Contains(option))
      //      reclassOptions.Add(option);
      //  }
      //}

      return reclassOptions;
    }
  }

  public class Condition()
  {
    public ConditionType CurrentCondition { get; set; }
    public Stats StatChange => GetStatChangeBasedOnCondtion();

    public Stats GetStatChangeBasedOnCondtion()
    {
      var statChange = new Stats();
      switch (CurrentCondition)
      {
        case ConditionType.Normal:
          break;
        case ConditionType.Serious:
          statChange.HP = -3;
          statChange.Str = -3;
          statChange.Mag = -3;
          statChange.Skl = -3;
          statChange.Spd = -3;
          statChange.Lck = -3;
          statChange.Def = -3;
          statChange.Res = -3;
          break;
        case ConditionType.Critical:
          statChange.HP = -6;
          statChange.Str = -6;
          statChange.Mag = -6;
          statChange.Skl = -6;
          statChange.Spd = -6;
          statChange.Lck = -6;
          statChange.Def = -6;
          statChange.Res = -6;
          break;
        case ConditionType.DEAD:
          statChange.HP = -99;
          statChange.Str = -99;
          statChange.Mag = -99;
          statChange.Skl = -99;
          statChange.Spd = -99;
          statChange.Lck = -99;
          statChange.Def = -99;
          statChange.Res = -99;
          break;
      }
      return statChange;
    }
  }
}
