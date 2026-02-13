using System.Collections.Generic;
using System.Linq;

public class PouncingPredator : RuinmawCardModel<PouncingPredator.CardTop, PouncingPredator.CardBottom>
{
	public override string Name => "Pouncing Predator";
	public override int Level => 6;
	public override int Initiative => 11;
	protected override int AtlasIndex => 22;

	public class CardTop : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithPush(3)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => IsSated(parameters.Performer) && parameters.AbilityState.Target.IsDamaged(),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(2);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}
					)
				)
				.Build()),
		];
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(6)
				.WithMoveType(MoveType.Jump)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Figure figure = await AbilityCmd.SelectFigure(state, list =>
					{
						list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1).Where(figure => figure.EnemiesWith(state.Performer)));
					}, hintText: () => $"Select an enemy to suffer {Icons.HintText(Icons.Damage)}3");

					if(figure == null)
					{
						return;
					}

					await AbilityCmd.SufferDamage(state, figure, 3);
					await AbilityCmd.AddConditions(state, figure, [Conditions.Rupture, Conditions.Wound1]);
				})
				.Build())
		];

		protected override bool Sate => true;
		public override int XP => 2;
		public override bool Loss => true;
	}
}