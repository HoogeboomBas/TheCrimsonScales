using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Whirlwind : BruiserCardModel<Whirlwind.CardTop, Whirlwind.CardBottom>
{
	public override string Name => "Whirlwind";
	public override int Level => 4;
	public override int Initiative => 91;
	protected override int AtlasIndex => 18;

	public class CardTop : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(3);
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Wound1);

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
				.WithUseSlot(new UseSlot(new Vector2(0.5f, 0.25f), GainXP))
				.Build())
		];

		public override bool Persistent => true;
	}

	public class CardBottom : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62086004f, 0.7210304f)))
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.51085865f, 0.82025903f)))
				.WithConditions(Conditions.Wound1)
				.Build())
		];
	}
}