using System.Collections.Generic;
using Fractural.Tasks;

public class SpreadDisease : SpiritCallerCardModel<SpreadDisease.CardTop, SpreadDisease.CardBottom>
{
	public override string Name => "Spread Disease";
	public override int Level => 3;
	public override int Initiative => 47;
	protected override int AtlasIndex => 28 - 16;

	public class WraithTrait() : FigureTrait
	{
		public override async GDTask Activate(Figure figure)
		{
			await base.Activate(figure);

			ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(figure, this,
				parameters =>
					parameters.Performer == figure &&
					parameters.AbilityState.Target.HasPoison(),
				async parameters =>
				{
					parameters.AbilityState.SingleTargetAddCondition(Conditions.Wound1);

					await GDTask.CompletedTask;
				}
			);
		}

		public override async GDTask Deactivate(Figure figure)
		{
			await base.Deactivate(figure);

			ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(figure, this);
		}
	}

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Leprous Wraith")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/leprous_wraith.png")
				.WithHealth(2)
				.WithMove(1)
				.WithAttack(2)
				.WithTraits(
					new ApplyConditionTrait(Conditions.Poison1),
					new WraithTrait())
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Poison1)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithRange(1)
				.WithCustomGetPerformHex(state => state.GetCustomValue<Hex>(this, "Hex"))
				.WithConditionalAbilityCheck(async state =>
				{
					Figure spirit = await Spirit.SelectSpirit(state);

					if(spirit == null)
					{
						return false;
					}

					state.SetCustomValue(this, "Hex", spirit.Hex);
					return true;
				})
				.Build()),
		];

		public override bool Round => true;
	}
}