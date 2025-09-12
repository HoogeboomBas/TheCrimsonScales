using System.Collections.Generic;
using Fractural.Tasks;

public class HideAndSeek : MirefootCardModel<HideAndSeek.CardTop, HideAndSeek.CardBottom>
{
	public override string Name => "Hide and Seek";
	public override int Level => 3;
	public override int Initiative => 43;
	protected override int AtlasIndex => 16;

	public class CardTop : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async abilityState =>
					{
						List<Hex> selectedHexes = await AbilityCmd.SelectHexes(abilityState,
							list =>
							{
								foreach(Hex possibleHex in RangeHelper.GetHexesInRange(abilityState.Performer.Hex, 3, true))
								{
									if(possibleHex != null && possibleHex.IsFeatureless())
									{
										list.Add(possibleHex);
									}
								}
							},
							0, 1, false, "Place difficult terrain in a featureless hex"
						);

						foreach(Hex selectedHex in selectedHexes)
						{
							await CreateDifficultTerrain(selectedHex);

							abilityState.SetPerformed();
						}
					}
				)
				.Build()),

			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.WithOnAbilityEnded(async state =>
				{
					List<Hex> selectedHexes = await AbilityCmd.SelectHexes(state,
						list =>
						{
							foreach(Hex possibleHex in RangeHelper.GetHexesInRange(state.Performer.Hex, 3, true))
							{
								if(possibleHex.HasHexObjectOfType<DifficultTerrain>())
								{
									list.Add(possibleHex);
								}
							}
						},
						0, 1, false, "Select additional hexes to loot"
					);

					foreach(Hex selectedHex in selectedHexes)
					{
						await LootAbility.LootHex(state, selectedHex, state.Performer);
					}

					if(state.TotalLootedCount >= 3)
					{
						await AbilityCmd.GainXP(state.Performer, 1);
					}
				})
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
					}
				)
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder().WithDistance(3).Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Invisible)
				.WithTarget(Target.Self)
				.WithConditionalAbilityCheck(async state =>
					{
						await GDTask.CompletedTask;

						return state.Performer.Hex.HasHexObjectOfType<DifficultTerrain>();
					}
				)
				.Build())
		];
	}
}