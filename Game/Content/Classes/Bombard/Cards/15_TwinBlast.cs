using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class TwinBlast : BombardCardModel<TwinBlast.CardTop, TwinBlast.CardBottom>
{
	public override string Name => "Twin Blast";
	public override int Level => 3;
	public override int Initiative => 80;
	protected override int AtlasIndex => 15;

	public class CardTop : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ProjectileAbility.Builder().WithGetAbilities(hex =>
				[
					new AttackAbility(3, pierce: 2, rangeType: RangeType.Range, targetHex: hex)
				])
				.WithAbilityCardSide(this)
				.WithRange(4)
				.WithTargets(2)
				.Build())
		];

		protected override int XP => 1;
		protected override bool Persistent => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new AttackAbility(2, range: 3)),

			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters =>
							parameters.Performer == state.Performer &&
							parameters.AbilityState.ActionState.ParentActionState != null &&
							parameters.AbilityState.ActionState.ParentActionState.AbilityStates.Any(parentAbilityState =>
								parentAbilityState is ProjectileAbility.State),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetSetHasAdvantage();

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
				.WithUseSlot(new UseSlot(new Vector2(0.5f, 0.85f), GainXP))
				.Build())
		];

		protected override bool Persistent => true;
	}
}