using System.Collections.Generic;
using Godot;
using Fractural.Tasks;

public class SolarEclipse : StarslingerCardModel<SolarEclipse.CardTop, SolarEclipse.CardBottom>
{
	public override string Name => "Solar Eclipse";
	public override int Level => 1;
	public override int Initiative => 60;
	protected override int AtlasIndex => 7;

	public class CardTop : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.4989511f, 0.20602313f)))
				.WithAOEPattern(new AOEPattern(
				[
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthWest).Add(Direction.NorthEast), AOEHexType.Yellow),
				]))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Invisible)
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
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						parameters =>
							parameters.Figure == state.Performer &&
							GameController.Instance.ElementManager.GetState(Element.Dark) != ElementState.Inert,
						async parameters =>
						{
							ActionState moveAbility = new ActionState(state.Performer,
							[
								MoveAbility.Builder()
									.WithDistance(2)
									.Build()
							]);
							await moveAbility.Perform();

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.26499918f, 0.7735108f), Heal),
					new UseSlot(new Vector2(0.47649646f, 0.7735108f), GainXP),
					new UseSlot(new Vector2(0.6810046f, 0.7735108f), Heal),
					new UseSlot(new Vector2(0.37349778f, 0.9015168f), GainXP),
					new UseSlot(new Vector2(0.58199996f, 0.9015168f), Heal),
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;

		private async GDTask Heal(AbilityState state)
		{
			ActionState healAbility = new ActionState(state.Performer, [
				HealAbility.Builder()
					.WithHealValue(1)
					.WithRange(3)
					.Build()
			]);
			await healAbility.Perform();
		}
	}
}