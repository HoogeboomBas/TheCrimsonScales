using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class DiamondRings : StarslingerCardModel<DiamondRings.CardTop, DiamondRings.CardBottom>
{
	public override string Name => "Diamond Rings";
	public override int Level => 1;
	public override int Initiative => 35;
	protected override int AtlasIndex => 2;

	public class CardTop : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.4732318f, 0.20598967f)))
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Yellow),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Yellow),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthWest).Add(Direction.NorthEast), AOEHexType.Red),
				]))
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithTarget(Target.Allies | Target.TargetAll)
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

		public override int XP => 1;
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62076f, 0.72278136f)))
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}
}