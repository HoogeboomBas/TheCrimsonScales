using System.Collections.Generic;
using Fractural.Tasks;

public class StandingGround : HierophantCardModel<StandingGround.CardTop, StandingGround.CardBottom>
{
	public override string Name => "Standing Ground";
	public override int Level => 1;
	public override int Initiative => 22;
	protected override int AtlasIndex => 13 - 1;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state =>
					[
						ShieldAbility.Builder()
							.WithShieldValue(2)
							.WithPierceable(false)
							.Build()
					]
				)
				.Build())
		];

		protected override IEnumerable<Element> Elements => [Element.Earth];
		protected override bool Round => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithRangeType(RangeType.Range)
				.WithCustomGetTargets((state, list) =>
					{
						foreach(Figure figure in GameController.Instance.Map.Figures)
						{
							foreach(Figure potentialAlly in RangeHelper.GetFiguresInRange(figure.Hex, 1))
							{
								if(state.Performer.AlliedWith(potentialAlly))
								{
									list.Add(figure);
									break;
								}
							}
						}
					}
				)
				.Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state =>
				[
					ShieldAbility.Builder()
						.WithShieldValue(1)
						.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Earth))
						.WithOnAbilityEndedPerformed(async state =>
						{
							await GDTask.CompletedTask;

							state.ActionState.SetOverrideRound();
						})
						.Build()
				])
				.WithCustomGetTargets((state, list) =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					foreach(Figure target in attackAbilityState.UniqueTargetedFigures)
					{
						list.AddRange(RangeHelper.GetFiguresInRange(target.Hex, 1));
					}
				})
				.WithConditionalAbilityCheck(async state =>
					{
						await GDTask.CompletedTask;

						AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

						return attackAbilityState.Performed;
					}
				)
				.Build())
		];
	}
}