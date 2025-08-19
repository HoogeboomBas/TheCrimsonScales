using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

public class AttackAbility : TargetedAbility<AttackAbility.State, SingleTargetState>
{
	public class State : TargetedAbilityState<SingleTargetState>
	{
		public List<Figure> KilledTargets { get; } = new List<Figure>();

		public int AbilityAttackValue { get; set; }
		public int AbilityPierce { get; set; }
		public bool AbilityHasAdvantage { get; set; }
		public bool AbilityHasDisadvantage { get; set; }

		public int SingleTargetAttackValue { get; set; }
		public int SingleTargetPierce { get; set; }
		public bool SingleTargetHasAdvantage { get; set; }
		public bool SingleTargetHasDisadvantage { get; set; }

		public void AbilityAdjustAttackValue(int amount)
		{
			AbilityAttackValue += amount;

			SingleTargetAttackValue += amount;
		}

		public void AbilityAdjustPierce(int amount)
		{
			AbilityPierce += amount;

			SingleTargetPierce += amount;
		}

		public void AbilitySetHasAdvantage()
		{
			AbilityHasAdvantage = true;

			SingleTargetHasAdvantage = true;
		}

		public void AbilitySetHasDisadvantage()
		{
			AbilityHasDisadvantage = true;

			SingleTargetHasDisadvantage = true;
		}

		public void SingleTargetAdjustAttackValue(int amount)
		{
			SingleTargetAttackValue += amount;
		}

		public void SingleTargetAdjustPierce(int amount)
		{
			SingleTargetPierce += amount;
		}

		public void SingleTargetSetHasAdvantage()
		{
			SingleTargetHasAdvantage = true;
		}

		public void SingleTargetSetHasDisadvantage()
		{
			SingleTargetHasDisadvantage = true;
		}
	}

	public DynamicInt<State> Damage { get; }
	public DynamicInt<State> Pierce { get; }
	public bool HasAdvantage { get; }
	public bool HasDisadvantage { get; }

	public List<ScenarioEvents.DuringAttack.Subscription> DuringAttackSubscriptions { get; }
	public List<ScenarioEvents.AttackAfterTargetConfirmed.Subscription> AfterTargetConfirmedSubscriptions { get; }
	public List<ScenarioEvents.AfterAttackPerformed.Subscription> AfterAttackPerformedSubscriptions { get; }

	public AttackAbility(DynamicInt<State> value, int targets = 1, int? range = null, RangeType? rangeType = null,
		Target target = Target.Enemies,
		bool requiresLineOfSight = true, bool mandatory = false,
		Hex targetHex = null,
		AOEPattern aoePattern = null, int push = 0, int pull = 0, DynamicInt<State> pierce = null, ConditionModel[] conditions = null,
		bool hasAdvantage = false, bool hasDisadvantage = false,
		Action<State, List<Figure>> customGetTargets = null,
		Func<State, GDTask> onAbilityStarted = null, Func<State, GDTask> onAbilityEnded = null, Func<State, GDTask> onAbilityEndedPerformed = null,
		ConditionalAbilityCheckDelegate conditionalAbilityCheck = null,
		Func<State, string> getTargetingHintText = null,
		List<ScenarioEvents.DuringAttack.Subscription> duringAttackSubscriptions = null,
		List<ScenarioEvents.AttackAfterTargetConfirmed.Subscription> afterTargetConfirmedSubscriptions = null,
		List<ScenarioEvents.AfterAttackPerformed.Subscription> afterAttackPerformedSubscriptions = null,
		List<ScenarioEvents.AbilityStarted.Subscription> abilityStartedSubscriptions = null,
		List<ScenarioEvents.AbilityEnded.Subscription> abilityEndedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> abilityPerformedSubscriptions = null)
		: base(targets, range, rangeType, target,
			requiresLineOfSight, mandatory, targetHex, aoePattern, push, pull, conditions,
			customGetTargets, onAbilityStarted, onAbilityEnded, onAbilityEndedPerformed,
			conditionalAbilityCheck, getTargetingHintText, abilityStartedSubscriptions, abilityEndedSubscriptions, abilityPerformedSubscriptions)
	{
		Damage = value;
		Pierce = pierce ?? 0;
		HasAdvantage = hasAdvantage;
		HasDisadvantage = hasDisadvantage;

		DuringAttackSubscriptions = duringAttackSubscriptions;
		AfterTargetConfirmedSubscriptions = afterTargetConfirmedSubscriptions;
		AfterAttackPerformedSubscriptions = afterAttackPerformedSubscriptions;
	}

	protected override void InitializeState(State abilityState)
	{
		base.InitializeState(abilityState);

		abilityState.AbilityAttackValue = Damage.GetValue(abilityState);
		abilityState.AbilityPierce = Pierce.GetValue(abilityState);
		abilityState.AbilityHasAdvantage = HasAdvantage;
		abilityState.AbilityHasDisadvantage = HasDisadvantage;
	}

	protected override async GDTask StartPerform(State abilityState)
	{
		await base.StartPerform(abilityState);

		ScenarioEvents.DuringAttackEvent.Subscribe(abilityState, this, DuringAttackSubscriptions);
		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(abilityState, this, AfterTargetConfirmedSubscriptions);
		ScenarioEvents.AfterAttackPerformedEvent.Subscribe(abilityState, this, AfterAttackPerformedSubscriptions);
	}

	protected override async GDTask EndPerform(State abilityState)
	{
		await base.EndPerform(abilityState);

		ScenarioEvents.DuringAttackEvent.Unsubscribe(DuringAttackSubscriptions);
		ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(AfterTargetConfirmedSubscriptions);
		ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(AfterAttackPerformedSubscriptions);
	}

