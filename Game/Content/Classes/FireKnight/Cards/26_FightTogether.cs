using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class FightTogether : FireKnightLevelUpCardModel<FightTogether.CardTop, FightTogether.CardBottom>
{
	public override string Name => "Fight Together";
	public override int Level => 8;
	public override int Initiative => 13;
	protected override int AtlasIndex => 2;

	public class CardTop : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					ShieldAbility.Builder()
						.WithShieldValue(2, new ShieldDiamondPlus(this, new Vector2(0.49963737f, 0.24985416f)))
						.Build(),
					ConditionAbility.Builder()
						.WithConditions(Conditions.Bless)
						.WithTarget(Target.Self)
						.Build()
				])
				.WithTarget(Target.SelfOrAllies | Target.TargetAll)
				.Build()),
			new AbilityCardAbility(GiveFireKnightItemAbility(
				state => [ModelDB.Item<FireKnightScrollOfProtection>()],
				customGetTargets: (state, list) =>
				{
					GrantAbility.State grantAbilityState = state.ActionState.GetAbilityState<GrantAbility.State>(0);
					list.AddRange(grantAbilityState.UniqueTargetedFigures);
				},
				conditionalAbilityCheck: async state =>
				{
					await GDTask.CompletedTask;

					GrantAbility.State grantAbilityState = state.ActionState.GetAbilityState<GrantAbility.State>(0);

					return grantAbilityState.UniqueTargetedFigures.Count(target => target != state.Performer) == 1;
				}
			))
		];

		public override bool Round => true;
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62018883f, 0.62458545f)))
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
						}
					);

					ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(abilityState, this,
						canApplyParameters => canApplyParameters.PotentialAbilityState?.Performer == abilityState.Performer,
						async applyParameters =>
						{
							applyParameters.SetAffectedByHazardousTerrain(false);
							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async abilityState =>
					{
						ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(abilityState, this);
						ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(abilityState, this);

						await GDTask.CompletedTask;
					}
				)
				.Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					AttackAbility.Builder()
						.WithDamage(3, new AttackDiamond(this, new Vector2(0.61666214f, 0.8298918f)))
						.Build()
				])
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
	}
}