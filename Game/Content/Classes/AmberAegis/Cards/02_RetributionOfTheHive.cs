using System.Collections.Generic;
using Godot;

public class RetributionOfTheHive : AmberAegisCardModel<RetributionOfTheHive.CardTop, RetributionOfTheHive.CardBottom>
{
	public override string Name => "Retribution of the Hive";
	public override int Level => 1;
	public override int Initiative => 16;
	protected override int AtlasIndex => 2;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(2)
				.Build())
		];

		public override IEnumerable<Element> Elements => [Element.Fire];
		public override int XP => 1;
		public override bool Round => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6213409f, 0.65625197f)))
				.Build()),
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					RetaliateAbility.Builder()
						.WithRetaliateValue(2)
						.Build()
				])
				.WithRange(1)
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.Build())
		];

		public override bool Round => true;
	}
}