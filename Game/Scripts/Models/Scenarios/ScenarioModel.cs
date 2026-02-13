using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public abstract class ScenarioModel : AbstractModel<ScenarioModel>, IEventSubscriber
{
	public ScenarioGoals ScenarioGoals { get; private set; }

	public abstract string ScenePath { get; }
	public abstract int ScenarioNumber { get; }
	public abstract ScenarioChain ScenarioChain { get; }
	public virtual IEnumerable<ScenarioConnection> Connections { get; } = [];
	public virtual int[] TreasureNumbers { get; } = [];

	protected virtual IEnumerable<ScenarioRequirement> ScenarioRequirements { get; } = [];

	public virtual string BGMPath => "res://Audio/BGM/Floral-Woods.ogg";
	public virtual string BGSPath => null;

	public virtual async GDTask StartBeforeFirstRoomRevealed()
	{
		ScenarioGoals = CreateScenarioGoals();
		UpdateScenarioText(null);

		await GDTask.CompletedTask;
	}

	public virtual async GDTask StartAfterFirstRoomRevealed()
	{
		ScenarioGoals.Start();

		ScenarioEvents.RoomRevealedEvent.Subscribe(this, parameters => true, OnRoomRevealed);

		await GDTask.CompletedTask;
	}

	protected virtual async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await GDTask.CompletedTask;
	}

	protected abstract ScenarioGoals CreateScenarioGoals();

	protected virtual void UpdateScenarioText(string text)
	{
		string displayText;
		if(text != null)
		{
			displayText = $"{ScenarioGoals.Text}\n\n{text}";
		}
		else
		{
			displayText = ScenarioGoals.Text;
		}

		GameController.Instance.SpecialRulesView.SetText(displayText);
	}

	protected async GDTask<Monster> SpawnMonster(Figure authority, MonsterModel monsterModel, MonsterType monsterType, Hex spawnHex,
		int? monsterLevel = null, Alignment alignment = Alignment.Enemies, Alignment enemies = Alignment.Characters, bool canHaveFeatures = false)
	{
		return await SpawnMonster(authority, monsterModel, monsterType, [spawnHex], monsterLevel, alignment, enemies, canHaveFeatures);
	}

	protected async GDTask<Monster> SpawnMonster(Figure authority, MonsterModel monsterModel, MonsterType monsterType, IEnumerable<Hex> spawnHexes,
		int? monsterLevel = null, Alignment alignment = Alignment.Enemies, Alignment enemies = Alignment.Characters, bool canHaveFeatures = false)
	{
		spawnHexes = spawnHexes.ToList();
		authority ??= GameController.Instance.CharacterManager.FirstAlive();
		List<Hex> hexes = RangeHelper.GetHexesInRange(spawnHexes.First(), 100, requiresLineOfSight: false).ToList();

		Hex chosenHex = await AbilityCmd.SelectHex(authority,
			list =>
			{
				int? minDistance = null;
				foreach(Hex spawnHex in spawnHexes)
				{
					hexes.Shuffle(GameController.Instance.StateRNG);
					hexes.Sort((otherHexA, otherHexB) =>
						RangeHelper.Distance(spawnHex, otherHexA).CompareTo(RangeHelper.Distance(spawnHex, otherHexB)));
					Hex firstHex = hexes.FirstOrDefault(hex => hex.IsEmpty() || (canHaveFeatures && hex.IsFeatureless()));

					if(firstHex == null)
					{
						return;
					}

					int distance = RangeHelper.Distance(spawnHex, firstHex);

					if(minDistance != null && distance > minDistance)
					{
						continue;
					}

					if(minDistance == null || distance < minDistance)
					{
						list.Clear();
						minDistance = distance;
					}

					list.AddRange(hexes.Where(hex =>
						(hex.IsEmpty() || canHaveFeatures && hex.IsFeatureless()) && RangeHelper.Distance(spawnHex, hex) == distance)
					);
				}
			},
			true,
			$"Select a hex to spawn the {monsterType} {monsterModel.Name}"
		);

		if(chosenHex == null)
		{
			return null;
		}

		return await AbilityCmd.SpawnMonster(monsterModel, monsterType, chosenHex, monsterLevel, alignment, enemies);
	}

	protected async GDTask SummonMonster(Figure authority, MonsterModel monsterModel, MonsterType monsterType, Hex summonHex,
		int? monsterLevel = null, Alignment alignment = Alignment.Enemies, Alignment enemies = Alignment.Characters)
	{
		authority ??= GameController.Instance.CharacterManager.FirstAlive();

		Hex chosenHex = await AbilityCmd.SelectHex(authority,
			list =>
			{
				list.AddRange(RangeHelper.GetHexesInRange(summonHex, 1).Where(hex => hex.IsEmpty()));
			},
			true,
			$"Select a hex to summon the {monsterType} {monsterModel.Name}"
		);

		if(chosenHex == null)
		{
			return;
		}

		await AbilityCmd.SummonMonster(monsterModel, monsterType, chosenHex, monsterLevel, alignment, enemies);
	}
}