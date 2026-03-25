using Fractural.Tasks;

public class MildBolsteringTonic : CS3Item
{
	public override string Name => "Mild Bolstering Tonic";
	public override int ItemNumber => 74;
	public override int ShopCount => 1;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 1;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringHeal(
			canApply: state => state.Performer == Owner && state.AbilityTarget.HasFlag(Target.Allies),
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AbilityAdjustHealValue(1);
					state.AbilityAddCondition(Conditions.Strengthen);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}