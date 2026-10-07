using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

/// <summary>
/// An <see cref="Ability{T}"/> that allows a figure to pick up loot tokens (coins and treasure chests) within range.
/// </summary>
public class LootAbility : TargetedAbility<LootAbility.State, SingleTargetState>
{
	public class State : TargetedAbilityState<SingleTargetState>
	{
		public List<LootableObject> LootedObjects { get; } = new List<LootableObject>();
		public List<Hex> LootedHexes { get; } = new List<Hex>();
		public int LootedCoinCount { get; set; }
		public int TotalLootedCount { get; set; }
		//public Hex AbilityPerformHex { get; set; }
		//public Hex GetPerformHex => AbilityPerformHex ?? Performer.Hex;

		//public async GDTask SetPerform
	}

	private Func<State, Figure> _customGetLootObtainer { get; set; }
	//public int Range { get; protected set; }

	/// <summary>
	/// A builder extending <see cref="Ability{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in LootAbility. Enables inheritors of LootAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending LootAbility.
	public new class AbstractBuilder<TBuilder, TAbility> : TargetedAbility<State, SingleTargetState>.AbstractBuilder<TBuilder, TAbility>

		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : LootAbility, new()
	{
		// public interface IRangeStep
		// {
		// 	TBuilder WithRange(int range);
		// }

		public TBuilder WithCustomGetLootObtainer(Func<State, Figure> customGetLootObtainer)
		{
			Obj._customGetLootObtainer = customGetLootObtainer;
			return (TBuilder)this;
		}

		// public TBuilder WithRange(int range)
		// {
		// 	Obj.Range = range;
		// 	return (TBuilder)this;
		// }
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class LootBuilder : AbstractBuilder<LootBuilder, LootAbility>
	{
		internal LootBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of LootBuilder.
	/// </summary>
	/// <returns></returns>
	public static LootBuilder Builder()
	{
		return new LootBuilder();
	}

	public LootAbility() { }

	protected override async GDTask Perform(State abilityState)
	{
		Figure lootObtainer = abilityState.Performer;

		if(_customGetLootObtainer != null)
		{
			lootObtainer = _customGetLootObtainer(abilityState);
		}

		LootPrompt.Answer confirmAnswer = await PromptManager.Prompt(new LootPrompt(list =>
		{
			foreach(Hex hex in RangeHelper.GetHexesInRange(abilityState.GetPerformHex, abilityState.AbilityRange))
			{
				foreach(HexObject hexObject in hex.HexObjects)
				{
					if(hexObject is LootableObject lootableObject && lootableObject.CanLoot(lootObtainer))
					{
						list.AddIfNew(hex);
					}
				}
			}
		}, null, () => "Perform loot?"), abilityState.Authority);

		if(!confirmAnswer.Skipped)
		{
			foreach(Vector2I coords in confirmAnswer.HexCoords)
			{
				Hex hex = GameController.Instance.Map.GetHex(coords);
				abilityState.LootedHexes.Add(hex);

				await LootHex(abilityState, hex, lootObtainer);
			}

			abilityState.SetPerformed();
		}
	}

	public static async GDTask LootHex(State abilityState, Hex hex, Figure lootObtainer)
	{
		for(int i = hex.HexObjects.Count - 1; i >= 0; i--)
		{
			HexObject hexObject = hex.HexObjects[i];
			if(hexObject is LootableObject lootableObject && lootableObject.CanLoot(lootObtainer))
			{
				abilityState.LootedObjects.Add(lootableObject);
				if(lootableObject is Coin)
				{
					abilityState.LootedCoinCount++;
					abilityState.TotalLootedCount++;
				}
				else
				{
					abilityState.TotalLootedCount++;
				}

				await lootableObject.Loot(lootObtainer);

				abilityState.SetPerformed();
			}
		}
	}
}