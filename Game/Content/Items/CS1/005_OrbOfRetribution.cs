using Fractural.Tasks;

public class OrbOfRetribution : CS1Item
{
	public override string Name => "Orb of Retribution";
	public override int ItemNumber => 5;
	public override int ShopCount => 2;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;
	public override int MaxUseCount => 3;

	protected override int AtlasIndex => 8;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeRetaliate(
			canApply: parameters =>
				parameters.RetaliatingFigure == Owner &&
				RangeHelper.Distance(parameters.AbilityState.Performer.Hex, parameters.RetaliatingFigure.Hex) <= 1 + parameters.ExtraRange,
			apply: async parameters =>
			{
				await Use(async user =>
				{
					parameters.AddRetaliate(1, 1);

					await GDTask.CompletedTask;
				});
			},
			canApplyMultipleTimesDuringAbility: false
		);
	}
}