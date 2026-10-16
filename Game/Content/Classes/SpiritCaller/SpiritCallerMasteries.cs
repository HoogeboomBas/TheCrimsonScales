using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class SpiritCallerMasteries
{
	public class Mastery1 : MasteryModel
	{
		public override string Description => "In a single scenario, kill at least 5 enemies, but never attack.";

		public override int MaxProgress => 5;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			ScenarioEvents.FigureKilledEvent.Subscribe(this,
				parameters => parameters.PotentialKiller == character,
				async _ =>
				{
					mastery.AdjustProgress();
					await GDTask.CompletedTask;
				});

			ScenarioEvents.AfterAttackPerformedEvent.Subscribe(this,
				parameters => parameters.Performer == character,
				async _ =>
				{
					mastery.SetTemporarilyFailed(true);
					await GDTask.CompletedTask;
				});
		}
	}

	public class Mastery2 : MasteryModel
	{
		public override string Description => "In a single scenario, have a Spirit survive for 5 rounds.";

		public override int MaxProgress => 5;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			Dictionary<Spirit, int> spawns = new Dictionary<Spirit, int>();

			ScenarioEvents.AbilityPerformedEvent.Subscribe(this,
				parameters => !mastery.ProgressFull && parameters.Performer == character && parameters.AbilityState is SpawnAbility.State,
				async parameters =>
				{
					Spirit spirit = ((SpawnAbility.State)parameters.AbilityState).Spirit;
					spawns.Add(spirit, 0);
					ScenarioEvents.FigureKilledEvent.Subscribe(spirit, this,
						killParameters => killParameters.Figure == spirit,
						async _ =>
						{
							spawns.Remove(spirit);
							ScenarioEvents.FigureKilledEvent.Unsubscribe(spirit, this);
							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				});

			ScenarioEvents.RoundEndedEvent.Subscribe(this,
				_ => !mastery.ProgressFull,
				async _ =>
				{
					foreach(Spirit spirit in spawns.Keys)
					{
						spawns[spirit]++;
					}

					mastery.SetProgress(spawns.Values.Max());
					await GDTask.CompletedTask;
				});
		}
	}
}