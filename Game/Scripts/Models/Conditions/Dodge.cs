using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Dodge : ConditionModel
{
	public override string Name => "Dodge";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Dodge.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Positive;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		if(condition.Owner is not Character)
		{
			//TODO: If we ever have to implement an ally that you can lose cards for this needs to be updated to allow for that
			await AbilityCmd.RemoveCondition(condition);
			//TODO: Change this to preventing the adding of the condition in the first place
			return;
		}

		Character characterOwner = (Character)condition.Owner;

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(condition,
			parameters => parameters.AbilityState.Target == condition.Owner,
			async parameters =>
			{
				if(ScenarioCheckEvents.ImmunitiesVisualCheckEvent
				   .Fire(new ScenarioCheckEvents.ImmunitiesVisualCheck.Parameters(parameters.AbilityState.Performer)).Immunities
				   .Any(conditionModel => conditionModel is Stun or Disarm))
				{
					condition.Flash();
					await AbilityCmd.RemoveCondition(condition, parameters.AbilityState);

					List<ScenarioEvents.GenericChoice.Subscription> subscriptions =
					[
						ScenarioEvents.GenericChoice.Subscription.New(
							_ => characterOwner.Cards.Any(card => card.CardState == CardState.Hand),
							async _ =>
							{
								ScenarioEvents.DodgeDiscardCard.Parameters dodgeDiscardParameters =
									await ScenarioEvents.DodgeDiscardCardEvent.CreatePrompt(
										new ScenarioEvents.DodgeDiscardCard.Parameters(characterOwner));

								if(dodgeDiscardParameters.DiscardCard)
								{
									AbilityCard card = await AbilityCmd.SelectAbilityCard(characterOwner, CardState.Hand, true,
										card => card.OriginalOwner == characterOwner,
										hintText: "Select a card to discard");

									await AbilityCmd.DiscardCard(card);
								}

								parameters.SetPrevented();
								await ScenarioEvents.DodgeUsedEvent.CreatePrompt(
									new ScenarioEvents.DodgeUsed.Parameters(condition.Owner, parameters.Performer));

								await GDTask.CompletedTask;
							}, EffectType.Selectable,
							effectButtonParameters: new IconEffectButton.Parameters(Icons.DiscardedCards),
							effectInfoViewParameters: new TextEffectInfoView.Parameters("Discard one card from your hand to prevent the attack")
						),
						ScenarioEvents.GenericChoice.Subscription.New(
							_ => characterOwner.Cards.Any(card =>
								card.CardState == CardState.Discarded && card.OriginalOwner == condition.Owner),
							async _ =>
							{
								parameters.SetPrevented();
								await ScenarioEvents.DodgeUsedEvent.CreatePrompt(
									new ScenarioEvents.DodgeUsed.Parameters(condition.Owner, parameters.Performer));

								await GDTask.CompletedTask;
							}, EffectType.Selectable,
							effectButtonParameters: new IconEffectButton.Parameters(Icons.LoseCard),
							effectInfoViewParameters: new TextEffectInfoView.Parameters(
								"Lose one card from your discard pile to prevent the attack")
						)
					];
					await AbilityCmd.GenericChoice(parameters.Performer, subscriptions,
						hintText: $"Apply {Icons.HintText(Icons.GetCondition(Conditions.Dodge))}?");
				}
			});
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(condition);
	}
}