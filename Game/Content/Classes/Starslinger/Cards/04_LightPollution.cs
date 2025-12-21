using System.Collections.Generic;
using Godot;
using System.Linq;
using Fractural.Tasks;
using System.Diagnostics;

public class LightPollution : StarslingerCardModel<LightPollution.CardTop, LightPollution.CardBottom>
{
	public override string Name => "Light Pollution";
	public override int Level => 1;
	public override int Initiative => 44;
	protected override int AtlasIndex => 4;

	public class CardTop : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Poison1)
				.WithAOEPattern(new AOEPattern([
							new AOEHex(Vector2I.Zero, AOEHexType.Gray),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Yellow),
							new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Yellow),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
						]))
				.Build()),
			new AbilityCardAbility(OtherTargetedAbility.Builder()
				.WithOnAfterConditionsApplied(async (state, target) =>
				{
					for(int i = target.Conditions.Count - 1; i >= 0; i--)
					{
						ConditionModel condition = target.Conditions[i];
						if(condition.IsNegative)
						{
							await AbilityCmd.RemoveCondition(target, condition);
						}
					}
				})
				.WithTarget(Target.Allies | Target.TargetAll)
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithCustomGetTargets((abilityState, list) =>
				{
					ConditionAbility.State conditionAbilityState = abilityState.ActionState.GetAbilityState<ConditionAbility.State>(0);

					foreach(Hex yellowHex in conditionAbilityState.GetYellowAOEHexes())
					{
						foreach(Figure figure in yellowHex.GetHexObjectsOfType<Figure>())
						{
							list.Add(figure);
						}
					}
				})
				.Build())
		];

		protected override int XP => 1;
		protected override IEnumerable<Element> Elements => [Element.Light];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(canApplyParameters => true,
						async parameters =>
						{
							List<Figure> targetedFiguresThisTurn = [];
							targetedFiguresThisTurn.AddRange(
								parameters.Performer.TurnPerformedActionStates
									.SelectMany(a => a.AbilityStates)
									.OfType<TargetedAbilityState>()
									.SelectMany(t => t.UniqueTargetedFigures)
									.Where(f => !targetedFiguresThisTurn.Contains(f))
							);
							int value = targetedFiguresThisTurn.Count;
							MoveAbility.State moveAbilityState = (MoveAbility.State)parameters.AbilityState;
							moveAbilityState.AdjustMoveValue(value);
							await GDTask.CompletedTask;
						}
					)
				)
				.Build()),
		];
	}
}