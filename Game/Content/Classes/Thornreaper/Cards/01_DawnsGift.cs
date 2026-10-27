using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class DawnsGift : ThornreaperCardModel<DawnsGift.CardTop, DawnsGift.CardBottom>
{
	public override string Name => "Dawn's Gift";
	public override int Level => 1;
	public override int Initiative => 56;
	protected override int AtlasIndex => 7;

	public class CardTop : ThornreaperCardSide
	{
		public Hex SelectedHex;
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async abilityState =>
				{
					Hex selectedHex = await AbilityCmd.SelectHex(abilityState , list =>
						{
							foreach (Hex possibleHex in RangeHelper.GetHexesInRange(abilityState.Performer.Hex, 1, true))
							{
								if (possibleHex != null && possibleHex.IsFeatureless())
								{
									list.Add(possibleHex);
								}
							}
						},
					false, "Create one 1-hex hazardous terrain in one adjacent featureless hex");
					if (selectedHex != null)
					{
						await CreateHazardousTerrain(selectedHex, abilityState.Performer);
					}

					if (GetLightElementState() is ElementState.Waning or ElementState.Strong)
					{
						ScenarioEvents.AbilityStartedEvent.Subscribe(abilityState, this, parameters => parameters.Performer == abilityState.Performer,
						async parameters =>
						{
								await AbilityCmd.InfuseElement(abilityState, Element.Earth);
								if (parameters.AbilityState is LootAbility.State lootAbilityState && selectedHex != null)
								{
									lootAbilityState.SetPerformHex(selectedHex);
								}
								abilityState.SetPerformed();
							
						}, EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters("res://Art/OverlayTiles/Thorns 1h.png"),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							"Perform the loot ability as if you were occupying the hex of the created hazardous terrain"));

						ScenarioEvents.AbilityEndedEvent.Subscribe(abilityState, this, parameters => parameters.Performer == abilityState.Performer,
							async parameters =>
							{
								if (parameters.AbilityState is LootAbility.State)
								{
									ScenarioEvents.AbilityStartedEvent.Unsubscribe(abilityState, this);
								}
							});
					}
				})
				.Build()),

			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.Build())
		];
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveSquare(this, new Vector2(0.5f, 0.6f)))
				.Build()
			)
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}
}