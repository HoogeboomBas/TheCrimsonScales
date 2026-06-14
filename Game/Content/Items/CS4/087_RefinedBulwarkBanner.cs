using System.Collections.Generic;
using System.Linq;

public class RefinedBulwarkBanner : CS4Item
{
	public override string Name => "Refined Bulwark Banner";
	public override int ItemNumber => 87;
	public override int ShopCount => 1;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Consume;
	public override bool Round => true;

	protected override int AtlasIndex => 3;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					await GetActionState(user,
					[
						GrantAbility.Builder()
							.WithGetAbilities(state =>
							{
								List<ActionState> activeActionStates = user.RoundCards.SelectMany(card => card.ActiveActionStates).ToList();
								List<Ability> roundAbilities = activeActionStates.SelectMany(actionState => actionState.Abilities).ToList();
								List<AbilityState> roundAbilityStates = activeActionStates.SelectMany(actionState => actionState.AbilityStates).ToList();

								List<Ability> shieldAbilitiesToGrant = [];

								for(int i = 0; i < roundAbilities.Count; i++)
								{
									if(roundAbilities[i] is ShieldAbility && roundAbilityStates[i] is ShieldAbility.State && roundAbilityStates[i].Performed)
									{
										shieldAbilitiesToGrant.Add(roundAbilities[i]);
									}
								}

								return shieldAbilitiesToGrant;
							})
							.WithRange(1)
							.WithRequiresLineOfSight(false)
							.WithTarget(Target.Allies | Target.TargetAll)
							.Build()
					]).Perform();
				});
			}
		);
	}
}