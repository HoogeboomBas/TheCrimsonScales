using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

/// <summary>
/// An <see cref="Ability{T}"/> that allows a figure to create a trap of a specific kind in an empty hex
/// </summary>
public class CreateTrapAbility : Ability<CreateTrapAbility.State>
{
	public class State : AbilityState
	{
		public int AbilityRange { get; set; }
		public List<ConditionModel> AbilityConditionModels { get; set; }
		public List<Trap> CreatedTraps { get; set; } = [];

		public void AbilityAdjustRange(int amount)
		{
			AbilityRange += amount;
		}
	}

	public int Range { get; private set; } = 1;
	public int Damage { get; private set; }
	public int TrapCount { get; private set; } = 1;
	public string AssetPath = "res://Content/OverlayTiles/Traps/BearTrap1H.tscn";

	public ConditionModel[] ConditionModels { get; private set; } = [];
	public Action<State, List<Hex>> CustomSelectHexes { get; private set; } = null;
	public bool Mandatory = false;

	/// <summary>
	/// A builder extending <see cref="Ability{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in CreateTrapAbility. Enables inheritors of CreateTrapAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending CreateTrapAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : Ability<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IDamageStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : CreateTrapAbility, new()
	{
		public interface IDamageStep
		{
			TBuilder WithDamage(int damage);
		}

		public TBuilder WithDamage(int damage)
		{
			Obj.Damage = damage;
			return (TBuilder)this;
		}

		public TBuilder WithRange(int range)
		{
			Obj.Range = range;
			return (TBuilder)this;
		}

		public TBuilder WithTrapCount(int trapCount)
		{
			Obj.TrapCount = trapCount;
			return (TBuilder)this;
		}

		public TBuilder WithConditions(params ConditionModel[] conditionModels)
		{
			Obj.ConditionModels = conditionModels;
			return (TBuilder)this;
		}

		public TBuilder WithCustomSelectHexes(Action<State, List<Hex>> selectHexes)
		{
			Obj.CustomSelectHexes = selectHexes;
			return (TBuilder)this;
		}

		public TBuilder WithCustomAsset(string assetPath)
		{
			Obj.AssetPath = assetPath;
			return (TBuilder)this;
		}

		public TBuilder WithMandatory(bool mandatory)
		{
			Obj.Mandatory = mandatory;
			return (TBuilder)this;
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class CreateTrapBuilder : AbstractBuilder<CreateTrapBuilder, CreateTrapAbility>
	{
		internal CreateTrapBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of CreateTrapBuilder.
	/// </summary>
	/// <returns></returns>
	public static CreateTrapBuilder.IDamageStep Builder()
	{
		return new CreateTrapBuilder();
	}

	public CreateTrapAbility() { }

	protected override void InitializeState(State abilityState)
	{
		base.InitializeState(abilityState);

		abilityState.AbilityRange = Range;
		abilityState.AbilityConditionModels = ConditionModels.ToList();
	}

	protected override async GDTask Perform(State abilityState)
	{
		List<Hex> targetHexes = await AbilityCmd.SelectHexes(abilityState, list =>
			{
				if(CustomSelectHexes != null) 
				{
					CustomSelectHexes(abilityState, list);
				}
				else
				{
					list.AddRange(RangeHelper.GetHexesInRange(abilityState.Performer.Hex, abilityState.AbilityRange).Where(hex => hex.IsEmpty()));
				}
			}, 
			minSelectionCount: 0, 
			maxSelectionCount: TrapCount, 
			autoSelectIfMaxCountIsValidCount: false, 
			hintText: (TrapCount == 1) ? $"Select a hex to place the trap" : $"Select up to {TrapCount} hexes to place the traps");

		if(targetHexes.Count > 0)
		{
			foreach(Hex hex in targetHexes)
			{
				abilityState.CreatedTraps.Add(await AbilityCmd.CreateTrap(hex, AssetPath, damage: Damage, conditions: ConditionModels));
			}

			abilityState.SetPerformed();
		}
	}
}