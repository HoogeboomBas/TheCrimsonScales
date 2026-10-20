using System.Collections.Generic;
using System.Data;
using Fractural.Tasks;
using Godot;

public class TwistedThistle : ThornreaperLevelUpCardModel<TwistedThistle.CardTop, TwistedThistle.CardBottom>
{
	public override string Name => "Twisted Thistle";
	public override int Level => 4;
	public override int Initiative => 26;
	protected override int AtlasIndex => 19;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Red)
				]), new AOEHexMark(Vector2I.Zero.Add(Direction.East), this, new Vector2(0.2f, 0.3f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => GetLightElementState() is ElementState.Strong or ElementState.Waning,
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustPush(1);
							await GDTask.CompletedTask;
						}))
				.WithAfterAttackPerformedSubscription(
					ScenarioEvents.AfterAttackPerformed.Subscription.New(
						parameters => true,
						async parameters =>
						{
							List<Hex> selectedHexes = await AbilityCmd.SelectHexes(parameters.AbilityState, list =>
							{
								foreach (Hex hex in parameters.AbilityState.TargetedHexes)
								{
									if (hex.IsFeatureless())
									{
										list.Add(hex);
									}
								}
							}, 0, 2, false,
							"Create hazardous terrain in up to two featureless targeted hexes");

							foreach (Hex hex in selectedHexes)
							{
								await CreateHazardousTerrain(hex);
							}
						})
				)
				.Build()),

			// new AbilityCardAbility(OtherAbility.Builder()
			// 	.WithPerformAbility(async state =>
			// 	{
			// 		// Create hazardous terrain in up to two featureless targeted hexes
			// 		// TODO: select from the AOE targets of the previous attack
			// 		List<Hex> selected = await AbilityCmd.SelectHexes(state, list =>
			// 		{
			// 			foreach (Hex hex in RangeHelper.GetHexesInRange(state.Performer.Hex, 2, false))
			// 			{
			// 				if (hex != null && hex.IsFeatureless())
			// 					list.Add(hex);
			// 			}
			// 		}, 0, 2, true, "Create hazardous terrain in up to two featureless hexes");

			// 		foreach (Hex hex in selected)
			// 		{
			// 			await CreateHazardousTerrain(hex, state.Performer);
			// 		}
			// 		state.SetPerformed();
			// 	})
			// 	.Build())
		];
	}

	public class CardBottom : ThornreaperCardSide
	{
		private int _terrainDestroyed = 0;
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveSquare(this, new Vector2(0.2f, 0.3f)))
				.WithOnAbilityStarted(async abilityState =>
				{
					ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(abilityState, this,
						canApplyParameters => canApplyParameters.PotentialAbilityState?.Performer == abilityState.Performer,
						applyParameters =>
						{
							applyParameters.SetAffectedByHazardousTerrain(false);
							return GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;

					ScenarioEvents.FigureExitingHexEvent.Subscribe(abilityState, this,
						canApplyParameters => canApplyParameters.PotentialAbilityState?.Performer == abilityState.Performer && canApplyParameters.Hex.HasHexObjectOfType<HazardousTerrain>(),
						async applyParameters =>
						{
							await AbilityCmd.DestroyHazardousTerrain(applyParameters.Hex.GetHexObjectOfType<HazardousTerrain>());
						}, EffectType.Selectable);

					ScenarioEvents.HexObjectDestroyedEvent.Subscribe(abilityState, this,
						parameters => parameters.HexObject is HazardousTerrain,
						async applyParameters =>
						{
							_terrainDestroyed++;
							await GDTask.CompletedTask;
						});
				})
				.WithOnAbilityEnded(async abilityState =>
				{
					ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(abilityState, this);
					ScenarioEvents.FigureExitingHexEvent.Unsubscribe(abilityState, this);
					ScenarioEvents.HexObjectDestroyedEvent.Unsubscribe(abilityState, this);
					await GDTask.CompletedTask;
				})
				.Build()),

			// TODO: "You may remove hazardous terrain created by you from any hex you exit with this movement
			// to perform Retaliate X where X is the number of hexes from which you removed hazardous terrain."
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(_terrainDestroyed)
				.Build())
		];

		public override bool Round => true;
	}
}
