using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class StalkingQuarry : HollowpactLevelUpCardModel<StalkingQuarry.CardTop, StalkingQuarry.CardBottom>
{
	public override string Name => "Stalking Quarry";
	public override int Level => 5;
	public override int Initiative => 14;
	protected override int AtlasIndex => 7;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(TeleportAbility.Builder()
				.WithDistance(3)
				.WithFilterHexes((state, teleportTargetHex) =>
				{
					return teleportTargetHex.Neighbours
						.Any(potentialEnemyHex =>
							potentialEnemyHex.GetFigures().Any(figure => figure.EnemiesWith(state.Performer))
							&& !potentialEnemyHex.Neighbours.Any(otherFigureHex => otherFigureHex.GetHexObjectsOfType<Figure>().Any(figure => figure != state.Performer)));
				})
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.6165889f, 0.33968875f)))
				.WithDuringAttackSubscriptions([
					LoseVoidEnergySubscription<ScenarioEvents.DuringAttack.Parameters>(2,
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(2);

							await GDTask.CompletedTask;
						},
						new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Damage)}")),

					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Dark,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Immobilize);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Immobilize))}")
					)
				])
				.Build())
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(4).Build()),

			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1, new ShieldCircle(this, new Vector2(0.6202777f, 0.77042186f)))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];

		public override bool Round => true;
	}
}