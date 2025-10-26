using System.Collections.Generic;
using Fractural.Tasks;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

/// <summary>
/// A <see cref="TargetedAbility{T, TSingleTargetState}"/> that allows a figure to attack other figures.
/// </summary>
public class AttackAbility : TargetedAbility<AttackAbility.State, SingleTargetState>
{
	public class State : TargetedAbilityState<SingleTargetState>
	{
		public List<Figure> KilledTargets { get; } = new List<Figure>();

		public int AbilityAttackValue { get; set; }
		public int AbilityPierce { get; set; }
		public bool AbilityHasAdvantage { get; set; }
		public bool AbilityHasDisadvantage { get; set; }
		public bool AbilityIgnoresAllShields { get; set; }

		public int SingleTargetAttackValue { get; set; }
		public int SingleTargetPierce { get; set; }
		public bool SingleTargetHasAdvantage { get; set; }
		public bool SingleTargetHasDisadvantage { get; set; }
		public bool SingleTargetIgnoresAllShields { get; set; }

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

		public void AbilitySetIgnoresAllShields()
		{
			AbilityIgnoresAllShields = true;

			SingleTargetIgnoresAllShields = true;
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

		public void SingleTargetSetIgnoresAllShields()
		{
			SingleTargetIgnoresAllShields = true;
		}
	}

	public DynamicInt<State> Damage { get; protected set; }
	public DynamicInt<State> Pierce { get; protected set; } = 0;
	public bool HasAdvantage { get; protected set; }
	public bool HasDisadvantage { get; protected set; }
	public bool IgnoresAllShields { get; protected set; }

	public List<ScenarioEvents.DuringAttack.Subscription> DuringAttackSubscriptions { get; protected set; } = [];

	public List<ScenarioEvents.AttackAfterTargetConfirmed.Subscription>
		AfterTargetConfirmedSubscriptions { get; protected set; } = [];

	public List<ScenarioEvents.AfterAttackPerformed.Subscription>
		AfterAttackPerformedSubscriptions { get; protected set; } = [];

	/// <summary>
	/// A builder extending <see cref="TargetedAbility{T, TSingleTargetState}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in AttackAbility. Enables inheritors of AttackAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending AttackAbility.
	public new class AbstractBuilder<TBuilder, TAbility> : TargetedAbility<State, SingleTargetState>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IDamageStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : AttackAbility, new()
	{
		public interface IDamageStep
		{
			TBuilder WithDamage(DynamicInt<State> damage);
		}

		public TBuilder WithDamage(DynamicInt<State> damage)
		{
			Obj.Damage = damage;
			return (TBuilder)this;
		}

		public TBuilder WithPierce(DynamicInt<State> pierce)
		{
			Obj.Pierce = pierce;
			return (TBuilder)this;
		}

		public TBuilder WithIgnoresAllShields()
		{
			Obj.IgnoresAllShields = true;
			return (TBuilder)this;
		}

		public TBuilder WithAdvantage()
		{
			Obj.HasAdvantage = true;
			return (TBuilder)this;
		}

		public TBuilder WithHasAdvantage(bool advantage)
		{
			Obj.HasAdvantage = advantage;
			return (TBuilder)this;
		}

		public TBuilder WithDisadvantage()
		{
			Obj.HasDisadvantage = true;
			return (TBuilder)this;
		}

		public TBuilder WithHasDisadvantage(bool disadvantage)
		{
			Obj.HasDisadvantage = disadvantage;
			return (TBuilder)this;
		}

		public TBuilder WithDuringAttackSubscription(ScenarioEvents.DuringAttack.Subscription movementSubscription)
		{
			Obj.DuringAttackSubscriptions.Add(movementSubscription);
			return (TBuilder)this;
		}

		public TBuilder WithDuringAttackSubscriptions(
			List<ScenarioEvents.DuringAttack.Subscription> movementSubscriptions)
		{
			Obj.DuringAttackSubscriptions = movementSubscriptions;
			return (TBuilder)this;
		}

		public TBuilder WithAfterTargetConfirmedSubscription(
			ScenarioEvents.AttackAfterTargetConfirmed.Subscription afterTargetConfirmedSubscription)
		{
			Obj.AfterTargetConfirmedSubscriptions.Add(afterTargetConfirmedSubscription);
			return (TBuilder)this;
		}

		public TBuilder WithAfterTargetConfirmedSubscriptions(
			List<ScenarioEvents.AttackAfterTargetConfirmed.Subscription> afterTargetConfirmedSubscriptions)
		{
			Obj.AfterTargetConfirmedSubscriptions = afterTargetConfirmedSubscriptions;
			return (TBuilder)this;
		}

		public TBuilder WithAfterAttackPerformedSubscription(
			ScenarioEvents.AfterAttackPerformed.Subscription afterAttackPerformedSubscription)
		{
			Obj.AfterAttackPerformedSubscriptions.Add(afterAttackPerformedSubscription);
			return (TBuilder)this;
		}

		public TBuilder WithAfterAttackPerformedSubscriptions(
			List<ScenarioEvents.AfterAttackPerformed.Subscription> afterAttackPerformedSubscriptions)
		{
			Obj.AfterAttackPerformedSubscriptions = afterAttackPerformedSubscriptions;
			return (TBuilder)this;
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class AttackBuilder : AbstractBuilder<AttackBuilder, AttackAbility>
	{
		internal AttackBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of AttackBuilder.
	/// </summary>
	/// <returns></returns>
	public static AttackBuilder.IDamageStep Builder()
	{
		return new AttackBuilder();
	}

	public AttackAbility() { }

	protected override void InitializeState(State abilityState)
	{
		base.InitializeState(abilityState);

		abilityState.AbilityAttackValue = Damage.GetValue(abilityState);
		abilityState.AbilityPierce = Pierce.GetValue(abilityState);
		abilityState.AbilityHasAdvantage = HasAdvantage;
		abilityState.AbilityHasDisadvantage = HasDisadvantage;
		abilityState.AbilityIgnoresAllShields = IgnoresAllShields;
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
		abilityState.SingleTargetIgnoresAllShields = abilityState.AbilityIgnoresAllShields;
	}

	protected override EffectCollection CreateDuringTargetedAbilityEffectCollection(State abilityState)
	{
		return ScenarioEvents.DuringAttackEvent.CreateEffectCollection(new ScenarioEvents.DuringAttack.Parameters(abilityState));
	}

	protected override async GDTask AfterTargetConfirmedBeforeConditionsApplied(State abilityState, Figure target)
	{
		bool rangeDisadvantage = abilityState.SingleTargetRangeType == RangeType.Range &&
		                         RangeHelper.Distance(abilityState.Performer.Hex, target.Hex) == 1;
		if(rangeDisadvantage)
		{
			abilityState.SingleTargetSetHasDisadvantage();
		}

		ScenarioEvents.AttackAfterTargetConfirmed.Parameters attackAfterTargetConfirmedParameters =
			await ScenarioEvents.AttackAfterTargetConfirmedEvent.CreatePrompt(
				new ScenarioEvents.AttackAfterTargetConfirmed.Parameters(abilityState), abilityState);
				
		if(attackAfterTargetConfirmedParameters.CannotGainDisadvantage)
        {
			attackAfterTargetConfirmedParameters.AbilityState.SingleTargetHasDisadvantage = false;
        }

		await GameController.Instance.AMDDrawView.DrawCards(abilityState);

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