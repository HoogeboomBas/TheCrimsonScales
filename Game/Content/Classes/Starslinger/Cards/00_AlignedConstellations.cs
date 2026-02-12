using System.Collections.Generic;
using Fractural.Tasks;

public class AlignedConstellations : StarslingerCardModel<AlignedConstellations.CardTop, AlignedConstellations.CardBottom>
{
	public override string Name => "Aligned Constellations";
	public override int Level => 1;
	public override int Initiative => 32;
	protected override int AtlasIndex => 0;

	public class CardTop : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Performer == state.Performer && !canApplyParameters.Performer.IsDamaged(),
						async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetSetHasAdvantage();

							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];

		protected override IEnumerable<Element> Elements => [Element.Dark];
		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantAbilityState =>
				[
					MoveAbility.Builder().WithDistance(1).Build()
				])
				.WithRange(3)
				.Build()
			),
		];
	}
}