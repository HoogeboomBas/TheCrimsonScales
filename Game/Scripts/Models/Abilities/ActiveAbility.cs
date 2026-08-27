using System;
using Fractural.Tasks;
using Godot;

public abstract class ActiveAbilityState : AbilityState
{
	private Func<ActiveAbilityState, GDTask> _onDeactivate;

	public int CharacterTokens { get; set; }
	public Vector2 CharacterTokenPosition { get; set; }

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

	public void AdjustCharacterTokens(int amount)
	{
		CharacterTokens += amount;
	}
}

/// <summary>
/// An <see cref="Ability{T}"/> that has some sort of active effect that lasts for some duration.
/// </summary>
public abstract class ActiveAbility<T> : Ability<T> where T : ActiveAbilityState, new()
{
	private Func<T, string> _getHintText;
	public bool Mandatory { get; private set; }
	public Vector2 CharacterTokenPosition { get; private set; }

	public new abstract class AbstractBuilder<TBuilder, TAbility> : Ability<T>.AbstractBuilder<TBuilder, TAbility>
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : ActiveAbility<T>, new()
	{
		private Func<T, string> _getHintText;

		public TBuilder WithGetHintText(Func<T, string> getHintText)
		{
			_getHintText = getHintText;
			Obj._getHintText = getHintText;
			return (TBuilder)this;
		}

		/// <summary>
		/// Overriding so we can set default values.
		/// </summary>
		public override TAbility Build()
		{
			Obj._getHintText = _getHintText ?? Obj.DefaultHintText;
			return base.Build();
		}

		public TBuilder WithMandatory(bool mandatory)
		{
			Obj.Mandatory = mandatory;
			return (TBuilder)this;
		}

		public TBuilder WithCharacterTokenPosition(Vector2 position)
		{
			Obj.CharacterTokenPosition = position;
			return (TBuilder)this;
		}
	}

	protected override void InitializeState(T abilityState)
	{
		base.InitializeState(abilityState);

		abilityState.CharacterTokenPosition = CharacterTokenPosition;
	}

	protected async GDTask AskConfirmAndActivate(T abilityState)
	{
		ConfirmPrompt.Answer confirmAnswer =
			await PromptManager.Prompt(new ConfirmPrompt(null, () => _getHintText(abilityState), Mandatory), abilityState.Authority);
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