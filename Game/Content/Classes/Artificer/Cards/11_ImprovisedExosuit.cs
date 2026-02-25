using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ImprovisedExosuit : ArtificerCardModel<ImprovisedExosuit.CardTop, ImprovisedExosuit.CardBottom>
{
	public override string Name => "Improvised Exosuit";
	public override int Level => 1;
	public override int Initiative => 90;
	protected override int AtlasIndex => 11;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Hollow Carapace")
				.WithTexturePath("res://Content/Classes/Artificer/Summons/HollowCarapace.png")
				.WithHealth(9)
				.WithOnAbilityEndedPerformed(async state =>
				{
					state.Summon.DestroyedEvent += async _ =>
					{
						await GainScrapToken(state);
						await AbilityCmd.GainXP(state.Performer, 2);
					};
					await GDTask.CompletedTask;
				})
				.Build()),
			TimedTrack(
			[
				new UseSlot(new Vector2(0.39703703f, 0.39999998f), GainScrapToken),
				new UseSlot(new Vector2(0.6064666f, 0.4008582f), GainScrapToken),
			])
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);
							parameters.AbilityState.SingleTargetAdjustPierce(1);
							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build()),
			TimedTrack(
			[
				new UseSlot(new Vector2(0.2911111f, 0.8513227f), GainScrapToken),
				new UseSlot(new Vector2(0.50074077f, 0.8513227f), GainXP),
				new UseSlot(new Vector2(0.7074074f, 0.8513227f), GainScrapToken),
			])
		];

		public override Func<Figure, GDTask<bool>> OnCardSideStarted => async figure => await TryLoseScrapTokens(figure, 3);
		public override bool Persistent => true;
	}
}