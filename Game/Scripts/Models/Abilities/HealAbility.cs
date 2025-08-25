using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

/// <summary>
/// A <see cref="TargetedAbility{T, TSingleTargetState}"/> that restores hit points to the target figure.
/// </summary>
public class HealAbility : TargetedAbility<HealAbility.State, HealAbility.HealAbilitySingleTargetState>
{
	public class HealAbilitySingleTargetState : SingleTargetState
	{
		public List<ConditionModel> RemovedConditions = new List<ConditionModel>();

		public void AddRemovedCondition(ConditionModel condition)
		{
			RemovedConditions.Add(condition);
		}
	}

	public class State : TargetedAbilityState<HealAbilitySingleTargetState>
	{
		public int AbilityHealValue { get; set; }

		public int SingleTargetHealValue { get; set; }

		public void AbilityAdjustHealValue(int amount)
		{
			AbilityHealValue += amount;

			SingleTargetHealValue += amount;
		}

		public void SingleTargetAdjustHealValue(int amount)
		{
			SingleTargetHealValue += amount;
		}
	}

	public DynamicInt<State> HealValue { get; protected set; }

	public List<ScenarioEvent<ScenarioEvents.DuringHeal.Parameters>.Subscription> DuringHealSubscriptions { get; protected set; } = [];
	public List<ScenarioEvent<ScenarioEvents.HealAfterTargetConfirmed.Parameters>.Subscription>
		AfterTargetConfirmedSubscriptions { get; protected set; } = [];
	public List<ScenarioEvent<ScenarioEvents.AfterHealPerformed.Parameters>.Subscription>
		AfterHealPerformedSubscriptions { get; protected set; } = [];

