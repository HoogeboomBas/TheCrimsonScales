using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class VengeanceOfAksut : IncarnateCardModel<VengeanceOfAksut.CardTop, VengeanceOfAksut.CardBottom>
{
	public override string Name => "Vengeance of Aksut";
	public override int Level => 3;
	public override int Initiative => 15;
	protected override int AtlasIndex => 17;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters =>
							parameters.Figure == state.Performer && parameters.FromAttack && parameters.WouldSufferDamage,
						async parameters =>
						{
							parameters.AdjustShield(3);

							object subscriber = new object();

							ScenarioEvents.RetaliateEvent.Subscribe(state, this,
								canApplyParameters => canApplyParameters.RetaliatingFigure == state.Performer &&
								                      RangeHelper.Distance(canApplyParameters.AbilityState.Performer.Hex, state.Performer.Hex) <= 1 &&
								                      canApplyParameters.AbilityState == parameters.PotentialAbilityState,
								async applyParameters =>
								{
									applyParameters.AdjustRetaliate(3);

									await GDTask.CompletedTask;
								}
							);

							ScenarioEvents.AfterAttackPerformedEvent.Subscribe(state, subscriber,
								canApplyParameters => canApplyParameters.AbilityState == parameters.PotentialAbilityState,
								async _ =>
								{
									ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
									ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(state, subscriber);

									await GDTask.CompletedTask;
								}
							);

							await state.AdvanceUseSlot();
						}
					);

					ScenarioCheckEvents.ShieldCheckEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Figure == state.Performer,
						applyParameters =>
						{
							applyParameters.AdjustShield(3);
						}
					);

					ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Figure == state.Performer,
						applyParameters =>
						{
							applyParameters.AddRetaliate(3, 1);
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.RetaliateCheckEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.29180175f, 0.32963988f), GainXP),
					new UseSlot(new Vector2(0.49901202f, 0.32963988f)),
					new UseSlot(new Vector2(0.7062223f, 0.32963988f), GainXP)
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2, new HealCircle(this, new Vector2(0.445411f, 0.65761775f)))
				.WithRange(2)
				.WithConditions(Incarnate.Empower)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Reaver))
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Ritualist, IncarnateSpirit.Conqueror];
		public override int XP => 1;
	}
}