using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class MagneticField : BrightsparkCardModel<MagneticField.CardTop, MagneticField.CardBottom>
{
	public override string Name => "Magnetic Field";
	public override int Level => 1;
	public override int Initiative => 28;
	protected override int AtlasIndex => 9;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(2)
				.WithTargets(2)
				.WithRange(1)
				.WithOnAbilityStarted(async state =>
				{
					await AbilityCmd.GenericChoice(state.Performer,
					[
						ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async applyParameters =>
							{
								await GDTask.CompletedTask;
							},
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Push),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"Perform {Icons.Inline(Icons.Push)} ability"),
							effectType: EffectType.SelectableMandatory
						),
						ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async applyParameters =>
							{
								state.SetBlocked();
								state.SetCustomValue(this, "ChosePull", true);
								await GDTask.CompletedTask;
							},
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Pull),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"Perform {Icons.Inline(Icons.Pull)} ability"),
							effectType: EffectType.SelectableMandatory
						)
					], hintText: "Select an ability to perform:");
				})
				.Build()),
			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(2)
				.WithTargets(2)
				.WithRange(3)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return state.ActionState.GetAbilityState<PushAbility.State>(0).GetCustomValue<bool>(this, "ChosePull");
				})
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Air)];
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.44888887f, 0.7171312f)))
				.WithRange(2, new RangeSquare(this, new Vector2(0.65979993f, 0.71693116f)))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Air,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustPush(1);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Push)}1")
					)
				)
				.Build())
		];
	}
}