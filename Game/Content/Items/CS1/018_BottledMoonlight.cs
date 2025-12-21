using System.Linq;
using Fractural.Tasks;

public class BottledMoonlight : CS1Item
{
	public override string Name => "Bottled Moonlight";
	public override int ItemNumber => 18;
	public override int ShopCount => 2;
	public override int Cost => 50;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 31;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringHeal(
			canApply: state => state.Performer == Owner,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AbilityAdjustHealValue(2);
					state.AbilityAddCondition(Conditions.Strengthen);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}