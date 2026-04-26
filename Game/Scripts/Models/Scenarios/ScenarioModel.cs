using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public abstract class ScenarioModel : AbstractModel<ScenarioModel>, IEventSubscriber
{
	private readonly List<ScenarioGoal> _goals = new List<ScenarioGoal>();
	private readonly List<ScenarioRule> _rules = new List<ScenarioRule>();

	private readonly object _subscriber = new object();

	public abstract string ScenePath { get; }

	public abstract int ScenarioNumber { get; }
	public abstract string Name { get; }

	public virtual List<ScenarioLink> Links { get; } = [];
	protected virtual List<ScenarioRequirement> Requirements { get; } = [];

	public abstract ScenarioChain ScenarioChain { get; }
	public virtual IEnumerable<ScenarioConnection> Connections { get; } = [];

	public abstract string IntroductionText { get; }
	public abstract string ConclusionText { get; }

	public abstract List<MonsterModel> MonsterModels { get; }
	public abstract List<SavedReward> Rewards { get; }

	public virtual string BGMPath => "res://Audio/BGM/Floral-Woods.ogg";
	public virtual string BGSPath => null;

	public event Action<ScenarioGoal> GoalAddedEvent;
	public event Action<ScenarioRule> RuleAddedEvent;
	public event Action<ScenarioRule> RuleRemovedEvent;

	public virtual async GDTask InitializeBeforeFirstRoomRevealed()
	{
		await GDTask.CompletedTask;
	}

	public virtual async GDTask InitializeAfterFirstRoomRevealed()
	{
		ScenarioEvents.RoomRevealedEvent.Subscribe(this, _subscriber,
			parameters => true,
			OnRoomRevealed
		);

		foreach(MonsterModel monsterModel in MonsterModels)
		{
			GameController.Instance.Map.AddMonsterGroup(monsterModel);
		}

		ScenarioEvents.RoundEndedEvent.Subscribe(this, _subscriber,
			parameters => _goals.All(goal => goal.Completed),
			async parameters =>
			{
				await AbilityCmd.Win();
			}, order: 1000000
		);

		await GDTask.CompletedTask;
	}

	public virtual async GDTask StartOfScenarioEffects(Character character)
	{
		await GDTask.CompletedTask;
	}

	public virtual async GDTask OnSetupCompleted()
	{
		await GDTask.CompletedTask;
	}

	protected virtual async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await GDTask.CompletedTask;
	}

	protected void UpdateScenarioText(string text)
	{
		for(int i = _rules.Count - 1; i >= 0; i--)
		{
			ScenarioRule scenarioRule = _rules[i];
			scenarioRule.Remove();
		}

		AddScenarioRule(text, 0);
	}

	protected async GDTask<T> AddGoal<T>(T goal)
		where T : ScenarioGoal
	{
		_goals.Add(goal);

		await goal.Start();

		GoalAddedEvent?.Invoke(goal);
		//UpdateScenarioText();

		return goal;
	}

	protected ScenarioRule AddScenarioRule(string text, int order = 0)
	{
		return AddScenarioRule(textParameters => text, order);
	}

	protected ScenarioRule AddScenarioRule(TextHelper.LabelTextDelegate getTextLabel, int order = 0)
	{
		return AddScenarioRule(new ScenarioRule(getTextLabel, order));
	}

	protected T AddScenarioRule<T>(T rule)
		where T : ScenarioRule
	{
		_rules.Add(rule);

		rule.TextRemovedEvent += OnTextRemovedEvent;

		RuleAddedEvent?.Invoke(rule);

		return rule;
	}

	protected async GDTask<Monster> SpawnMonster(Figure potentialAuthority, MonsterModel monsterModel, MonsterType monsterType, Hex spawnHex,
		int? monsterLevel = null, Alignment alignment = Alignment.Enemies, Alignment enemies = Alignment.Characters, bool canHaveFeatures = false)
	{
		return await SpawnMonster(potentialAuthority, monsterModel, monsterType, [spawnHex], monsterLevel, alignment, enemies, canHaveFeatures);
	}

	protected async GDTask<Monster> SpawnMonster(Figure potentialAuthority, MonsterModel monsterModel, MonsterType monsterType,
		IEnumerable<Hex> spawnHexes,
		int? monsterLevel = null, Alignment alignment = Alignment.Enemies, Alignment enemies = Alignment.Characters, bool canHaveFeatures = false)
	{
		return await SpawnOrSummonMonster(potentialAuthority, monsterModel, monsterType, spawnHexes, true, monsterLevel, alignment, enemies,
			canHaveFeatures);
	}

	protected async GDTask<Monster> SummonMonster(Figure potentialAuthority, MonsterModel monsterModel, MonsterType monsterType,
		IEnumerable<Hex> spawnHexes,
		int? monsterLevel = null, Alignment alignment = Alignment.Enemies, Alignment enemies = Alignment.Characters, bool canHaveFeatures = false)
	{
		return await SpawnOrSummonMonster(potentialAuthority, monsterModel, monsterType, spawnHexes, false, monsterLevel, alignment, enemies,
			canHaveFeatures);
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

	protected async GDTask ShowText(string text)
	{
		await ShowText("Story", text);
	}

	protected async GDTask ShowText(TextHelper.LabelTextDelegate getText)
	{
		await ShowText("Story", getText);
	}

	protected async GDTask ShowText(string title, string text)
	{
		await ShowText(title, textParameters => text);
	}

	protected async GDTask ShowText(string title, TextHelper.LabelTextDelegate getText)
	{
		if(GameController.FastForward)
		{
			return;
		}

		PopupRequest popupRequest = new TextPopup.Request(title, getText, new TextButton.Parameters("Continue", null));
		AppController.Instance.PopupManager.RequestPopup(popupRequest);
		await GDTask.WaitWhile(AppController.Instance.PopupManager.IsPopupOpen, cancellationToken: GameController.CancellationToken);
	}

	private async GDTask<Monster> SpawnOrSummonMonster(Figure potentialAuthority, MonsterModel monsterModel, MonsterType monsterType,
		IEnumerable<Hex> spawnHexes, bool spawn,
		int? monsterLevel = null, Alignment alignment = Alignment.Enemies, Alignment enemies = Alignment.Characters, bool canHaveFeatures = false)
	{
		spawnHexes = spawnHexes.ToList();
		potentialAuthority ??= GameController.Instance.CharacterManager.FirstAlive();
		List<Hex> hexes = RangeHelper.GetHexesInRange(spawnHexes.First(), 100, requiresLineOfSight: false).ToList();

		Hex chosenHex = await AbilityCmd.SelectHex(potentialAuthority,
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
			$"Select a hex to {(spawn ? "spawn" : "summon")} the {monsterType} {monsterModel.Name}"
		);

		if(chosenHex == null)
		{
			return null;
		}

		if(spawn)
		{
			return await AbilityCmd.SpawnMonster(monsterModel, monsterType, chosenHex, monsterLevel, alignment, enemies);
		}
		else
		{
			return await AbilityCmd.SummonMonster(monsterModel, monsterType, chosenHex, monsterLevel, alignment, enemies);
		}
	}

	private void OnTextRemovedEvent(ScenarioRule scenarioRule)
	{
		scenarioRule.TextRemovedEvent -= OnTextRemovedEvent;

		_rules.Remove(scenarioRule);
		RuleRemovedEvent?.Invoke(scenarioRule);
	}
}