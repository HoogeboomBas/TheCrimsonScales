using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ImprovisedMortar : ArtificerCardModel<ImprovisedMortar.CardTop, ImprovisedMortar.CardBottom>
{
	public override string Name => "Improvised Mortar";
	public override int Level => 1;
	public override int Initiative => 93;
	protected override int AtlasIndex => 1;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Clockwork Bombard")
				.WithTexturePath("res://Content/Classes/Artificer/Summons/ClockworkBombard.png")
				.WithHealth(2, new SummonHealthSquare(this, new Vector2(0.4474074f, 0.24867724f)))
				.WithAttack(2, new SummonAttackSquare(this, new Vector2(0.4474074f, 0.3238095f)))
				.WithRange(5, new SummonRangeSquare(this, new Vector2(0.67777777f, 0.3238095f)))
				.WithTraits(new PierceTrait(1))
				.Build()),
			TimedTrack(
			[
				new UseSlot(new Vector2(0.2911111f, 0.42857143f)),
				new UseSlot(new Vector2(0.49925926f, 0.42857143f), GainXP),
				new UseSlot(new Vector2(0.7074074f, 0.42857143f), GainScrapToken),
			])
		];

		public override Func<Figure, GDTask<bool>> OnCardSideStarted => async figure => await TryLoseScrapTokens(figure, 2);
		public override bool Persistent => true;
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithRange(5, new RangeSquare(this, new Vector2(0.6059259f, 0.70687824f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => RangeHelper.Distance(parameters.AbilityState.Target.Hex, parameters.Performer.Hex) >= 4,
						async parameters =>
						{
							parameters.AbilityState.SetCustomValue(this, "TargetedFarEnemy", true);
							await GDTask.CompletedTask;
						}))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await GainScrapToken(state);
					await AbilityCmd.GainXP(state.Performer, 1);
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<AttackAbility.State>(0).GetCustomValue<bool>(this, "TargetedFarEnemy");
				})
				.Build())
		];
	}
}