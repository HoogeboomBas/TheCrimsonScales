using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Scenario009 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario009.tscn";
	public override int ScenarioNumber => 9;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario013>(), new ScenarioConnection<Scenario014>()];

	protected override ScenarioGoals CreateScenarioGoals() =>
		new CustomScenarioGoals("Kill all revealed enemies and loot the treasure chest to win this scenario.");

	private bool _lootedTreasure;
	private readonly List<Door> _firstDoors = new List<Door>();

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		UpdateScenarioText("The doors are locked.\nSomething will happen once all enemies in this room are killed.");

		GameController.Instance.Map.Treasures[0].SetObtainLootFunction(OnTreasureLooted);

		foreach(Marker marker in GameController.Instance.Map.Markers)
		{
			if(marker.MarkerType == Marker.Type._1)
			{
				_firstDoors.Add(marker.GetHexObject<Door>());
			}
		}

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters =>
			{
				if(!_lootedTreasure)
				{
					return false;
				}

				foreach(Figure figure in GameController.Instance.Map.Figures)
				{
					if(figure.Alignment == Alignment.Enemies)
					{
						return false;
					}
				}

				return true;
			},
			async parameters =>
			{
				await ((CustomScenarioGoals)ScenarioGoals).Win();
			}
		);

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
			{
				foreach(Figure figure in GameController.Instance.Map.Figures)
				{
					if(figure.Alignment == Alignment.Enemies)
					{
						return false;
					}
				}

				return true;
			},
			async parameters =>
			{
				ScenarioEvents.FigureKilledEvent.Unsubscribe(this);

				await SpawnBear();

				foreach(Door door in _firstDoors)
				{
					await door.Unlock();
				}

				UpdateScenarioText(null);
			}
		);
	}

	private async GDTask OnTreasureLooted(Character lootingCharacter)
	{
		_lootedTreasure = true;

		await GDTask.CompletedTask;
	}

	private async GDTask SpawnBear()
	{
		MonsterModel monsterModel = ModelDB.Monster<Granurso>();
		MonsterType monsterType = GameController.Instance.CharacterManager.Characters.Count == 2 ? MonsterType.Normal : MonsterType.Elite;

		Hex chosenHex = await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.FirstAlive(),
			list =>
			{
				// Find the hexes closest to the doors
				List<Hex> hexes = RangeHelper.GetHexesInRange(_firstDoors[0].Hex, 100, false, false).Where(hex => hex.IsEmpty()).ToList();
				hexes.Sort((hexA, hexB) => GetMinDistanceToDoor(hexA).CompareTo(GetMinDistanceToDoor(hexB)));

				if(hexes.Count == 0)
				{
					return;
				}

				int minDistance = GetMinDistanceToDoor(hexes[0]);

				foreach(Hex otherHex in hexes)
				{
					int otherDistance = Mathf.Min(RangeHelper.Distance(_firstDoors[0].Hex, otherHex),
						RangeHelper.Distance(_firstDoors[1].Hex, otherHex));
					if(otherDistance > minDistance)
					{
						break;
					}

					list.Add(otherHex);
				}
			}, true, $"Select where to spawn {monsterModel.Name}"
		);

		if(chosenHex == null)
		{
			return;
		}

		await AbilityCmd.SpawnMonster(monsterModel, monsterType, chosenHex);
	}

	private int GetMinDistanceToDoor(Hex hex)
	{
		return Mathf.Min(RangeHelper.Distance(_firstDoors[0].Hex, hex), RangeHelper.Distance(_firstDoors[1].Hex, hex));
	}
}