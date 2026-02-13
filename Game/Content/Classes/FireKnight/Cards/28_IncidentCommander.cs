using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class IncidentCommander : FireKnightLevelUpCardModel<IncidentCommander.CardTop, IncidentCommander.CardBottom>
{
	public override string Name => "Incident Commander";
	public override int Level => 9;
	public override int Initiative => 17;
	protected override int AtlasIndex => 0;

	public class CardTop : FireKnightCardSide
	{
		private MoveSquare _moveEnhancementMark;
		private AttackDiamond _attackEnhancementMark;
		private HealDiamondPlus _healEnhancementMark;

		protected override void InitExtraEnhancements()
		{
			base.InitExtraEnhancements();

			_moveEnhancementMark = new MoveSquare(this, new Vector2(0.6176377f, 0.26352012f));
			_attackEnhancementMark = new AttackDiamond(this, new Vector2(0.62063766f, 0.33751917f));
			_healEnhancementMark = new HealDiamondPlus(this, new Vector2(0.500713f, 0.40776294f));
		}

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantAbilityState =>
				[
					OtherAbility.Builder()
						.WithPerformAbility(async state =>
						{
							state.SetPerformed();
							await AbilityCmd.GenericChoice(state.Performer,
							[
								ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
									applyFunction: async applyParameters =>
									{
										await MoveAbility.Builder()
											.WithDistance(3, _moveEnhancementMark)
											.Build()
											.Perform(state.ActionState);
									},
									effectButtonParameters: new IconEffectButton.Parameters(Icons.Move),
									effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Move)}3"),
									effectType: EffectType.Selectable
								),
								ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
									applyFunction: async applyParameters =>
									{
										await AttackAbility.Builder()
											.WithDamage(3, _attackEnhancementMark)
											.Build()
											.Perform(state.ActionState);
									},
									effectButtonParameters: new IconEffectButton.Parameters(Icons.Attack),
									effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Attack)}3"),
									effectType: EffectType.Selectable
								),
								ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
									applyFunction: async applyParameters =>
									{
										await HealAbility.Builder()
											.WithHealValue(3, _healEnhancementMark)
											.WithTarget(Target.Self)
											.Build()
											.Perform(state.ActionState);
									},
									effectButtonParameters: new IconEffectButton.Parameters(Icons.Heal),
									effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Heal)}3, Self"),
									effectType: EffectType.Selectable
								),
							], canSelectMultiple: true, hintText: "Choose an ability to perform");
						})
						.Build()
				])
				.WithRange(3)
				.Build())
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Strengthen, new ConditionDiamondPlus(this, new Vector2(0.40304643f, 0.66341674f)))
				.WithRange(3)
				.WithTarget(Target.Allies)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					foreach(Figure target in state.ActionState.GetAbilityState<ConditionAbility.State>(0).UniqueTargetedFigures)
					{
						bool attackPerformedYet = target.RoundPerformedActionStates
							.SelectMany(a => a.AbilityStates)
							.OfType<AttackAbility.State>()
							.Any(a => a.UniqueTargetedFigures.Count > 0);
						if(!attackPerformedYet)
						{
							ScenarioEvents.DuringAttackEvent.Subscribe(target, this,
								canApplyParameters => canApplyParameters.Performer == target,
								async parameters =>
								{
									parameters.AbilityState.AbilityAdjustAttackValue(2);
									ScenarioEvents.DuringAttackEvent.Unsubscribe(target, this);

									await GDTask.CompletedTask;
								}
							);
						}

						bool movePerformedYet = target.RoundPerformedActionStates
							.SelectMany(a => a.AbilityStates)
							.OfType<MoveAbility.State>()
							.Any(a => a.Hexes.Count > 0);
						if(!movePerformedYet)
						{
							ScenarioEvents.DuringMovementEvent.Subscribe(target, this,
								canApplyParameters => canApplyParameters.Performer == target,
								async parameters =>
								{
									parameters.AbilityState.AdjustMoveValue(2);
									ScenarioEvents.DuringMovementEvent.Unsubscribe(target, this);

									await GDTask.CompletedTask;
								});
						}

						bool healPerformedYet = target.RoundPerformedActionStates
							.SelectMany(a => a.AbilityStates)
							.OfType<HealAbility.State>()
							.Any(a => a.UniqueTargetedFigures.Count > 0);
						if(!healPerformedYet)
						{
							ScenarioEvents.DuringHealEvent.Subscribe(target, this,
								canApplyParameters => canApplyParameters.Performer == target,
								async parameters =>
								{
									parameters.AbilityState.AbilityAdjustHealValue(2);
									ScenarioEvents.DuringHealEvent.Unsubscribe(target, this);

									await GDTask.CompletedTask;
								});
						}
					}

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					foreach(Figure target in state.ActionState.GetAbilityState<ConditionAbility.State>(0).UniqueTargetedFigures)
					{
						ScenarioEvents.DuringAttackEvent.Unsubscribe(target, this);
						ScenarioEvents.DuringMovementEvent.Unsubscribe(target, this);
						ScenarioEvents.DuringHealEvent.Unsubscribe(target, this);
						ScenarioEvents.RoundEndedEvent.Unsubscribe(target, this);
					}

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		public override int XP => 1;
		public override bool Round => true;
	}
}