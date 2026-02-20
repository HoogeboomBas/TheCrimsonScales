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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Performer == Chieftain.GetMount(state.Performer),
						async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetAdjustAttackValue(1);

							await GDTask.CompletedTask;
						});

					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Performer == Chieftain.GetMount(state.Performer),
						async applyParameters =>
						{
							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.28949338f, 0.2740003f)),
					new UseSlot(new Vector2(0.49549526f, 0.2740003f)),
					new UseSlot(new Vector2(0.70750487f, 0.2740003f)),
					new UseSlot(new Vector2(0.39499655f, 0.4039986f)),
					new UseSlot(new Vector2(0.59799652f, 0.4039986f), GainXP),
				])
				.Build())
		];

		public override bool Persistent => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62123585f, 0.695882f)))
				.Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					AbilityCmd.SummonMovePlusX(1).Build()
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