using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class MeteorHammer : ChainguardLevelUpCardModel<MeteorHammer.CardTop, MeteorHammer.CardBottom>
{
	public override string Name => "Meteor Hammer";
	public override int Level => 7;
	public override int Initiative => 45;
	protected override int AtlasIndex => 15 - 11;

	public class CardTop : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(5, new AttackDiamond(this, new Vector2(0.6216293f, 0.23962134f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.Target.HasCondition(Chainguard.Shackle),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Disarm);

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
				.WithDistance(3, new MoveCircle(this, new Vector2(0.620109f, 0.6659502f)))
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						canApply: canApplyParameters => state.Performer == canApplyParameters.Performer &&
						                                canApplyParameters.AbilityState.Target.HasCondition(Chainguard.Shackle),
						async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetSetIgnoresAllShields();

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