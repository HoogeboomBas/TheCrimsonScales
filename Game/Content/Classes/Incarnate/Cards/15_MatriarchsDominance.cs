using System.Collections.Generic;
using Godot;

public class MatriarchsDominance : IncarnateCardModel<MatriarchsDominance.CardTop, MatriarchsDominance.CardBottom>
{
	public override string Name => "Matriarch's Dominance";
	public override int Level => 2;
	public override int Initiative => 22;
	protected override int AtlasIndex => 15;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.4951317f, 0.22216067f)))
				.WithPierce(3)
				.Build()),
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1, new ShieldDiamondPlus(this, new Vector2(0.52084196f, 0.3357341f)))
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Conqueror))
				.WithOnAbilityEndedPerformed(async state =>
				{
					state.ActionState.SetOverrideRound();

					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.Build())
		];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Ritualist))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.InfuseElement(state, Element.Air);
				})
				.Build()),
			new AbilityCardAbility(ControlAbility.Builder()
				.WithAbilities(
				[
					MoveAbility.Builder().WithDistance(1).Build()
				])
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6202788f, 0.8386812f)))
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Reaver];
	}
}