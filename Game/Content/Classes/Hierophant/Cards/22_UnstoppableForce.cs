using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class UnstoppableForce : HierophantLevelUpCardModel<UnstoppableForce.CardTop, UnstoppableForce.CardBottom>
{
	public override string Name => "Unstoppable Force";
	public override int Level => 6;
	public override int Initiative => 21;
	protected override int AtlasIndex => 15 - 8;

	public class CardTop : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AfterSufferDamageEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.SufferDamageParameters.FromAttack &&
						                      ((AttackAbility.State)canApplyParameters.PotentialAbilityState).Target.AlliedWith(state.Performer) &&
						                      RangeHelper.Distance(((AttackAbility.State)canApplyParameters.PotentialAbilityState).Target.Hex,
							                      state.Performer.Hex) <= 1,
						async applyParameters =>
						{
							await AbilityCmd.SufferDamage(state, applyParameters.PotentialAbilityState.Performer,
								applyParameters.DamageSuffered);

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AfterSufferDamageEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.28100282f, 0.3734997f), GainXP),
						new UseSlot(new Vector2(0.48650017f, 0.3734997f)),
						new UseSlot(new Vector2(0.68950886f, 0.3734997f), GainXP)
					]
				)
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.44777906f, 0.6208354f)))
				.WithRange(4, new RangeSquare(this, new Vector2(0.67059785f, 0.6208354f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => RangeHelper.GetFiguresInRange(parameters.AbilityState.Target.Hex, 1)
							.Any(figure => figure.AlliedWith(parameters.AbilityState.Performer)),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Wound1);

							await GDTask.CompletedTask;
						}
					)
				)
				.Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state =>
				[
					ShieldAbility.Builder()
						.WithShieldValue(2)
						.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Earth))
						.WithOnAbilityEndedPerformed(async state =>
						{
							await GDTask.CompletedTask;

							state.ActionState.SetOverrideRound();
						})
						.Build()
				])
				.WithCustomGetTargets((state, list) =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					foreach(Figure target in attackAbilityState.UniqueTargetedFigures)
					{
						list.AddRange(RangeHelper.GetFiguresInRange(target.Hex, 1));
					}
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];
	}
}