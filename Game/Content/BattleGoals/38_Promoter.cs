using System.Linq;
using Fractural.Tasks;

public class Promoter : TheCrimsonScalesBattleGoal
{
	public override string Title => "Promoter";
	public override string Description => "Perform an ability targeting an ally before your first rest and in between each of your rests.";

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		bool targetedAlly = false;

		ScenarioEvents.AbilityPerformedEvent.Subscribe(this,
			parameters => 
				parameters.Performer == character &&
				parameters.AbilityState is TargetedAbilityState targetedState &&
				targetedState.UniqueTargetedFigures.Any(figure => figure.AlliedWith(character)),
			async parameters =>
			{
				targetedAlly = true;

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.ShortRestStartedEvent.Subscribe(this,
			parameters => 
				parameters.Character == character,
			async parameters =>
			{
				if (targetedAlly)
				{
					targetedAlly = false;
				}
				else
				{
					battleGoal.AdjustProgress(1);
				}

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.LongRestStartedEvent.Subscribe(this,
			parameters => 
				parameters.Character == character,
			async parameters =>
			{
				if (targetedAlly)
				{
					targetedAlly = false;
				}
				else
				{
					battleGoal.AdjustProgress(1);
				}

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}