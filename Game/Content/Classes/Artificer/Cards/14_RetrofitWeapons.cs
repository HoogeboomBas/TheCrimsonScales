using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class RetrofitWeapons : ArtificerCardModel<RetrofitWeapons.CardTop, RetrofitWeapons.CardBottom>
{
	public override string Name => "Retrofit Weapons";
	public override int Level => 2;
	public override int Initiative => 70;
	protected override int AtlasIndex => 14;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Gray),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red)
						]
					), new AOEHexMark(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), this, new Vector2(0.7158962f, 0.16666666f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.SouthEast).Add(Direction.East), this, new Vector2(0.71555555f, 0.28994706f)))
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

					return state.ActionState.GetAbilityState<AttackAbility.State>(0).UniqueTargetedFigures.Count >= 2;
				})
				.Build())
		];
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					List<Figure> usedAttackBonusThisTurn = [];
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.Performer.AlliedWith(state.Performer, true) &&
						              !usedAttackBonusThisTurn.Contains(parameters.Performer),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetSetHasAdvantage();
							parameters.AbilityState.SingleTargetAdjustPierce(2);
							usedAttackBonusThisTurn.Add(parameters.Performer);
							await GDTask.CompletedTask;
						});
					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
						_ => true,
						async _ =>
						{
							usedAttackBonusThisTurn.Clear();
							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build()),
			TimedTrack(
			[
				new UseSlot(new Vector2(0.39703703f, 0.837037f)),
				new UseSlot(new Vector2(0.60518515f, 0.837037f))
			])
		];

		public override Func<Figure, GDTask<bool>> OnCardSideStarted => async figure => await TryLoseScrapTokens(figure, 1);
		public override int XP => 1;
		public override bool Persistent => true;
	}
}