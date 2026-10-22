using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Fractural.Tasks;
using Godot;

public class RabidUndergrowth : ThornreaperLevelUpCardModel<RabidUndergrowth.CardTop, RabidUndergrowth.CardBottom>
{
	public override string Name => "Rabid Undergrowth";
	public override int Level => 7;
	public override int Initiative => 27;
	protected override int AtlasIndex => 24;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			ConsumeEarthOrSkipAction(),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
				.WithRange(3)
				.WithPierce(2)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.NorthEast), AOEHexType.Red),
				]))
				.WithAfterAttackPerformedSubscription(
					ScenarioEvents.AfterAttackPerformed.Subscription.New(
						applyFunction: async applyParameters =>
						{
							var redHexes = applyParameters.AbilityState.GetRedAOEHexes();
							if (redHexes == null)
							{
								return;
							}

							foreach (Hex hex in redHexes)
							{
								if (hex != null && hex.IsFeatureless())
								{
									await CreateHazardousTerrain(hex, applyParameters.Performer);
								}
							}
						}
					)
				)
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					if (GetLightElementState() is ElementState.Strong or ElementState.Waning)
					{
						await AbilityCmd.InfuseElement(state, Element.Earth);
					}
					state.SetPerformed();
				})
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Hex selected = await AbilityCmd.SelectHex(state, list =>
					{
						foreach (Hex hex in RangeHelper.GetHexesInRange(state.Performer.Hex, 1, true))
						{
							if (hex != null && hex.IsFeatureless())
								list.Add(hex);
						}
					}, false, "Create hazardous terrain in one adjacent featureless hex");

					if (selected != null)
					{
						await CreateHazardousTerrain(selected, state.Performer);
						var attack = new AttackAbility.AttackBuilder()
							.WithDamage(3, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
							.WithCustomGetTargets((state, list) =>
							{
								foreach (Figure figure in RangeHelper.GetFiguresInRange(selected, 1, true, true))
								{
									list.Add(figure);
								}
							})
							.Build();
						await attack.Perform(state.ActionState);
						state.SetPerformed();
					}
				})
				.Build()),
		];
	}
}
