using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class BombardMasteries
{
	public class Mastery1 : MasteryModel
	{
		public override string Description =>
			$"In a single scenario, successfully resolve 4 different {Icons.Inline(BombardCardSide.ProjectileIconPath)} abilities twice each.";

		public override int MaxProgress => 4;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			Dictionary<AbilityCardSide, int> projectileSides = [];

			ScenarioEvents.AbilityPerformedEvent.Subscribe(this,
				parameters => parameters.Performer == character &&
				              (parameters.AbilityState.ActionState.ParentActionState?.AbilityStates.Any(state => state is ProjectileAbility.State) ??
				               false),
				async parameters =>
				{
					AbilityCardSide cardSide = (AbilityCardSide)parameters.AbilityState.ActionState.ActionSource;
					if(projectileSides.ContainsKey(cardSide))
					{
						if(projectileSides[cardSide] == 1)
						{
							mastery.AdjustProgress();
							projectileSides[cardSide]++;
						}
					}
					else
					{
						projectileSides.Add(cardSide, 1);
					}

					await GDTask.CompletedTask;
				});
		}
	}

	public class Mastery2 : MasteryModel
	{
		public override string Description =>
			$"For an entire scenario, deal more {Icons.Inline(Icons.Damage)} with {Icons.Inline(Icons.Retaliate)} than you suffer whenever you are attacked, and be targeted by at least 5 attacks.";

		public override int MaxProgress => 5;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			ScenarioEvents.AfterAttackPerformedEvent.Subscribe(this,
				parameters => parameters.AbilityState.Target == character,
				async _ =>
				{
					mastery.AdjustProgress();
					await GDTask.CompletedTask;
				});

			ScenarioEvents.AfterSufferDamageEvent.Subscribe(this,
				parameters => parameters.SufferDamageParameters.FromAttack && parameters.Figure == character,
				async parameters =>
				{
					ScenarioEvents.RetaliateEvent.Subscribe(parameters.PotentialAbilityState, this,
						retaliateParameters => retaliateParameters.RetaliatingFigure == character &&
						                       retaliateParameters.AbilityState == parameters.PotentialAbilityState &&
						                       retaliateParameters.Retaliate <= parameters.DamageSuffered,
						async _ =>
						{
							mastery.SetTemporarilyFailed(true);
							await GDTask.CompletedTask;
						}, order: 1000);

					await GDTask.CompletedTask;
				});
		}
	}
}