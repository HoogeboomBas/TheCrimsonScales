using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class VoidEnhancedArmory : HollowpactCardModel<VoidEnhancedArmory.CardTop, VoidEnhancedArmory.CardBottom>
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
					new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Damage)}")))
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
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this, 
						parameters => parameters.Figure == state.Performer,
						async parameters =>
						{
							ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this, 
								LoseVoidEnergySubscription<ScenarioEvents.AttackAfterTargetConfirmed.Parameters>(1,
									async parameters =>
									{
										ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
											parameters => parameters.Performer == state.Performer,
											async parameters =>
											{
												parameters.AbilityState.AbilityAdjustAttackValue(1);

												await GDTask.CompletedTask;
											});

										ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

										await GDTask.CompletedTask;
									}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Damage)} to all your attacks for the round.")));
						});

					ScenarioEvents.RoundEndedEvent.Subscribe(state, this, 
						parameters => true,
						async parameters =>
						{
							ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
							ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
						});

				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);
					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}