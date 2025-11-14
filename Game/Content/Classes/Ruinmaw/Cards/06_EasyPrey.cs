using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class EasyPrey : RuinmawCardModel<EasyPrey.CardTop, EasyPrey.CardBottom>
{
	public override string Name => "Easy Prey";
	public override int Level => 1;
	public override int Initiative => 55;
	protected override int AtlasIndex => 6;

	public class CardTop : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.WithMoveType(MoveType.Jump)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					if (IsSated(state.Performer))
                    {
                        await AbilityCmd.GainXP(state.Performer, 1);
                    }
					return IsSated(state.Performer);
				})
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithConditions(Conditions.Wound1)
				.Build()),
		];
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
		new AbilityCardAbility(UseSlotAbility.Builder()
			.WithOnActivate(async state =>
			{
				ScenarioEvents.JustBeforeSufferDamageEvent.Subscribe(state, this,
					parameters =>
						parameters.Figure == state.Performer &&
						!parameters.Prevented &&
						parameters.Figure.Health <= parameters.Damage,
					async parameters =>
					{
						parameters.SetPrevented();

						ActionState actionState = new ActionState(parameters.Figure,
							[HealAbility.Builder()
								.WithHealValue(4)
								.WithConditions(Conditions.EmpowerRuinmaw, Conditions.EmpowerRuinmaw)
								.WithTarget(Target.Self)
								.Build()]);
						await actionState.Perform();
						await SateRuinmaw(parameters.Figure);

						await state.AdvanceUseSlot();
					}
				);

				await GDTask.CompletedTask;
			})
			.WithOnDeactivate(async state =>
				{
					ScenarioEvents.JustBeforeSufferDamageEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				}
			)
			.WithUseSlot(new UseSlot(new Vector2(0.5f, 0.857998f)))
			//TODO: Fix use slot positioning
			.Build())
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}