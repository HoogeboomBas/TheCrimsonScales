using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Luminescence : LuminaryCardModel<Luminescence.CardTop, Luminescence.CardBottom>
{
	public override string Name => "Luminescence";
	public override int Level => 2;
	public override int Initiative => 66;
	protected override int AtlasIndex => 15;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GlowActiveAbility.Builder()
				.WithGlowAbility(new GlowAbilityModel([Element.Ice], GlowAbility,
					$"Perform {Icons.Inline(Icons.Heal)}2 ability", Icons.Heal))
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;

		private Ability GlowAbility(List<Element> elements)
		{
			return HealAbility.Builder()
				.WithHealValue(2)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					]
				))
				.WithOnAbilityStarted(async state =>
				{
					state.SetCustomValue(state.Performer, "Glow Ability", true);
					state.SetCustomValue(state.Performer, "Consumed Elements", elements);

					await GDTask.CompletedTask;
				})
				.Build();
		}
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62133586f, 0.6296019f)))
				.WithDuringMovementSubscription(
					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Ice,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(1);
							parameters.AbilityState.SetCustomValue(this, "IceConsumed", true);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"+1{Icons.Inline(Icons.Move)}, {Icons.Inline(Icons.Heal)}3, self")
					)
				)
				.WithOnAbilityEndedPerformed(async state =>
				{
					if(state.GetCustomValue<bool>(this, "IceConsumed"))
					{
						await AbilityCmd.GainXP(state.Performer, 1);
					}
				})
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<MoveAbility.State>(0).GetCustomValue<bool>(this, "IceConsumed");
				})
				.Build())
		];
	}
}