	/// <summary>
	/// A builder extending <see cref="TargetedAbility{T, TSingleTargetState}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in HealAbility. Enables inheritors of HealAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending HealAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> :
		TargetedAbility<State, HealAbilitySingleTargetState>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IHealValueStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : HealAbility, new()
	{
		public interface IHealValueStep
		{
			TBuilder WithHealValue(DynamicInt<State> healValue);
		}

		public TBuilder WithHealValue(DynamicInt<State> healValue)
		{
			Obj.HealValue = healValue;
			return (TBuilder)this;
		}

		public TBuilder WithDuringHealSubscription(ScenarioEvent<ScenarioEvents.DuringHeal.Parameters>.Subscription duringHealSubscription)
		{
			Obj.DuringHealSubscriptions.Add(duringHealSubscription);
			return (TBuilder)this;
		}

		public TBuilder WithDuringHealSubscriptions(List<ScenarioEvent<ScenarioEvents.DuringHeal.Parameters>.Subscription> duringHealSubscriptions)
		{
			Obj.DuringHealSubscriptions = duringHealSubscriptions;
			return (TBuilder)this;
		}

		public TBuilder WithAfterTargetConfirmedSubscription(
			ScenarioEvent<ScenarioEvents.HealAfterTargetConfirmed.Parameters>.Subscription afterTargetConfirmedSubscription)
		{
			Obj.AfterTargetConfirmedSubscriptions.Add(afterTargetConfirmedSubscription);
			return (TBuilder)this;
		}

		public TBuilder WithAfterTargetConfirmedSubscriptions(
			List<ScenarioEvent<ScenarioEvents.HealAfterTargetConfirmed.Parameters>.Subscription> afterTargetConfirmedSubscriptions)
		{
			Obj.AfterTargetConfirmedSubscriptions = afterTargetConfirmedSubscriptions;
			return (TBuilder)this;
		}

		public TBuilder WithAfterHealPerformedSubscription(
			ScenarioEvent<ScenarioEvents.AfterHealPerformed.Parameters>.Subscription afterHealPerformedSubscriptions)
		{
			Obj.AfterHealPerformedSubscriptions.Add(afterHealPerformedSubscriptions);
			return (TBuilder)this;
		}

		public TBuilder WithAfterHealPerformedSubscriptions(
			List<ScenarioEvent<ScenarioEvents.AfterHealPerformed.Parameters>.Subscription> afterHealPerformedSubscriptionss)
		{
			Obj.AfterHealPerformedSubscriptions = afterHealPerformedSubscriptionss;
			return (TBuilder)this;
		}

		/// <summary>
		/// Overriding so we can set default values.
		/// </summary>
		public override TAbility Build()
		{
			Obj.Target = _target ?? Target.SelfOrAllies;
			return base.Build();
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class HealBuilder : AbstractBuilder<HealBuilder, HealAbility>
	{
		internal HealBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of HealBuilder.
	/// </summary>
	/// <returns></returns>
	public static AbstractBuilder<HealBuilder, HealAbility>.IHealValueStep Builder()
	{
		return new HealBuilder();
	}

	public HealAbility() { }

	public HealAbility(DynamicInt<State> value, int targets = 1, int? range = null, RangeType? rangeType = null,
		Target target = Target.SelfOrAllies,
		bool requiresLineOfSight = true, bool mandatory = false,
		Hex targetHex = null,
		AOEPattern aoePattern = null, int push = 0, int pull = 0, ConditionModel[] conditions = null,
		Action<State, List<Figure>> customGetTargets = null,
		Func<State, GDTask> onAbilityStarted = null, Func<State, GDTask> onAbilityEnded = null, Func<State, GDTask> onAbilityEndedPerformed = null,
		ConditionalAbilityCheckDelegate conditionalAbilityCheck = null,
		Func<State, string> getTargetingHintText = null,
		List<ScenarioEvents.DuringHeal.Subscription> duringHealSubscriptions = null,
		List<ScenarioEvents.HealAfterTargetConfirmed.Subscription> afterTargetConfirmedSubscriptions = null,
		List<ScenarioEvents.AfterHealPerformed.Subscription> afterHealPerformedSubscriptions = null,
		List<ScenarioEvents.AbilityStarted.Subscription> abilityStartedSubscriptions = null,
		List<ScenarioEvents.AbilityEnded.Subscription> abilityEndedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> abilityPerformedSubscriptions = null)
		: base(targets, range, rangeType, target,
			requiresLineOfSight, mandatory, targetHex, aoePattern, push, pull, conditions,
			customGetTargets, onAbilityStarted, onAbilityEnded, onAbilityEndedPerformed,
			conditionalAbilityCheck, getTargetingHintText, abilityStartedSubscriptions, abilityEndedSubscriptions, abilityPerformedSubscriptions)
	{
		HealValue = value;
		DuringHealSubscriptions = duringHealSubscriptions;
		AfterTargetConfirmedSubscriptions = afterTargetConfirmedSubscriptions;
		AfterHealPerformedSubscriptions = afterHealPerformedSubscriptions;
	}

	protected override void InitializeState(State abilityState)
	{
		base.InitializeState(abilityState);

		abilityState.AbilityHealValue = HealValue.GetValue(abilityState);
	}

	protected override async GDTask StartPerform(State abilityState)
	{
		await base.StartPerform(abilityState);

		ScenarioEvents.DuringHealEvent.Subscribe(abilityState, this, DuringHealSubscriptions);
		ScenarioEvents.HealAfterTargetConfirmedEvent.Subscribe(abilityState, this, AfterTargetConfirmedSubscriptions);
		ScenarioEvents.AfterHealPerformedEvent.Subscribe(abilityState, this, AfterHealPerformedSubscriptions);
	}

	protected override async GDTask EndPerform(State abilityState)
	{
		await base.EndPerform(abilityState);

		ScenarioEvents.DuringHealEvent.Unsubscribe(DuringHealSubscriptions);
		ScenarioEvents.HealAfterTargetConfirmedEvent.Unsubscribe(AfterTargetConfirmedSubscriptions);
		ScenarioEvents.AfterHealPerformedEvent.Unsubscribe(AfterHealPerformedSubscriptions);
	}

	// protected override async GDTask InitAbilityState(State abilityState)
	// {
	// 	await base.InitAbilityState(abilityState);
	//
	// 	abilityState.AbilityHealValue = HealValue.GetValue(abilityState);
	// }

	protected override void InitAbilityStateForSingleTarget(State abilityState)
	{
		base.InitAbilityStateForSingleTarget(abilityState);

		abilityState.SingleTargetHealValue = abilityState.AbilityHealValue;
	}

	protected override EffectCollection CreateDuringTargetedAbilityEffectCollection(State abilityState)
	{
		return ScenarioEvents.DuringHealEvent.CreateEffectCollection(new ScenarioEvents.DuringHeal.Parameters(abilityState));
	}

	// protected override void SyncDuringTargetedAbilityParameters(State abilityState, ScenarioEvents.DuringTargetedAbilityParametersBase<State> abilityStateParameters)
	// {
	// 	base.SyncDuringTargetedAbilityParameters(abilityState, abilityStateParameters);
	//
	// 	ScenarioEvents.DuringHeal.Parameters castAbilityStateParameters = (ScenarioEvents.DuringHeal.Parameters)abilityStateParameters;
	// 	abilityState.AbilityHealValue = castAbilityStateParameters.HealValue;
	// }

	protected override async GDTask AfterTargetConfirmedBeforeConditionsApplied(State abilityState, Figure target)
	{
		await ScenarioEvents.HealAfterTargetConfirmedEvent.CreatePrompt(
			new ScenarioEvents.HealAfterTargetConfirmed.Parameters(abilityState), abilityState);

		ScenarioEvents.HealBlockTime.Parameters blockedAbilityStateParameters =
			await ScenarioEvents.HealBlockTimeEvent.CreatePrompt(
				new ScenarioEvents.HealBlockTime.Parameters(abilityState), abilityState);

		if(!blockedAbilityStateParameters.IsBlocked)
		{
			AppController.Instance.AudioController.PlayFastForwardable(SFX.Heal, delay: 0.0f);

			int newHealth = Mathf.Min(target.Health + abilityState.SingleTargetHealValue, target.MaxHealth);

			target.SetHealth(newHealth);
		}

		for(int i = target.Conditions.Count - 1; i >= 0; i--)
		{
			ConditionModel condition = target.Conditions[i];
			if(condition.RemovedByHeal)
			{
				await AbilityCmd.RemoveCondition(target, condition);
				abilityState.SingleTargetState.AddRemovedCondition(condition);
			}
		}

		await ScenarioEvents.AfterHealPerformedEvent.CreatePrompt(
			new ScenarioEvents.AfterHealPerformed.Parameters(abilityState, blockedAbilityStateParameters.IsBlocked), abilityState);
	}

	protected override string DefaultTargetingHintText(State abilityState)
	{
		if(Target == Target.Self)
		{
			return $"Perform {Icons.HintText(Icons.Heal)}{abilityState.AbilityHealValue} self?";
		}

		return $"Select a target for {Icons.HintText(Icons.Heal)}{abilityState.SingleTargetHealValue}";
	}
}