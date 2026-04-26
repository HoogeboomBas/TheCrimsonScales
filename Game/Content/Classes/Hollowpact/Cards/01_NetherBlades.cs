using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class NetherBlades : HollowpactCardModel<NetherBlades.CardTop, NetherBlades.CardBottom>
{
	public override string Name => "Nether Blades";
	public override int Level => 1;
	public override int Initiative => 55;
	protected override int AtlasIndex => 1;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(VoidsightAbility.Builder().Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
				]))
				.Build())
			
			//TODO: Produce 1 for each target
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.WithConditionalAbilityCheck(async state =>
				{
					//TODO: Don't do if consumed
					await GDTask.CompletedTask;

					return true;
				})
				.Build()),
			
			new AbilityCardAbility(TeleportAbility.Builder()
				.WithDistance(4)
				.WithConditionalAbilityCheck(async state =>
				{
					//TODO: Do if consumed
					await GDTask.CompletedTask;

					return false;
				})
				.Build()),
		];
		
		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];
	}
}