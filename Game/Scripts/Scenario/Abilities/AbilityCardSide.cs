using System.Linq;
using Fractural.Tasks;
using Godot;

public class AbilityCardSide : IActionSource
{
	public AbilityCard AbilityCard { get; }
	public AbilityCardSideModel Model { get; }
	public AbilityCardSideType AbilityCardSideType => Model.AbilityCardSideType;

	public AbilityCardSide(AbilityCard abilityCard, AbilityCardSideModel model)
	{
		AbilityCard = abilityCard;
		Model = model;
	}

	public async GDTask Perform(Figure performer)
	{
		ScenarioEvents.AbilityCardSideStarted.Parameters startedParameters =
			await ScenarioEvents.AbilityCardSideStartedEvent.CreatePrompt(
				new ScenarioEvents.AbilityCardSideStarted.Parameters(this, performer));

		CardState resultingState = CardState.Discarded;
		bool performed = false;

		if(!startedParameters.ForgoneAction)
		{
			ActionState actionState = new ActionState(this, performer, Model.Abilities.Select(ability => ability.Ability).ToList(), //null, 
				onFirstActivateAbilityActivated: OnFirstActivateAbilityActivated, onDiscardOrLoseRequested: OnDiscardOrLoseRequested);
			await actionState.Perform();

			if(actionState.GetHasPerformed())
			{
				performed = true;

				await Model.OnActionPerformed(actionState.Performer);

				await AbilityCmd.GainXP(performer, Model.XP);

				foreach(CardElementInfusion elementInfusion in Model.Elements)
				{
					if(elementInfusion.ConsumableElements == null ||
					   await AbilityCmd.AskConsumeElement(performer, elementInfusion.ConsumableElements) != null)
					{
						await AbilityCmd.InfuseElement(null, elementInfusion.PossibleInfusedElements, performer);
					}
				}

				bool round = Model.Round || actionState.OverrideRound;
				bool persistent = !actionState.OverrideNoPersistent && (actionState.OverridePersistent || Model.Persistent);
				bool loss = !actionState.OverrideNoLoss && (actionState.OverrideLoss || Model.Loss);

				if(round && persistent)
				{
					Log.Error(
						$"Ability card side {Model.GetType()} is supposed to be both only active for the round, and persistent. This is not allowed.");
				}

				AbilityCard.SetUnrecoverable(Model.Unrecoverable);

				// If no persistent/round ability has been performed, discard or lose it instead
				if(actionState.HasPerformedActiveAbility && !actionState.OverrideNoPersistent && !actionState.OverrideNoRound)
				{
					if(round)
					{
						resultingState = loss ? CardState.RoundLoss : CardState.Round;
					}
					else if(persistent)
					{
						resultingState = loss
							? CardState.PersistentLoss
							: (Model.CanDeactivate ? CardState.Persistent : CardState.PersistentNoDeactivate);
					}
					else
					{
						Log.Error(
							$"Ability card side {Model.GetType()} performed an active ability, but is not marked as a round or persistent card.");
					}
				}
				else
				{
					if(loss)
					{
						resultingState = Model.Unrecoverable ? CardState.UnrecoverablyLost : CardState.Lost;
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

		await ScenarioEvents.AbilityCardSideEndedEvent.CreatePrompt(
			new ScenarioEvents.AbilityCardSideEnded.Parameters(this, performer, resultingState, performed));
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