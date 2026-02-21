using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class UntetheredAdvance : HollowpactCardModel<UntetheredAdvance.CardTop, UntetheredAdvance.CardBottom>
{
	public override string Name => "Untethered Advance";
	public override int Level => 1;
	public override int Initiative => 46;
	protected override int AtlasIndex => 6;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthWest), AOEHexType.Red)
					//TODO: Consume 1, +1dmg, +1xp
				]))
				.Build()),
			
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Dark))
				.Build())
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Hex hex = await AbilityCmd.SelectHex(state, list =>
					{
						list.AddRange(RangeHelper.GetHexesInRange(state.Performer.Hex, range: 1)
							.Where(hex => (hex.GetHexObjectsOfType<Trap>().Any(hexObject => !hexObject.CannotBeDestroyed) ||
							               hex.GetHexObjectsOfType<Obstacle>().Any(hexObject => !hexObject.CannotBeDestroyed))));
					});

					if(hex != null)
					{
						await hex.HexObjects.First(hexObject => hexObject is Trap or Obstacle).Destroy();
						//TODO: Produce 1
						
						state.SetPerformed();
					}
				})
				.Build()),
		];
	}
}