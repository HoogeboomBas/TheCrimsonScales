using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class SlakeThirst : RuinmawCardModel<SlakeThirst.CardTop, SlakeThirst.CardBottom>
{
	public override string Name => "Slake Thirst";
	public override int Level => 6;
	public override int Initiative => 26;
	protected override int AtlasIndex => 14;

	public class CardTop : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithTarget(Target.Self)
				.WithConditions(Ruinmaw.Empower)
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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AfterHealPerformedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Performer == state.Performer &&
						                      RangeHelper.GetFiguresInRange(state.Performer.Hex, 1)
							                      .Any(figure => figure.EnemiesWith(state.Performer)),
						async applyParameters =>
						{
							Figure figure = await AbilityCmd.SelectFigure(state, list =>
							{
								list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1)
									.Where(figure => figure.EnemiesWith(state.Performer)));
							});
							if(figure != null)
							{
								await AbilityCmd.SufferDamage(state, figure, 2);
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
					new UseSlot(new Vector2(0.27549905f, 0.77401024f), SateRuinmaw),
					new UseSlot(new Vector2(0.48599634f, 0.77401024f)),
					new UseSlot(new Vector2(0.690505f, 0.77401024f), GainXP),
					new UseSlot(new Vector2(0.36249793f, 0.8955159f)),
					new UseSlot(new Vector2(0.5704994f, 0.8955159f), GainXP)
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}
}