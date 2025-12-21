using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class HookAndLadder : FireKnightLevelUpCardModel<HookAndLadder.CardTop, HookAndLadder.CardBottom>
{
	public override string Name => "Hook and Ladder";
	public override int Level => 5;
	public override int Initiative => 32;
	protected override int AtlasIndex => 9;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithRange(2)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
					]
				))
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => parameters.Performer.Hex.HasHexObjectOfType<Ladder>(),
						async parameters =>
						{
							await AbilityCmd.GenericChoice(parameters.Performer,
							[
								ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
									applyFunction: async applyParameters =>
									{
										((AttackAbility.State)parameters.AbilityState).AbilityAdjustPierce(2);

										await GDTask.CompletedTask;
									},
									effectButtonParameters: new IconEffectButton.Parameters(Icons.Pierce),
									effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Pierce)} 2"),
									effectType: EffectType.Selectable
								),
								ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
									applyFunction: async applyParameters =>
									{
										((AttackAbility.State)parameters.AbilityState).AbilityAdjustPull(2);

										await GDTask.CompletedTask;
									},
									effectButtonParameters: new IconEffectButton.Parameters(Icons.Pull),
									effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Pull)} 2"),
									effectType: EffectType.Selectable
								),
							], hintText: "Choose an effect to apply");
						}
					)
				)
				.WithAbilityEndedSubscription(
					ScenarioEvents.AbilityEnded.Subscription.New(
						parameters => parameters.Performer.Hex.HasHexObjectOfType<Ladder>() && parameters.AbilityState.Performed,
						async parameters =>
						{
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}
					)
				)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Strengthen)
				.WithTarget(Target.Allies | Target.TargetAll)
				.WithCustomGetTargets((abilityState, list) =>
				{
					AttackAbility.State attackAbilityState = abilityState.ActionState.GetAbilityState<AttackAbility.State>(0);

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
				})
				.WithConditionalAbilityCheck(async state =>
					{
						await GDTask.CompletedTask;

						AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

						if(attackAbilityState.AOEHexes == null || attackAbilityState.AOEHexes.Count == 0)
						{
							return false;
						}

						return true;
					}
				)
				.Build())
		];

		protected override IEnumerable<Element> Elements => [Element.Fire];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						canApplyParameters => CanApply(canApplyParameters.Performer, state, false),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetSetHasAdvantage();

							await GDTask.CompletedTask;
						});

					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						canApplyParameters => CanApply(canApplyParameters.Figure, state, true),
						async parameters =>
						{
							StrengthenRemove(parameters.Figure);

							await GDTask.CompletedTask;
						},
						EffectType.Selectable,
						effectButtonParameters: new TextEffectButton.Parameters(Icons.GetCondition(Conditions.Strengthen)),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"Remove {Icons.Inline(Icons.GetCondition(Conditions.Strengthen))} " +
							$"to add +1{Icons.Inline(Icons.Attack)} to your first attack this round"));

					ScenarioEvents.AbilityEndedEvent.Subscribe(state, this,
						canApplyParameters => CanApply(canApplyParameters.Performer, state, true),
						async parameters =>
						{
							StrengthenRemove(parameters.Performer);
							
							await GDTask.CompletedTask;
						},
						EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Strengthen)),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"Remove {Icons.Inline(Icons.GetCondition(Conditions.Strengthen))} " +
							$"to add +1{Icons.Inline(Icons.Attack)} to your first attack this round"));
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);
					ScenarioEvents.CardSideSelectionEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;

		private bool CanApply(Figure performer, OtherActiveAbility.State state, bool requireStrengthen)
		{
			return performer.AlliedWith(state.Performer) &&
				state.Performer.Hex.HasHexObjectOfType<Ladder>() &&
				RangeHelper.Distance(state.Performer.Hex, performer.Hex) <= 1 &&
				(!requireStrengthen || performer.HasCondition(Conditions.Strengthen));
		}

		private async void StrengthenRemove(Figure performer)
		{
			await AbilityCmd.RemoveCondition(performer, Conditions.Strengthen);
			bool attackPerformedYet = performer.RoundPerformedActionStates
				.SelectMany(a => a.AbilityStates)
				.OfType<AttackAbility.State>()
				.Any(a => a.UniqueTargetedFigures.Count > 0);
			if(!attackPerformedYet)
			{
				ScenarioEvents.DuringAttackEvent.Subscribe(performer, this,
					canApplyParameters => canApplyParameters.Performer == performer,
					async applyParameters =>
					{
						applyParameters.AbilityState.SingleTargetAdjustAttackValue(1);
						ScenarioEvents.DuringAttackEvent.Unsubscribe(performer, this);

						await GDTask.CompletedTask;
					});
				ScenarioEvents.RoundEndedEvent.Subscribe(performer, this,
					canApplyParameters => true,
					async applyParameters =>
					{
						ScenarioEvents.DuringAttackEvent.Unsubscribe(performer, this);
						ScenarioEvents.RoundEndedEvent.Unsubscribe(performer, this);
							
						await GDTask.CompletedTask;
					});
			}
		}
	}
}
