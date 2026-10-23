using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ScorchingArcbolt : TempestCardModel<ScorchingArcbolt.CardTop, ScorchingArcbolt.CardBottom>
{
	public override string Name => "Scorching Arcbolt";
	public override int Level => 1;
	public override int Initiative => 45;
	protected override int AtlasIndex => 4;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.40588385f, 0.12797785f)))
				.WithRange(2, new RangeSquare(this, new Vector2(0.6153223f, 0.12783185f)))
				.WithConditions(Conditions.Wound1)
				.WithDuringAttackSubscriptions(
				[
					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(1, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustRange(1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Range)}")),
					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(1, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustRange(1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Range)}"))
				])
				.WithAfterAttackPerformedSubscription(
					LoseSparksSubscription<ScenarioEvents.AfterAttackPerformed.Parameters>(1, async parameters =>
						{
							Hex hex = parameters.Performer.Hex;
							List<Hex> path = [];
							for(int i = parameters.AbilityState.SingleTargetRange - 1; i >= 0; i--)
							{
								hex = await AbilityCmd.SelectHex(parameters.AbilityState,
									hexes => hexes.AddRange(RangeHelper.GetHexesInRange(hex, 1, false).Where(possibleHex =>
										RangeHelper.Distance(possibleHex, parameters.AbilityState.Target.Hex) <= i)), true,
									"Select the next hex in the path");
								path.Add(hex);
							}

							path.RemoveLast();

							foreach(Figure figure in path.SelectMany(hexInPath => hexInPath.GetFigures())
								        .Where(figure => parameters.Performer.EnemiesWith(figure)))
							{
								await AbilityCmd.SufferDamage(parameters.AbilityState, figure, 3);
							}

							parameters.AbilityState.ActionState.SetOverrideLoss();

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						new TextEffectInfoView.Parameters(
							$"All enemies on a single direct or indirect path of your choice to the target, whose length is equal to the {Icons.Inline(Icons.Range)}, suffer {Icons.Inline(Icons.Damage)}3, {Icons.Inline(Icons.LoseCard)}")))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.6185266f, 0.68088645f)))
				.WithDuringMovementSubscriptions(
				[
					LoseSparksSubscription<ScenarioEvents.DuringMovement.Parameters>(1, async parameters =>
					{
						parameters.AbilityState.AdjustMoveValue(1);

						await GDTask.CompletedTask;
					}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Move)}")),

					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Air,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(1);
							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Move)}"))
				])
				.Build())
		];
	}
}