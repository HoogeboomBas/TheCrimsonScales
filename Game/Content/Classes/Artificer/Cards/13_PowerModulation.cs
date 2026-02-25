using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class PowerModulation : ArtificerCardModel<PowerModulation.CardTop, PowerModulation.CardBottom>
{
	public override string Name => "Power Modulation";
	public override int Level => 2;
	public override int Initiative => 21;
	protected override int AtlasIndex => 13;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(6)
				.WithRange(3)
				.WithOnAbilityStarted(async state =>
				{
					await AbilityCmd.GenericChoice(state.Performer,
					[
						ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async _ =>
							{
								state.SetCustomValue(this, "ChoseSingleTarget", true);
								await GDTask.CompletedTask;
							},
							effectButtonParameters: new TextEffectButton.Parameters($"{Icons.Inline(Icons.Attack)}6"),
							effectInfoViewParameters: new TextEffectInfoView.Parameters(
								$"Perform {Icons.Inline(Icons.Attack)}6, {Icons.Inline(Icons.Range)}3"),
							effectType: EffectType.SelectableMandatory
						),
						ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async _ =>
							{
								state.SetBlocked();
								await GDTask.CompletedTask;
							},
							effectButtonParameters: new TextEffectButton.Parameters($"{Icons.Inline(Icons.Attack)}3"),
							effectInfoViewParameters: new TextEffectInfoView.Parameters(
								$"Perform {Icons.Inline(Icons.Attack)}3, {Icons.Inline(Icons.Targets)}3, {Icons.Inline(Icons.Range)}4"),
							effectType: EffectType.SelectableMandatory
						)
					], hintText: "Select an ability to perform:");
				})
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithTargets(3)
				.WithRange(4)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return !state.ActionState.GetAbilityState<AttackAbility.State>(0).GetCustomValue<bool>(this, "ChoseSingleTarget");
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6214815f, 0.64656085f)))
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithRange(2)
				.Build()),
			MoveCharacterTokenBackwardAbility()
		];
	}
}