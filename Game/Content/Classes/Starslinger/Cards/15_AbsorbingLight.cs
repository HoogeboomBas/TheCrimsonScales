using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class AbsorbingLight : StarslingerCardModel<AbsorbingLight.CardTop, AbsorbingLight.CardBottom>
{
	public override string Name => "Absorbing Light";
	public override int Level => 3;
	public override int Initiative => 15;
	protected override int AtlasIndex => 15;

	public class CardTop : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.61900824f, 0.1985234f)))
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(0)
				.WithTarget(Target.Self)
				.WithOnAbilityStarted(async state =>
				{
					if(await AbilityCmd.HasPerformedAbility(state, 0))
					{
						AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
						state.AbilityAdjustHealValue(attackAbilityState.DamageDealt);
						await GDTask.CompletedTask;
					}
				})
				.Build())
		];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.61790806f, 0.73756313f)))
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => !parameters.Performer.IsDamaged(),
						async parameters =>
						{
							((MoveAbility.State)parameters.AbilityState).AddJump();
							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light);
						}
					)
				)
				.Build()),
		];
	}
}