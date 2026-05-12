using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ShriekingSpirit : SpiritCallerCardModel<ShriekingSpirit.CardTop, ShriekingSpirit.CardBottom>
{
	public override string Name => "Shrieking Spirit";
	public override int Level => 1;
	public override int Initiative => 26;
	protected override int AtlasIndex => 28 - 7;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Wailing Banshee")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/wailing_banshee.png")
				.WithHealth(2)
				.WithMove(3)
				.WithAttack(2)
				.WithTraits(new PierceTrait(99), new ApplyConditionTrait(Conditions.Immobilize))
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Air)];
		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.WithMoveType(MoveType.Jump)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(Element.Air,
						applyFunction: async parameters =>
						{
							ActionState actionState = new ActionState(parameters.AbilityState.ActionState, parameters.AbilityState.Performer,
							[
								HealAbility.Builder()
									.WithHealValue(2)
									.WithRange(2)
									.Build()
							]);
							await actionState.Perform();

							AbilityState state = parameters.AbilityState;

							state.SetPerformed();
							state.SetBlocked();
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Perform {Icons.Inline(Icons.Heal)}2, {Icons.Inline(Icons.Range)}2 instead.")
					))
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(1)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(Element.Dark,
						applyFunction: async parameters =>
						{
							ActionState actionState = new ActionState(parameters.AbilityState.ActionState, parameters.AbilityState.Performer,
							[
								AttackAbility.Builder()
									.WithDamage(2)
									.Build()
							]);
							await actionState.Perform();

							AbilityState state = parameters.AbilityState;

							state.SetPerformed();
							state.SetBlocked();
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Perform {Icons.Inline(Icons.Attack)}2 instead.")
					))
				.Build()),
		];
	}
}