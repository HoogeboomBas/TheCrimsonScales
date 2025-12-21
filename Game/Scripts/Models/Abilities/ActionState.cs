using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

/// <summary>
/// An Action State is a collection of abilities that can be performed, keeping track of the resulting states of the abilities.
/// </summary>
public sealed partial class ActionState
{
	private readonly List<AbilityState> _abilityStates = new List<AbilityState>();

	private readonly Func<ActionState, GDTask> _onFirstActivateAbilityActivated;
	private readonly Func<ActionState, GDTask> _onDiscardOrLoseRequested;

	private bool _discardOrLoseRequested;

	public Figure Performer { get; }
	public Figure Authority { get; }

	public IList<Ability> Abilities { get; }

	public AbilityState CurrentAbilityState { get; private set; }
	public bool HasPerformedActiveAbility { get; private set; }

	public ActionState ParentActionState { get; }
	public List<ActionState> ChildActionStates { get; } = new List<ActionState>();

	public bool OverrideRound { get; private set; }
	public bool OverridePersistent { get; private set; }
	public bool OverrideLoss { get; private set; }

	public IReadOnlyList<AbilityState> AbilityStates => _abilityStates;
	public int CurrentAbilityStateIndex => _abilityStates.Count - 1;

	public ActionState(Figure performerAndAuthority, IList<Ability> abilities, ActionState parentActionState = null,
		Func<ActionState, GDTask> onFirstActivateAbilityActivated = null, Func<ActionState, GDTask> onDiscardOrLoseRequested = null)
		: this(performerAndAuthority, performerAndAuthority, abilities, parentActionState, onFirstActivateAbilityActivated, onDiscardOrLoseRequested)
	{
	}

	public ActionState(Figure performer, Figure authority, IList<Ability> abilities, ActionState parentActionState = null,
		Func<ActionState, GDTask> onFirstActivateAbilityActivated = null, Func<ActionState, GDTask> onDiscardOrLoseRequested = null)
	{
		Performer = performer;
		Authority = authority;

		Abilities = abilities;

		ParentActionState = parentActionState;
		ParentActionState?.ChildActionStates.Add(this);

		_onFirstActivateAbilityActivated = onFirstActivateAbilityActivated;
		_onDiscardOrLoseRequested = onDiscardOrLoseRequested;
	}

	public async GDTask Perform()
	{
		if(_abilityStates.Count > 0)
		{
			Log.Error("Running an action state a second time is not allowed.");
		}

		if(Performer.TakingTurn)
		{
			Performer.TurnPerformedActionStates.Add(this);
		}

		Performer.RoundPerformedActionStates.Add(this);

		await ScenarioEvents.ActionStartedEvent.CreatePrompt(new ScenarioEvents.ActionStarted.Parameters(this));

		foreach(Ability ability in Abilities)
		{
			await ability.Perform(this);
		}

		await ScenarioEvents.ActionEndedEvent.CreatePrompt(new ScenarioEvents.ActionEnded.Parameters(this));
	}

	public void AddAbilityState(AbilityState state)
	{
		CurrentAbilityState = state;
		_abilityStates.Add(state);
	}

	public T GetAbilityState<T>(int index)
		where T : AbilityState
	{
		return (T)_abilityStates[index];
	}

	public bool GetHasPerformed()
	{
		return _abilityStates.Any(state => state.Performed);
	}

	public async GDTask SetPerformedActiveAbility(ActiveAbilityState activeAbilityState)
	{
		if(ParentActionState != null)
		{
			await ParentActionState.SetPerformedActiveAbility(activeAbilityState);
		}

		if(HasPerformedActiveAbility)
		{
			return;
		}

		HasPerformedActiveAbility = true;

		if(_onFirstActivateAbilityActivated != null)
		{
			await _onFirstActivateAbilityActivated(this);
		}
	}

	public async GDTask RequestDiscardOrLose()
	{
		if(ParentActionState != null)
		{
			await ParentActionState.RequestDiscardOrLose();
		}

		if(_discardOrLoseRequested)
		{
			return;
		}

		_discardOrLoseRequested = true;

		if(_onDiscardOrLoseRequested != null)
		{
			await _onDiscardOrLoseRequested(this);
		}
	}

	public async GDTask RemoveFromActive()
	{
		foreach(AbilityState abilityState in _abilityStates)
		{
			await abilityState.RemoveFromActive();
		}

		foreach(ActionState childActionState in ChildActionStates)
		{
			await childActionState.RemoveFromActive();
		}
	}

	public void SetOverrideRound()
	{
		OverrideRound = true;

		ParentActionState?.SetOverrideRound();
	}

	public void SetOverridePersistent()
	{
		OverridePersistent = true;

		ParentActionState?.SetOverridePersistent();
	}

	public void SetOverrideLoss()
	{
		OverrideLoss = true;

		ParentActionState?.SetOverrideLoss();
	}
}