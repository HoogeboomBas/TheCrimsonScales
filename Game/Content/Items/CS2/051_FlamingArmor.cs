using Fractural.Tasks;

public class FlamingArmor : CS2Item
{
	public override string Name => "Flaming Armor";
	public override int ItemNumber => 51;
	public override int ShopCount => 1;
	public override int Cost => 0;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Always;
	public override bool IsSolo => true;

	protected override int AtlasIndex => 24;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeRetaliate(
			canApply: parameters => parameters.RetaliatingFigure == Owner &&
			                        RangeHelper.Distance(parameters.AbilityState.Performer.Hex, parameters.RetaliatingFigure.Hex) <= 1 &&
			                        AbilityCmd.CanConsumeElement(Element.Fire, Owner),
			apply: async parameters =>
			{
				await Use(async user =>
				{
					if(await AbilityCmd.AskConsumeElement(user, Element.Fire, mandatory: true))
					{
						parameters.AdjustRetaliate(3);
					}
				});
			});
	}
}