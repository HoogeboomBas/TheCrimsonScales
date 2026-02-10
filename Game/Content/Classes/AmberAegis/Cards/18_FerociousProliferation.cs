using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class FerociousProliferation : AmberAegisCardModel<FerociousProliferation.CardTop, FerociousProliferation.CardBottom>
{
	public override string Name => "Ferocious Proliferation";
	public override int Level => 4;
	public override int Initiative => 58;
	protected override int AtlasIndex => 18;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PlaceColonyTokenAbility<FirespitterAntColony>()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => IsAdjacentToColonyToken<FirespitterAntColony>(parameters.Performer) &&
						              parameters.Performer.AlliedWith(state.Performer, true) &&
						              parameters.AbilityState.SingleTargetRangeType == RangeType.Melee,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);
							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override string CustomTag => "Cultivate";
		public override IEnumerable<Element> Elements => [Element.Fire];
		public override bool Persistent => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.35175186f, 0.69874173f)))
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithConditions(Conditions.Wound1)
				.Build())
			//TODO: Add Perform Hex (requires scenarios)
		];

		public override int XP => 1;
	}
}