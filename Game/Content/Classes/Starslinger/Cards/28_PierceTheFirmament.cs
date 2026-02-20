using System.Collections.Generic;
using Godot;
using System.Linq;
using Fractural.Tasks;

public class PierceTheFirmament : StarslingerCardModel<PierceTheFirmament.CardTop, PierceTheFirmament.CardBottom>
{
	public override string Name => "Interplanar Voyage";
	public override int Level => 9;
	public override int Initiative => 33;
	protected override int AtlasIndex => 28;

	public class CardTop : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(6)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.New(
						parameters => !parameters.Performer.IsDamaged(),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustRange(5);
							parameters.AbilityState.SingleTargetSetRangeType(RangeType.Range);

							await GDTask.CompletedTask;
						}
					)
				)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements =>
			[CardElementInfusion.Infuse(Element.Dark), CardElementInfusion.Infuse(Element.Light)];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(2);
							await GDTask.CompletedTask;
						});
					ScenarioEvents.DuringHealEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustHealValue(2);
							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
					ScenarioEvents.DuringHealEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Round => true;
	}
}