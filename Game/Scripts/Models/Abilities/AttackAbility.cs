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
		public List<Figure> DamagedTargets { get; } = new List<Figure>();
		public int DamageDealt { get; set; } = 0;

		public int AbilityAttackValue { get; set; }
		public int AbilityPierce { get; set; }
		public bool AbilityHasAdvantage { get; set; }
		public bool AbilityHasDisadvantage { get; set; }
		public bool AbilityIgnoresAllShields { get; set; }
		public bool AbilityDrawAMDCard { get; set; }

		public int SingleTargetAttackValue { get; set; }
		public int SingleTargetPierce { get; set; }
		public bool SingleTargetHasAdvantage { get; set; }
		public bool SingleTargetHasDisadvantage { get; set; }
		public bool SingleTargetIgnoresAllShields { get; set; }
		public bool SingleTargetDrawAMDCard { get; set; }
		public bool IsSingleTarget => AbilityAOEPattern == null && AbilityTargets <= 1;

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

		public void AbilitySetDrawAMDCard(bool drawAMDCard)
		{
			AbilityDrawAMDCard = drawAMDCard;

			SingleTargetDrawAMDCard = drawAMDCard;
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

		public void SingleTargetSetDrawAMDCard(bool drawAMDCard)
		{
			SingleTargetDrawAMDCard = drawAMDCard;
		}
	}

	public DynamicInt<State> Damage { get; protected set; }
	public DynamicInt<State> Pierce { get; protected set; } = 0;
	public bool HasAdvantage { get; protected set; }
	public bool HasDisadvantage { get; protected set; }
	public bool IgnoresAllShields { get; protected set; }
	public bool DrawAMDCards { get; protected set; } = true;

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
			TBuilder WithDamage(DynamicInt<State> damage, params AttackEnhancementMark[] enhancementMarks);
		}

		public TBuilder WithDamage(DynamicInt<State> damage, params AttackEnhancementMark[] enhancementMarks)
		{
			Obj.Damage = damage;
			AddEnhancements(enhancementMarks);
			return (TBuilder)this;
		}

		public TBuilder WithPierce(DynamicInt<State> pierce, params PierceSquare[] enhancementMarks)
		{
			Obj.Pierce = pierce;
			AddEnhancements(enhancementMarks);
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

		public TBuilder WithNoAMDCards()
		{
			Obj.DrawAMDCards = false;
			return (TBuilder)this;
		}

		public TBuilder WithDuringAttackSubscription(ScenarioEvents.DuringAttack.Subscription duringAttackSubscription)
		{
			Obj.DuringAttackSubscriptions.Add(duringAttackSubscription);
			return (TBuilder)this;
		}

		public TBuilder WithDuringAttackSubscriptions(
			List<ScenarioEvents.DuringAttack.Subscription> duringAttackSubscriptions)
		{
			Obj.DuringAttackSubscriptions.AddRange(duringAttackSubscriptions);
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
			Obj.AfterTargetConfirmedSubscriptions.AddRange(afterTargetConfirmedSubscriptions);
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
			Obj.AfterAttackPerformedSubscriptions.AddRange(afterAttackPerformedSubscriptions);
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
		abilityState.AbilityDrawAMDCard = DrawAMDCards;
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

	protected override void InitAbilityStateForSingleTarget(State abilityState)
	{
		base.InitAbilityStateForSingleTarget(abilityState);

		abilityState.SingleTargetAttackValue = abilityState.AbilityAttackValue;
		abilityState.SingleTargetPierce = abilityState.AbilityPierce;
		abilityState.SingleTargetHasAdvantage = abilityState.AbilityHasAdvantage;
		abilityState.SingleTargetHasDisadvantage = abilityState.AbilityHasDisadvantage;
		abilityState.SingleTargetIgnoresAllShields = abilityState.AbilityIgnoresAllShields;
		abilityState.SingleTargetDrawAMDCard = abilityState.AbilityDrawAMDCard;
	}

	protected override EffectCollection CreateDuringTargetedAbilityEffectCollection(State abilityState)
	{
		return ScenarioEvents.DuringAttackEvent.CreateEffectCollection(new ScenarioEvents.DuringAttack.Parameters(abilityState));
	}

	protected override async GDTask AfterTargetConfirmedBeforeConditionsApplied(State abilityState, Figure target)
	{
		if(CheckRangeDisadvantage(abilityState.Performer.Hexes, target.Hexes, abilityState.SingleTargetRangeType))
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

		if(attackAfterTargetConfirmedParameters.AbilityState.SingleTargetDrawAMDCard)
		{
			await GameController.Instance.AMDDrawView.DrawCards(abilityState);
		}

		int damageDealt = await AbilityCmd.SufferDamage(abilityState, target, abilityState.SingleTargetAttackValue, fromAttack: true);

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
				if(damageDealt > 0)
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

				if(damageDealt > 0)
				{
					AppController.Instance.AudioController.PlayFastForwardable(SFX.GetBowHit(), delay: 0.2f);
				}
				else
				{
					AppController.Instance.AudioController.PlayFastForwardable(SFX.GetBowBlocked(), delay: 0.2f);
				}
			}

			if(damageDealt > 0)
			{
				GTweenSequenceBuilder.New()
					.AppendTime(0.25f)
					.Append(target.TweenGlobalPosition(targetOrigin + normal * Map.HexSize * 0.2f, 0.15f).SetEasing(Easing.OutQuart))
					.Append(target.TweenGlobalPosition(targetOrigin, 0.2f).SetEasing(Easing.OutBack))
					.Build().PlayFastForwardable();
			}

			await GDTask.DelayFastForwardable(0.6f);
		}

		if(damageDealt > 0)
		{
			abilityState.DamageDealt += damageDealt;
			abilityState.DamagedTargets.AddIfNew(target);
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
			await AbilityCmd.SufferDamage(abilityState.Performer, retaliateParameters.Retaliate,
				potentialDamageDealer: retaliateParameters.RetaliatingFigure);
		}

		await ScenarioEvents.AfterAttackPerformedEvent.CreatePrompt(
			new ScenarioEvents.AfterAttackPerformed.Parameters(abilityState), abilityState);
	}

	protected override string DefaultTargetingHintText(State abilityState)
	{
		return $"Select a target for {Icons.HintText(Icons.Attack)}{abilityState.SingleTargetAttackValue}";
	}

	public static bool CheckRangeDisadvantage(Hex[] performerHexes, Hex[] targetHexes, RangeType rangeType)
	{
		if(rangeType == RangeType.Range)
		{
			foreach(Hex performerHex in performerHexes)
			{
				foreach(Hex targetHex in targetHexes)
				{
					if(RangeHelper.Distance(performerHex, targetHex) == 1)
					{
						return true;
					}
				}
			}
		}

		return false;
	}
}