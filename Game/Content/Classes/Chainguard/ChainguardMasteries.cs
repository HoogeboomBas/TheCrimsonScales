using System.Collections.Generic;
using Fractural.Tasks;

public class ChainguardMasteries
{
	public class Mastery1 : MasteryModel
	{
		public override string Description => $"Give {Icons.Inline(Icons.GetCondition(Chainguard.Shackle))} to 30 different enemies.";

		public override bool SingleScenario => false;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			List<Figure> shackledMonsters = [];

			ScenarioEvents.ConditionAddedEvent.Subscribe(this,
				parameters => parameters.PotentialConditionGiver == character && parameters.ConditionModel is Shackle &&
				              character.EnemiesWith(parameters.Target) && !shackledMonsters.Contains(parameters.Target),
				async parameters =>
				{
					shackledMonsters.Add(parameters.Target);
					mastery.AdjustProgress();
					await GDTask.CompletedTask;
				});
		}
	}

	public class Mastery2 : MasteryModel
	{
		public override string Description =>
			$"Deal at least {Icons.Inline(Icons.Damage)}20 to enemies with {Icons.Inline(Icons.GetCondition(Chainguard.Shackle))} in a single round.";

		public override int MaxProgress => 20;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			ScenarioEvents.AfterSufferDamageEvent.Subscribe(this,
				parameters => parameters.SufferDamageParameters.PotentialDamageDealer == character &&
				              character.EnemiesWith(parameters.Figure) && parameters.Figure.HasCondition(Chainguard.Shackle),
				async parameters =>
				{
					mastery.AdjustProgress(parameters.DamageDealt);
					await GDTask.CompletedTask;
				});

			ScenarioEvents.RoundEndedEvent.Subscribe(this,
				_ => !mastery.ProgressFull,
				async _ =>
				{
					mastery.ResetProgress();
					await GDTask.CompletedTask;
				});
		}
	}
}