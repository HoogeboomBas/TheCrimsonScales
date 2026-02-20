using Fractural.Tasks;

public class ComfortableShoes : Prosperity4Item
{
	public override string Name => "Comfortable Shoes";
	public override int ItemNumber => 30;
	public override int ShopCount => 2;
	public override int Cost => 29;
	public override ItemType ItemType => ItemType.Feet;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 0;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		ScenarioEvents.AbilityStartedEvent.Subscribe(this, _subscriber,
			parameters =>
				parameters.Performer == Owner &&
				parameters.AbilityState is MoveAbility.State moveAbilityState &&
				moveAbilityState.ActionState.ActionSource is AbilityCardSide abilityCardSide &&
				abilityCardSide.AbilityCardSideType == AbilityCardSideType.BasicBottom,
			async parameters =>
			{
				await Use(async user =>
				{
					MoveAbility.State moveAbilityState = ((MoveAbility.State)parameters.AbilityState);
					moveAbilityState.AdjustMoveValue(1);

					await GDTask.CompletedTask;
				});
			}
		);
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		ScenarioEvents.AbilityStartedEvent.Unsubscribe(this, _subscriber);
	}
}