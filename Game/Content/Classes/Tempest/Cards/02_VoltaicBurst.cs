using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class VoltaicBurst : TempestCardModel<VoltaicBurst.CardTop, VoltaicBurst.CardBottom>
{
	public override string Name => "Voltaic Burst";
	public override int Level => 1;
	public override int Initiative => 27;
	protected override int AtlasIndex => 2;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithPierce(2)
				.WithAbilityStartedSubscription(
					LoseSparksSubscription<ScenarioEvents.AbilityStarted.Parameters>(3, async parameters =>
					{
						((AttackAbility.State)parameters.AbilityState).AbilitySetAOEPattern(AOEPattern);
						parameters.AbilityState.SetCustomValue(this, "3SparkUsed", true);

						await AbilityCmd.GainXP(parameters.AbilityState.Performer, 1);
					}, new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetAOEPattern(AOEPattern))}")))
				.WithDuringAttackSubscription(
					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(1, async parameters =>
						{
							parameters.AbilityState.AbilityAdjustRange(1);

							await GDTask.CompletedTask;
						}, new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Targets)}1 enemy within 2 hexes"),
						parameters => !parameters.AbilityState.GetCustomValue<bool>(this, "3SparkUsed")))
				.Build())
		];

		private static readonly AOEPattern AOEPattern = new AOEPattern(
			[
				new AOEHex(Vector2I.Zero, AOEHexType.Gray),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
			]
		);
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62113076f, 0.6553017f)))
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(1, new MoveCircle(this, new Vector2(0.623959f, 0.89030474f)))
				.WithConditionalAbilityCheck(async state =>
				{
					MoveAbility.State previousMoveState = state.ActionState.GetAbilityState<MoveAbility.State>(0);

					bool performAbility = previousMoveState.Performed &&
					                      AbilityCmd.HexesInStraightLine(previousMoveState.Origin, previousMoveState.Hexes, 3);
					if(!performAbility)
					{
						return false;
					}

					await AbilityCmd.InfuseElement(state, Element.Air);
					await AbilityCmd.GainXP(state.Performer, 1);
					return true;
				})
				.Build()),
		];
	}
}