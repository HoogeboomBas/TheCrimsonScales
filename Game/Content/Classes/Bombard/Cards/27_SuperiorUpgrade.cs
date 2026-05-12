using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SuperiorUpgrade : BombardCardModel<SuperiorUpgrade.CardTop, SuperiorUpgrade.CardBottom>
{
	public override string Name => "Superior Upgrade";
	public override int Level => 9;
	public override int Initiative => 09;
	protected override int AtlasIndex => 27;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					bool immobilizedThisRound = false;
					//TODO: Make it possible to do during any time in the round (or maybe not because that'd cause a lot of extra prompts)
					AbilityCmd.SubscribeDuringCharacterTurn(ScenarioEvents.GetSubscriberPair(state, this),
						EffectType.Selectable, character => character == state.Performer && !immobilizedThisRound,
						async character =>
						{
							immobilizedThisRound = true;
							await AbilityCmd.AddCondition(state, character, Conditions.Immobilize);
							ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
								parameters => parameters.Performer == state.Performer,
								async parameters =>
								{
									parameters.AbilityState.SingleTargetAdjustAttackValue(2);
									ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
									await GDTask.CompletedTask;
								});
						}, new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Immobilize)),
						new TextEffectInfoView.Parameters(
							$"Gain {Icons.Inline(Icons.GetCondition(Conditions.Immobilize))} to add +2{Icons.Inline(Icons.Attack)} to your next attack this round"));
					ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
						_ => true,
						async _ =>
						{
							immobilizedThisRound = false;
							ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					AbilityCmd.UnsubscribeDuringCharacterTurn(ScenarioEvents.GetSubscriberPair(state, this));
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(1, new MoveCircle(this, new Vector2(0.62398136f, 0.7137654f)))
				.Build()),
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(3)
				.Build())
		];

		public override bool Round => true;
	}
}