using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

[Serializable]
public abstract class MonsterAbilityCardModel : AbstractModel<MonsterAbilityCardModel> //, IDeckCard
{
	public abstract string CardsAtlasPath { get; }

	public virtual bool Reshuffles => false;
	public virtual IEnumerable<MonsterAbilityCardElementInfusion> ElementInfusions { get; } = [];
	public virtual IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } = [];

	public bool RemoveAfterDraw => false;

	public abstract int Initiative { get; }
	public abstract int CardIndex { get; }

	public abstract IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster);

	protected MoveAbility MoveAbility(Monster monster, int extraDistance, MoveType moveType = MoveType.Regular)
	{
		if(!monster.Stats.Move.HasValue)
		{
			Log.Error("Trying to perform a move ability with a monster that does not move.");
			return null;
		}

		return global::MoveAbility.Builder()
			.WithDistance(monster.Stats.Move.Value + extraDistance)
			.WithMoveType(moveType)
			.Build();
	}

	protected AttackAbility AttackAbility(Monster monster,
		int? extraDamage, DynamicInt<AttackAbility.State>.GetValueDelegate dynamicValue = null, int extraRange = 0,
		int targets = 1, int? range = null, RangeType? rangeType = null, Target target = Target.Enemies,
		Hex targetHex = null, bool requiresLineOfSight = true,
		AOEPattern aoePattern = null, int push = 0, int pull = 0, int swing = 0, DynamicInt<AttackAbility.State> pierce = null,
		ConditionModel[] conditions = null,
		Action<AttackAbility.State, List<Figure>> customGetTargets = null,
		Ability<AttackAbility.State>.ConditionalAbilityCheckDelegate conditionalAbilityCheck = null,
		List<ScenarioEvents.AbilityStarted.Subscription> abilityStartedSubscriptions = null,
		List<ScenarioEvents.DuringAttack.Subscription> duringAttackSubscriptions = null,
		List<ScenarioEvents.AttackAfterTargetConfirmed.Subscription> afterTargetConfirmedSubscriptions = null,
		List<ScenarioEvents.AfterAttackPerformed.Subscription> afterAttackPerformedSubscriptions = null)
	{
		DynamicInt<AttackAbility.State> dynamicAttackValue =
			new DynamicInt<AttackAbility.State>(extraDamage.HasValue ? monster.Stats.Attack + extraDamage.Value : null, dynamicValue);
		//Monster monster = (Monster)parameters.Performer;
		int finalRange = range ?? ((monster.Stats.Range ?? 1) + extraRange);
		RangeType finalRangeType =
			rangeType ??
			(aoePattern != null && aoePattern.Hexes.Any(hex => hex.Type == AOEHexType.Gray)
				? RangeType.Melee
				: (finalRange > 1 ? RangeType.Range : monster.Stats.RangeType));

		return global::AttackAbility.Builder()
			.WithDamage(dynamicAttackValue) //extraDamage.HasValue ? monster.Stats.Attack + extraDamage.Value : null, getValue: getValue,
			.WithTargets(targets)
			.WithRange(finalRange)
			.WithRangeType(finalRangeType)
			.WithTarget(target)
			.WithTargetHex(targetHex)
			.WithRequiresLineOfSight(requiresLineOfSight)
			.WithAOEPattern(aoePattern)
			.WithPush(push)
			.WithPull(pull)
			.WithSwing(swing)
			.WithPierce(pierce ?? 0)
			.WithConditions(conditions ?? [])
			.WithCustomGetTargets(customGetTargets)
			.WithConditionalAbilityCheck(conditionalAbilityCheck)
			.WithAbilityStartedSubscriptions(abilityStartedSubscriptions)
			.WithDuringAttackSubscriptions(duringAttackSubscriptions)
			.WithAfterTargetConfirmedSubscriptions(afterTargetConfirmedSubscriptions)
			.WithAfterAttackPerformedSubscriptions(afterAttackPerformedSubscriptions)
			.Build();
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
			CardIndex, 3, 3,
			ResourceLoader.Load<Texture2D>(CardsAtlasPath));
	}
}