using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class GatewayToTheAbyss : HollowpactCardModel<GatewayToTheAbyss.CardTop, GatewayToTheAbyss.CardBottom>
{
	public override string Name => "Gateway to the Abyss";
	public override int Level => 7;
	public override int Initiative => 66;
	protected override int AtlasIndex => 11;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreateVoidPitObstacleAbilityBuilder()
				.WithRange(2)
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithConditions(Conditions.Wound1)
				.WithConditionalAbilityCheck(async state =>
				{
					return await LoseVoidEnergyConditionalAbilityCheck(state.Performer, 2, 
						new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Attack)}3{Icons.Inline(Icons.GetCondition(Conditions.Wound1))}, {Icons.Inline(Icons.Targets)}all enemies adjacent to at least one Void Pit."));
				})
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(GameController.Instance.Map.GetChildrenOfType<Obstacle>()
											.Where(obstacle => obstacle is VoidPit)
											.SelectMany(obstacle => obstacle.Hex.Neighbours.SelectMany(hex => hex.GetFigures()))
											.Where(figure => figure.EnemiesWith(state.Performer))
											.Distinct());
				})
				.WithOnAbilityEndedPerformed(async state =>
				{
					await GainXP(state);

					IEnumerable<Figure> alliedFigures = GameController.Instance.Map.GetChildrenOfType<Obstacle>()
												.Where(obstacle => obstacle is VoidPit)
												.SelectMany(obstacle => obstacle.Hex.Neighbours.SelectMany(hex => hex.GetFigures()))
												.Where(figure => figure.AlliedWith(state.Performer))
												.Distinct();

					foreach(Figure figure in alliedFigures)
					{
						await AbilityCmd.SufferDamage(state, figure, 2);
					}
				})
				.Build()),
		];

		public override bool Loss => true;
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.44718847f, 0.19982125f)))
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.GenericChoice(state.Performer,
					[
						ScenarioEvents.GenericChoice.Subscription.New(
							canApplyFunction: parameters => HasXVoidEnergy(state.Performer, 1),
							applyFunction: async applyParameters =>
							{
								state.SetCustomValue(this, "ChoseGrant", true);
								state.SetPerformed();

								LoseVoidEnergy(state.Performer, 1);

								await GDTask.CompletedTask;
							},
							effectButtonParameters: new IconEffectButton.Parameters(Hollowpact.VoidEnergy),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"Grant one adjacent ally: {Icons.Inline(Icons.Teleport)} to a hex within {Icons.Inline(Icons.Range)}5 of the Hollowpact."),
							effectType: EffectType.Selectable
						),
						ScenarioEvents.GenericChoice.Subscription.New(
							canApplyFunction: parameters => HasXVoidEnergy(state.Performer, 1),
							applyFunction: async applyParameters =>
							{
								state.SetCustomValue(this, "ChoseControl", true);
								state.SetPerformed();

								LoseVoidEnergy(state.Performer, 1);

								await GDTask.CompletedTask;
							},
							effectButtonParameters: new IconEffectButton.Parameters(Hollowpact.VoidEnergy),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"Control one adjacent enemy: {Icons.Inline(Icons.Teleport)} to a hex within {Icons.Inline(Icons.Range)}5 of the Hollowpact."),
							effectType: EffectType.Selectable
						),
					], hintText: "Select an ability to perform:");
				})
				.Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					TeleportAbility.Builder()
					.WithCustomGetHexes((state, hexes) =>
					{
						hexes.AddRange(RangeHelper.GetHexesInRange(state.Performer.Hex, 5, 
							includeOrigin: false, 
							requiresLineOfSight: false, 
							requiresHexesRevealed: true, 
							allowDoors: true)
						.Where(hex => hex.IsEmpty()));
					})
					.Build()
				])
				.WithRange(1)
				.WithConditionalAbilityCheck(async state =>
				{
					OtherAbility.State abilityState = state.ActionState.GetAbilityState<OtherAbility.State>(0);

					return abilityState.Performed && abilityState.GetCustomValue<bool>(this, "ChoseGrant");
				})
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.SufferDamage(state, state.Performer, 2);
				})
				.Build()),

			new AbilityCardAbility(ControlAbility.Builder()
				.WithAbilities(
				[
					TeleportAbility.Builder()
					.WithCustomGetHexes((state, hexes) =>
					{
						hexes.AddRange(RangeHelper.GetHexesInRange(state.Performer.Hex, 5, 
							includeOrigin: false, 
							requiresLineOfSight: false, 
							requiresHexesRevealed: true, 
							allowDoors: false)
						.Where(hex => hex.IsEmpty()));
					})
					.Build()
				])
				.WithRange(1)
				.WithConditionalAbilityCheck(async state =>
				{
					OtherAbility.State abilityState = state.ActionState.GetAbilityState<OtherAbility.State>(0);

					return abilityState.Performed && abilityState.GetCustomValue<bool>(this, "ChoseControl");
				})
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.SufferDamage(state, state.Performer, 2);
				})
				.Build()),
		];
	}
}