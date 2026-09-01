using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

[Serializable]
public abstract class MonsterAbilityCardModel : AbstractModel //, IDeckCard
{
	public abstract string CardsAtlasPath { get; }
	public virtual int ColumnCount => 3;
	public virtual int RowCount => 3;

	public virtual bool Reshuffles => false;
	public virtual IEnumerable<CardElementInfusion> ElementInfusions { get; } = [];
	public virtual IEnumerable<CardElementConsumption> ElementConsumptions { get; } = [];

	public bool RemoveAfterDraw => false;

	public abstract int Initiative { get; }
	public abstract int CardIndex { get; }

	public abstract IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster);

	public static MoveAbility.MoveBuilder MoveAbility(Monster monster, int extraDistance)
	{
		if(!monster.Stats.Move.HasValue)
		{
			Log.Error("Trying to perform a move ability with a monster that does not move.");
			return null;
		}

		return global::MoveAbility.Builder().WithDistance(monster.Stats.Move.Value + extraDistance);
	}

	public static AttackAbility.AttackBuilder AttackAbility(Monster monster, DynamicInt<AttackAbility.State> extraDamage,
		DynamicInt extraRange = null)
	{
		DynamicInt<AttackAbility.State> dynamicAttackValue =
			new DynamicInt<AttackAbility.State>(state => monster.Stats.Attack + extraDamage.GetValue(state));

		int defaultRange = monster.Stats.Range ?? 1;
		DynamicInt dynamicRange = new DynamicInt(() => defaultRange + (extraRange?.GetValue() ?? 0));

		return global::AttackAbility.Builder()
			.WithDamage(dynamicAttackValue)
			.WithRange(dynamicRange);
	}

	protected DynamicInt<TState> ConsumeElementDynamicValue<TState>(IReadOnlyCollection<Element> possibleElements, int normalValue, int consumedValue)
		where TState : AbilityState, new()
	{
		//CheckOrRegisterElementConsumption(possibleElements);

		return new DynamicInt<TState>(state =>
		{
			return CheckElementConsumed(state, possibleElements) ? consumedValue : normalValue;
		});
	}

	protected Ability<TState>.ConditionalAbilityCheckDelegate ConsumeElementAbilityCheck<TState>(IReadOnlyCollection<Element> possibleElements)
		where TState : AbilityState, new()
	{
		//CheckOrRegisterElementConsumption(possibleElements);

		return async state =>
		{
			await GDTask.CompletedTask;

			return CheckElementConsumed(state, possibleElements);
		};
	}

	public static ScenarioEvent<T>.Subscription ConsumeElementCheckSubscription<T>(Monster monster, IReadOnlyCollection<Element> possibleElements,
		ScenarioEvent<T>.CanApplyFunction canApplyFunction = null, ScenarioEvent<T>.ApplyFunction applyFunction = null,
		EffectType effectType = EffectType.MandatoryBeforeOptionals, int order = 0, bool canApplyMultipleTimes = false)
		where T : ScenarioEvent.ParametersBase
	{
		return ScenarioEvent<T>.Subscription.New(parameters =>
			{
				if(!CheckElementConsumed(monster, possibleElements))
				{
					return false;
				}

				return canApplyFunction == null || canApplyFunction.Invoke(parameters);
			},
			async parameters =>
			{
				if(applyFunction != null)
				{
					await applyFunction.Invoke(parameters);
				}
			}, effectType, order, canApplyMultipleTimes
		);
	}

	// private void CheckOrRegisterElementConsumption(IReadOnlyCollection<Element> possibleElements)
	// {
	// 	bool consumptionAlreadyRegistered = false;
	// 	foreach(MonsterAbilityCardElementConsumption elementConsumption in ElementConsumptions)
	// 	{
	// 		bool sameElements = true;
	// 		foreach(Element consumableElement in elementConsumption.ConsumableElements)
	// 		{
	// 			if(!possibleElements.Contains(consumableElement))
	// 			{
	// 				sameElements = false;
	// 			}
	// 		}
	//
	// 		if(sameElements && possibleElements.Count == elementConsumption.ConsumableElements.Count)
	// 		{
	// 			consumptionAlreadyRegistered = true;
	// 			break;
	// 		}
	// 	}
	//
	// 	if(!consumptionAlreadyRegistered)
	// 	{
	// 		ElementConsumptions.Add(new MonsterAbilityCardElementConsumption(possibleElements));
	// 	}
	// }

	protected static bool CheckElementConsumed<TState>(TState state, IReadOnlyCollection<Element> possibleElements)
		where TState : AbilityState, new()
	{
		if(state.Performer is not Monster monster)
		{
			return false;
		}

		return CheckElementConsumed(monster, possibleElements);
	}

	protected static bool CheckElementConsumed(Monster monster, IReadOnlyCollection<Element> possibleElements)
	{
		foreach(Element element in possibleElements)
		{
			if(monster.MonsterGroup.AbilityCardConsumedElements.Contains(element))
			{
				return true;
			}
		}

		return false;
	}

	public Texture2D GetTexture()
	{
		return AtlasTextureHelper.CreateAtlasTexture(
			CardIndex, ColumnCount, RowCount,
			ResourceLoader.Load<Texture2D>(CardsAtlasPath));
	}
}