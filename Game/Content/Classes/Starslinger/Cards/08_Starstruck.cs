using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Starstruck : StarslingerCardModel<Starstruck.CardTop, Starstruck.CardBottom>
{
	public override string Name => "Starstruck";
	public override int Level => 1;
	public override int Initiative => 80;
	protected override int AtlasIndex => 8;

	public class CardTop : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithPierce(1)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Yellow),
				]))
				.Build()),
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantAbilityState =>
				[
					ConditionAbility.Builder().WithConditions(Conditions.Bless).WithTarget(Target.Self).Build(),
					AttackAbility.Builder().WithDamage(3).WithPierce(1).Build()
				])
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithCustomGetTargets((abilityState, list) =>
				{
					AttackAbility.State attackAbilityState = abilityState.ActionState.GetAbilityState<AttackAbility.State>(0);

					foreach(Hex yellowHex in attackAbilityState.GetYellowAOEHexes())
					{
						foreach(Figure figure in yellowHex.GetHexObjectsOfType<Figure>())
						{
							list.Add(figure);
						}
					}
				})
				.Build())
		];

		protected override int XP => 2;
		protected override bool Loss => true;
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithRange(1)
				.WithOnAbilityEndedPerformed(async state =>
				{
					if(state.Target.AlliedWith(state.Performer))
					{
						await AbilityCmd.InfuseElement(Element.Light);
					}
				})
				.Build())
		];
	}
}