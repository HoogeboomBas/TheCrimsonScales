public class PoxAntidote : CS1Item
{
	public override string Name => "Pox Antidote";
	public override int ItemNumber => 11;
	public override int ShopCount => 1;
	public override int Cost => 10;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 20;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{	
					Figure figure = await AbilityCmd.SelectFigure(user, list =>
					{
						foreach(Figure figure in RangeHelper.GetFiguresInRange(user.Hex, 1))
						{
							if(user.AlliedWith(figure, true) && figure.HasCondition(Conditions.Infect))
							{
								list.Add(figure);
							}
						}
					}, hintText: $"Select a figure to lose {Icons.HintText(Icons.GetCondition(Conditions.Infect))}");

					if(figure == null)
					{
						return;
					}

					await AbilityCmd.RemoveCondition(figure, Conditions.Infect);
				});
			}
		);
	}
}