using System.Collections.Generic;
using Fractural.Tasks;

public class RoundhouseSwing : ChainguardCardModel<RoundhouseSwing.CardTop, RoundhouseSwing.CardBottom>
{
	public override string Name => "Roundhouse Swing";
	public override int Level => 1;
	public override int Initiative => 79;
	protected override int AtlasIndex => 12 - 12;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.WithOnAbilityStarted(async state =>
				{
					await AbilityCmd.GenericChoice(state.Performer, 
					[
						ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async applyParameters =>
							{
								state.SetCustomValue(this, "ChoseLoot", true);
								await GDTask.CompletedTask;
							},
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Loot),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"Perform {Icons.Inline(Icons.Loot)} ability"),
							effectType: EffectType.SelectableMandatory
						),
						ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async applyParameters =>
							{
								state.SetBlocked();
								await GDTask.CompletedTask;
							},
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Swing),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"Perform {Icons.Inline(Icons.Swing)} ability"),
							effectType: EffectType.SelectableMandatory
						)
					], hintText: "Select an ability to perform:");
				})
				.Build()),

			new AbilityCardAbility(SwingAbility.Builder()
				.WithSwing(3)
				.WithRange(3)
				.WithConditions(Chainguard.Shackle)
				.WithOnAbilityStarted(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state.Performer, this,
						canApply: parameters => parameters.AbilityState == state,
						async parameters =>
						{
							await AbilityCmd.LootHex(state.Performer, parameters.Figure.Hex);
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state.Performer, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(async state => 
				{
					await GDTask.CompletedTask;

					return !state.ActionState.GetAbilityState<LootAbility.State>(0).GetCustomValue<bool>(this, "ChoseLoot");
				})
				.Build())
		];
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
						canApply: parameters => parameters.Performer == state.Performer && 
								parameters.AbilityState is TargetedAbilityState && 
								((TargetedAbilityState)parameters.AbilityState).AbilitySwing > 0,
						async parameters =>
						{
							((TargetedAbilityState)parameters.AbilityState).AbilityAdjustSwing(2);
							ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);

							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()
			)
		];

		protected override bool Round => true;
	}
}