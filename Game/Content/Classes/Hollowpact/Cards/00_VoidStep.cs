using System.Collections.Generic;
using Fractural.Tasks;

public class VoidStep : HollowpactCardModel<VoidStep.CardTop, VoidStep.CardBottom>
{
	public override string Name => "Void Step";
	public override int Level => 1;
	public override int Initiative => 20;
	protected override int AtlasIndex => 0;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(TeleportAbility.Builder()
				.WithDistance(2)
				.Build()),
			
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.Build())
			//TODO: Consume +1dmg, +1xp
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build()),

			//TODO: Produce
			
			new AbilityCardAbility(TeleportAbility.Builder()
				.WithDistance(4)
				.WithConditionalAbilityCheck(async state =>
				{
					//TODO: Consume
					await GDTask.CompletedTask;

					return false;
				})
				.Build()),
		];
	}
}