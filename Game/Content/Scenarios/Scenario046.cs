using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario046 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario046.tscn";
	public override int ScenarioNumber => 46;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();

	protected override ScenarioGoals CreateScenarioGoals() =>
		new CustomScenarioGoals("Destroy the Tree of Death to win this scenario");

	private IEnumerable<Obstacle> _bushesOfHope;
	private IEnumerable<Obstacle> _bushesOfForgiveness;
	private Objective _treeOfHope;
	private Door _door1;
	private Door _door2;

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		UpdateScenarioText($"The bushes marked {Icons.Inline(Icons.GetMarker(Marker.Type.a))} are the Bushes of Hope and cannot be destroyed. Whenever a figure adjacent to a Bush of Hope draws a negative modifier, treat it as a {Icons.Inline(Icons.GetAMDValue("+0"))} instead.");

		_door1 = GameController.Instance.Map.GetMarker(Marker.Type._1).GetHexObject<Door>();
		_door2 = GameController.Instance.Map.GetMarker(Marker.Type._2).GetHexObject<Door>();

		_bushesOfHope = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.GetHexObject<Obstacle>());
		_bushesOfForgiveness = GameController.Instance.Map.GetMarkers(Marker.Type.b).Select(marker => marker.GetHexObject<Obstacle>());
		_treeOfHope = GameController.Instance.Map.GetMarker(Marker.Type.c).GetHexObject<Objective>();
		_treeOfHope.Init(GameController.Instance.SavedCampaign.Characters.Count * (GameController.Instance.SavedScenario.ScenarioLevel + 5), "Tree of Death");

		ScenarioEvents.AMDCardDrawnEvent.Subscribe(this, _bushesOfHope,
			parameters => parameters.Type == AMDCardType.Value && parameters.Value < 0 &&
				RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Intersect(_bushesOfHope.Select(bush => bush.Hex)).Any(),
			async parameters =>
            {
                parameters.SetValue(0);
				await GDTask.CompletedTask;
            });

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => _treeOfHope.IsDead,
			async parameters =>
			{
				await ((CustomScenarioGoals)ScenarioGoals).Win();
			}
		);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if (parameters.OpenedDoor == _door1)
        {
            UpdateScenarioText($"The bushes marked {Icons.Inline(Icons.GetMarker(Marker.Type.b))} are the Bushes of Forgiveness and cannot be destroyed. All figures adjacent to a Bush of Forgiveness add +1{Icons.Inline(Icons.Heal)} to all heal abilities. Whenever a figure adjacent to a Bush of Forgiveness draws a {Icons.Inline(Icons.GetAMDValue("2x"))} modifier, treat it as a {Icons.Inline(Icons.GetAMDValue("+0"))} instead.");

			ScenarioEvents.AMDCardDrawnEvent.Subscribe(this, _bushesOfForgiveness,
				parameters => parameters.Type == AMDCardType.Crit &&
					RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Intersect(_bushesOfForgiveness.Select(bush => bush.Hex)).Any(),
				async parameters =>
				{
					parameters.SetValue(0);
					await GDTask.CompletedTask;
				});

			ScenarioEvents.AbilityStartedEvent.Subscribe(this, _bushesOfForgiveness,
				parameters => parameters.AbilityState is HealAbility.State &&
					RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Intersect(_bushesOfForgiveness.Select(bush => bush.Hex)).Any(),
				async parameters =>
				{
					((HealAbility.State)parameters.AbilityState).AbilityAdjustHealValue(1);
					await GDTask.CompletedTask;
				});
        }
		else if (parameters.OpenedDoor == _door2)
        {
            UpdateScenarioText("The tree is the Tree of Death");
        }
	}
}