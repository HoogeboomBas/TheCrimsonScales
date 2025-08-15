using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class FireWhirl : FireKnightCardModel<FireWhirl.CardTop, FireWhirl.CardBottom>
{
	public override string Name => "Fire Whirl";
	public override int Level => 1;
	public override int Initiative => 56;
	protected override int AtlasIndex => 12 - 6;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new AttackAbility(1, range: 2,
				aoePattern: new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add((Direction)0), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add((Direction)1), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add((Direction)2), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add((Direction)3), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add((Direction)4), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add((Direction)5), AOEHexType.Red),
					]
				),
				duringAttackSubscriptions:
				[
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")
					)
				]
			)),

			new AbilityCardAbility(new ConditionAbility([Conditions.Wound1], target: Target.TargetAll | Target.Any, mandatory: true,
				customGetTargets: (state, list) =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					foreach((Vector2I coords, AOEHexType hexType) in attackAbilityState.AOEHexes)
					{
						if(hexType == AOEHexType.Red)
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

		protected override IEnumerable<Element> Elements => [Element.Fire, Element.Air];
		protected override int XP => 1;
		protected override bool Loss => true;
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new MoveAbility(3,
				onAbilityStarted: async abilityState =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Subscribe(abilityState, this,
						canApplyParameters =>
							canApplyParameters.AbilityState == abilityState &&
							(canApplyParameters.Hex.HasHexObjectOfType<DifficultTerrain>() || canApplyParameters.Hex.HasHexObjectOfType<HazardousTerrain>()),
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
						canApplyParameters => canApplyParameters.AbilityState.Performer == abilityState.Performer,
						async applyParameters =>
						{
							applyParameters.SetAffectedByHazardousTerrain(false);
							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				},
				onAbilityEnded: async abilityState =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(abilityState, this);
					ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(abilityState, this);

					await GDTask.CompletedTask;
				}
			))
		];
	}
}