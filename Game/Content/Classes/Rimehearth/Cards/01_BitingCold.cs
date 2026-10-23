using System.Collections.Generic;
using System.Linq;
using Godot;

public class BitingCold : RimehearthCardModel<BitingCold.CardTop, BitingCold.CardBottom>
{
	public override string Name => "Biting Cold";
	public override int Level => 1;
	public override int Initiative => 60;
	protected override int AtlasIndex => 1;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(5)
				.WithRange(2)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => parameters.Performer.HasWound(),
						async parameters =>
						{
							await AbilityCmd.RemoveWound(parameters.Performer, parameters.AbilityState);

							((SufferDamageAbility.State)parameters.AbilityState).AbilityAdjustRange(2);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}, effectType: EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Wound1)),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Remove {Icons.Inline(Icons.GetCondition(Conditions.Wound1))} from self for +2{Icons.Inline(Icons.Range)}")
					)
				)
				.WithOnAbilityEndedPerformed(async state =>
				{
					Figure target = state.UniqueTargetedFigures[0];
					foreach(Figure figure in RangeHelper.GetFiguresInRange(target.Hex, 1, false))
					{
						await AbilityCmd.AddCondition(state, figure, Conditions.Chill);
					}
				})
				.Build())
		];

		public override int XP => 1;
		public override bool Loss => true;
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6210789f, 0.7463604f)))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Ice)];
	}
}