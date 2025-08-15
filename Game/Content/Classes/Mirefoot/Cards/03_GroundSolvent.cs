using System.Collections.Generic;
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
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new ConditionAbility([Conditions.Poison1], range: 3,
				aoePattern: new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red)
					]
				)
			)),

			new AbilityCardAbility(new OtherAbility(async abilityState =>
			{
				ConditionAbility.State conditionAbilityState = abilityState.ActionState.GetAbilityState<ConditionAbility.State>(0);

				if(conditionAbilityState.Performed)
				{
					List<Hex> hexes = new List<Hex>();
					foreach((Vector2I coords, AOEHexType hexType) in conditionAbilityState.AOEHexes)
					{
						Hex hex = GameController.Instance.Map.GetHex(coords);
						if(hex != null && hex.IsFeatureless())
						{
							hexes.Add(hex);
						}
					}

					List<Hex> selectedHexes =
						await AbilityCmd.SelectHexes(abilityState, list => list.AddRange(hexes), 0, hexes.Count, true, "Select hexes to place difficult terrain in");

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
			})),

			new AbilityCardAbility(new AttackAbility(0, target: Target.Enemies | Target.TargetAll,
				customGetTargets: (abilityState, list) =>
				{
					ConditionAbility.State conditionAbilityState = abilityState.ActionState.GetAbilityState<ConditionAbility.State>(0);

					if(conditionAbilityState.Performed)
					{
						foreach((Vector2I coords, AOEHexType hexType) in conditionAbilityState.AOEHexes)
						{
							Hex hex = GameController.Instance.Map.GetHex(coords);
							if(hex != null)
							{
								foreach(Figure figure in hex.GetHexObjectsOfType<Figure>())
								{
									list.Add(figure);
								}
							}
						}
					}
				},
				conditionalAbilityCheck: state => AbilityCmd.HasPerformedAbility(state, 0)
			))
		];
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new MoveAbility(3,
				onAbilityStarted: async state =>
				{
					ScenarioCheckEvents.MoveCanStopAtCheckEvent.Subscribe(state.Performer, this,
						parameters => parameters.AbilityState == state && !parameters.Hex.HasHexObjectOfType<DifficultTerrain>(),
						parameters =>
						{
							parameters.SetCannotStopAt();
						}
					);

					await GDTask.CompletedTask;
				},
				onAbilityEnded: async state =>
				{
					ScenarioCheckEvents.MoveCanStopAtCheckEvent.Unsubscribe(state.Performer, this);

					await GDTask.CompletedTask;
				}
			)),

			new AbilityCardAbility(new AttackAbility(2))
		];
	}
}