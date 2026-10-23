using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Steamburst : RimehearthCardModel<Steamburst.CardTop, Steamburst.CardBottom>
{
	public override string Name => "Steamburst";
	public override int Level => 1;
	public override int Initiative => 41;
	protected override int AtlasIndex => 9;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.61725044f, 0.16378893f), EnhancementCostType.MultiTarget),
					new AttackDiamond(this, new Vector2(0.70542216f, 0.16343491f), EnhancementCostType.MultiTarget))
				.WithDuringAttackSubscriptions(
					[
						ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
							applyFunction: async applyParameters =>
							{
								applyParameters.AbilityState.AdjustTargets(1);

								await AbilityCmd.GainXP(applyParameters.Performer, 1);
							},
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Targets)}")
						),
						ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Ice,
							applyFunction: async applyParameters =>
							{
								applyParameters.AbilityState.AbilityAdjustAttackValue(2);

								await AbilityCmd.GainXP(applyParameters.Performer, 1);
							},
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Attack)}")
						),
						ScenarioEvents.DuringAttack.Subscription.ConsumeElement([CardElementConsumption.Consume([Element.Fire, Element.Ice])],
							applyFunction: async applyParameters =>
							{
								applyParameters.AbilityState.AbilitySetHasAdvantage();

								await AbilityCmd.GainXP(applyParameters.Performer, 1);
							},
							effectInfoViewParameters: new TextEffectInfoView.Parameters("advantage")
						)
					]
				)
				.Build()),
		];

		public override bool Loss => true;
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithMoveType(MoveType.Jump)
				.WithOnAbilityStarted(async state =>
				{
					if(state.Performer.HasWound())
					{
						state.AdjustMoveValue(1);
					}

					if(state.Performer.HasCondition(Conditions.Chill))
					{
						state.AdjustMoveValue(1);
					}

					await GDTask.CompletedTask;
				})
				.Build())
		];
	}
}