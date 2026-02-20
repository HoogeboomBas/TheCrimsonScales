using System.Collections.Generic;
using Fractural.Tasks;

public class SymphonyOfOppression : HierophantLevelUpCardModel<SymphonyOfOppression.CardTop, SymphonyOfOppression.CardBottom>
{
	public override string Name => "Symphony of Oppression";
	public override int Level => 7;
	public override int Initiative => 86;
	protected override int AtlasIndex => 15 - 11;

	public class CardTop : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantAbilityState =>
				[
					AttackAbility.Builder()
						.WithDamage(3)
						.WithRange(3)
						.WithOnAbilityStarted(async state =>
						{
							if(grantAbilityState.GetCustomValue<bool>(this, "TargetOneAlly"))
							{
								state.AbilityAdjustAttackValue(1);
							}
							else if(grantAbilityState.GrantAbilityActionStates.Count > 1)
							{
								state.SetAbilityFilterTargets((attackAbilityState, figure) =>
									grantAbilityState.GrantAbilityActionStates[0].GetAbilityState<AttackAbility.State>(0).Target == figure);
							}

							await GDTask.CompletedTask;
						})
						.Build(),
				])
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => true,
						async parameters =>
						{
							parameters.AbilityState.SetCustomValue(this, "TargetOneAlly", true);
							((GrantAbility.State)parameters.AbilityState).AdjustTargets(-1);
							await GDTask.CompletedTask;
						}, EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.Attack),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Target only one ally for +1{Icons.Inline(Icons.Attack)}, give that ally a Prayer card")
					)
				)
				.WithOnAbilityEndedPerformed(async grantAbilityState =>
				{
					if(grantAbilityState.GetCustomValue<bool>(this, "TargetOneAlly"))
					{
						await GivePrayerCard(grantAbilityState, grantAbilityState.Target);
					}
				})
				.WithTargets(2)
				.Build()),
		];
	}

	public class CardBottom : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
						parameters => parameters.AbilityState.Performer.AlliedWith(state.Performer) &&
						              (parameters.AbilityState is ShieldAbility.State || parameters.AbilityState is RetaliateAbility.State),
						async parameters =>
						{
							if(parameters.AbilityState is ShieldAbility.State shieldAbilityState)
							{
								shieldAbilityState.AdjustAdditionalShield(1);
							}
							else if(parameters.AbilityState is RetaliateAbility.State retaliateAbilityState)
							{
								retaliateAbilityState.AdjustRetaliateValue(1);
							}

							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build()),
		];

		public override int XP => 2;
		public override bool Round => true;
		public override bool Loss => true;
	}
}