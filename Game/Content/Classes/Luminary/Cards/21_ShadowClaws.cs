using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ShadowClaws : LuminaryCardModel<ShadowClaws.CardTop, ShadowClaws.CardBottom>
{
	public override string Name => "Shadow Claws";
	public override int Level => 5;
	public override int Initiative => 25;
	protected override int AtlasIndex => 21;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GlowActiveAbility.Builder()
				.WithGlowAbility(new GlowAbilityModel([Element.Dark], GlowAbility,
					$"Perform {Icons.Inline(Icons.GetCondition(Conditions.Muddle))} ability", Icons.GetCondition(Conditions.Muddle)))
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;

		private Ability GlowAbility(List<Element> elements)
		{
			return ConditionAbility.Builder()
				.WithConditions(Conditions.Muddle)
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
				.WithOnAbilityEnded(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state.Performer, this);
					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state.Performer, this);
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state.Performer, this,
						parameters => parameters.AbilityState.Target.HasCondition(Conditions.Muddle),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetSetHasAdvantage();

							await GDTask.CompletedTask;
						}
					);

					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state.Performer, this,
						parameters => true,
						async parameters =>
						{
							ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state.Performer, this);
							ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state.Performer, this);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.Build();
		}
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.RemoveOneNegativeCondition(state.Performer);
				})
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6213844f, 0.674216f)))
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AbilityEndedEvent.Subscribe(state, this,
						canApply: parameters => parameters.AbilityState.Performer == state.Performer &&
						                        parameters.AbilityState.GetCustomValue<bool>(state.Performer, "Glow Ability") &&
						                        parameters.AbilityState.TryGetCustomValue<List<Element>>(state.Performer, "Consumed Elements", out _),
						async parameters =>
						{
							await state.ActionState.RequestDiscardOrLose();
							await AbilityCmd.InfuseElement(state, global::Elements.All
								.Except(parameters.AbilityState.GetCustomValue<List<Element>>(state.Performer, "Consumed Elements"))
								.ToList());
						},
						effectButtonParameters: new IconEffectButton.Parameters(Icons.WildElement),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Infuse {Icons.Inline(Icons.WildElement)} other than any of the consumed elements"),
						effectType: EffectType.Selectable
					);
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityEndedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build()),
		];

		public override bool Persistent => true;
	}
}