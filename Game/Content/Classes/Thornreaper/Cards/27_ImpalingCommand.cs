using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ImpalingCommand : ThornreaperLevelUpCardModel<ImpalingCommand.CardTop, ImpalingCommand.CardBottom>
{
	public override string Name => "Impaling Command";
	public override int Level => 8;
	public override int Initiative => 19;
	protected override int AtlasIndex => 26;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(6, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
				.WithPierce(3)
				.WithCustomGetTargets((state, list) =>
					{
						foreach (Hex hex in RangeHelper.GetHexesInRange(state.Performer.Hex, 5, true, true))
						{
							if (hex.HasHexObjectOfType<HazardousTerrain>() == true && hex.GetFigures() != null)
							{
								list.Add(hex.GetFigures().First());
							}
						}
					})
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					if (GetLightElementState() is ElementState.Waning)
					{
						await AbilityCmd.InfuseElement(state, Element.Light, state.Performer);
					}
				})
				.Build())
		];
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
				]), new AOEHexMark(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), this, new Vector2(0.2f, 0.3f)))
				.WithAfterAttackPerformedSubscription(
					ScenarioEvents.AfterAttackPerformed.Subscription.New(
						applyFunction: async applyParameters =>
						{
							var redHexes = applyParameters.AbilityState.GetRedAOEHexes();
							if (redHexes == null)
							{
								return;
							}

							Hex selectedHex = await AbilityCmd.SelectHex(
								applyParameters.AbilityState, 
								list =>
								{
									foreach (Hex hex in redHexes)
									{
										if (hex != null && hex.IsFeatureless())
										{
											list.Add(hex);
										}
									}
								}, 
								false, 
								"Create hazardous terrain in one featureless targeted hex");

							await CreateHazardousTerrain(selectedHex, applyParameters.Performer);
						}
					)
				)
				.Build())
		];
	}
}
