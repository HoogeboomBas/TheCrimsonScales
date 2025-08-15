using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class BackupSupport : FireKnightCardModel<BackupSupport.CardTop, BackupSupport.CardBottom>
{
	public override string Name => "Backup Support";
	public override int Level => 1;
	public override int Initiative => 19;
	protected override int AtlasIndex => 12 - 1;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new AttackAbility(3,
				duringAttackSubscriptions:
				[
					ScenarioEvents.DuringAttack.Subscription.New(
						parameters => parameters.Performer.Hex.HasHexObjectOfType<Ladder>(),
						async parameters =>
						{
							parameters.AbilityState.SetCustomValue(this, "AddedRange", true);
							parameters.AbilityState.AbilityAdjustRange(2);
							parameters.AbilityState.AbilitySetRangeType(RangeType.Range);

							await GDTask.CompletedTask;
						},
						effectType: EffectType.Selectable,
						canApplyMultipleTimesDuringSubscription: false,
						effectButtonParameters: new IconEffectButton.Parameters(LadderIconPath),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Range)}")
					)
				]
			)),

			new AbilityCardAbility(new ConditionAbility([Conditions.Strengthen], target: Target.Allies,
				conditionalAbilityCheck: async state =>
				{
					await GDTask.CompletedTask;

					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					return attackAbilityState.Performed && attackAbilityState.GetCustomValue<bool>(this, "AddedRange");
				}
			))
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new MoveAbility(5)),

			new AbilityCardAbility(new UseSlotAbility([new UseSlot(new Vector2(0.5560003f, 0.8259989f), GainXP)],
				async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters =>
							parameters.Performer == state.Performer &&
							RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, false).Any(figure => state.Performer.AlliedWith(figure)),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(3);

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				},
				async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				}
			))
		];

		protected override IEnumerable<Element> Elements => [Element.Fire];
		protected override int XP => 1;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}