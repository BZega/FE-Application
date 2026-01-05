using Fire_Emblem.Common.TypeCodes;

namespace Fire_Emblem.Common.Models
{
  /// <summary>
  /// Base class for all combat units (Characters and Enemies)
  /// </summary>
  public abstract class CombatUnit
  {
    // Common Properties
    public int Level { get; set; } = 1;
    public int CumulativeLevel { get; set; } = 0;
    public int CurrentHP { get; set; }
    public Equipment EquippedWeapon { get; set; }
    public UnitClass CurrentClass { get; set; }
    public List<Ability> EquippedAbilities { get; set; }
    public Terrain Terrain { get; set; }
    public List<UnitType> UnitTypes { get; set; }

    // Combat State
    public bool IsAttacking { get; set; } = false;
    public bool IsInCombat { get; set; } = false;
    public bool IsWeaponTriangleAdvantage { get; set; } = false;
    public bool IsWeaponTriangleDisadvantage { get; set; } = false;

    // Calculated Properties - Must be implemented by derived classes
    public abstract Stats CurrentStats { get; }

    // Common Calculated Properties
    public virtual int InternalLevel => GetInternalLevel();
    public virtual bool DealsEffectiveDamage => CheckForEffectiveDamage();
    public virtual List<UnitType> EffectiveDamageUnitTypes => CheckEffectiveType();
    public virtual int Attack => GetAttack();
    public virtual int Damage => GetDamage();
    public virtual int Crit => GetCrit();
    public virtual int Avoid => GetAvoid();
    public virtual int Dodge => GetDodge();
    public virtual int AttackSpeed => GetAttackSpeed();
    public virtual int DamageReceived => GetDamageReceived();

    // Common Combat Methods
    protected virtual int GetAttack()
    {
      var attack = 0;
      attack += (CurrentStats.Skl * 3 + CurrentStats.Lck) / 2;

      if (EquippedWeapon != null && EquippedWeapon.Hit != null)
      {
        attack += EquippedWeapon.Hit.Value;
        if (EquippedWeapon.StatBonus?.Attributes != null)
        {
          attack += EquippedWeapon.StatBonus.Attributes.Hit;
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

      if (CurrentClass.InnateBonus?.Attributes != null)
      {
        attack += CurrentClass.InnateBonus.Attributes.Hit;
      }

      return attack;
    }

    protected virtual int GetDamage()
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

      return damage;
    }

    protected virtual int GetCrit()
    {
      var crit = 0;
      crit += CurrentStats.Skl / 2;

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

      if (CurrentClass.InnateBonus?.Attributes != null)
      {
        crit += CurrentClass.InnateBonus.Attributes.Crit;
      }

      return crit;
    }

    protected virtual int GetAvoid()
    {
      var avoid = 0;
      avoid += (CurrentStats.Spd * 3 + CurrentStats.Lck) / 2;

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

      return avoid;
    }

    protected virtual int GetDodge()
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

      if (CurrentClass.InnateBonus?.Attributes != null)
      {
        dodge += CurrentClass.InnateBonus.Attributes.Dodge;
      }

      return dodge;
    }

    protected virtual int GetDamageReceived()
    {
      var damageReceived = 0;

      if (EquippedAbilities != null)
      {
        foreach (var ability in EquippedAbilities)
        {
          if (ability.StatBonus?.Attributes?.AttackSpeed != null && ability.StatBonus?.Attributes?.AttackSpeed != 0)
          {
            damageReceived += ability.StatBonus.Attributes.AttackSpeed;
          }
        }
      }

      if (EquippedWeapon?.StatBonus?.Attributes?.AttackSpeed != null && EquippedWeapon?.StatBonus?.Attributes?.AttackSpeed != 0)
      {
        damageReceived += EquippedWeapon.StatBonus.Attributes.AttackSpeed;
      }

      return damageReceived;
    }

    protected virtual int GetAttackSpeed()
    {
      var attackSpeed = 0;

      if (EquippedAbilities != null)
      {
        foreach (var ability in EquippedAbilities)
        {
          if (ability.StatBonus?.Attributes?.AttackSpeed != null && ability.StatBonus?.Attributes?.AttackSpeed != 0)
          {
            attackSpeed += ability.StatBonus.Attributes.AttackSpeed;
          }
        }
      }

      if (EquippedWeapon?.StatBonus?.Attributes?.AttackSpeed != null && EquippedWeapon?.StatBonus?.Attributes?.AttackSpeed != 0)
      {
        attackSpeed += EquippedWeapon.StatBonus.Attributes.AttackSpeed;
      }

      return attackSpeed;
    }

    public virtual StatBonus GetHitWeaponTriangleDisadvantage(Rank advantageWeaponRank)
    {
      var attribute = new StatBonus() { Attributes = new Attributes() };
      var hit = 0;
      var damage = 0;

      switch (advantageWeaponRank)
      {
        case Rank.E:
        case Rank.D:
          hit -= 5;
          break;
        case Rank.C:
        case Rank.B:
          hit -= 10;
          break;
        case Rank.A:
        case Rank.S:
          hit -= 15;
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

    public virtual bool CheckForEffectiveDamage()
    {
      var effective = false;

      if (EquippedAbilities != null && EquippedAbilities.Count > 0)
      {
        foreach (var ability in EquippedAbilities)
        {
          if (ability.Name == "Beastbane" || ability.Name == "Wyrmsbane" || ability.Name == "Golembane")
          {
            effective = true;
          }
        }
      }

      if (EquippedWeapon?.DoesEffectiveDamage == true)
      {
        effective = true;
      }

      return effective;
    }

    public virtual List<UnitType> CheckEffectiveType()
    {
      var units = new HashSet<UnitType>();

      if (EquippedAbilities != null && EquippedAbilities.Count > 0)
      {
        foreach (var ability in EquippedAbilities)
        {
          switch (ability.Name)
          {
            case "Beastbane":
              units.Add(UnitType.Beast);
              break;
            case "Wyrmsbane":
              units.Add(UnitType.Dragon);
              break;
            case "Golembane":
              units.Add(UnitType.Mechanists);
              units.Add(UnitType.Puppets);
              units.Add(UnitType.Golems);
              break;
          }
        }
      }

      if (EquippedWeapon != null && EquippedWeapon.DoesEffectiveDamage && EquippedWeapon.EffectiveUnitTypes.Any())
      {
        foreach (var unitType in EquippedWeapon.EffectiveUnitTypes)
        {
          units.Add(unitType);
        }
      }

      return units.ToList();
    }

    public virtual int GetInternalLevel()
    {
      var promotionBonus = CurrentClass.IsPromoted ? 20 : 0;
      var internalLevel = Level + promotionBonus + CumulativeLevel;
      return internalLevel;
    }
  }
}
