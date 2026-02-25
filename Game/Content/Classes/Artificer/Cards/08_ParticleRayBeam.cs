using System.Collections.Generic;
using Godot;

public class ParticleRayBeam : ArtificerCardModel<ParticleRayBeam.CardTop, ParticleRayBeam.CardBottom>
{
	public override string Name => "Particle Ray Beam";
	public override int Level => 1;
	public override int Initiative => 36;
	protected override int AtlasIndex => 8;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.4463667f, 0.24074073f)))
				.WithPierce(2, new PierceSquare(this, new Vector2(0.66888887f, 0.24034072f)))
				.WithAOEPattern(new AOEPattern(
				[
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.East), AOEHexType.Red)
				]))
				.Build())
		];

		public override int XP => 1;
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