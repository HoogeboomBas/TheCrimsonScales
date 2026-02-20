using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BlueMoon : StarslingerCardModel<BlueMoon.CardTop, BlueMoon.CardBottom>
{
	public override string Name => "Blue Moon";
	public override int Level => 6;
	public override int Initiative => 79;
	protected override int AtlasIndex => 22;

	public class CardTop : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.5010511f, 0.15792547f)))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Dark);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.GetElement(Element.Dark)}")
					))
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthWest), AOEHexType.Yellow),
				]))
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(4)
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithCustomGetTargets((abilityState, list) =>
				{
					AttackAbility.State attackAbilityState = abilityState.ActionState.GetAbilityState<AttackAbility.State>(0);

					foreach(Hex yellowHex in attackAbilityState.GetYellowAOEHexes())
					{
						foreach(Figure figure in yellowHex.GetHexObjectsOfType<Figure>())
						{
							list.Add(figure);
						}
					}
				})
				.WithOnAbilityEndedPerformed(async state =>
					{
						await AbilityCmd.GainXP(state.Performer, 1);
					}
				)
				.Build())
		];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.61790806f, 0.674216f)))
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(5)
				.WithTarget(Target.Self)
				.WithOnAbilityStarted(async state =>
				{
					MoveAbility.State moveAbilityState = state.ActionState.GetAbilityState<MoveAbility.State>(0);
					state.AbilityAdjustHealValue(-moveAbilityState.Hexes.Count);
					await GDTask.CompletedTask;
				})
				.Build())
		];
	}
}