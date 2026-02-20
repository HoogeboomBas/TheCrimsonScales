using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class SpontaneousCombustion : FireKnightLevelUpCardModel<SpontaneousCombustion.CardTop, SpontaneousCombustion.CardBottom>
{
	public override string Name => "Spontaneous Combustion";
	public override int Level => 6;
	public override int Initiative => 23;
	protected override int AtlasIndex => 7;

	public class CardTop : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(8, new AttackDiamond(this, new Vector2(0.5020886f, 0.14570932f)))
				.WithConditions(Conditions.Wound1)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.New(
						parameters => parameters.Performer.Hex.HasHexObjectOfType<Ladder>(),
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustRange(2);
							parameters.AbilityState.AbilitySetRangeType(RangeType.Range);

							await GDTask.CompletedTask;
						},
						effectType: EffectType.Selectable,
						canApplyMultipleTimesDuringSubscription: false,
						effectButtonParameters: new IconEffectButton.Parameters(LadderIconPath),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Range)}")
					)
				)
				.WithOnAbilityStarted(async abilityState =>
				{
					ScenarioEvents.FigureKilledEvent.Subscribe(abilityState, this,
						canApplyParameters => canApplyParameters.PotentialAbilityState == abilityState,
						async applyParameters =>
						{
							Hex hex = applyParameters.Figure.Hex;
							List<Figure> figures = RangeHelper.GetFiguresInRange(hex, 1, false).ToList();
							foreach(Figure figure in figures)
							{
								await AbilityCmd.AddCondition(abilityState, figure, Conditions.Wound1);
							}
						});
					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async abilityState =>
				{
					ScenarioEvents.FigureKilledEvent.Unsubscribe(abilityState, this);

					await GDTask.CompletedTask;
				})
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6199888f, 0.6535839f)))
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
			new AbilityCardAbility(GiveFireKnightItemAbility(
				state =>
				[
					ModelDB.Item<FireKnightExplosiveTonic>(), ModelDB.Item<FireKnightRescueAxe>(), ModelDB.Item<FireKnightScrollOfInvigoration>()
				],
				customGetTargets: (state, list) =>
				{
					list.AddRange(GameController.Instance.Map.Figures
						.Where(figure =>
							figure.AlliedWith(state.Performer) &&
							state.Performer.TurnMovedHexes.Any(hex => RangeHelper.Distance(hex, figure.Hex) <= 1))
						.ToList());
				}
			))
		];
	}
}