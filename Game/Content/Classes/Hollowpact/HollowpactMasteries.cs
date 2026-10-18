using Fractural.Tasks;

public class HollowpactMasteries
{
	public class Mastery1 : MasteryModel
	{
		public override string Description =>
			$"Spend at least 2{Icons.Inline(Hollowpact.VoidEnergy)} each round for four consecutive rounds.";

		public override int MaxProgress => 4;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			int roundVoidSpent = 0;

			ScenarioEvents.VoidEnergySpentEvent.Subscribe(this,
				parameters => parameters.TokenSpender == character,
				async parameters =>
				{
					roundVoidSpent += parameters.VoidSpent;
					await GDTask.CompletedTask;
				});

			ScenarioEvents.RoundEndedEvent.Subscribe(this,
				_ => !mastery.ProgressFull,
				async _ =>
				{
					if(roundVoidSpent >= 2)
					{
						mastery.AdjustProgress();
					}
					else
					{
						mastery.ResetProgress();
					}

					roundVoidSpent = 0;
					await GDTask.CompletedTask;
				});
		}
	}

	public class Mastery2 : MasteryModel
	{
		public override string Description => "In a single scenario, destroy 9 void pit obstacles using Hollowpact actions.";

		public override int MaxProgress => 9;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			ScenarioEvents.HexObjectDestroyedEvent.Subscribe(this,
				parameters => parameters.HexObject is VoidPit &&
				              parameters.PotentialAbilityState.ActionState.ActionSource is AbilityCardSide cardSide &&
				              cardSide.Model is HollowpactCardSide && parameters.PotentialDestroyer == character,
				async _ =>
				{
					mastery.AdjustProgress();
					await GDTask.CompletedTask;
				});
		}
	}
}