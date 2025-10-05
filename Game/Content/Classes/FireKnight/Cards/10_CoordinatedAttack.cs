using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class CoordinatedAttack : FireKnightCardModel<CoordinatedAttack.CardTop, CoordinatedAttack.CardBottom>
{
	public override string Name => "Coordinated Attack";
	public override int Level => 1;
	public override int Initiative => 22;
	protected override int AtlasIndex => 12 - 10;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
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
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => true,
						async parameters =>
						{
							bool targetNextToAlly = RangeHelper.GetFiguresInRange(parameters.AbilityState.Target.Hex, 1, false)
								.Any(figure => parameters.Performer.AlliedWith(figure));
							if(targetNextToAlly)
							{
								parameters.AbilityState.SingleTargetAdjustAttackValue(1);
							}

							bool performerNextToAlly = RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 1, false)
								.Any(figure => parameters.Performer.AlliedWith(figure));
							if(performerNextToAlly)
							{
								parameters.AbilityState.SingleTargetSetHasAdvantage();
							}

							if(targetNextToAlly && performerNextToAlly)
							{
								await AbilityCmd.GainXP(parameters.Performer, 1);
							}
						}
					)
				)
				.Build())
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async parameters =>
						{
							bool hasPerformedAttack = false;
							foreach(ActionState performedActionState in parameters.Figure.TurnPerformedActionStates)
							{
								foreach(AbilityState abilityState in performedActionState.AbilityStates)
								{
									if(abilityState is AttackAbility.State attackAbilityState && attackAbilityState.Performed)
									{
										hasPerformedAttack = true;
										break;
									}
								}
							}

							if(hasPerformedAttack)
							{
								ActionState actionState = new ActionState(state.Performer,
									[
										GrantAbility.Builder()
											.WithGetAbilities(state => [AttackAbility.Builder().WithDamage(2).Build()])
											.WithCustomGetTargets((grantState, list) =>
												{
													foreach(Figure potentialTarget in RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, false))
													{
														if(state.Performer.AlliedWith(potentialTarget) &&
														   potentialTarget.HasCondition(Conditions.Strengthen))
														{
															list.Add(potentialTarget);
														}
													}
												}
											)
											.Build()
									]
								);

								await actionState.Perform();
							}
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}