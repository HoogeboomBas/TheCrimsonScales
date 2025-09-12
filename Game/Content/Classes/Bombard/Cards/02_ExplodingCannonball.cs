using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ExplodingCannonball : BombardCardModel<ExplodingCannonball.CardTop, ExplodingCannonball.CardBottom>
{
	public override string Name => "Exploding Cannonball";
	public override int Level => 1;
	public override int Initiative => 88;
	protected override int AtlasIndex => 2;

	public class CardTop : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ProjectileAbility.Builder()
				.WithGetAbilities(hex =>
				[
					AttackAbility.Builder()
						.WithDamage(3)
						.WithRangeType(RangeType.Range)
						.WithPierce(1)
						.WithTargetHex(hex)
						.WithAOEPattern(new AOEPattern([
							new AOEHex(Vector2I.Zero, AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add((Direction)0), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add((Direction)1), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add((Direction)2), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add((Direction)3), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add((Direction)4), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add((Direction)5), AOEHexType.Red)
						]))
						.Build()
				])
				.WithAbilityCardSide(this)
				.WithRange(4)
				.Build())
		];

		protected override IEnumerable<Element> Elements => [Element.Earth];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithOnAbilityStarted(async abilityState =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Subscribe(abilityState, this,
						canApplyParameters =>
							canApplyParameters.AbilityState == abilityState &&
							(canApplyParameters.Hex.HasHexObjectOfType<DifficultTerrain>() ||
							 canApplyParameters.Hex.HasHexObjectOfType<HazardousTerrain>()),
						applyParameters =>
						{
							if(applyParameters.Hex.HasHexObjectOfType<DifficultTerrain>())
							{
								applyParameters.SetMoveCost(1);
							}

							if(applyParameters.Hex.HasHexObjectOfType<HazardousTerrain>())
							{
								applyParameters.SetAffectedByNegativeHex(false);
							}
						});

					ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(abilityState, this,
						canApplyParameters => canApplyParameters.AbilityState.Performer == abilityState.Performer,
						applyParameters =>
						{
							applyParameters.SetAffectedByHazardousTerrain(false);
							return GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async abilityState =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(abilityState, this);
					ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(abilityState, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];
	}
}