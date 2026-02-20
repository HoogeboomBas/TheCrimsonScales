using System.Collections.Generic;
using Godot;
using Fractural.Tasks;

public class StoneMeteorite : StarslingerCardModel<StoneMeteorite.CardTop, StoneMeteorite.CardBottom>
{
	public override string Name => "Stone Meteorite";
	public override int Level => 7;
	public override int Initiative => 90;
	protected override int AtlasIndex => 24;

	public class CardTop : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(6, new AttackDiamond(this, new Vector2(0.45299226f, 0.19047385f)))
				.WithRange(5, new RangeSquare(this, new Vector2(0.66311485f, 0.19015712f)))
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => !parameters.Performer.IsDamaged(),
						async parameters =>
						{
							((AttackAbility.State)parameters.AbilityState).AbilitySetAOEPattern(new AOEPattern([
								new AOEHex(Vector2I.Zero, AOEHexType.Red),
								new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
							]));
							parameters.AbilityState.SetCustomValue(this, "Undamaged", 1);

							await GDTask.CompletedTask;
						}
					)
				)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Dark,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Stun);
							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Stun))}")
					))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, state.GetCustomValue<int>(this, "Undamaged"));
				})
				.Build()),
		];

		public override int XP => 1;
		public override bool Loss => true;
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5, new MoveCircle(this, new Vector2(0.6210601f, 0.71188086f)))
				.WithDuringMovementSubscription(
					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(2);
							parameters.AbilityState.AdjustMoveType(MoveType.Jump);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Move)}, {Icons.Inline(Icons.Jump)}")
					)
				)
				.Build()),
		];
	}
}