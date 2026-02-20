using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class GroundSolvent : MirefootCardModel<GroundSolvent.CardTop, GroundSolvent.CardBottom>
{
	public override string Name => "Ground Solvent";
	public override int Level => 1;
	public override int Initiative => 63;
	protected override int AtlasIndex => 3;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Poison1)
				.WithRange(3, new RangeSquare(this, new Vector2(0.47981167f, 0.16940814f)))
				.WithAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red)
						]
					), new AOEHexMark(Vector2I.Zero.Add(Direction.East), this, new Vector2(0.76916414f, 0.20017323f))
				)
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async abilityState =>
				{
					ConditionAbility.State conditionAbilityState = abilityState.ActionState.GetAbilityState<ConditionAbility.State>(0);

					if(conditionAbilityState.Performed)
					{
						IEnumerable<Hex> hexes = conditionAbilityState.GetRedAOEHexes().Where(hex => hex.IsFeatureless());

						List<Hex> selectedHexes =
							await AbilityCmd.SelectHexes(abilityState, list => list.AddRange(hexes), 0, hexes.Count(), true,
								"Select hexes to place difficult terrain in");

						foreach(Hex selectedHex in selectedHexes)
						{
							await CreateDifficultTerrain(selectedHex);
						}

						if(selectedHexes.Count > 0)
						{
							abilityState.SetPerformed();
						}
					}

					await GDTask.CompletedTask;
				})
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(0)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithRangeType(RangeType.Range)
				.WithCustomGetTargets((abilityState, list) =>
				{
					ConditionAbility.State conditionAbilityState = abilityState.ActionState.GetAbilityState<ConditionAbility.State>(0);
					list.AddRange(conditionAbilityState.GetRedAOEHexes().SelectMany(hex => hex.GetHexObjectsOfType<Figure>()));
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62063575f, 0.68009883f)))
				.WithOnAbilityStarted(async state =>
				{
					ScenarioCheckEvents.MoveCanStopAtCheckEvent.Subscribe(state.Performer, this,
						parameters => parameters.AbilityState == state && !parameters.Hex.HasHexObjectOfType<DifficultTerrain>(),
						parameters =>
						{
							parameters.SetCannotStopAt();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async state =>
					{
						ScenarioCheckEvents.MoveCanStopAtCheckEvent.Unsubscribe(state.Performer, this);

						await GDTask.CompletedTask;
					}
				)
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.61965984f, 0.8603736f)))
				.Build())
		];
	}
}