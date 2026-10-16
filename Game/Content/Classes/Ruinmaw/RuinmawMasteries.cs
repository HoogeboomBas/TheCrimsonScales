using Fractural.Tasks;

public class RuinmawMasteries
{
	public class Mastery1 : MasteryModel
	{
		public override string Description =>
			$"In a single scenario, be {Icons.Inline(Ruinmaw.SatedIconPath)} at the start or end of each of your turns.";

		public override int MaxProgress => 1;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			mastery.AdjustProgress();

			bool sated = false;

			ScenarioEvents.FigureTurnStartedEvent.Subscribe(this,
				parameters => parameters.Figure == character,
				async _ =>
				{
					sated = RuinmawCardSide.IsSated(character);
					await GDTask.CompletedTask;
				});

			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
				parameters => parameters.Figure == character && !sated && !RuinmawCardSide.IsSated(character),
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
		public override string Description => $"{Icons.Inline(Icons.Push)} one enemy 7 or more hexes with a single attack.";

		public override int MaxProgress => 1;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			ScenarioEvents.AfterAttackPerformedEvent.Subscribe(this,
				parameters => parameters.Performer == character && character.EnemiesWith(parameters.AbilityState.Target) &&
				              parameters.AbilityState.SingleTargetState.PushHexes.Count >= 7,
				async _ =>
				{
					mastery.AdjustProgress();
					await GDTask.CompletedTask;
				});
		}
	}
}