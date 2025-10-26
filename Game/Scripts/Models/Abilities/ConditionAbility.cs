using System.Collections.Generic;
using Fractural.Tasks;

/// <summary>
/// A <see cref="TargetedAbility{T, TSingleTargetState}"/> that allows a figure to apply conditions to other figures.
/// </summary>
public class ConditionAbility : TargetedAbility<ConditionAbility.State, SingleTargetState>
{
	public class State : TargetedAbilityState<SingleTargetState>
	{
	}

	public List<ScenarioEvents.ConditionAfterTargetConfirmed.Subscription>
		AfterTargetConfirmedSubscriptions { get; private set; } = [];

	/// <summary>
	/// A builder extending <see cref="TargetedAbility{T, TSingleTargetState}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in ConditionAbility Enables inheritors of ConditionAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending ConditionAbility
	public new abstract class AbstractBuilder<TBuilder, TAbility> : TargetedAbility<State, SingleTargetState>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IConditionsStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : ConditionAbility, new()
	{
		public interface IConditionsStep
		{
			TBuilder WithConditions(params ConditionModel[] conditions);
		}

		public TBuilder WithAfterTargetConfirmedSubscription(
			ScenarioEvents.ConditionAfterTargetConfirmed.Subscription afterTargetConfirmedSubscription)
		{
			Obj.AfterTargetConfirmedSubscriptions.Add(afterTargetConfirmedSubscription);
			return (TBuilder)this;
		}

		public TBuilder WithAfterTargetConfirmedSubscriptions(
			List<ScenarioEvents.ConditionAfterTargetConfirmed.Subscription> afterTargetConfirmedSubscriptions)
		{
			Obj.AfterTargetConfirmedSubscriptions = afterTargetConfirmedSubscriptions;
			return (TBuilder)this;
		}

		/// <summary>
		/// Overriding so we can set default values.
		/// </summary>
		public override TAbility Build()
		{
			// TODO varadski 21.08.2025: I would maybe rather throw an exception if there are no conditions; should be mandatory for ConditionAbility
			_target ??= ((Obj.Conditions.Length > 0 && Obj.Conditions[0].IsPositive) ? Target.SelfOrAllies : Target.Enemies);
			return base.Build();
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class ConditionBuilder : AbstractBuilder<ConditionBuilder, ConditionAbility>
	{
		internal ConditionBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of ConditionBuilder.
	/// </summary>
	/// <returns></returns>
	public static ConditionBuilder.IConditionsStep Builder()
	{
		return new ConditionBuilder();
	}

	public ConditionAbility() { }

	protected override async GDTask StartPerform(State abilityState)
	{
		await base.StartPerform(abilityState);

		ScenarioEvents.ConditionAfterTargetConfirmedEvent.Subscribe(abilityState, this, AfterTargetConfirmedSubscriptions);
	}

	protected override async GDTask EndPerform(State abilityState)
	{
		await base.EndPerform(abilityState);

		ScenarioEvents.ConditionAfterTargetConfirmedEvent.Unsubscribe(AfterTargetConfirmedSubscriptions);
	}

	protected override async GDTask AfterTargetConfirmedBeforeConditionsApplied(State abilityState, Figure target)
	{
		await ScenarioEvents.ConditionAfterTargetConfirmedEvent.CreatePrompt(
			new ScenarioEvents.ConditionAfterTargetConfirmed.Parameters(abilityState), abilityState);
	}

	protected override string DefaultTargetingHintText(State abilityState)
	{
		string conditonIconsText = string.Empty;
		foreach(ConditionModel conditionModel in Conditions)
		{
			conditonIconsText += Icons.HintText(Icons.GetCondition(conditionModel));
		}

		if(abilityState.AbilityTarget == Target.Self)
		{
			return $"Perform {conditonIconsText} self?";
		}

		return $"Select a target for {conditonIconsText}";
	}
}