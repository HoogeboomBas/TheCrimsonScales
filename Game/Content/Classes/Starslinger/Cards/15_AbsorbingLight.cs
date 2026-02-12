using System.Collections.Generic;
using Fractural.Tasks;

public class AbsorbingLight : StarslingerCardModel<AbsorbingLight.CardTop, AbsorbingLight.CardBottom>
{
	public override string Name => "Absorbing Light";
	public override int Level => 3;
	public override int Initiative => 15;
	protected override int AtlasIndex => 15;

	public class CardTop : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(0)
				.WithTarget(Target.Self)
				.WithOnAbilityStarted(async state =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					state.AbilityAdjustHealValue(attackAbilityState.DamageDealt);
					await GDTask.CompletedTask;
				})
				.Build())
		];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => !parameters.Performer.IsDamaged(),
						async parameters =>
						{
							((MoveAbility.State)parameters.AbilityState).AddJump();
							await AbilityCmd.InfuseElement(Element.Light);
						}
					)
				)
				.Build()),
		];
	}
}