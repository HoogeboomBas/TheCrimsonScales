using System.Collections.Generic;
using System.Linq;
using Godot;

public class ProlificPerforation : BombardCardModel<ProlificPerforation.CardTop, ProlificPerforation.CardBottom>
{
	public override string Name => "Prolific Perforation";
	public override int Level => 6;
	public override int Initiative => 62;
	protected override int AtlasIndex => 21;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.44518518f, 0.23872432f)))
				.WithPierce(1, new PierceSquare(this, new Vector2(0.66306293f, 0.23746613f)))
				.WithAOEPattern(new AOEPattern(
				[
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Empty),
					new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.East).Add(Direction.East), AOEHexType.Red)
				]))
				.Build())
		];
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(6, new MoveCircle(this, new Vector2(0.6192593f, 0.6550264f)))
				.Build()),
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(1, new PushCircle(this, new Vector2(0.2851852f, 0.78095233f)))
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(state.ActionState.GetAbilityState<MoveAbility.State>(0).Hexes
						.SelectMany(hex => RangeHelper.GetFiguresInRange(hex, 1)));
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}