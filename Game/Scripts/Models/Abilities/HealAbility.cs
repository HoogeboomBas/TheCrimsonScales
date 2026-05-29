using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using GTweens.Builders;
using GTweens.Easings;

/// <summary>
/// A <see cref="TargetedAbility{T, TSingleTargetState}"/> that allows a figure to restore hit points to other figures.
/// </summary>
public class HealAbility : TargetedAbility<HealAbility.State, HealAbility.HealAbilitySingleTargetState>
{
	public class HealAbilitySingleTargetState : SingleTargetState
	{
		public List<ConditionModel> RemovedConditions { get; } = new List<ConditionModel>();

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

	public DynamicInt<State> HealValue { get; private set; }

	public List<ScenarioEvents.DuringHeal.Subscription> DuringHealSubscriptions { get; private set; } = [];

	public List<ScenarioEvents.HealAfterTargetConfirmed.Subscription> AfterTargetConfirmedSubscriptions { get; private set; } = [];

	public List<ScenarioEvents.AfterHealPerformed.Subscription> AfterHealPerformedSubscriptions { get; private set; } = [];

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
			TBuilder WithHealValue(DynamicInt<State> healValue, params HealEnhancementMark[] enhancementMarks);
		}

		public TBuilder WithHealValue(DynamicInt<State> healValue, params HealEnhancementMark[] enhancementMarks)
		{
			Obj.HealValue = healValue;
			AddEnhancements(enhancementMarks);
			return (TBuilder)this;
		}

		public TBuilder WithDuringHealSubscription(ScenarioEvents.DuringHeal.Subscription duringHealSubscription)
		{
			Obj.DuringHealSubscriptions.Add(duringHealSubscription);
			return (TBuilder)this;
		}

		public TBuilder WithDuringHealSubscriptions(List<ScenarioEvents.DuringHeal.Subscription> duringHealSubscriptions)
		{
			Obj.DuringHealSubscriptions.AddRange(duringHealSubscriptions);
			return (TBuilder)this;
		}

		public TBuilder WithAfterTargetConfirmedSubscription(
			ScenarioEvents.HealAfterTargetConfirmed.Subscription afterTargetConfirmedSubscription)
		{
			Obj.AfterTargetConfirmedSubscriptions.Add(afterTargetConfirmedSubscription);
			return (TBuilder)this;
		}

		public TBuilder WithAfterTargetConfirmedSubscriptions(
			List<ScenarioEvents.HealAfterTargetConfirmed.Subscription> afterTargetConfirmedSubscriptions)
		{
			Obj.AfterTargetConfirmedSubscriptions.AddRange(afterTargetConfirmedSubscriptions);
			return (TBuilder)this;
		}

		public TBuilder WithAfterHealPerformedSubscription(
			ScenarioEvents.AfterHealPerformed.Subscription afterHealPerformedSubscriptions)
		{
			Obj.AfterHealPerformedSubscriptions.Add(afterHealPerformedSubscriptions);
			return (TBuilder)this;
		}

		public TBuilder WithAfterHealPerformedSubscriptions(
			List<ScenarioEvents.AfterHealPerformed.Subscription> afterHealPerformedSubscriptions)
		{
			Obj.AfterHealPerformedSubscriptions.AddRange(afterHealPerformedSubscriptions);
			return (TBuilder)this;
		}

		/// <summary>
		/// Overriding so we can set default values.
		/// </summary>
		public override TAbility Build()
		{
			_target ??= Target.SelfOrAllies;
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
	public static HealBuilder.IHealValueStep Builder()
	{
		return new HealBuilder();
	}

	public HealAbility() { }

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

	protected override void InitAbilityStateForSingleTarget(State abilityState)
	{
		base.InitAbilityStateForSingleTarget(abilityState);

		abilityState.SingleTargetHealValue = abilityState.AbilityHealValue;
	}

	protected override EffectCollection CreateDuringTargetedAbilityEffectCollection(State abilityState)
	{
		return ScenarioEvents.DuringHealEvent.CreateEffectCollection(new ScenarioEvents.DuringHeal.Parameters(abilityState));
	}

	protected override async GDTask AfterTargetConfirmedBeforeConditionsApplied(State abilityState, Figure target)
	{
		await ScenarioEvents.HealAfterTargetConfirmedEvent.CreatePrompt(
			new ScenarioEvents.HealAfterTargetConfirmed.Parameters(abilityState), abilityState);

		ScenarioEvents.HealBlockTime.Parameters blockedAbilityStateParameters =
			await ScenarioEvents.HealBlockTimeEvent.CreatePrompt(
				new ScenarioEvents.HealBlockTime.Parameters(abilityState), abilityState);

		if(!blockedAbilityStateParameters.IsBlocked)
		{
			int newHealth = Mathf.Min(target.Health + abilityState.SingleTargetHealValue, target.MaxHealth);

			target.SetHealth(newHealth);
		}

		if(!GameController.FastForward)
		{
			AppController.Instance.AudioController.PlayFastForwardable(SFX.Heal, delay: 0.0f);

			PackedScene healEffectScene = ResourceLoader.Load<PackedScene>("res://Scenes/Scenario/Effects/HealEffect.tscn");
			HealEffect healEffect = healEffectScene.Instantiate<HealEffect>();
			target.AddChild(healEffect);
			healEffect.Init();

			Color healColor = Color.Color8(44, 199, 10);

			target.Visual.SetSelfModulate(healColor);
			GTweenSequenceBuilder.New()
				.Append(target.Visual.TweenInstanceShaderPropertyFloat("tintFactor", 0.4f, 0.6f))
				.AppendTime(0.1f)
				.Append(target.Visual.TweenInstanceShaderPropertyFloat("tintFactor", 0f, 0.5f))
				.Build().PlayFastForwardable();

			GTweenSequenceBuilder.New()
				.Append(target.TweenScale(1.2f, 0.4f).SetEasing(Easing.InOutBack))
				.AppendTime(0.4f)
				.Append(target.TweenScale(1f, 0.2f).SetEasing(Easing.InBack))
				.Build().PlayFastForwardable();

			await GDTask.DelayFastForwardable(1.2f);

			target.Visual.SetSelfModulate(Colors.White);
		}

		for(int i = target.Conditions.Count - 1; i >= 0; i--)
		{
			Condition condition = target.Conditions[i];
			if(condition.ConditionModel.RemovedByHeal)
			{
				await AbilityCmd.RemoveCondition(condition, abilityState);
				abilityState.SingleTargetState.AddRemovedCondition(condition.ConditionModel);
			}
		}

		await ScenarioEvents.AfterHealPerformedEvent.CreatePrompt(
			new ScenarioEvents.AfterHealPerformed.Parameters(abilityState, blockedAbilityStateParameters.IsBlocked), abilityState);
	}

	protected override void GetValidTargets(State abilityState, List<Figure> figures, int targetsOutOfAOE)
	{
		base.GetValidTargets(abilityState, figures, targetsOutOfAOE);

		if(abilityState.Authority is not Character && figures.Count != 0)
		{
			int mostHealthLost = figures.Select(figure => figure.MaxHealth - figure.Health).Max();
			figures.RemoveAll(figure => figure.MaxHealth - figure.Health < mostHealthLost);
		}
	}

	protected override string DefaultTargetingHintText(State abilityState)
	{
		if(abilityState.AbilityTarget == Target.Self)
		{
			return $"Perform {Icons.HintText(Icons.Heal)}{abilityState.AbilityHealValue} self?";
		}

		return $"Select a target for {Icons.HintText(Icons.Heal)}{abilityState.SingleTargetHealValue}";
	}
}