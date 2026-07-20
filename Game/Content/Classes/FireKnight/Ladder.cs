using Fractural.Tasks;
using Godot;

public partial class Ladder : DifficultTerrain, IEventSubscriber
{
	protected override string DisplayName => "Ladder - Difficult Terrain";

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);

		ScenarioCheckEvents.MoveCheckEvent.Subscribe(this,
			canApplyParameters =>
				canApplyParameters.Hex == Hex &&
				canApplyParameters.Hex.HasHexObjectOfType<HazardousTerrain>(),
			applyParameters =>
			{
				applyParameters.SetAffectedByNegativeHex(false);
			}
		);

		ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(this,
			canApplyParameters => canApplyParameters.Hex == Hex,
			async applyParameters =>
			{
				applyParameters.SetAffectedByHazardousTerrain(false);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.TrapTriggeredEvent.Subscribe(this,
			canApplyParameters => canApplyParameters.Hex == Hex,
			async applyParameters =>
			{
				applyParameters.SetTriggersTrap(false);

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.CanEnterObstacleCheckEvent.Subscribe(this,
			parameters => parameters.Hex == Hex,
			parameters =>
			{
				parameters.SetCanEnter();
			}
		);

		SetScale(Vector2.Zero);
	}

	public override async GDTask Destroy(bool immediately = false, bool forceDestroy = false)
	{
		await base.Destroy(immediately, forceDestroy);

		ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(this);
		ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(this);
		ScenarioEvents.TrapTriggeredEvent.Unsubscribe(this);
		ScenarioCheckEvents.CanEnterObstacleCheckEvent.Unsubscribe(this);
	}
}