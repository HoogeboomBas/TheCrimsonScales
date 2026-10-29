using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class PakhsLunge : IncarnateCardModel<PakhsLunge.CardTop, PakhsLunge.CardBottom>
{
	public override string Name => "Pakh's Lunge";
	public override int Level => 1;
	public override int Initiative => 53;
	protected override int AtlasIndex => 7;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.61930263f, 0.23157895f), EnhancementCostType.MultiTarget))
				.WithAbilityStartedSubscription(
					InSpiritSubscription<ScenarioEvents.AbilityStarted.Parameters>(IncarnateSpirit.Ritualist,
						async parameters =>
						{
							((AttackAbility.State)parameters.AbilityState).AbilitySetAOEPattern(new AOEPattern(
								[
									new AOEHex(Vector2I.Zero, AOEHexType.Gray),
									new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
									new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
								]
							));

							await GDTask.CompletedTask;
						}))
				.WithDuringAttackSubscription(
					InSpiritSubscription<ScenarioEvents.DuringAttack.Parameters>(IncarnateSpirit.Conqueror,
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustPush(2);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						}))
				.Build())
		];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.4159727f, 0.70249313f)))
				.WithDuringMovementSubscription(
					InSpiritSubscription<ScenarioEvents.DuringMovement.Parameters>(IncarnateSpirit.Ritualist,
						async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(1);
							parameters.AbilityState.AdjustMoveType(MoveType.Jump);

							await GDTask.CompletedTask;
						}))
				.Build()),
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithRange(1)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Reaver))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.InfuseElement(state, Element.Fire);
				})
				.Build()),
		];
	}
}