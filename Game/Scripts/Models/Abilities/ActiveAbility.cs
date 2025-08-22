using System;
using System.Collections.Generic;
using Fractural.Tasks;

public abstract class ActiveAbilityState : AbilityState
{
	private Func<ActiveAbilityState, GDTask> _onDeactivate;

	public void SetOnDeactivate(Func<ActiveAbilityState, GDTask> onDeactivate)
	{
		_onDeactivate = onDeactivate;
	}

	public override async GDTask RemoveFromActive()
	{
		await base.RemoveFromActive();

		if(_onDeactivate != null)
		{
			await _onDeactivate(this);
		}
	}
}

/// <summary>
/// An <see cref="Ability{T}"/> that has some sort of persistent effect.
/// </summary>
public abstract class ActiveAbility<T> : Ability<T> where T : ActiveAbilityState, new()
{
	private Func<T, string> GetHintText { get; set; }

	public new abstract class AbstractBuilder<TBuilder, TAbility> : Ability<T>.AbstractBuilder<TBuilder, TAbility>
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : ActiveAbility<T>, new()
	{
		private Func<T, string> _getHintText;
		public TBuilder WithGetHintText(Func<T, string> getHintText)
		{
			_getHintText = getHintText;
			Obj.GetHintText = getHintText;
			return (TBuilder)this;
		}

		/// <summary>
		/// Overriding so we can set default values.
		/// </summary>
		public override TAbility Build()
		{
			Obj.GetHintText = _getHintText ?? Obj.DefaultHintText;
			return base.Build();
		}
	}

	protected ActiveAbility(
		Func<T, GDTask> onAbilityStarted = null, Func<T, GDTask> onAbilityEnded = null, Func<T, GDTask> onAbilityEndedPerformed = null,
		ConditionalAbilityCheckDelegate conditionalAbilityCheck = null,
		Func<T, string> getHintText = null,
		List<ScenarioEvent<ScenarioEvents.AbilityStarted.Parameters>.Subscription> abilityStartedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityEnded.Parameters>.Subscription> abilityEndedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> abilityPerformedSubscriptions = null)
		: base(onAbilityStarted, onAbilityEnded, onAbilityEndedPerformed, conditionalAbilityCheck, abilityStartedSubscriptions,
			abilityEndedSubscriptions, abilityPerformedSubscriptions)
	{
		GetHintText = getHintText ?? DefaultHintText;
	}

	protected async GDTask AskConfirmAndActivate(T abilityState)
	{
		ConfirmPrompt.Answer confirmAnswer =
			await PromptManager.Prompt(new ConfirmPrompt(null, () => GetHintText(abilityState)), abilityState.Authority);
		if(confirmAnswer.Confirmed)
		{
			await Activate(abilityState);
		}
	}

	protected virtual async GDTask Activate(T abilityState)
	{
		abilityState.SetOnDeactivate(state => Deactivate((T)state));
		abilityState.SetPerformed();
		await abilityState.ActionState.SetPerformedActiveAbility(abilityState);
	}

	protected virtual async GDTask Deactivate(T abilityState)
	{
		await GDTask.CompletedTask;
	}

	protected virtual string DefaultHintText(T abilityState)
	{
		return "Perform the active ability?";
	}
}