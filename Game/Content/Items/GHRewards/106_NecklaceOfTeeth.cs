public class NecklaceOfTeeth : GHRewardsItem
{
	public override string Name => "Necklace of Teeth";
	public override int ItemNumber => 106;
	public override int ShopCount => 1;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 11;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeFigureKilled(
			canApply: parameters =>
				Owner.EnemiesWith(parameters.Figure) &&
				parameters.PotentialAbilityState != null &&
				parameters.PotentialAbilityState.Authority == Owner,
			apply: async parameters =>
			{
				await Use(async user =>
				{
					ActionState actionState = new ActionState(user, [HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build()]);
					await actionState.Perform();
				});
			}
		);
	}
}