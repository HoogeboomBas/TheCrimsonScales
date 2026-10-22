using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class FissiveEruption : ThornreaperLevelUpCardModel<FissiveEruption.CardTop, FissiveEruption.CardBottom>
{
	public override string Name => "Fissive Eruption";
	public override int Level => 8;
	public override int Initiative => 40;
	protected override int AtlasIndex => 27;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			ConsumeEarthOrSkipAction(),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					List<Hex> selected = await AbilityCmd.SelectHexes(state, list =>
					{
						foreach (Hex hex in RangeHelper.GetHexesInRange(state.Performer.Hex, 4, true, true))
						{
							if (hex != null && hex.IsFeatureless())
								list.Add(hex);
						}
					}, 0, 2, true, "Create hazardous terrain in two featureless hexes within four hexes");

					foreach (Hex hex in selected)
					{
						await CreateHazardousTerrain(hex, state.Performer);
					}
					var attack = new AttackAbility.AttackBuilder()
							.WithDamage(4, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
							.WithCustomGetTargets((state, list) =>
							{
								foreach (Hex hex in selected)
								{
									foreach (Figure figure in RangeHelper.GetFiguresInRange(hex, 0, true, true))
									{
										list.Add(figure);
									}
								}
							})
							.Build();
					await attack.Perform(state.ActionState);
					state.SetPerformed();
				})
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveSquare(this, new Vector2(0.2f, 0.3f)))
				.WithMoveType(MoveType.Jump)
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Hex hex = state.Performer.Hex;
					if (hex.IsFeatureless())
					{
						await CreateHazardousTerrain(hex, state.Performer);
						state.SetPerformed();
					}
				})
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}
}
