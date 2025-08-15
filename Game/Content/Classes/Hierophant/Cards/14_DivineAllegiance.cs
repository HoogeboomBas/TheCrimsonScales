using System.Collections.Generic;
using Godot;

public class DivineAllegiance : HierophantCardModel<DivineAllegiance.CardTop, DivineAllegiance.CardBottom>
{
	public override string Name => "Divine Allegiance";
	public override int Level => 2;
	public override int Initiative => 63;
	protected override int AtlasIndex => 29 - 14;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new AttackAbility(4, range: 3,
				aoePattern: new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					]
				)
			)),

			new AbilityCardAbility(new OtherAbility(
				async state =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					foreach(Figure targetedFigure in attackAbilityState.UniqueTargetedFigures)
					{
						if(!targetedFigure.IsDead)
						{
							await AbilityCmd.SufferDamage(null, targetedFigure, 1);
						}
					}
				},
				conditionalAbilityCheck: state => AbilityCmd.HasPerformedAbility(state, 0)
			))
		];

		protected override IEnumerable<Element> Elements => [Element.Light];
		protected override bool Loss => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new MoveAbility(3)),

			new AbilityCardAbility(new ConditionAbility([Conditions.Bless], target: Target.Allies, range: 1))
		];
	}
}