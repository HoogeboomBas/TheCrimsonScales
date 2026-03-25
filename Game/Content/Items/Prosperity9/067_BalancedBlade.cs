using Fractural.Tasks;

public class BalancedBlade : Prosperity9Item
{
	public override string Name => "Balanced Blade";
	public override int ItemNumber => 67;
	public override int ShopCount => 2;
	public override int Cost => 60;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 6;

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
				parameters.AbilityState is AttackAbility.State attackAbilityState &&
				attackAbilityState.ActionState.ActionSource is AbilityCardSide abilityCardSide &&
				abilityCardSide.AbilityCardSideType == AbilityCardSideType.BasicTop,
			async parameters =>
			{
				await Use(async user =>
				{
					AttackAbility.State moveAbilityState = ((AttackAbility.State)parameters.AbilityState);
					moveAbilityState.AbilityAdjustAttackValue(2);

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