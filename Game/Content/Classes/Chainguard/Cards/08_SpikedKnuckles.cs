using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SpikedKnuckles : ChainguardCardModel<SpikedKnuckles.CardTop, SpikedKnuckles.CardBottom>
{
	public override string Name => "Spiked Knuckles";
	public override int Level => 1;
	public override int Initiative => 66;
	protected override int AtlasIndex => 12 - 8;

	public class CardTop : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.51325023f, 0.24095783f)))
				.WithConditions(Conditions.Wound1)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.Target.HasCondition(Chainguard.Shackle),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						}
					)
				)
				.Build()),
		];
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62026906f, 0.7068813f)))
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer &&
						              parameters.AbilityState.Target.HasCondition(Chainguard.Shackle),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustPierce(1);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Round => true;
	}
}