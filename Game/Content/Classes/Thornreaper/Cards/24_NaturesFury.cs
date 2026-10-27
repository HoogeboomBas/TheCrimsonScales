using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class NaturesFury : ThornreaperLevelUpCardModel<NaturesFury.CardTop, NaturesFury.CardBottom>
{
	public override string Name => "Nature's Fury";
	public override int Level => 7;
	public override int Initiative => 37;
	protected override int AtlasIndex => 25;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					bool buffedThisRound = false;

					ScenarioEvents.ElementInfusedEvent.Subscribe(state, this, 
						parameters => parameters.Element == Element.Earth &&
						parameters.PotentialInfuser == state.Performer,
						async applyParameters =>
						{
							if (!buffedThisRound)
							{
								buffedThisRound = true;
							}
							await GDTask.CompletedTask;
						});

					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => buffedThisRound && parameters.AbilityState.Performer == state.Performer,
						async apply =>
						{
							apply.AbilityState.AbilityAdjustAttackValue(1);
						});

					ScenarioEvents.RoundEndedEvent.Subscribe(state, this, _ => true, async _ =>
					{
						buffedThisRound = false;
						await GDTask.CompletedTask;
					});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.ElementInfusedEvent.Unsubscribe(state, this);
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 1;
		public override bool Loss => true;
		public override bool Persistent => true;
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			ConsumeEarthOrSkipAction(),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
				.WithRange(4, new RangeSquare(this, new Vector2(0.2f, 0.3f)))
				.WithPull(3, new PullSquare(this, new Vector2(0.2f, 0.3f)))
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build()),

			// You may ignore any requirements to [consume Earth] on your next action this turn.
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Thornreaper.SetIgnoreEarthConsume(true);

					ScenarioEvents.RoundEndedEvent.Subscribe(state.Performer, this,
						_ => true,
						async _ =>
						{
							Thornreaper.SetIgnoreEarthConsume(false);
							ScenarioEvents.RoundEndedEvent.Unsubscribe(state.Performer, this);
							await GDTask.CompletedTask;
						});
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}
}
