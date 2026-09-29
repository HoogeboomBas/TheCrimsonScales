using Fractural.Tasks;
using Godot;

public class MagmaWaders : GHRewardsItem
{
	public override string Name => "Magma Waders";
	public override int ItemNumber => 99;
	public override int ShopCount => 1;
	public override int Cost => 50;
	public override ItemType ItemType => ItemType.Feet;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 4;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		bool heal = false;

		ScenarioCheckEvents.MoveCheckEvent.Subscribe(this, _subscriber,
			canApplyParameters =>
			{
				return canApplyParameters.Performer == Owner &&
				       canApplyParameters.Hex.HasHexObjectOfType<HazardousTerrain>();
			},
			applyParameters =>
			{
				applyParameters.SetAffectedByNegativeHex(false);
			}
		);

		ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(this, _subscriber,
			canApplyParameters => canApplyParameters.PotentialAbilityState?.Performer == Owner,
			async applyParameters =>
			{
				await Use(async user =>
				{
					applyParameters.SetAffectedByHazardousTerrain(false);
					heal = true;
					await GDTask.CompletedTask;
				});
			});

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _subscriber,
			_ => heal,
			async _ =>
			{
				await Use(async user =>
				{
					await new ActionState(Owner, [HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()]).Perform();
				});
			});
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(this, _subscriber);
	}
}