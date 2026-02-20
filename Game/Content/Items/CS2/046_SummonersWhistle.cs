public class SummonersWhistle : CS2Item
{
	public override string Name => "Summoner's Whistle";
	public override int ItemNumber => 46;
	public override int ShopCount => 1;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 19;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					ActionState actionState = new ActionState(user,
					[
						GrantAbility.Builder()
							.WithAbilities(
							[
								AbilityCmd.SummonMovePlusX(0).Build(),
								AbilityCmd.SummonAttackPlusX(0).Build()
							])
							.WithCustomGetTargets((grantState, figures) =>
							{
								figures.AddRange(((Character)grantState.Performer).Summons);
							})
							.WithTarget(Target.Allies)
							.Build()
					]);
					await actionState.Perform();
				});
			}
		);
	}
}