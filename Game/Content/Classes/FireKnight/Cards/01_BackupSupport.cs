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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.6203842f, 0.2045231f)))
				.WithDuringAttackSubscription(
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
				)
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Strengthen, new ConditionDiamondPlus(this, new Vector2(0.40208158f, 0.37323144f)))
				.WithTarget(Target.Allies)
				.WithConditionalAbilityCheck(async state =>
					{
						await GDTask.CompletedTask;

						AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
						return attackAbilityState.Performed && attackAbilityState.GetCustomValue<bool>(this, "AddedRange");
					}
				)
				.Build())
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5, new MoveCircle(this, new Vector2(0.61780804f, 0.62103546f)))
				.Build()),

			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
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
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlot(new UseSlot(new Vector2(0.5560003f, 0.8259989f), GainXP))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
		public override int XP => 1;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}