using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class EternalEndurance : SpiritCallerCardModel<EternalEndurance.CardTop, EternalEndurance.CardBottom>
{
	public override string Name => "Eternal Endurance";
	public override int Level => 9;
	public override int Initiative => 06;
	protected override int AtlasIndex => 28 - 28;

	public class WanderingSoulTrait : FigureTrait
	{
		private static readonly object OtherSubscriber = new object();

		public override async GDTask Activate(Figure figure)
		{
			await base.Activate(figure);

			ScenarioEvents.FigureTurnEndingEvent.Subscribe(figure, this,
				ScenarioEvents.FigureTurnEnding.Subscription.ConsumeElement(Element.Air,
					canApplyFunction: parameters => parameters.Figure == figure,
					applyFunction: async parameters =>
					{
						ActionState actionState = new ActionState(figure,
							[
								HealAbility.Builder()
									.WithHealValue(2)
									.WithRange(1)
									.Build()
							]
						);

						await actionState.Perform();
					},
					effectInfoViewParameters: new TextEffectInfoView.Parameters($"Perform {Icons.Inline(Icons.Heal)}2, {Icons.Inline(Icons.Range)}1.")
				)
			);

			ScenarioEvents.FigureTurnEndingEvent.Subscribe(figure, OtherSubscriber,
				ScenarioEvents.FigureTurnEnding.Subscription.ConsumeElement(Element.Dark,
					canApplyFunction: parameters => parameters.Figure == figure,
					applyFunction: async parameters =>
					{
						ActionState actionState = new ActionState(figure,
							[
								SufferDamageAbility.Builder()
									.WithDamage(1)
									.WithRange(1)
									.Build()
							]
						);

						await actionState.Perform();
					},
					effectInfoViewParameters: new TextEffectInfoView.Parameters(
						$"One enemy within {Icons.Inline(Icons.Range)}1 suffers {Icons.Inline(Icons.Damage)}1.")
				)
			);
		}

		public override async GDTask Deactivate(Figure figure)
		{
			await base.Deactivate(figure);

			ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(figure, this);
			ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(figure, OtherSubscriber);
		}
	}

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Wandering Soul")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/wandering_soul.png")
				.WithHealth(99)
				.WithMove(3)
				.WithTraits(new WanderingSoulTrait())
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Spirit spirit = state.ActionState.GetAbilityState<SpawnAbility.State>(0).Spirit;

					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters => parameters.Figure == spirit,
						async parameters =>
						{
							parameters.SetDamagePrevented();

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithMandatory(true)
				.WithSkipConfirmation()
				.Build())
		];

		public override int XP => 4;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Disembodied Goliath")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/disembodied_goliath.png")
				.WithHealth(3)
				.WithMove(1)
				.WithMove(2)
				.WithTraits(new PushTrait(1))
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Spirit spirit = state.ActionState.GetAbilityState<SpawnAbility.State>(0).Spirit;

					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters =>
							Spirit.CountsAsSpirit(parameters.Figure) &&
							parameters.Figure != spirit,
						async parameters =>
						{
							int damage = parameters.CalculatedCurrentDamage;
							parameters.SetDamagePrevented();

							await AbilityCmd.SufferDamage(state, spirit, damage);
						},
						effectType: EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.Damage),
						effectInfoViewParameters: new TextEffectInfoView.Parameters("Redirect the damage token to the Disembodied Goliath.")
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithMandatory(true)
				.WithSkipConfirmation()
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Ice)];
		public override int XP => 1;
		public override bool Persistent => true;
	}
}