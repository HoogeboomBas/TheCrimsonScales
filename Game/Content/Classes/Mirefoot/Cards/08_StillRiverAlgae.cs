using System.Collections.Generic;

public class StillRiverAlgae : MirefootCardModel<StillRiverAlgae.CardTop, StillRiverAlgae.CardBottom>
{
	public override string Name => "Still River Algae";
	public override int Level => 1;
	public override int Initiative => 09;
	protected override int AtlasIndex => 8;

	public class CardTop : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder().WithRange(1).Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Strengthen)
				.WithRange(1)
				.Build())
		];
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async abilityState =>
				{
					Hex hex = abilityState.Performer.Hex;

					if(hex.IsFeatureless())
					{
						List<Hex> selectedHexes =
							await AbilityCmd.SelectHexes(abilityState, list => list.Add(hex), 0, 1, true, "Place difficult terrain?");

						foreach(Hex selectedHex in selectedHexes)
						{
							await CreateDifficultTerrain(selectedHex);
							abilityState.SetPerformed();
						}
					}
				})
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder().WithDistance(3).Build())
		];
	}
}