using System.Linq;
using Fractural.Tasks;

public class HierophantMasteries
{
	public class Mastery1 : MasteryModel
	{
		public override string Description =>
			$"In a single scenario, perform an action with {Icons.Inline(Icons.LoseCard)} before your first rest and in between each of your rests.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			mastery.AdjustProgress();

			bool performedLoss = false;

			ScenarioEvents.AbilityCardSideEndedEvent.Subscribe(this,
				parameters => parameters.Performer == character &&
				              parameters.AbilityCardSide.AbilityCard.CardState is CardState.Lost or CardState.PersistentLoss or CardState.RoundLoss
					              or CardState.UnrecoverablyLost,
				async _ =>
				{
					performedLoss = true;

					await GDTask.CompletedTask;
				}
			);

			ScenarioEvents.ShortRestStartedEvent.Subscribe(this,
				parameters => parameters.Character == character,
				async _ =>
				{
					if(performedLoss)
					{
						performedLoss = false;
					}
					else
					{
						mastery.ResetProgress();
						mastery.SetTemporarilyFailed(true);
					}

					await GDTask.CompletedTask;
				}
			);

			ScenarioEvents.LongRestStartedEvent.Subscribe(this,
				parameters => parameters.Character == character,
				async _ =>
				{
					if(performedLoss)
					{
						performedLoss = false;
					}
					else
					{
						mastery.ResetProgress();
						mastery.SetTemporarilyFailed(true);
					}

					await GDTask.CompletedTask;
				}
			);
		}
	}

	public class Mastery2 : MasteryModel
	{
		public override string Description =>
			"For an entire scenario, give an ally an ability card, target an ally with a heal ability, grant an ally shield, or grant an ally retaliate on each of your turns.";

		public override int MaxProgress => 1;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			mastery.AdjustProgress();

			bool gaveAlly = false;

			ScenarioEvents.FigureTurnStartedEvent.Subscribe(this,
				parameters => parameters.Figure == character,
				async _ =>
				{
					gaveAlly = false;
					await GDTask.CompletedTask;
				});

			ScenarioEvents.AbilityCardGivenEvent.Subscribe(this,
				parameters => parameters.CardGiver == character && character.AlliedWith(parameters.CardReceiver),
				async _ =>
				{
					gaveAlly = true;
					await GDTask.CompletedTask;
				});

			ScenarioEvents.AfterHealPerformedEvent.Subscribe(this,
				parameters => parameters.Performer == character && character.AlliedWith(parameters.AbilityState.Target),
				async _ =>
				{
					gaveAlly = true;
					await GDTask.CompletedTask;
				});

			ScenarioEvents.AbilityPerformedEvent.Subscribe(this,
				parameters => character.AlliedWith(parameters.Performer) &&
				              parameters.AbilityState.ActionState.ParentActionState.Performer == character &&
				              parameters.AbilityState.ActionState.ParentActionState.AbilityStates.Any(state => state is GrantAbility.State) &&
				              parameters.AbilityState is ShieldAbility.State or RetaliateAbility.State,
				async _ =>
				{
					gaveAlly = true;
					await GDTask.CompletedTask;
				});

			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
				parameters => parameters.Figure == character && !gaveAlly,
				async _ =>
				{
					mastery.ResetProgress();
					mastery.SetTemporarilyFailed(true);
					await GDTask.CompletedTask;
				});
		}
	}
}