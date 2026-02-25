using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class TrudgingBulwark : ArtificerCardModel<TrudgingBulwark.CardTop, TrudgingBulwark.CardBottom>
{
	public override string Name => "Trudging Bulwark";
	public override int Level => 3;
	public override int Initiative => 83;
	protected override int AtlasIndex => 16;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Clockwork Guardian")
				.WithTexturePath("res://Content/Classes/Artificer/Summons/ClockworkGuardian.png")
				.WithHealth(7, new SummonHealthSquare(this, new Vector2(0.4474074f, 0.24708992f)))
				.WithMove(1, new SummonMoveSquare(this, new Vector2(0.67777777f, 0.24784814f)))
				.WithAttack(1, new SummonAttackSquare(this, new Vector2(0.44666666f, 0.32433861f)))
				.WithTraits(new ShieldTrait(2))
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
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					Figure figure = state.GetCustomValue<Figure>(this, "Figure");
					ScenarioCheckEvents.ShieldCheckEvent.Subscribe(state, this,
						parameters => parameters.Figure == figure,
						parameters =>
						{
							parameters.AdjustShield(3);
						});


					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters => parameters.Figure == figure && parameters.FromAttack && parameters.WouldSufferDamage,
						async parameters =>
						{
							parameters.AdjustShield(3);
							ScenarioEvents.AfterAttackPerformedEvent.Subscribe(state, this,
								afterAttackParameters => parameters.PotentialAbilityState == afterAttackParameters.AbilityState &&
								                         afterAttackParameters.AbilityState.Target == figure,
								async afterAttackParameters =>
								{
									if(!afterAttackParameters.AbilityState.DamagedFigures.Contains(figure))
									{
										await GainScrapToken(state);
										await AbilityCmd.GainXP(state.Performer, 1);
									}

									await state.AdvanceUseSlot();
								});
							await GDTask.CompletedTask;
						}
					);
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					await AbilityCmd.RemoveCharacterToken(state, state.GetCustomValue<Figure>(this, "Figure"));
					ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(state, this);
				})
				.WithUseSlot(new UseSlot(new Vector2(0.5f, 0.8730158f)))
				.WithConditionalAbilityCheck(async state =>
				{
					Figure figure = await AbilityCmd.SelectFigure(state,
						figures => figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 3)
							.Where(figure => state.Performer.AlliedWith(figure))), hintText: () => "Select an ally to place a character token on");
					if(figure == null)
					{
						return false;
					}

					state.SetCustomValue(this, "Figure", figure);

					await AbilityCmd.AddCharacterToken(state, figure,
						$"On the next source of {Icons.Inline(Icons.Damage)} from an attack, gain {Icons.Inline(Icons.Shield)}3. If no {Icons.Inline(Icons.Damage)} is suffered, Artificer gains 1{Icons.Inline(Artificer.ScrapToken)}.");
					return true;
				})
				//With mandatory so forced to activate if placing a character token
				.WithMandatory(true)
				.Build())
		];

		public override bool Persistent => true;
	}
}