	// protected override async GDTask InitAbilityState(State abilityState)
	// {
	// 	await base.InitAbilityState(abilityState);
	//
	// 	abilityState.AbilityAttackValue = Damage.GetValue(abilityState);
	// 	abilityState.AbilityPierce = Pierce.GetValue(abilityState);
	// 	abilityState.AbilityHasAdvantage = HasAdvantage;
	// 	abilityState.AbilityHasDisadvantage = HasDisadvantage;
	//
	// 	await ScenarioEvents.AttackAbilityStartEvent.CreatePrompt(
	// 		new ScenarioEvents.AttackAbilityStart.Parameters(abilityState), abilityState);
	// }

	protected override void InitAbilityStateForSingleTarget(State abilityState)
	{
		base.InitAbilityStateForSingleTarget(abilityState);

		abilityState.SingleTargetAttackValue = abilityState.AbilityAttackValue;
		abilityState.SingleTargetPierce = abilityState.AbilityPierce;
		abilityState.SingleTargetHasAdvantage = abilityState.AbilityHasAdvantage;
		abilityState.SingleTargetHasDisadvantage = abilityState.AbilityHasDisadvantage;
	}

	protected override EffectCollection CreateDuringTargetedAbilityEffectCollection(State abilityState)
	{
		return ScenarioEvents.DuringAttackEvent.CreateEffectCollection(new ScenarioEvents.DuringAttack.Parameters(abilityState));
	}

	protected override async GDTask AfterTargetConfirmedBeforeConditionsApplied(State abilityState, Figure target)
	{
		bool rangeDisadvantage = abilityState.SingleTargetRangeType == RangeType.Range && RangeHelper.Distance(abilityState.Performer.Hex, target.Hex) == 1;
		if(rangeDisadvantage)
		{
			abilityState.SingleTargetSetHasDisadvantage();
		}

		await ScenarioEvents.AttackAfterTargetConfirmedEvent.CreatePrompt(
			new ScenarioEvents.AttackAfterTargetConfirmed.Parameters(abilityState), abilityState);

		AMDCard terminal = await GameController.Instance.AMDDrawView.DrawCards(abilityState);

		int finalDamage = await AbilityCmd.SufferDamage(abilityState, target, abilityState.SingleTargetAttackValue);

		if(!GameController.FastForward)
		{
			Figure performer = abilityState.Performer;
			Vector2 performerOrigin = performer.GlobalPosition;
			Vector2 targetOrigin = target.GlobalPosition;
			Vector2 normal = (targetOrigin - performerOrigin).Normalized();

			GTweenSequenceBuilder.New()
				.Append(performer.TweenGlobalPosition(performerOrigin - normal * Map.HexSize * 0.1f, 0.1f))
				.Append(performer.TweenGlobalPosition(performerOrigin + normal * Map.HexSize * 0.2f, 0.15f).SetEasing(Easing.OutQuart))
				.Append(performer.TweenGlobalPosition(performerOrigin, 0.2f).SetEasing(Easing.InOutQuad))
				.Build().PlayFastForwardable();

			if(abilityState.SingleTargetRangeType == RangeType.Melee)
			{
				if(finalDamage > 0)
				{
					AppController.Instance.AudioController.PlayFastForwardable(SFX.GetSwordHit(), delay: 0.2f);
				}
				else
				{
					AppController.Instance.AudioController.PlayFastForwardable(SFX.GetSwordBlocked(), delay: 0.2f);
				}
			}
			else
			{
				AppController.Instance.AudioController.PlayFastForwardable(SFX.GetBowAttack(), delay: 0f);

				if(finalDamage > 0)
				{
					AppController.Instance.AudioController.PlayFastForwardable(SFX.GetBowHit(), delay: 0.2f);
				}
				else
				{
					AppController.Instance.AudioController.PlayFastForwardable(SFX.GetBowBlocked(), delay: 0.2f);
				}
			}

			if(finalDamage > 0)
			{
				GTweenSequenceBuilder.New()
					.AppendTime(0.25f)
					.Append(target.TweenGlobalPosition(targetOrigin + normal * Map.HexSize * 0.2f, 0.15f).SetEasing(Easing.OutQuart))
					.Append(target.TweenGlobalPosition(targetOrigin, 0.2f).SetEasing(Easing.OutBack))
					.Build().PlayFastForwardable();
			}

			await GDTask.DelayFastForwardable(0.6f);
		}

		await ScenarioEvents.AMDTerminalDrawnEvent.CreatePrompt(
			new ScenarioEvents.AMDTerminalDrawn.Parameters(abilityState, terminal), abilityState);

		if(target.IsDead)
		{
			abilityState.KilledTargets.Add(target);
		}
	}

	protected override async GDTask AfterEffects(State abilityState, Figure target)
	{
		ScenarioEvents.Retaliate.Parameters retaliateParameters =
			await ScenarioEvents.RetaliateEvent.CreatePrompt(
				new ScenarioEvents.Retaliate.Parameters(abilityState, target), abilityState);

		if(!retaliateParameters.RetaliateBlocked && retaliateParameters.Retaliate > 0)
		{
			await AbilityCmd.SufferDamage(null, abilityState.Performer, retaliateParameters.Retaliate);
		}

		await ScenarioEvents.AfterAttackPerformedEvent.CreatePrompt(
			new ScenarioEvents.AfterAttackPerformed.Parameters(abilityState), abilityState);
	}

	protected override string DefaultTargetingHintText(State abilityState)
	{
		return $"Select a target for {Icons.HintText(Icons.Attack)}{abilityState.SingleTargetAttackValue}";
	}
}