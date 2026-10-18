using System.Linq;
using Fractural.Tasks;

public class MirefootMasteries
{
	public class Mastery1 : MasteryModel
	{
		public override string Description =>
			$"For an entire scenario, give an enemy {Icons.Inline(Icons.GetCondition(Conditions.Poison1))} on each of your turns that starts with an enemy on the map that does not have {Icons.Inline(Icons.GetCondition(Conditions.Poison1))}.";

		public override int MaxProgress => 1;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			mastery.AdjustProgress();

			bool gavePoison = false;

			ScenarioEvents.FigureTurnStartedEvent.Subscribe(this,
				parameters => parameters.Figure == character,
				async _ =>
				{
					gavePoison = !GameController.Instance.Map.Figures.Where(figure => character.EnemiesWith(figure))
						.Any(figure => !figure.HasPoison());
					await GDTask.CompletedTask;
				});

			ScenarioEvents.ConditionAddedEvent.Subscribe(this,
				parameters => parameters.PotentialConditionGiver == character && parameters.ConditionModel is Poison &&
				              character.EnemiesWith(parameters.Target),
				async _ =>
				{
					gavePoison = true;
					await GDTask.CompletedTask;
				});

			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
				parameters => parameters.Figure == character && !gavePoison,
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
		public override string Description => "In a single scenario, deal 12 or more damage from attacks in a single turn 4 or more times.";

		public override int MaxProgress => 4;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			int currentDamage = 0;

			ScenarioEvents.FigureTurnStartedEvent.Subscribe(this,
				parameters => parameters.Figure == character,
				async _ =>
				{
					currentDamage = 0;
					await GDTask.CompletedTask;
				});

			ScenarioEvents.AfterSufferDamageEvent.Subscribe(this,
				parameters => parameters.SufferDamageParameters.PotentialDamageDealer == character && parameters.SufferDamageParameters.FromAttack,
				async parameters =>
				{
					currentDamage += parameters.DamageDealt;
					await GDTask.CompletedTask;
				});

			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
				parameters => parameters.Figure == character && currentDamage >= 12,
				async _ =>
				{
					mastery.AdjustProgress();
					await GDTask.CompletedTask;
				});
		}
	}
}