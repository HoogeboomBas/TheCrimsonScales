using System.Linq;
using Fractural.Tasks;

public class FireKnightMasteries
{
	public class Mastery1 : MasteryModel
	{
		public override string Description => "In a single scenario, grant allies 10 attacks with advantage.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			ScenarioEvents.AfterAttackPerformedEvent.Subscribe(this,
				parameters => character.AlliedWith(parameters.Performer) &&
				              parameters.AbilityState.ActionState.ParentActionState.Performer == character &&
				              parameters.AbilityState.ActionState.ParentActionState.AbilityStates.Any(state => state is GrantAbility.State) &&
				              parameters.AbilityState.SingleTargetHasAdvantage,
				async _ =>
				{
					mastery.AdjustProgress();
					await GDTask.CompletedTask;
				});
		}
	}

	public class Mastery2 : MasteryModel
	{
		public override string Description =>
			$"For an entire scenario, give {Icons.Inline(Icons.GetCondition(Conditions.Strengthen))} to an ally or give {Icons.Inline(Icons.GetCondition(Conditions.Wound1))} to an enemy during each of your turns except while long resting.";

		public override int MaxProgress => 1;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			mastery.AdjustProgress();

			bool gaveCondition = false;

			ScenarioEvents.ConditionAddedEvent.Subscribe(this,
				parameters => parameters.PotentialConditionGiver == character &&
				              ((parameters.ConditionModel is Wound && character.EnemiesWith(parameters.Target)) ||
				               (parameters.ConditionModel is Strengthen && character.AlliedWith(parameters.Target))),
				async _ =>
				{
					gaveCondition = true;
					await GDTask.CompletedTask;
				});

			ScenarioEvents.LongRestStartedEvent.Subscribe(this,
				parameters => parameters.Character == character,
				async _ =>
				{
					gaveCondition = true;
					await GDTask.CompletedTask;
				});

			ScenarioEvents.RoundEndedEvent.Subscribe(this,
				_ => !gaveCondition,
				async _ =>
				{
					mastery.ResetProgress();
					mastery.SetTemporarilyFailed(true);
					await GDTask.CompletedTask;
				});
		}
	}
}