using Fractural.Tasks;

public class CloakOfPhasing : Prosperity8Item
{
	public override string Name => "Cloak of Phasing";
	public override int ItemNumber => 58;
	public override int ShopCount => 2;
	public override int Cost => 75;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 2;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeAbilityStarted<AttackAbility.State>(state => state.Performer == Owner && Owner.Hex.HasHexObjectOfType<Obstacle>(),
			async state =>
			{
				await Use(async user =>
				{
					state.SetBlocked();

					await GDTask.CompletedTask;
				});
			});

		ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(this, _subscriber,
			parameters => ItemState is ItemState.Available && parameters.PotentialTarget == Owner && parameters.Performer.EnemiesWith(Owner) &&
			              Owner.Hex.HasHexObjectOfType<Obstacle>(),
			parameters =>
			{
				parameters.SetCannotBeFocused();
			}
		);

		ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(this, _subscriber,
			parameters => ItemState is ItemState.Available && parameters.PotentialTarget == Owner && parameters.Performer.EnemiesWith(Owner) &&
			              Owner.Hex.HasHexObjectOfType<Obstacle>(),
			parameters =>
			{
				parameters.SetCannotBeTargeted();
			}
		);

		ScenarioCheckEvents.CanPassEnemyCheckEvent.Subscribe(this, _subscriber,
			parameters => ItemState is ItemState.Available && parameters.EnemyFigure == Owner && Owner.Hex.HasHexObjectOfType<Obstacle>(),
			parameters =>
			{
				parameters.SetCanPass();
			}
		);

		ScenarioCheckEvents.FlyingCheckEvent.Subscribe(this, _subscriber,
			parameters => ItemState is ItemState.Available && parameters.Figure == Owner && ItemState is ItemState.Available,
			parameters =>
			{
				parameters.SetFlying(true);
			});
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		ScenarioCheckEvents.FlyingCheckEvent.Unsubscribe(this, _subscriber);
		ScenarioCheckEvents.CanBeFocusedCheckEvent.Unsubscribe(this, _subscriber);
		ScenarioCheckEvents.CanBeTargetedCheckEvent.Unsubscribe(this, _subscriber);
		ScenarioCheckEvents.CanPassEnemyCheckEvent.Unsubscribe(this, _subscriber);
	}
}