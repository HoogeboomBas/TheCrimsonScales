using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class LuminousBlitz : StarslingerCardModel<LuminousBlitz.CardTop, LuminousBlitz.CardBottom>
{
	public override string Name => "Luminous Blitz";
	public override int Level => 1;
	public override int Initiative => 17;
	protected override int AtlasIndex => 6;

	public class CardTop : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Yellow),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
				]))
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
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
				.WithOnAbilityEndedPerformed(async state =>
					{
						await AbilityCmd.GainXP(state.Performer, 1);
					}
				)
				.Build())
		];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithTarget(Target.Self)
				.Build()),
		];
	}
}