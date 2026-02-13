using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class SpiritualGains : HierophantLevelUpCardModel<SpiritualGains.CardTop, SpiritualGains.CardBottom>
{
	public override string Name => "Spiritual Gains";
	public override int Level => 6;
	public override int Initiative => 94;
	protected override int AtlasIndex => 15 - 7;

	public class CardTop : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					int characterTokens = 0;
					//TODO: Add visual for character tokens
					ScenarioEvents.AbilityCardSideEndedEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer &&
						              parameters.AbilityCardSide.AbilityCard.CardState is
							              CardState.PersistentLoss or
							              CardState.Lost or
							              CardState.RoundLoss or
							              CardState.UnrecoverablyLost &&
						              parameters.AbilityCardSide.Model != this,
						async parameters =>
						{
							characterTokens++;
							await GDTask.CompletedTask;
						});

					ScenarioEvents.LongRestStartedEvent.Subscribe(state, this,
						parameters => parameters.Character == state.Performer && characterTokens > 0,
						async parameters =>
						{
							parameters.SetLoseCard(false);
							characterTokens--;
							await GDTask.CompletedTask;
						}, EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.LoseCard),
						effectInfoViewParameters: new AbilityCardEffectInfoView.Parameters(GetAbilityCardSide(state)));

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityCardSideEndedEvent.Unsubscribe(state, this);
					ScenarioEvents.LongRestStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5, new MoveCircle(this, new Vector2(0.619969f, 0.62733525f)))
				.Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.ShortRestStartedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Character == state.Performer,
						async applyParameters =>
						{
							applyParameters.SetLoseCard(false);
							foreach(ItemModel item in applyParameters.Character.Items.Where(item => item.ItemUseType == ItemUseType.Spend))
							{
								await AbilityCmd.RefreshItem(item);
							}

							ActionState actionState = new ActionState(applyParameters.Character,
							[
								HealAbility.Builder()
									.WithHealValue(7)
									.WithTarget(Target.Self)
									.WithConditions(Conditions.Invisible)
									.Build()
							]);
							await actionState.Perform();
							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlot(new UseSlot(new Vector2(0.48200023f, 0.90800273f), GainXP))
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}
}