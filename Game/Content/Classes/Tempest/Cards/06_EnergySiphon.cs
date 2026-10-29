using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class EnergySiphon : TempestCardModel<EnergySiphon.CardTop, EnergySiphon.CardBottom>
{
	public override string Name => "Energy Siphon";
	public override int Level => 1;
	public override int Initiative => 19;
	protected override int AtlasIndex => 6;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.WithOnAbilityEndedPerformed(async state =>
				{
					if(state.LootedObjects.Count(lootedObject => lootedObject is TempestSpark) +
					   state.LootedHexes.SelectMany(hex => hex.GetHexObjectsOfType<TempestSpark>()).Count() >= 2)
					{
						await AbilityCmd.AddCondition(state, state.Performer, Conditions.Strengthen);
					}
				})
				.Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DodgeDiscardCardEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async parameters =>
						{
							List<ScenarioEvents.GenericChoice.Subscription> subscriptions =
							[
								ScenarioEvents.GenericChoice.Subscription.New(
									_ => HasXSparks(state.Performer, 1),
									async _ =>
									{
										LoseSparks(state.Performer);
										parameters.SetDiscardCard(false);

										await GDTask.CompletedTask;
									}, EffectType.Selectable,
									effectButtonParameters: new IconEffectButton.Parameters($"1{Icons.HintText(Tempest.SparkIconPath)}"),
									effectInfoViewParameters: new TextEffectInfoView.Parameters(
										"Prevent the attack")
								)
							];
							await AbilityCmd.GenericChoice(parameters.Figure, subscriptions,
								hintText: $"Spend 1{Tempest.SparkIconPath} instead of discarding a card?");

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DodgeDiscardCardEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlot(new UseSlot(new Vector2(0.50056416f, 0.4249685f), GainXP))
				.Build())
		];

		public override bool Persistent => true;
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.6187266f, 0.65615577f)))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);

							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")))
				.Build()),
			new AbilityCardAbility(GainSparkAbilityBuilder()
				.WithConditionalAbilityCheck(async state =>
				{
					if(state.ActionState.GetAbilityState<AttackAbility.State>(0).DamagedTargets.Count <= 0)
					{
						return false;
					}

					await AbilityCmd.GainXP(state.Performer, 1);
					return true;
				})
				.Build())
		];
	}
}