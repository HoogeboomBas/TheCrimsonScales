using System.Collections.Generic;
using System.Linq;

public class BorrowedVitality : HollowpactCardModel<BorrowedVitality.CardTop, BorrowedVitality.CardBottom>
{
	public override string Name => "Borrowed Vitality";
	public override int Level => 1;
	public override int Initiative => 37;
	protected override int AtlasIndex => 5;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithTarget(Target.Self)
				.WithDuringHealSubscription(ScenarioEvents.DuringHeal.Subscription.New(
					parameters => true,
					async parameters =>
					{
						Figure figure = await AbilityCmd.SelectFigure(parameters.AbilityState,
							list =>
							{
								list.AddRange(RangeHelper.GetFiguresInRange(parameters.Performer.Hex, range: 1, includeOrigin: false)
									.Where(figure => parameters.Performer.AlliedWith(figure)));
							});

						if(figure == null)
						{
							return;
						}

						await AbilityCmd.SufferDamage(parameters.AbilityState, figure, 2);

						parameters.AbilityState.AbilityAdjustHealValue(2);

						await GainVoidEnergy(parameters.AbilityState, 2);
					}, 
					effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Heal)}, +2{Icons.Inline(Hollowpact.VoidEnergy)}")))
				.Build())
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithTarget(Target.Self)
				.WithConditions(Conditions.Regenerate)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Dark))
				.WithOnAbilityEndedPerformed(state => AbilityCmd.GainXP(state.Performer, 1))
				.Build())
		];
	}
}