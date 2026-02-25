using System.Collections.Generic;
using System.Linq;
using Godot;

public class PhaseFieldEmitter : ArtificerCardModel<PhaseFieldEmitter.CardTop, PhaseFieldEmitter.CardBottom>
{
	public override string Name => "Phase Field Emitter";
	public override int Level => 1;
	public override int Initiative => 12;
	protected override int AtlasIndex => 6;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithRange(2)
				.WithAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Empty),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthWest), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.SouthWest), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.SouthEast).Add(Direction.SouthWest), AOEHexType.Red),
						]
					), new AOEHexMark(Vector2I.Zero.Add(Direction.West), this, new Vector2(0.6637037f, 0.27619046f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.East), this, new Vector2(0.8622222f, 0.27619046f)))
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithTarget(Target.TargetAll | Target.SelfOrAllies)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(state.ActionState.GetAbilityState<AttackAbility.State>(0).GetRedAOEHexes()
						.SelectMany(hex => hex.GetHexObjectsOfType<Figure>()));
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.6207408f, 0.7608465f)))
				.Build())
		];
	}
}