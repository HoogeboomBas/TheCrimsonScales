using System.Linq;
using Fractural.Tasks;
using System.Collections.Generic;
using Godot;

public class Scenario004 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario004.tscn";
	public override int ScenarioNumber => 4;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<InfectiousScenarioChain>();
	//public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario005>()];

	protected override ScenarioGoals CreateScenarioGoals() =>
		new CustomScenarioGoals("Kill all enemies and cure four sick warriors to win this scenario." + 
			System.Environment.NewLine + System.Environment.NewLine +
			"Any character may forgo the top action of their turn to perform a" + 
			$"“{Icons.Inline(Icons.Heal)}1, {Icons.Inline(Icons.Range)}2” ability.");

	private List<InfectedWarrior> _infectedWarriors = [];
	private bool _roomRevealed = false;

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<BonecladShawl>());

		// This implements
		// One character gains the “Pox Antidote” item. 
		// During this scenario, this item may be equipped without it occupying an item slot.
		Map map = GameController.Instance.Map;

		ItemModel itemModel = ModelDB.Item<PoxAntidote>();
		Character character = (Character)map.Figures.FirstOrDefault(figure => figure is Character character && character.SavedCharacter.HasItem(itemModel), null);

		bool poxAntidoteGiven = GameController.Instance.SavedScenarioProgress.CustomValues.ContainsKey("PoxAntidoteGiven");

		// Antidote given previously and a character still has it (not sold)
		// take it away and give back as "temporary" item
		if(poxAntidoteGiven && character != null)
		{
			// Take it and give it back at the end of the scenario
			SavedItem savedItem = GameController.Instance.SavedCampaign.GetSavedItem(itemModel);
			savedItem.RemovedUnlocked(1);
			character.SavedCharacter.RemoveItem(itemModel);
		}
		// Item not given previously - give if scenario 7 is not completed yet
		else if(!poxAntidoteGiven && !GameController.Instance.SavedCampaign.CollectedPartyAchievements.Contains(PartyAchievement.FollowTheMoney))
		{
			character = (Character)await AbilityCmd.SelectFigure(authority: null,
				figures => figures.AddRange(GameController.Instance.Map.Figures.Where(figure => figure is Character)),
				mandatory: true, autoSelectIfOne: true, hintText:
					$"Select a character to receive Pox Antidote." + System.Environment.NewLine + System.Environment.NewLine + 
					"During this scenario, this item is equipped" + System.Environment.NewLine +
					$"without it occupying an {Icons.Inline(Icons.GetItem(ItemType.Small))} item slot.");

			GameController.Instance.SavedScenarioProgress.CustomValues.Add("PoxAntidoteGiven", true);
		}

		if(character != null)
		{ 
			await AbilityCmd.PermanentlyGiveItem(character, itemModel);
		}

		// Allow using Heal 1 instead of any top action
		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
			parameters => !parameters.ForgoneAction &&
				(parameters.AbilityCardSide.IsTop || parameters.AbilityCardSide.IsBasicTop),
			async parameters =>
			{
				parameters.ForgoAction();

				ActionState actionState = new ActionState(parameters.Performer, [HealAbility.Builder().WithHealValue(1).WithRange(2).Build()]);
				await actionState.Perform();
			},
			EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.Heal),
			effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Heal)}1, {Icons.Inline(Icons.Range)}2"
		);

		// Win when all infected warriors are healed and all enemies are dead
		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters =>
			{
				if(_infectedWarriors.Count < 4 || _infectedWarriors.Any(infectedWarrior => !infectedWarrior.IsHealed))
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
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(!_roomRevealed)
		{
			UpdateScenarioText(
				$"City Archers and City Guards suffer from {Icons.Inline(Icons.GetCondition(Conditions.Infect))}" +
				"They are considered allies to you." + 
				"If you perform a heal ability targeting the infected warrior, you have successfully cured them.");
			
			_roomRevealed = true;
		}

		foreach(Marker marker in GameController.Instance.Map.Markers)
		{
			if(marker.GetParent<Room>() == parameters.Room)
			{
				await SpawnGuard(marker);
			}
		}

		await GDTask.CompletedTask;
	}

	private async GDTask SpawnGuard(Marker marker)
	{
		MonsterModel monsterModel = marker.MarkerType == Marker.Type.a ? ModelDB.Monster<CityArcher>() : ModelDB.Monster<CityGuard>();

		Monster monster = await AbilityCmd.SpawnMonster(monsterModel, MonsterType.Normal, marker.Hex, false);

		monster.SetAlignment(Alignment.Characters);
		monster.SetEnemies(Alignment.Enemies);
		monster.SetHealth(4);
		monster.SetMaxHealth(4);
		await AbilityCmd.AddCondition(null, monster, Conditions.Infect);

		InfectedWarrior infectedWarrior = new();
		await infectedWarrior.Init(monster);
		_infectedWarriors.Add(infectedWarrior);
	}

	public class InfectedWarrior
	{
		public bool IsHealed = false;

		public async GDTask Init(Monster monster)
		{
			ScenarioEvents.InflictConditionEvent.Subscribe(monster, this,
				parameters => parameters.Target == monster,
				async parameters =>
				{
					parameters.SetPrevented(true);

					await GDTask.CompletedTask;
				}
			);

			// Can be targeted only with a heal
			ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(monster, this,
				parameters =>
					parameters.PotentialTarget == monster && 
					parameters.PotentialAbilityState != null &&
					parameters.PotentialAbilityState is not HealAbility.State,
				parameters =>
				{
					parameters.SetCannotBeTargeted();
				}
			);

			ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(monster, this,
				parameters => parameters.PotentialTarget == monster,
				parameters =>
				{
					parameters.SetCannotBeFocused();
				}
			);

			ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Subscribe(monster, this,
				parameters => parameters.Figure == monster,
				parameters =>
				{
					parameters.SetImmuneToForcedMovement();
				}
			);

			ScenarioEvents.SufferDamageEvent.Subscribe(monster, this,
				parameters => parameters.Figure == monster,
				async parameters =>
				{
					parameters.SetDamagePrevented();

					await GDTask.CompletedTask;
				}
			);

			ScenarioEvents.RemoveConditionEvent.Subscribe(monster, this,
				parameters => parameters.Figure == monster && parameters.Condition == Conditions.Infect,
				async parameters =>
				{
					IsHealed = true;

					monster.MonsterGroup.RegisterMonster(monster);
					GameController.Instance.Map.RegisterFigure(monster);

					await Unsubscribe(monster);
				}
			);

			ScenarioEvents.AfterHealPerformedEvent.Subscribe(monster, this,
				parameters => parameters.AbilityState.Target == monster,
				async parameters => 
				{
					IsHealed = true;

					monster.MonsterGroup.RegisterMonster(monster);
					GameController.Instance.Map.RegisterFigure(monster);

					await Unsubscribe(monster);
				}
			);

			ScenarioEvents.FigureKilledEvent.Subscribe(monster, this,
				parameters => parameters.Figure == monster,
				async parameters => 
				{
					await Unsubscribe(monster);
				}
			);

			await GDTask.CompletedTask;
		}

		private async GDTask Unsubscribe(Monster monster)
		{
			ScenarioEvents.InflictConditionEvent.Unsubscribe(monster, this);
			ScenarioCheckEvents.CanBeTargetedCheckEvent.Unsubscribe(monster, this);
			ScenarioCheckEvents.CanBeFocusedCheckEvent.Unsubscribe(monster, this);
			ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Unsubscribe(monster, this);
			ScenarioEvents.SufferDamageEvent.Unsubscribe(monster, this);
			ScenarioEvents.RemoveConditionEvent.Unsubscribe(monster, this);
			ScenarioEvents.AfterHealPerformedEvent.Unsubscribe(monster, this);
			ScenarioEvents.FigureKilledEvent.Unsubscribe(monster, this);
		}
	}
}