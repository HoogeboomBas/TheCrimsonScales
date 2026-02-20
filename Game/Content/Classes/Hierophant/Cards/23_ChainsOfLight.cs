using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ChainsOfLight : HierophantLevelUpCardModel<ChainsOfLight.CardTop, ChainsOfLight.CardBottom>
{
	public override string Name => "Chains of Light";
	public override int Level => 6;
	public override int Initiative => 31;
	protected override int AtlasIndex => 15 - 9;

	public class CardTop : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(2)
				.WithTargets(2)
				.WithRange(3, new RangeSquare(this, new Vector2(0.6776391f, 0.19290696f)))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Stun);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Add {Icons.Inline(Icons.GetCondition((Conditions.Stun)))} to the next attack")
					)
				)
				.Build())
		];
	}

	public class CardBottom : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),

			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(3)
				.WithRange(1)
				.WithOnAbilityEndedPerformed(async state =>
				{
					foreach(Figure enemy in state.UniqueTargetedFigures
						        .Where(enemy => RangeHelper.GetFiguresInRange(enemy.Hex, 1)
							        .Any(f => f.AlliedWith(state.Performer))))
					{
						await AbilityCmd.AddCondition(state, enemy, Conditions.Immobilize);
					}
				})
				.Build())
		];
	}
}