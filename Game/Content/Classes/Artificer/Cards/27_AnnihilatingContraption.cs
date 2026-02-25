using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class AnnihilatingContraption : ArtificerCardModel<AnnihilatingContraption.CardTop, AnnihilatingContraption.CardBottom>
{
	public override string Name => "Annihilating Contraption";
	public override int Level => 9;
	public override int Initiative => 89;
	protected override int AtlasIndex => 27;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Clockwork Ravager")
				.WithTexturePath("res://Content/Classes/Artificer/Summons/ClockworkRavager.png")
				.WithHealth(6, new SummonHealthSquare(this, new Vector2(0.44784817f, 0.2174603f)))
				.WithMove(3, new SummonMoveSquare(this, new Vector2(0.67777777f, 0.2173603f)))
				.WithAttack(3, new SummonAttackSquare(this, new Vector2(0.4475482f, 0.29367992f), EnhancementCostType.MultiTarget))
				.WithTraits(new TargetsTrait(2), new ApplyConditionTrait(Conditions.Wound1))
				.Build()),
			TimedTrack(
			[
				new UseSlot(new Vector2(0.15925926f, 0.3962963f)),
				new UseSlot(new Vector2(0.36666667f, 0.3962963f), GainXP),
				new UseSlot(new Vector2(0.5748148f, 0.3962963f), GainScrapToken),
				new UseSlot(new Vector2(0.78666663f, 0.3962963f), GainXP)
			])
		];

		public override Func<Figure, GDTask<bool>> OnCardSideStarted => async figure => await TryLoseScrapTokens(figure, 4);
		public override bool Persistent => true;
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreateTrapAbility.Builder()
				.WithDamage(3)
				.WithConditions(Conditions.Poison1)
				.WithRange(3)
				.WithTrapCount(3)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					CreateTrapAbility.State trapState = state.ActionState.GetAbilityState<CreateTrapAbility.State>(0);

					ScenarioEvents.TrapTriggeredEvent.Subscribe(state, this,
						canApply: canApplyParameters => trapState.CreatedTraps.Contains(canApplyParameters.Trap),
						async _ =>
						{
							await GainScrapToken(state);
						}
					);
					ScenarioEvents.HexObjectDestroyedEvent.Subscribe(state, this,
						_ => trapState.CreatedTraps.All(trap => trap.IsDestroyed),
						async _ =>
						{
							await state.ActionState.RequestDiscardOrLose();
						});
					foreach(Trap trap in trapState.CreatedTraps)
					{
						await AbilityCmd.AddCharacterToken(state, trap,
							$"Artificer gains 1{Icons.Inline(Artificer.ScrapToken)} when this trap is sprung.");
					}
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.TrapTriggeredEvent.Unsubscribe(state, this);
					ScenarioEvents.HexObjectDestroyedEvent.Unsubscribe(state, this);
					foreach(Trap trap in state.ActionState.GetAbilityState<CreateTrapAbility.State>(0).CreatedTraps)
					{
						await AbilityCmd.RemoveCharacterToken(state, trap);
					}
				})
				.Build())
		];

		public override bool Persistent => true;
	}
}