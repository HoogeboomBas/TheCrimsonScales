using Fractural.Tasks;

public class RobeOfSoothing : CS2Item
{
	public override string Name => "Robe of Soothing";
	public override int ItemNumber => 42;
	public override int ShopCount => 1;
	public override int Cost => 0;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Always;
	public override bool IsSolo => true;

	protected override int AtlasIndex => 15;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		ScenarioEvents.AbilityCardGivenEvent.Subscribe(this, _subscriber,
			parameters => parameters.CardGiver == Owner && parameters.AbilityCard.Model.Top is HierophantPrayerCardSide,
			async parameters =>
			{
				await Use(async user =>
				{
					await new ActionState(parameters.CardReceiver,
							[HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()])
						.Perform();

					await GDTask.CompletedTask;
				});
			}
		);
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		ScenarioEvents.AbilityCardGivenEvent.Unsubscribe(this, _subscriber);
	}
}