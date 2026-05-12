using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class FlurryOfMadness : SpiritCallerCardModel<FlurryOfMadness.CardTop, FlurryOfMadness.CardBottom>
{
	public override string Name => "Flurry of Madness";
	public override int Level => 2;
	public override int Initiative => 42;
	protected override int AtlasIndex => 28 - 13;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Bat Cloud")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/bat_cloud.png")
				.WithHealth(2)
				.WithMove(2)
				.WithAttack(1)
				.WithTraits(
					new TargetAllAdjacentTrait(),
					new ApplyConditionTrait(Conditions.Muddle))
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62118435f, 0.6292018f)))
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
						parameters =>
							parameters.AbilityState is AttackAbility.State &&
							Spirit.CountsAsSpirit(parameters.Performer),
						async parameters =>
						{
							AttackAbility.State attackAbilityState = (AttackAbility.State)parameters.AbilityState;
							attackAbilityState.AbilityAddCondition(Conditions.Muddle);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
						parameters =>
							parameters.AbilityState is AttackAbility.State &&
							parameters.Performer == state.Performer,
						async parameters =>
						{
							AttackAbility.State attackAbilityState = (AttackAbility.State)parameters.AbilityState;
							attackAbilityState.AbilityAddCondition(Conditions.Muddle);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Air))
				.Build()),
		];

		public override bool Round => true;
	}
}