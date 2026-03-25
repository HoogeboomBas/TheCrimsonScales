using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class DynamicBalance : BrightsparkCardModel<DynamicBalance.CardTop, DynamicBalance.CardBottom>
{
	public override string Name => "Dynamic Balance";
	public override int Level => 1;
	public override int Initiative => 22;
	protected override int AtlasIndex => 6;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.6191593f, 0.19725448f)))
				.WithOnAbilityStarted(async state =>
				{
					await AbilityCmd.GenericChoice(state.Performer,
					[
						ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async applyParameters =>
							{
								await GDTask.CompletedTask;
							},
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Attack),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Attack)}3"),
							effectType: EffectType.SelectableMandatory
						),
						ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async applyParameters =>
							{
								state.SetBlocked();
								state.SetCustomValue(this, "ChoseMove", true);
								await GDTask.CompletedTask;
							},
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Move),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Move)}3"),
							effectType: EffectType.SelectableMandatory
						),
						ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async applyParameters =>
							{
								state.SetBlocked();
								state.SetCustomValue(this, "ChoseHeal", true);
								await GDTask.CompletedTask;
							},
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Heal),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Heal)}3, {Icons.Inline(Icons.Range)}1"),
							effectType: EffectType.SelectableMandatory
						)
					], hintText: "Select an ability to perform:");
				})
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6214815f, 0.315873f)))
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return state.ActionState.GetAbilityState<AttackAbility.State>(0).GetCustomValue<bool>(this, "ChoseMove");
				})
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithRange(1)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return state.ActionState.GetAbilityState<AttackAbility.State>(0).GetCustomValue<bool>(this, "ChoseHeal");
				})
				.Build())
		];
	}

	public class CardBottom : BrightsparkCardSide
	{
		private MoveCircle _moveEnhancementMark;
		private AttackDiamond _attackEnhancementMark;

		protected override void InitExtraEnhancements()
		{
			base.InitExtraEnhancements();

			_moveEnhancementMark = new MoveCircle(this, new Vector2(0.6176377f, 0.26352012f));
			_attackEnhancementMark = new AttackDiamond(this, new Vector2(0.62063766f, 0.33751917f));
		}

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					state.SetPerformed();
					await AbilityCmd.GenericChoice(state.Performer,
					[
						ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
							applyFunction: async applyParameters =>
							{
								await AttackAbility.Builder().WithDamage(3, _attackEnhancementMark).Build()
									.Perform(state.ActionState);
							},
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Attack),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Attack)}3"),
							effectType: EffectType.Selectable
						),
						ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
							applyFunction: async applyParameters =>
							{
								await MoveAbility.Builder().WithDistance(3, _moveEnhancementMark).Build()
									.Perform(state.ActionState);
							},
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Move),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Move)}3"),
							effectType: EffectType.Selectable
						),
						ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
							applyFunction: async applyParameters =>
							{
								await HealAbility.Builder().WithHealValue(3).WithRange(1).Build().Perform(state.ActionState);
							},
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Heal),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Heal)}3, {Icons.Inline(Icons.Range)}1"),
							effectType: EffectType.Selectable
						),
					], canSelectMultiple: true, hintText: "Choose an ability to perform");
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}