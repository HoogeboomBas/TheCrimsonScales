using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public abstract class AbilityCardSide
{
	private IEnumerable<AbilityCardAbility> _abilities;

	protected virtual IEnumerable<Element> Elements { get; } = [];
	protected virtual int XP => 0;

	protected virtual bool Round => false;
	protected virtual bool Persistent => false;
	protected virtual bool Loss => false;
	protected virtual bool Unrecoverable => false;

	public bool IsTop => AbilityCard.Top == this;
	public bool IsBasicTop => AbilityCard.BasicTop == this;
	public bool IsBottom => AbilityCard.Bottom == this;
	public bool IsBasicBottom => AbilityCard.BasicBottom == this;

	public IEnumerable<AbilityCardAbility> Abilities
	{
		get
		{
			if(_abilities == null)
			{
				_abilities = GetAbilities();
			}

			return _abilities;
		}
	}

	public AbilityCard AbilityCard { get; init; }

	protected abstract IEnumerable<AbilityCardAbility> GetAbilities();

	public async GDTask Perform(Figure performer)
	{
		ScenarioEvents.AbilityCardSideStarted.Parameters startedParameters =
			await ScenarioEvents.AbilityCardSideStartedEvent.CreatePrompt(
				new ScenarioEvents.AbilityCardSideStarted.Parameters(this, performer));

		if(!startedParameters.ForgoneAction)
		{
			ActionState actionState = new ActionState(performer, Abilities.Select(ability => ability.Ability).ToList(), //null, 
				onFirstActivateAbilityActivated: OnFirstActivateAbilityActivated, onDiscardOrLoseRequested: OnDiscardOrLoseRequested);
			await actionState.Perform();

			if(actionState.GetHasPerformed())
			{
				await AbilityCmd.GainXP(performer, XP);

				foreach(Element element in Elements)
				{
					await AbilityCmd.InfuseElement(element);
				}

				CardState resultingState = CardState.Discarded;

				bool round = Round || actionState.OverrideRound;
				bool persistent = Persistent || actionState.OverridePersistent;
				bool loss = Loss || actionState.OverrideLoss;

				if(round && persistent)
				{
					Log.Error($"Ability card side {this} is supposed to be both only active for the round, and persistent. This is not allowed.");
				}

				AbilityCard.SetUnrecoverable(Unrecoverable);

				// If no persistent/round ability has been performed, discard or lose it instead
				if(actionState.HasPerformedActiveAbility)
				{
					if(round)
					{
						resultingState = loss ? CardState.RoundLoss : CardState.Round;
					}
					else if(persistent)
					{
						resultingState = loss ? CardState.PersistentLoss : CardState.Persistent;
					}
					else
					{
						Log.Error($"Ability card side {this} performed an active ability, but is not marked as a round or persistent card.");
					}
				}
				else
				{
					if(loss)
					{
						resultingState = Unrecoverable ? CardState.UnrecoverablyLost : CardState.Lost;
					}
				}

				await AbilityCard.SetCardState(resultingState);
			}
			else
			{
				await AbilityCmd.DiscardCard(AbilityCard);
			}
		}
		else
		{
			await AbilityCmd.DiscardCard(AbilityCard);
		}

		await ScenarioEvents.AbilityCardSideEndedEvent.CreatePrompt(new ScenarioEvents.AbilityCardSideEnded.Parameters(this, performer));
	}

	protected async GDTask GainXP(AbilityState abilityState)
	{
		await AbilityCmd.GainXP(abilityState.Performer, 1);
	}

	private async GDTask OnFirstActivateAbilityActivated(ActionState actionState)
	{
		AbilityCard.SetActionStateActive(actionState);

		await GDTask.CompletedTask;
	}

	private async GDTask OnDiscardOrLoseRequested(ActionState actionState)
	{
		await AbilityCmd.DiscardOrLose(AbilityCard);
	}
}