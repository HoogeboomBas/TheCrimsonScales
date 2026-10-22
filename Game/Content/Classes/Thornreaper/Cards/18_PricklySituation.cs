using System.Collections.Generic;
using System.Data;
using Fractural.Tasks;
using Godot;

public class PricklySituation : ThornreaperLevelUpCardModel<PricklySituation.CardTop, PricklySituation.CardBottom>
{
	public override string Name => "Prickly Situation";
	public override int Level => 4;
	public override int Initiative => 20;
	protected override int AtlasIndex => 18;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			ConsumeEarthOrSkipAction(),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(6)
				.WithRange(2, new RangeSquare(this, new Vector2(0.2f, 0.3f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => true,
						async parameters =>
						{
							Hex performerHex = parameters.AbilityState.Performer.Hex;
							Hex targetHex = parameters.AbilityState.Target.Hex;

							HashSet<Hex> pathHexes = RangeHelper.GetHexesOnAnyShortestPath(performerHex, targetHex);

							List<Hex> selected = await AbilityCmd.SelectHexes(
								parameters.AbilityState,
								validHexes =>
								{
									foreach (Hex hex in pathHexes)
									{
										validHexes.Add(hex);
									}
								}, 1, parameters.AbilityState.AbilityRange, true,
								"Select path to attack target:");
							
							foreach (Hex hex in selected)
							{
								if (hex != null && hex.IsFeatureless())
								{
									await CreateHazardousTerrain(hex, parameters.AbilityState.Performer);
								}
							}
						}))
				.Build())
		];
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

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveSquare(this, new Vector2(0.2f, 0.3f)))
				.Build()),

			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.WithOnAbilityStarted(async state =>
				{
					if (GetLightElementState() is ElementState.Strong or ElementState.Waning)
					{
						state.AdjustAdditionalShield(1);
					}

					ScenarioEvents.FinishElementConsumedEvent.Subscribe(state, this, parameters => parameters.ConsumedElement == Element.Light,
					async parameters =>
					{
						state.AdjustAdditionalShield(-1);
						await GDTask.CompletedTask;
					});
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Round => true;
	}
}
