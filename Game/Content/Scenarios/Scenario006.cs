using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario006 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario006.tscn";
	public override int ScenarioNumber => 6;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<InfectiousScenarioChain>();

	protected override ScenarioGoals CreateScenarioGoals() => new CustomScenarioGoals(
		"Purify the poisoned water supply to win this scenario." +
		$"Place {GameController.Instance.CharacterManager.Characters.Count} bottles of antidote in the fountain.");

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		List<Hex> hexesWithAntidote = GameController.Instance.Map.Markers
			.Where(marker => marker.MarkerType == Marker.Type._1)
			.Select(marker => marker.Hex)
			.ToList();

		Hex hexWithFountain = GameController.Instance.Map.Markers
			.First(marker => marker.MarkerType == Marker.Type._2).Hex;

		Dictionary<Figure, bool> characterHasAntidote = [];
		int antidoteBottlesPicked = 0;
		int antidoteBottlesPlaced = 0;
		bool triggerMonsterSpawn = false;

		UpdateScenarioText(antidoteBottlesPlaced);

		foreach(Figure character in GameController.Instance.CharacterManager.Characters)
		{
			characterHasAntidote.Add(character, false);
		}

		//TODO: Scenario effect
		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			await AbilityCmd.AddCondition(null, character, Conditions.Poison1);
		}

		// Allow picking up the antidote
		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
			parameters =>
				!parameters.ForgoneAction && 
				RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Any(hex => hexesWithAntidote.Contains(hex) && 
				hex.HasHexObjectOfType<Obstacle>()) &&
				!characterHasAntidote[parameters.Performer],
			async parameters =>
			{
				Hex chosenHex = await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.FirstAlive(),
				list =>
				{
					list.AddRange(RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Where(hex => hexesWithAntidote.Contains(hex)));
				}, mandatory: true);

				parameters.ForgoAction();

				characterHasAntidote[parameters.Performer] = true;
				antidoteBottlesPicked++;

				await chosenHex.GetHexObjectOfType<Obstacle>().Destroy(false, true);
				hexesWithAntidote.Remove(chosenHex);

				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(parameters.Performer, this,
					infoParameters => infoParameters.Figure == parameters.Performer,
					infoParameters =>
					{
						infoParameters.Add(new FigureInfoTextExtraEffect.Parameters(
							$"This character carries an antidote bottle."));
					}
				);

				triggerMonsterSpawn = true;

				await SpawnMonsters(antidoteBottlesPicked);
			},
			EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.StartHexMove),
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Pick up a bottle of antidote")
		);

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				if(antidoteBottlesPlaced == GameController.Instance.CharacterManager.Characters.Count)
				{
					await ((CustomScenarioGoals)ScenarioGoals).Win();
				}

				if(triggerMonsterSpawn)
				{
					await SpawnMonsters(antidoteBottlesPicked);
					triggerMonsterSpawn = false;
				}
			}
		);

		// Allow placing the antidote into the fountain
		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
			parameters => !parameters.ForgoneAction && 
				RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Any(hex => hexWithFountain == hex) &&
				characterHasAntidote[parameters.Performer],
			async parameters =>
			{
				await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.FirstAlive(), list => list.Add(hexWithFountain), mandatory: true);
				parameters.ForgoAction();
				characterHasAntidote[parameters.Performer] = false;
				antidoteBottlesPlaced++;

				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(parameters.Performer, this);
			},
			EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.StartHexMove),
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Place the bottle of antidote in the fountain")
		);

		// If a character exhausts while holding an antidote, the scenario is immediately lost
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure is Character && characterHasAntidote[parameters.Figure],
			async parameters =>
			{
				await AbilityCmd.Lose();
			}
		);
	}

	private void UpdateScenarioText(int antidoteBottlesPlaced)
	{
		UpdateScenarioText(
			"The crate and cabinet obstacles contain the bottles of antidote and cannot be destroyed." +
			"Any character may sacrifice the top or bottom action of their turn while adjacent to an antidote to it pick up." + 
			"Any character may sacrifice the top or bottom action of their turn while adjacent to the fountain to place the antidote in the fountain." +
			"Each character may only hold one antidote at a time, and if a character exhausts while holding an antidote, the scenario is immediately lost." +
			$"{antidoteBottlesPlaced}/{GameController.Instance.CharacterManager.Characters.Count} antidote bottles placed in the fountain.");
	}

	private async GDTask SpawnMonsters(int antidoteBottlesPicked)
	{
		Hex hexA = GameController.Instance.Map.Markers.First(marker => marker.MarkerType == Marker.Type.a).Hex;
		Hex hexB = GameController.Instance.Map.Markers.First(marker => marker.MarkerType == Marker.Type.b).Hex;

		switch(antidoteBottlesPicked)
		{
			case 1: // 6G
			{
				await SummonMonster(hexA, ModelDB.Monster<BloodOoze>(), MonsterType.Elite);
				await SummonMonster(hexB, ModelDB.Monster<ContaminatedWaterSpirit>(), MonsterType.Normal);
				break;
			}
			case 2: // 6D
			{
				await SummonMonster(hexA, ModelDB.Monster<FlamingDrake>(), MonsterType.Normal);
				await SummonMonster(hexB, ModelDB.Monster<FlamingDrake>(), MonsterType.Normal);
				break;
			}
			case 3: // 6F
			{
				await SummonMonster(hexA, ModelDB.Monster<ToxicImp>(), MonsterType.Normal);
				await SummonMonster(hexA, ModelDB.Monster<ToxicImp>(), MonsterType.Elite);
				await SummonMonster(hexB, ModelDB.Monster<ToxicImp>(), MonsterType.Normal);
				await SummonMonster(hexB, ModelDB.Monster<ToxicImp>(), MonsterType.Elite);
				break;
			}
			case 4: // 6E
			{
				await SummonMonster(hexA, ModelDB.Monster<ContaminatedWaterSpirit>(), MonsterType.Elite);
				await SummonMonster(hexB, ModelDB.Monster<ContaminatedWaterSpirit>(), MonsterType.Elite);
				break;
			}
		}
	}

	private async GDTask SummonMonster(Hex hex, MonsterModel monsterModel, MonsterType monsterType)
	{
		List<Hex> hexes = RangeHelper.GetHexesInRange(hex, 100, requiresLineOfSight: false).ToList();

		Hex chosenHex = await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.FirstAlive(),
			list =>
			{
				Hex firstHex = null;
				foreach(Hex hex in hexes)
				{
					if(hex.IsEmpty())
					{
						firstHex = hex;
						break;
					}
				}

				if(firstHex == null)
				{
					return;
				}

				int distance = RangeHelper.Distance(hex, firstHex);

				foreach(Hex otherHex in hexes)
				{
					int otherDistance = RangeHelper.Distance(hex, otherHex);
					if(otherHex.IsEmpty() && otherDistance == distance)
					{
						list.Add(otherHex);
					}
				}
			}, true, $"Select where to summon the {monsterType.ToString()} {monsterModel.Name}"
		);

		if(chosenHex == null)
		{
			return;
		}

		await AbilityCmd.SummonMonster(monsterModel, monsterType, chosenHex);
	}
}
