using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BurningStingers : AmberAegisCardModel<BurningStingers.CardTop, BurningStingers.CardBottom>
{
	public override string Name => "Burning Stingers";
	public override int Level => 1;
	public override int Initiative => 73;
	protected override int AtlasIndex => 5;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.40538517f, 0.23703702f)))
				.WithTargets(3, new TargetsSquare(this, new Vector2(0.59447414f, 0.236237f)))
				.WithRange(3)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async applyParameters =>
						{
							ScenarioEvents.AfterAttackPerformedEvent.Subscribe(applyParameters.AbilityState, this,
								parameters => parameters.AbilityState == applyParameters.AbilityState,
								async parameters =>
								{
									await AbilityCmd.SufferDamage(parameters.AbilityState, parameters.AbilityState.Target, 1);
								}
							);
							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"All targets suffer {Icons.Inline(Icons.Damage)}1")))
				.WithOnAbilityEnded(async state =>
				{
					ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6213221f, 0.72169304f)))
				.Build()),
		];

		public override IEnumerable<Element> Elements => [Element.Fire];
	}
}