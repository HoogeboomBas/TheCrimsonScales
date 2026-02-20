using System.Collections.Generic;
using Godot;

public class DivineAllegiance : HierophantLevelUpCardModel<DivineAllegiance.CardTop, DivineAllegiance.CardBottom>
{
	public override string Name => "Divine Allegiance";
	public override int Level => 2;
	public override int Initiative => 63;
	protected override int AtlasIndex => 15 - 1;

	public class CardTop : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.41209206f, 0.1612586f)))
				.WithRange(3)
				.WithAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						]
					)
				)
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, state.UniqueTargetedFigures.Count);
				})
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					foreach(Figure targetedFigure in attackAbilityState.UniqueTargetedFigures)
					{
						if(!targetedFigure.IsDead)
						{
							await AbilityCmd.SufferDamage(state, targetedFigure, 1);
							state.SetPerformed();
						}
					}
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
		public override bool Loss => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.619769f, 0.72193056f)))
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Bless)
				.WithTarget(Target.Allies)
				.WithRange(1)
				.Build())
		];
	}
}