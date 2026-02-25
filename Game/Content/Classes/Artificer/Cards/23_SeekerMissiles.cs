using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SeekerMissiles : ArtificerCardModel<SeekerMissiles.CardTop, SeekerMissiles.CardBottom>
{
	public override string Name => "Seeker Missiles";
	public override int Level => 7;
	public override int Initiative => 77;
	protected override int AtlasIndex => 23;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithTargets(4, new TargetsSquare(this, new Vector2(0.49980003f, 0.2356079f)))
				.WithRange(4, new RangeSquare(this, new Vector2(0.7037037f, 0.23597883f)))
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

					return state.ActionState.GetAbilityState<AttackAbility.State>(0).KilledTargets.Count >= 1;
				})
				.Build())
		];
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Clockwork Rocket")
				.WithTexturePath("res://Content/Classes/Artificer/Summons/ClockworkRocket.png")
				.WithHealth(1)
				.WithMove(6)
				.WithAttack(5)
				.WithTraits(new FlyingTrait(), new TargetAllAdjacentTrait(), new IgnoreRetaliateTrait())
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Summon summon = state.ActionState.GetAbilityState<SummonAbility.State>(0).Summon;
					ScenarioEvents.AbilityPerformedEvent.Subscribe(state, this,
						parameters => parameters.Performer == summon && parameters.AbilityState is AttackAbility.State,
						async parameters =>
						{
							await AbilityCmd.KillOrExhaust(state, parameters.Performer);
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityPerformedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithMandatory(true)
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		public override Func<Figure, GDTask<bool>> OnCardSideStarted => async figure => await TryLoseScrapTokens(figure, 2);
		public override int XP => 1;
		public override bool Persistent => true;
	}
}