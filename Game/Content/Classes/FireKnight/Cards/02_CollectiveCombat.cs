using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class CollectiveCombat : FireKnightCardModel<CollectiveCombat.CardTop, CollectiveCombat.CardBottom>
{
	public override string Name => "Collective Combat";
	public override int Level => 1;
	public override int Initiative => 64;
	protected override int AtlasIndex => 12 - 2;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					]
				))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")
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
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters =>
							parameters.Performer == state.Performer &&
							RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 1, false)
								.Any(figure => parameters.Performer.AlliedWith(figure)) &&
							RangeHelper.GetFiguresInRange(parameters.AbilityState.Target.Hex, 1, false)
								.Any(figure => parameters.Performer.AlliedWith(figure)),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Stun);

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlot(new UseSlot(new Vector2(0.5f, 0.83799946f), GainXP))
				.Build())
		];

		protected override bool Persistent => true;
	}
}