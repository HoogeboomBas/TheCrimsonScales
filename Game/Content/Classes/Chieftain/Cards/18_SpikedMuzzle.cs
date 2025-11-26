using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SpikedMuzzle : ChieftainCardModel<SpikedMuzzle.CardTop, SpikedMuzzle.CardBottom>
{
	public override string Name => "Spiked Muzzle";
	public override int Level => 4;
	public override int Initiative => 47;
	protected override int AtlasIndex => 18;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						canApplyParameters => Chieftain.GetIsMounted(state.Performer),
						async applyParameters =>
						{
							if(applyParameters.Performer == Chieftain.GetMount(state.Performer))
							{
								applyParameters.AbilityState.SingleTargetAdjustAttackValue(1);

								await state.AdvanceUseSlot();
							}
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.2869934f, 0.30899984f)),
					new UseSlot(new Vector2(0.49549526f, 0.30899984f)),
					new UseSlot(new Vector2(0.70750487f, 0.30899984f)),
					new UseSlot(new Vector2(0.603f, 0.43299824f)),
					new UseSlot(new Vector2(0.39799652f, 0.43299824f), GainXP),
				])
				.Build())
		];

		protected override bool Persistent => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(2).Build()),
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantState =>
				[
					MoveAbility.Builder()
						.WithDistance(1)
						.WithOnAbilityStarted(async moveState =>
						{
							moveState.AdjustMoveValue(((Summon)moveState.Performer).Stats.Move ?? 0);

							await GDTask.CompletedTask;
						})
						.Build()
				])				
				.WithCustomGetTargets((grantState, figures) =>
				{
					figures.AddRange(((Character)grantState.Performer).Summons);
				})
				.WithTarget(Target.Allies)
				.Build()
			),
		];
	}
}