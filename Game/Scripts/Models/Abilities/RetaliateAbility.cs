using System;
using System.Collections.Generic;
using Fractural.Tasks;

/// <summary>
/// An <see cref="ActiveAbility{T}"/> that deals direct damage to those that attacked the owner of the ability at a given range.
/// </summary>
public class RetaliateAbility : ActiveAbility<RetaliateAbility.State>
{
	public class State : ActiveAbilityState, IConditionsAbilityState
	{
		public int RetaliateValue { get; set; }
		public int Range { get; set; }

		public List<ConditionModel> ConditionModels { get; } = new List<ConditionModel>();

		public void AdjustRetaliateValue(int amount)
		{
			RetaliateValue += amount;
		}

		public void AdjustRange(int amount)
		{
			Range += amount;
		}

		public void AbilityAddCondition(ConditionModel conditionModel)
		{
			ConditionModels.Add(conditionModel);
		}
	}

	private Func<ScenarioEvents.Retaliate.Parameters, bool> _customCanApply;
	private bool _customCanApplyReplaceFully;

	public DynamicInt<State> RetaliateValue { get; private set; }
	public int Range { get; private set; }

	/// <summary>
	/// A builder extending <see cref="ActiveAbility{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in RetaliateAbility. Enables inheritors of RetaliateAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending RetaliateAbility.
	public new class AbstractBuilder<TBuilder, TAbility> : ActiveAbility<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IRetaliateValueStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : RetaliateAbility, new()
	{
		protected int? _range;

		public interface IRetaliateValueStep
		{
			TBuilder WithRetaliateValue(DynamicInt<State> retaliateValue, params RetaliateEnhancementMark[] enhancementMarks);
		}

		public TBuilder WithRetaliateValue(DynamicInt<State> retaliateValue, params RetaliateEnhancementMark[] enhancementMarks)
		{
			Obj.RetaliateValue = retaliateValue;
			AddEnhancements(enhancementMarks);
			return (TBuilder)this;
		}

		public TBuilder WithRange(int range)
		{
			_range = range;
			Obj.Range = range;
			return (TBuilder)this;
		}

		public TBuilder WithCustomCanApply(Func<ScenarioEvents.Retaliate.Parameters, bool> customCanApply)
		{
			Obj._customCanApply = customCanApply;
			return (TBuilder)this;
		}

		public TBuilder WithCustomCanApplyReplaceFully(bool customCanApplyReplaceFully)
		{
			Obj._customCanApplyReplaceFully = customCanApplyReplaceFully;
			return (TBuilder)this;
		}

		public override TAbility Build()
		{
			Obj.Range = _range ?? 1;
			return base.Build();
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class RetaliateBuilder : AbstractBuilder<RetaliateBuilder, RetaliateAbility>
	{
		internal RetaliateBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of RetaliateBuilder.
	/// </summary>
	/// <returns></returns>
	public static AbstractBuilder<RetaliateBuilder, RetaliateAbility>.IRetaliateValueStep Builder()
	{
		return new RetaliateBuilder();
	}

	public RetaliateAbility() { }

	protected override void InitializeState(State abilityState)
	{
		base.InitializeState(abilityState);

		abilityState.RetaliateValue = RetaliateValue.GetValue(abilityState);
		abilityState.Range = Range;
	}

	protected override async GDTask Perform(State abilityState)
	{
		await AskConfirmAndActivate(abilityState);
	}

	protected override async GDTask Activate(State abilityState)
	{
		await base.Activate(abilityState);

		await AbilityCmd.AddRetaliate(abilityState.Performer, this, abilityState.RetaliateValue, Range, _customCanApply, _customCanApplyReplaceFully);

		foreach(ConditionModel conditionModel in abilityState.ConditionModels)
		{
			await AbilityCmd.AddCondition(abilityState, abilityState.Performer, conditionModel);
		}
	}

	protected override async GDTask Deactivate(State abilityState)
	{
		await base.Deactivate(abilityState);

		AbilityCmd.RemoveRetaliate(abilityState.Performer, this);
	}

	protected override string DefaultHintText(State abilityState)
	{
		return $"Perform the {Icons.HintText(Icons.Retaliate)} ability?";
	}
}