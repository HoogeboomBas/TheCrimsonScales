using System.Collections.Generic;
using Fractural.Tasks;

public class BrightsparkMasteries
{
	public class Mastery1 : MasteryModel
	{
		public override string Description => "For an entire scenario, infuse or consume an element on each of your turns.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			mastery.AdjustProgress();

			bool noElement = true;

			ScenarioEvents.FigureTurnStartedEvent.Subscribe(this,
				parameters => parameters.Figure == character,
				async _ =>
				{
					noElement = true;
					await GDTask.CompletedTask;
				});

			ScenarioEvents.ElementInfusedEvent.Subscribe(this,
				parameters => parameters.PotentialInfuser == character,
				async _ =>
				{
					noElement = false;
					await GDTask.CompletedTask;
				});

			ScenarioEvents.FinishElementConsumedEvent.Subscribe(this,
				parameters => parameters.PotentialConsumer == character,
				async _ =>
				{
					noElement = false;
					await GDTask.CompletedTask;
				});

			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
				parameters => parameters.Figure == character && noElement,
				async _ =>
				{
					mastery.ResetProgress();
					mastery.SetTemporarilyFailed(true);
					await GDTask.CompletedTask;
				});
		}
	}

	public class Mastery2 : MasteryModel
	{
		public override string Description =>
			"In a single scenario, move your character token off of the last slot of 6 different persistent abilities.";

		public override int MaxProgress => 6;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			List<AbilityCardSide> persistentSides = [];

			ScenarioEvents.UseSlotAdvancedEvent.Subscribe(this,
				parameters => parameters.AbilityState.Performer == character &&
				              parameters.AbilityState.UseSlotIndex >= parameters.AbilityState.Slots.Count &&
				              !persistentSides.Contains((AbilityCardSide)parameters.AbilityState.ActionState.ActionSource),
				async parameters =>
				{
					mastery.AdjustProgress();
					persistentSides.Add((AbilityCardSide)parameters.AbilityState.ActionState.ActionSource);
					await GDTask.CompletedTask;
				});
		}
	}
}