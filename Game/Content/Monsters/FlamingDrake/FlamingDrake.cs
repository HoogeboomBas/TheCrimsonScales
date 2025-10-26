using System.Collections.Generic;

public class FlamingDrake : SpittingDrake
{
	public override string Name => "Flaming Drake";

	public override IEnumerable<MonsterAbilityCardModel> Deck => FlamingDrakeAbilityCard.Deck;
}