using System.Collections.Generic;
using Fractural.Tasks;

public class ChieftainMasteries
{
	public class Mastery1 : MasteryModel
	{
		public override string Description => "For an entire scenario, end each of your turns mounted on a summon you summoned on the first round.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			mastery.AdjustProgress();

			List<Summon> firstRoundSummons = [];

			ScenarioEvents.AbilityPerformedEvent.Subscribe(this,
				parameters => parameters.Performer == character && parameters.AbilityState is SummonAbility.State,
				async parameters =>
				{
					firstRoundSummons.Add(((SummonAbility.State)parameters.AbilityState).Summon);
					await GDTask.CompletedTask;
				});

			ScenarioEvents.RoundEndedEvent.Subscribe(this,
				_ => true,
				async _ =>
				{
					ScenarioEvents.AbilityPerformedEvent.Unsubscribe(this);
					ScenarioEvents.RoundEndedEvent.Unsubscribe(this);
					await GDTask.CompletedTask;
				});

			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
				parameters => parameters.Figure == character && !firstRoundSummons.Contains((Summon)Chieftain.GetMount(character)),
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
		public override string Description => "In a single scenario, perform at least two actions mounted on each of six different summons.";

		public override int MaxProgress => 6;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			Dictionary<Summon, int> mountedSides = [];

			ScenarioEvents.AbilityCardSideEndedEvent.Subscribe(this,
				parameters => parameters.Performer == character && Chieftain.GetIsMounted(character),
				async _ =>
				{
					Summon mount = (Summon)Chieftain.GetMount(character);
					if(mountedSides.ContainsKey(mount))
					{
						if(mountedSides[mount] == 1)
						{
							mastery.AdjustProgress();
							mountedSides[mount]++;
						}
					}
					else
					{
						mountedSides.Add(mount, 1);
					}

					await GDTask.CompletedTask;
				});
		}
	}
}