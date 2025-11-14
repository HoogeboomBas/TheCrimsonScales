using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class SlakeThirst : RuinmawCardModel<SlakeThirst.CardTop, SlakeThirst.CardBottom>
{
	public override string Name => "Slake Thirst";
	public override int Level => 2;
	public override int Initiative => 26;
	protected override int AtlasIndex => 14;

	public class CardTop : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithTarget(Target.Self)
				.WithConditions(Conditions.EmpowerRuinmaw)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					return attackAbilityState.UniqueTargetedFigures.Any(figure => figure.HasCondition(Conditions.Rupture));
				})
				.Build())
		];
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AfterHealPerformedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Performer == state.Performer &&
							RangeHelper.GetFiguresInRange(state.Performer.Hex, 1).Any(figure => figure.EnemiesWith(state.Performer)),
						async applyParameters =>
						{
							Figure figure = await AbilityCmd.SelectFigure(state, list => {
								list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1).Where(figure => figure.EnemiesWith(state.Performer)));
							});
							if(figure != null)
                            {
								await AbilityCmd.SufferDamage(state, figure, 2);
                            }
							if(state.UseSlotIndex == 0)
                            {
								await SateRuinmaw(applyParameters.Performer);
                            }
							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AfterHealPerformedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.16650043f, 0.3549993f)),
					new UseSlot(new Vector2(0.16650043f, 0.3549993f)),
					new UseSlot(new Vector2(0.36999783f, 0.3549993f), GainXP),
					new UseSlot(new Vector2(0.57749975f, 0.3549993f)),
					new UseSlot(new Vector2(0.78700954f, 0.3549993f), GainXP)
				])
				//TODO: Fix use slot positioning
				.Build())
		];

		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}