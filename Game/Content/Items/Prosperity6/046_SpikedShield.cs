using Fractural.Tasks;

public class SpikedShield : Prosperity6Item
{
	public override string Name => "Spiked Shield";
	public override int ItemNumber => 46;
	public override int ShopCount => 2;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 6;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeSufferDamage(
			canApply: parameters => parameters.FromAttack && parameters.Figure == Owner && parameters.WouldSufferDamage,
			apply: async parameters =>
			{
				await Use(async user =>
				{
					parameters.AdjustShield(1);

					object subscriber = new object();

					await AbilityCmd.AddRetaliate(user, subscriber, 2, 1,
						customCanApplyParameters => customCanApplyParameters.AbilityState == parameters.PotentialAbilityState);

					ScenarioEvents.AfterAttackPerformedEvent.Subscribe(this, subscriber,
						canApplyParameters => canApplyParameters.AbilityState == parameters.PotentialAbilityState,
						async applyParameters =>
						{
							AbilityCmd.RemoveRetaliate(user, subscriber);
							ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(this, subscriber);

							await GDTask.CompletedTask;
						}
					);
				});
			}
		);
	}
}