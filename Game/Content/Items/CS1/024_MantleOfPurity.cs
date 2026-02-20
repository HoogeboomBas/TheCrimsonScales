using Fractural.Tasks;

public class MantleOfPurity : CS1Item
{
	public override string Name => "Mantle Of Purity";
	public override int ItemNumber => 24;
	public override int ShopCount => 2;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 41;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		ScenarioEvents.InflictConditionsEvent.Subscribe(this, _subscriber,
			canApply: parameters => parameters.Target == Owner,
			apply: async parameters =>
			{
				await Use(async user =>
				{
					for(int i = parameters.ConditionModels.Count - 1; i >= 0; i--)
					{
						ConditionModel conditionModel = parameters.ConditionModels[i];
						if(conditionModel.IsNegative)
						{
							parameters.PreventCondition(conditionModel);
						}
					}

					await GDTask.CompletedTask;
				});
			}
		);
	}
}