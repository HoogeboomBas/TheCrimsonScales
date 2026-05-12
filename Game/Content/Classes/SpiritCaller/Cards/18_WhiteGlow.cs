using System.Collections.Generic;
using Fractural.Tasks;

public class WhiteGlow : SpiritCallerCardModel<WhiteGlow.CardTop, WhiteGlow.CardBottom>
{
	public override string Name => "White Glow";
	public override int Level => 4;
	public override int Initiative => 77;
	protected override int AtlasIndex => 28 - 18;

	public class OrbOfLightTrait() : FigureTrait
	{
		public override async GDTask Activate(Figure figure)
		{
			await base.Activate(figure);

			ScenarioEvents.AbilityStartedEvent.Subscribe(figure, this,
				parameters =>
					parameters.Performer == figure &&
					parameters.AbilityState is AttackAbility.State,
				async parameters =>
				{
					ActionState actionState = new ActionState(parameters.AbilityState.ActionState, parameters.AbilityState.Performer,
					[
						HealAbility.Builder()
							.WithHealValue(2)
							.WithRange(2)
							.WithDuringHealSubscription(
								ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Air,
									applyFunction: async parameters =>
									{
										parameters.AbilityState.AbilityAdjustHealValue(1);
										parameters.AbilityState.AbilityAdjustRange(1);

										await GDTask.CompletedTask;
									},
									effectInfoViewParameters: new TextEffectInfoView.Parameters(
										$"+1{Icons.Inline(Icons.Heal)}, +1{Icons.Inline(Icons.Range)}")
								))
							.Build()
					]);
					await actionState.Perform();

					AbilityState state = parameters.AbilityState;

					state.SetPerformed();
					state.SetBlocked();
				},
				effectType: EffectType.Selectable,
				effectButtonParameters: new IconEffectButton.Parameters(Icons.Heal),
				effectInfoViewParameters: new TextEffectInfoView.Parameters(
					$"Perform {Icons.Inline(Icons.Heal)}2, {Icons.Inline(Icons.Range)}1 instead.")
			);
		}

		public override async GDTask Deactivate(Figure figure)
		{
			await base.Deactivate(figure);

			ScenarioEvents.AbilityStartedEvent.Unsubscribe(figure, this);
		}
	}

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Orb of Light")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/orb_of_light.png")
				.WithHealth(2)
				.WithMove(3)
				.WithAttack(2)
				.WithTraits(new OrbOfLightTrait())
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Air)];
		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Shifting Discs")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/orb_of_light.png")
				.WithHealth(2)
				.WithMove(2)
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					bool canUse = true;

					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
						parameters => true,
						async parameters =>
						{
							canUse = true;

							await GDTask.CompletedTask;
						}
					);

					SpawnAbility.State spawnAbilityState = state.ActionState.GetAbilityState<SpawnAbility.State>(0);

					AbilityCmd.SubscribeDuringCharacterTurn(ScenarioEvents.GetSubscriberPair(state, this), EffectType.Selectable,
						character =>
							canUse &&
							AbilityCmd.CanSwap(spawnAbilityState.Spirit, character),
						async character =>
						{
							canUse = false;
							await AbilityCmd.TrySwap(state, spawnAbilityState.Spirit, character);
						},
						new IconEffectButton.Parameters(Icons.Teleport),
						new TextEffectInfoView.Parameters($"Swap hexes with Shifting Discs.")
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);
					AbilityCmd.UnsubscribeDuringCharacterTurn(ScenarioEvents.GetSubscriberPair(state, this));

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithMandatory(true)
				.WithSkipConfirmation()
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}
}