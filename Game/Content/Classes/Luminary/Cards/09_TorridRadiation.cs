using System.Collections.Generic;
using System.Linq;
using Godot;

public class TorridRadiation : LuminaryCardModel<TorridRadiation.CardTop, TorridRadiation.CardBottom>
{
	public override string Name => "Torrid Radiation";
	public override int Level => 1;
	public override int Initiative => 76;
	protected override int AtlasIndex => 9;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.32640356f, 0.14524278f)))
				.WithPierce(1)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Empty),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
					]
				))
				.Build()),
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithCustomGetTargets((state, figures) => figures.Add(state.ActionState.GetAbilityState<AttackAbility.State>(0).Target))
				.WithConditionalAbilityCheck(async state =>
				{
					return await AbilityCmd.HasPerformedAbility(state, 0) &&
					       await AbilityCmd.AskConsumeElement(state.Performer, Element.Fire);
				})
				.WithOnAbilityEndedPerformed(async state => await AbilityCmd.GainXP(state.Performer, 1))
				.Build()),
			Scuttle(1, [Element.Dark]),
		];
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6213844f, 0.6537314f)))
				.Build()),
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, false)
						        .Where(figure => figure.EnemiesWith(state.Performer)));
				})
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithConditionalAbilityCheck(async state =>
				{
					return await AbilityCmd.AskConsumeElement(state.Performer, Element.Fire);
				})
				.WithOnAbilityEndedPerformed(async state =>
				{
					for(int i = 0; i < state.DamagedFigures.Count; i++)
					{
						await AbilityCmd.InfuseWildElement(state);
					}
				})
				.Build()),
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}