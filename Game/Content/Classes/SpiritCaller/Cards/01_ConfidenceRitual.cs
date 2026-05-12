using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ConfidenceRitual : SpiritCallerCardModel<ConfidenceRitual.CardTop, ConfidenceRitual.CardBottom>
{
	public override string Name => "Confidence Ritual";
	public override int Level => 1;
	public override int Initiative => 30;
	protected override int AtlasIndex => 28 - 1;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(new DynamicInt<AttackAbility.State>(state => Spirit.GetAllSpirits().Count + 3),
					new AttackDiamond(this, new Vector2(0.49890798f, 0.2658228f)))
				.WithRange(2, new RangeSquare(this, new Vector2(0.7096053f, 0.26492292f)))
				.Build()),
		];
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Figure spirit = state.GetCustomValue<Figure>(this, "Spirit");
					await Spirit.RemoveDamageCounters(spirit, 1);

					await AbilityCmd.AddCharacterToken(state, spirit,
						textParameters =>
							$"While this Spirit is alive, its owner adds +1{Icons.Inline(Icons.Attack, textParameters)} and advantage to all their attacks.");

					ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
						parameters =>
							parameters.AbilityState is AttackAbility.State &&
							parameters.AbilityState.Performer == state.Performer,
						async parameters =>
						{
							AttackAbility.State attackAbilityState = (AttackAbility.State)parameters.AbilityState;
							attackAbilityState.AbilityAdjustAttackValue(1);
							attackAbilityState.AbilitySetHasAdvantage();

							await GDTask.CompletedTask;
						}
					);

					ScenarioEvents.HexObjectDestroyedEvent.Subscribe(state, this,
						parameters => parameters.HexObject == spirit,
						async parameters =>
						{
							await state.ActionState.RequestDiscardOrLose();
						}
					);
				})
				.WithOnDeactivate(async state =>
				{
					Figure spirit = state.GetCustomValue<Figure>(this, "Spirit");

					await AbilityCmd.RemoveCharacterToken(state, spirit);

					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);
					ScenarioEvents.HexObjectDestroyedEvent.Unsubscribe(state, this);
				})
				.WithConditionalAbilityCheck(async state =>
				{
					Figure spirit = await Spirit.SelectSpirit(state);

					if(spirit == null)
					{
						return false;
					}

					state.SetCustomValue(this, "Spirit", spirit);
					return true;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}