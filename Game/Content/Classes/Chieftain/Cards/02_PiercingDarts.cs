using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class PiercingDarts : ChieftainCardModel<PiercingDarts.CardTop, PiercingDarts.CardBottom>
{
	public override string Name => "Piercing Darts";
	public override int Level => 1;
	public override int Initiative => 17;
	protected override int AtlasIndex => 2;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithRange(3, new RangeSquare(this, new Vector2(0.648278f, 0.23412162f)))
				.WithTargets(2, new TargetsSquare(this, new Vector2(0.43628073f, 0.23412162f)))
				.WithPierce(2)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Earth,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AbilityAddCondition(Conditions.Poison1);

							await AbilityCmd.GainXP(applyParameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Poison1))}")
					)
				)
				.Build())
		];
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							if(Chieftain.GetIsMounted(state.Performer))
							{
								parameters.AbilityState.SingleTargetAdjustAttackValue(1);
								parameters.AbilityState.SingleTargetAdjustPierce(1);
							}

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()
			),
		];

		public override bool Round => true;
	}
}