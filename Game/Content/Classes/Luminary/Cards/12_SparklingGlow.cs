using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class SparklingGlow : LuminaryCardModel<SparklingGlow.CardTop, SparklingGlow.CardBottom>
{
	public override string Name => "Sparkling Glow";
	public override int Level => 1;
	public override int Initiative => 27;
	protected override int AtlasIndex => 12;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.4578724f, 0.19449025f)))
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					]
				))
				.WithDuringAttackSubscriptions(
				[
					ScenarioEvents.DuringAttack.Subscription.ConsumeElements([Element.Fire, Element.Light],
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustTargets(1);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Targets)}")
					),
					ScenarioEvents.DuringAttack.Subscription.ConsumeElements([Element.Ice, Element.Dark],
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustTargets(1);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Targets)}")
					)
				])
				.Build()),
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6210601f, 0.6518512f)))
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AbilityEndedEvent.Subscribe(state, this,
						canApply: parameters => parameters.AbilityState.Performer == state.Performer &&
						                        parameters.AbilityState.GetCustomValue<bool>(state.Performer, "Glow Ability"),
						apply: async parameters =>
						{
							//TODO: Change to work with the damage glow
							if(parameters.AbilityState is TargetedAbilityState targetedAbilityState && targetedAbilityState.GetRedAOEHexes().Any())
							{
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
											$"Select an ally to grant {Icons.HintText(Icons.Heal)}2, Self"
										)
										.Build()
								]);
								await actionState.Perform();
							}

							await state.ActionState.RequestDiscardOrLose();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityEndedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
		];

		public override bool Round => true;
	}
}