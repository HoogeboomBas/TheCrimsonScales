using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class VoidEnhancedArmory : HollowpactLevelUpCardModel<VoidEnhancedArmory.CardTop, VoidEnhancedArmory.CardBottom>
{
	public override string Name => "Void-Enhanced Armory";
	public override int Level => 4;
	public override int Initiative => 17;
	protected override int AtlasIndex => 4;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ShieldAbility.Builder().WithShieldValue(1).Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.44718847f, 0.19982125f)))
				.WithDuringAttackSubscription(LoseVoidEnergySubscription<ScenarioEvents.DuringAttack.Parameters>(2,
					async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(2);
						await AbilityCmd.GainXP(parameters.AbilityState.Performer, 1);
					},
					new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Damage)}")))
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];

		public override bool Round => true;
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					// Effect to start immediately
					await _attackSubscription(state, this);
		
					// Effect to start each following turn
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this, 
						parameters => parameters.Figure == state.Performer,
						async parameters =>
						{
							await _attackSubscription(state, this);
						});

					ScenarioEvents.RoundEndedEvent.Subscribe(state, this, 
						parameters => true,
						async parameters =>
						{
							ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);
							ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
						});
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);
					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);
					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
				})
				.Build())
		];

		Func<OtherActiveAbility.State, HollowpactCardSide, GDTask> _attackSubscription = 
			async (state, cardSide) => ScenarioEvents.AbilityStartedEvent.Subscribe(state, cardSide,
				LoseVoidEnergySubscription<ScenarioEvents.AbilityStarted.Parameters>(1,
					async parameters =>
					{
						ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, cardSide);

						ScenarioEvents.DuringAttackEvent.Subscribe(state, cardSide,
							parameters => parameters.Performer == state.Performer,
							async parameters =>
							{
								parameters.AbilityState.SingleTargetAdjustAttackValue(1);

								await GDTask.CompletedTask;
							});

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Damage)} to all your attacks for the round.")));

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}