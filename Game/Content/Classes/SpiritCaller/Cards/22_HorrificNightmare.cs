using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class HorrificNightmare : SpiritCallerCardModel<HorrificNightmare.CardTop, HorrificNightmare.CardBottom>
{
	public override string Name => "Horrific Nightmare";
	public override int Level => 6;
	public override int Initiative => 71;
	protected override int AtlasIndex => 28 - 22;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Soul Snatcher")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/soul_snatcher.png")
				.WithHealth(3)
				.WithMove(2)
				.WithAttack(2)
				.WithRange(3)
				.WithTraits(
					new PierceTrait(2),
					new ApplyConditionTrait(Conditions.Curse))
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Spirit spirit = state.ActionState.GetAbilityState<SpawnAbility.State>(0).Spirit;

					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						parameters =>
							parameters.PotentialKiller == spirit,
						async parameters =>
						{
							await AbilityCmd.AddCondition(null, state.Performer, Conditions.Bless, state.Performer);
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithMandatory(true)
				.WithSkipConfirmation()
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithRange(4, new RangeSquare(this, new Vector2(0.616032f, 0.6833825f)))
				.WithDuringAttackSubscriptions(
					[
						ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Air,
							applyFunction: async parameters =>
							{
								parameters.AbilityState.AbilityAdjustPull(3);

								await GDTask.CompletedTask;
							}
						),
						ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Dark,
							applyFunction: async parameters =>
							{
								parameters.AbilityState.AbilityAddCondition(Conditions.Curse);

								await GDTask.CompletedTask;
							}
						)
					]
				)
				.Build())
		];
	}
}