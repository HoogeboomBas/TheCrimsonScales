using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SpiritualEnergy : SpiritCallerCardModel<SpiritualEnergy.CardTop, SpiritualEnergy.CardBottom>
{
	public override string Name => "Spiritual Energy";
	public override int Level => 5;
	public override int Initiative => 34;
	protected override int AtlasIndex => 28 - 20;

	public class ImbuedMonolithTrait() : FigureTrait
	{
		public override async GDTask Activate(Figure figure)
		{
			await base.Activate(figure);

			ScenarioEvents.DuringAttackEvent.Subscribe(figure, this,
				parameters =>
					figure.AlliedWith(parameters.Performer) &&
					RangeHelper.Distance(parameters.Performer.Hex, figure.Hex) <= 1,
				async parameters =>
				{
					parameters.AbilityState.SingleTargetAdjustAttackValue(1);

					await GDTask.CompletedTask;
				}
			);
		}

		public override async GDTask Deactivate(Figure figure)
		{
			await base.Deactivate(figure);

			ScenarioEvents.DuringAttackEvent.Unsubscribe(figure, this);
		}
	}

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Imbued Monolith")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/imbued_monolith.png")
				.WithHealth(2)
				.WithTraits(new ImbuedMonolithTrait())
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Ice)];
		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Disarm)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Disarm)
				.WithRange(1)
				.WithMandatory(true)
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6210601f, 0.7654614f)))
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.RemoveOneNegativeCondition(state, state.Performer);
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return Spirit.HasSpirit(state.Performer.Hex);
				})
				.Build())
		];
	}
}