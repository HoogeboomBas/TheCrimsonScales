using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BlackRose : ThornreaperCardModel<BlackRose.CardTop, BlackRose.CardBottom>
{
	public override string Name => "Black Rose";
	public override int Level => 1;
	public override int Initiative => 12;
	protected override int AtlasIndex => 11;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.3f, 0.3f)))
				.WithTarget(Target.TargetAll)
				.WithRange(2)
				.WithRangeType(RangeType.Melee)
				.WithConditions(Conditions.Immobilize)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					foreach (Figure figure in RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, true, true))
					{
						if (figure.Hex.IsFeatureless())
						{
							await CreateHazardousTerrain(figure.Hex, state.Performer);
						}
					}
					state.SetPerformed();
				})
				.Build()),

			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.Build()),

			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(2, new PushSquare(this, new Vector2(0.2f, 0.4f)))
				.WithTarget(Target.Enemies | Target.TargetAll)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];
		public override int XP => 2;
		public override bool Loss => true;
	}
}