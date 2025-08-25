using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

/// <summary>
/// An <see cref="Ability{T}"/> that picks up coins from hexes within range.
/// </summary>
public class LootAbility : Ability<LootAbility.State>
{
	public class State : AbilityState
	{
		public List<LootableObject> LootedObjects { get; } = new List<LootableObject>();
		public int LootedCoinCount { get; set; }
		public int TotalLootedCount { get; set; }
	}

	private Func<State, Figure> _customGetLootObtainer { get; set; }
	// TODO varadski 20.08.2025: Common ability between loot and TargetedAbility; move it to Ability.cs?
	public int Range { get; protected set; }

	/// <summary>
	/// A builder extending <see cref="Ability{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in LootAbility. Enables inheritors of LootAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending LootAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : Ability<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IRangeStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : LootAbility, new()
	{

		public interface IRangeStep
		{
			TBuilder WithRange(int range);
		}

		public TBuilder WithCustomGetLootObtainer(Func<State, Figure> customGetLootObtainer)
		{
			Obj._customGetLootObtainer = customGetLootObtainer;
			return (TBuilder)this;
		}

		public TBuilder WithRange(int range)
		{
			Obj.Range = range;
			return (TBuilder)this;
		}
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
	public static AbstractBuilder<LootBuilder, LootAbility>.IRangeStep Builder()
	{
		return new LootBuilder();
	}


	public LootAbility() { }

	public LootAbility(int range, Func<State, Figure> customGetLootObtainer = null,
		Func<State, GDTask> onAbilityStarted = null, Func<State, GDTask> onAbilityEnded = null,
		Func<State, GDTask> onAbilityEndedPerformed = null,
		ConditionalAbilityCheckDelegate conditionalAbilityCheck = null,
		List<ScenarioEvent<ScenarioEvents.AbilityStarted.Parameters>.Subscription> abilityStartedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityEnded.Parameters>.Subscription> abilityEndedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> abilityPerformedSubscriptions = null)
		: base(onAbilityStarted, onAbilityEnded, onAbilityEndedPerformed, conditionalAbilityCheck,
			abilityStartedSubscriptions, abilityEndedSubscriptions, abilityPerformedSubscriptions)
	{
		_customGetLootObtainer = customGetLootObtainer;
		Range = range;
	}

	protected override async GDTask Perform(State abilityState)
	{
		Figure lootObtainer = abilityState.Performer;

		if(_customGetLootObtainer != null)
		{
			lootObtainer = _customGetLootObtainer(abilityState);
		}

		LootPrompt.Answer confirmAnswer = await PromptManager.Prompt(new LootPrompt(list =>
		{
			foreach(Hex hex in RangeHelper.GetHexesInRange(abilityState.Performer.Hex, Range))
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
				if(lootableObject is CoinStack coinStack)
				{
					abilityState.LootedCoinCount += coinStack.CoinCount;
					abilityState.TotalLootedCount += coinStack.CoinCount;
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