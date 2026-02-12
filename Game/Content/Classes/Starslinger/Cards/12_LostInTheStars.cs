using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using GTweens.Builders;
using GTweens.Easings;

public class LostInTheStars : StarslingerCardModel<LostInTheStars.CardTop, LostInTheStars.CardBottom>
{
	public override string Name => "Lost In The Stars";
	public override int Level => 1;
	public override int Initiative => 06;
	protected override int AtlasIndex => 12;

	public class CardTop : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					StarslingerTokenLostInTheStars characterToken = ResourceLoader
						.Load<PackedScene>("res://Content/Classes/Starslinger/StarslingerTokenLostInTheStars.tscn")
						.Instantiate<StarslingerTokenLostInTheStars>();
					GameController.Instance.Map.AddChild(characterToken);
					await characterToken.Init(state.Performer, state.Performer.Hex);

					state.Performer.RemoveFromMap();

					if(!GameController.FastForward)
					{
						characterToken.SetScale(Vector2.Zero);
						GTweenSequenceBuilder.New()
							.AppendTime(0.4f)
							.Append(characterToken.TweenScale(1f, 0.2f))
							.Build().PlayFastForwardable();

						await GameController.Instance.ScreenDistortion.Disappear(state.Performer, 1.4f, true).PlayFastForwardableAsync();
					}
					else
					{
						state.Performer.SetScale(Vector2.Zero);
					}

					state.Performer.SetTakingTurn(false);

					foreach(AbilityCard card in ((Character)state.Performer).RoundCards)
					{
						if(card.CardState == CardState.Playing)
						{
							await AbilityCmd.DiscardCard(card);
						}
					}

					//TODO: Display proper initiative
					ScenarioCheckEvents.InitiativeCheckEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						parameters =>
						{
							parameters.SetInitiative(100);
							parameters.SetSortingInitiative(int.MaxValue);
						}
					);

					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure == state.Performer,
						async applyParameters =>
						{
							await state.RemoveFromActive();
							await state.ActionState.RequestDiscardOrLose();
							Hex returnHex = await AbilityCmd.SelectHex(applyParameters.Figure,
								possibleEndHexes =>
								{
									List<Hex> hexes = RangeHelper.GetHexesInRange(characterToken.Hex, 100, requiresLineOfSight: false).ToList();
									hexes.Shuffle(GameController.Instance.StateRNG);
									hexes.Sort((otherHexA, otherHexB) => RangeHelper.Distance(characterToken.Hex, otherHexA)
										.CompareTo(RangeHelper.Distance(characterToken.Hex, otherHexB)));
									Hex firstHex = null;
									foreach(Hex hex in hexes)
									{
										if(hex.IsUnoccupied() && MoveHelper.CanStopAt(null, state.Performer, hex))
										{
											firstHex = hex;
											break;
										}
									}

									if(firstHex == null)
									{
										return;
									}

									int distance = RangeHelper.Distance(characterToken.Hex, firstHex);

									foreach(Hex otherHex in hexes)
									{
										int otherDistance = RangeHelper.Distance(characterToken.Hex, otherHex);
										if(otherHex.IsUnoccupied() && otherDistance == distance &&
										   MoveHelper.CanStopAt(null, state.Performer, otherHex))
										{
											possibleEndHexes.Add(otherHex);
										}
									}
								}, true, "Select a hex to return to"
							);

							await characterToken.Destroy();

							if(!GameController.FastForward)
							{
								state.Performer.SetGlobalPosition(returnHex.GlobalPosition);
								await GameController.Instance.ScreenDistortion.Appear(state.Performer, 1.4f, true).PlayFastForwardableAsync();
							}
							else
							{
								state.Performer.SetScale(Vector2.One);
							}

							await AbilityCmd.EnterHex(state, applyParameters.Figure, applyParameters.Figure, returnHex, true, true);
						}
					);
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioCheckEvents.InitiativeCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override bool Persistent => true;
		protected override bool CanDeactivate => false;
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(6)
				.WithMoveType(MoveType.Jump)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => !parameters.Performer.IsDamaged(),
						async parameters =>
						{
							((MoveAbility.State)parameters.AbilityState).AdjustMoveValue(2);
							await GDTask.CompletedTask;
						}
					)
				)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Stun)
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Invisible)
				.WithTarget(Target.Enemies)
				.WithCustomGetTargets((state, targets) =>
				{
					ConditionAbility.State conditionAbilityState = state.ActionState.GetAbilityState<ConditionAbility.State>(1);
					targets.AddRange(conditionAbilityState.UniqueTargetedFigures);
				})
				.WithMandatory(true)
				.Build())
		];

		protected override int XP => 2;
		protected override bool Loss => true;
	}
}