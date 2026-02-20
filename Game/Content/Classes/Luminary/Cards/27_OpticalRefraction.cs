using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class OpticalRefraction : LuminaryCardModel<OpticalRefraction.CardTop, OpticalRefraction.CardBottom>
{
	public override string Name => "Optical Refraction";
	public override int Level => 8;
	public override int Initiative => 37;
	protected override int AtlasIndex => 27;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Empty),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Empty),
						new AOEHex(Vector2I.Zero.Add(Direction.SouthEast).Add(Direction.East), AOEHexType.Empty),
					]
				))
				.WithDuringAttackSubscriptions(
				[
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Dark,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustPierce(3);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Pierce)}3")
					),
					ScenarioEvents.DuringAttack.Subscription.ConsumeWildElement(
						applyFunction: async parameters =>
						{
							await AbilityCmd.InfuseWildElement(parameters.AbilityState);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.WildElement)}")
					)
				])
				.Build()),
			Scuttle(2, [Element.Ice]),
		];
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					bool glowDiscarded = false;
					ScenarioEvents.AbilityPerformedEvent.Subscribe(state, this,
						canApply: canApplyParameters => canApplyParameters.Performer == state.Performer && !glowDiscarded &&
						                                canApplyParameters.AbilityState.GetCustomValue<bool>(state.Performer, "Glow Ability"),
						async applyParameters =>
						{
							glowDiscarded = true;
							ActionState glowActionState = ((Character)state.Performer).Cards
								.SelectMany(card => card.ActiveActionStates)
								.FirstOrDefault(actionState =>
									actionState.AbilityStates.Any(activeAbilityState => activeAbilityState is GlowActiveAbility.State));

							if(glowActionState != null)
							{
								await glowActionState.RequestDiscardOrLose();
							}

							//TODO: Change to work with the damage glow
							if(applyParameters.AbilityState is TargetedAbilityState targetedAbilityState &&
							   targetedAbilityState.GetRedAOEHexes().Any())
							{
								foreach(Figure figure in targetedAbilityState.GetRedAOEHexes().SelectMany(hex => hex.GetHexObjectsOfType<Figure>())
									        .Where(figure => figure.EnemiesWith(applyParameters.Performer)))
								{
									await AbilityCmd.SufferDamage(applyParameters.AbilityState, figure, 2);
								}

								ActionState actionState = new ActionState(state.Performer, [
									GrantAbility.Builder()
										.WithGetAbilities(grantAbilityState =>
											[HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()])
										.WithTarget(Target.Allies | Target.TargetAll)
										.WithCustomGetTargets((state, targets) =>
										{
											targets.AddRange(
												targetedAbilityState.GetRedAOEHexes()
													.SelectMany(hex => hex.GetHexObjectsOfType<Figure>())
											);
										})
										.WithGetTargetingHintText(grantAbilityState =>
											$"Select an ally to grant {Icons.HintText(Icons.Heal)}2, self"
										)
										.Build()
								]);
								await actionState.Perform();
							}

							await GDTask.CompletedTask;
						}, effectType: EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters("res://Content/Classes/Luminary/Glow.svg"),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Discard your active {Icons.Inline("res://Content/Classes/Luminary/Glow.svg")}")
					);

					ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
						canApply: canApplyParameters => true,
						async applyParameters =>
						{
							glowDiscarded = false;

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);
					ScenarioEvents.AbilityPerformedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.InfuseWild()];
		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}