using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class AerialDominance : TempestCardModel<AerialDominance.CardTop, AerialDominance.CardBottom>
{
	public override string Name => "Aerial Dominance";
	public override int Level => 1;
	public override int Initiative => 39;
	protected override int AtlasIndex => 11;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer && parameters.Performer.HasCondition(Conditions.Dodge),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(2);

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots([
					new UseSlot(new Vector2(0.16142225f, 0.32963988f), async state => await AbilityCmd.InfuseElement(state, Element.Air)),
					new UseSlot(new Vector2(0.36785644f, 0.32963988f), GainXP),
					new UseSlot(new Vector2(0.5766189f, 0.32963988f), async state => await AbilityCmd.InfuseElement(state, Element.Air)),
					new UseSlot(new Vector2(0.7869334f, 0.32963988f), GainXP),
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.5239462f, 0.68688065f)))
				.WithMoveType(MoveType.Jump)
				.WithDuringMovementSubscription(
					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Air, applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(1);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Move)}")))
				.WithOnAbilityEndedPerformed(async state =>
				{
					foreach(Figure figure in state.Hexes.SelectMany(hex => hex.GetFigures().Where(figure => state.Performer.EnemiesWith(figure)))
						        .Distinct())
					{
						await AbilityCmd.AddCondition(state, figure, Conditions.Muddle);
					}
				})
				.Build()),
		];
	}
}