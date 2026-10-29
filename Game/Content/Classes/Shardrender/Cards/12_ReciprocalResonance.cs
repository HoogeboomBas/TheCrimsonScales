using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ReciprocalResonance : ShardrenderCardModel<ReciprocalResonance.CardTop, ReciprocalResonance.CardBottom>
{
	public override string Name => "Reciprocal Resonance";
	public override int Level => 1;
	public override int Initiative => 21;
	protected override int AtlasIndex => 12;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AbilityPerformedEvent.Subscribe(state, this,
						//TODO: Include push/pull? Not sure if they actually would proc this
						parameters => parameters.AbilityState is MoveAbility.State &&
						              state.Performer.EnemiesWith(parameters.Performer) &&
						              RangeHelper.Distance(parameters.Performer.Hex, state.Performer.Hex) <= 1,
						async parameters =>
						{
							await AbilityCmd.SufferDamage(parameters.AbilityState, parameters.Performer, 1);
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityPerformedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(CrystallizeAbility.Builder()
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.2910257f, 0.38337952f)),
					new UseSlot(new Vector2(0.49823594f, 0.38337952f)),
					new UseSlot(new Vector2(0.7062223f, 0.38337952f))
				])
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.Build()),
			new AbilityCardAbility(MoveCharacterTokenBackAbility(1).Build())
		];

		public override int XP => 1;
		public override bool Round => true;
	}
}