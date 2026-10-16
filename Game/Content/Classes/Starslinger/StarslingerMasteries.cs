using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class StarslingerMasteries
{
	public class Mastery1 : MasteryModel
	{
		public override string Description => "For an entire scenario, end each of your turns at full health.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			mastery.AdjustProgress();

			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
				parameters => parameters.Figure == character && character.IsDamaged(),
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
		public override string Description => "In a single scenario, target 3 or more figures with 4 different actions.";

		public override int MaxProgress => 4;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			List<Figure> uniqueTargetedFigures = [];
			List<AbilityCardSide> abilityCardSides = [];

			ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
				parameters => parameters.Performer == character,
				async _ =>
				{
					uniqueTargetedFigures.Clear();
					await GDTask.CompletedTask;
				});

			ScenarioEvents.AbilityPerformedEvent.Subscribe(this,
				parameters => parameters.Performer == character && parameters.AbilityState is TargetedAbilityState,
				async parameters =>
				{
					foreach(Figure figure in ((TargetedAbilityState)parameters.AbilityState).UniqueTargetedFigures)
					{
						uniqueTargetedFigures.AddIfNew(figure);
					}

					await GDTask.CompletedTask;
				});

			ScenarioEvents.AbilityCardSideEndedEvent.Subscribe(this,
				parameters => !abilityCardSides.Contains(parameters.AbilityCardSide) && parameters.Performer == character &&
				              uniqueTargetedFigures.Count >= 3,
				async parameters =>
				{
					abilityCardSides.Add(parameters.AbilityCardSide);
					mastery.AdjustProgress();
					await GDTask.CompletedTask;
				});
		}
	}
}