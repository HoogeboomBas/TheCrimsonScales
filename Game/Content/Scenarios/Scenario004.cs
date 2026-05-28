using System.Linq;
using Fractural.Tasks;
using System.Collections.Generic;

public class Scenario004 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario004.tscn";

	public override int ScenarioNumber => 4;
	public override string Name => "Infected Warriors";

	public override List<ScenarioLink> Links => [GloomhavenLink.Instance];

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<InfectiousScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario005>()];

	public override string IntroductionText =>
		"""
		Following the old man’s warning, you cautiously head in the direction he came from. Bloody pox is a constant threat to the city of Gloomhaven, and is both highly contagious and lethal. It can be healed, but time is of the essence.
		""";

	public override string ConclusionText =>
		"""
		As the last monster is destroyed, the Captain of the Guard approaches you.

		“Thank you” he nods, “but the work is not yet complete. We gained the pox from a creature that is threatening Gloomhaven’s water supply. You need to kill the creature, and cleanse the water, or the whole of Gloomhaven will be poisoned.”
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<CityArcher>(),
		ModelDB.Monster<CityGuard>(),
		ModelDB.Monster<BloodOoze>(),
		ModelDB.Monster<FlamingDrake>(),
		ModelDB.Monster<ToxicImp>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainRandomOrbEachReward(),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario005>()),
		//new UnlockScenarioReward(ModelDB.Scenario<Scenario006>()), // Unlock is stated in the section book, but doesn't really make sense...
	];

	private readonly List<InfectedWarrior> _infectedWarriors = [];
	private bool _roomRevealed = false;

	private CustomScenarioGoal _cureGoal;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());
		_cureGoal = await AddGoal(new CustomScenarioGoal(textParameters => "Cure 4 sick warriors.", hasProgress: true, maxProgress: 4));

		AddScenarioRule(textParameters =>
			$"Any character may forgo the top action of their turn to perform a “{Icons.Inline(Icons.Heal, textParameters)}1, {Icons.Inline(Icons.Range, textParameters)}2” ability.");

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<BonecladShawl>());

		// Allow using Heal 1 instead of any top action
		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
			parameters =>
				!parameters.ForgoneAction &&
				(parameters.AbilityCardSide.AbilityCardSideType is AbilityCardSideType.Top or AbilityCardSideType.BasicTop),
			async parameters =>
			{
				parameters.ForgoAction();

				ActionState actionState = new ActionState(parameters.Performer, [HealAbility.Builder().WithHealValue(1).WithRange(2).Build()]);
				await actionState.Perform();
			},
			EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.Heal),
			effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Heal)}1, {Icons.Inline(Icons.Range)}2")
		);
	}

	public override async GDTask OnSetupCompleted()
	{
		await base.OnSetupCompleted();

		// This implements
		// One character gains the “Pox Antidote” item. 
		// During this scenario, this item may be equipped without it occupying an item slot.
		Map map = GameController.Instance.Map;

		ItemModel itemModel = ModelDB.Item<PoxAntidote>();
		Character character =
			(Character)map.Figures.FirstOrDefault(figure => figure is Character character && character.SavedCharacter.HasItem(itemModel), null);

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
			await ShowText(
				"As you approach the stricken guards, you are spotted by Shiela, a regular from the Sleeping Lion, famed for her potion making. “Thank you for coming so quickly. Take this—it will help cure the stricken.”");

			// character = (Character)await AbilityCmd.SelectFigure(authority: null,
			// 	figures => figures.AddRange(map.Figures.Where(figure => figure is Character)),
			// 	mandatory: true, autoSelectIfOne: true, hintText: () =>
			// 		$"Select a character to receive Pox Antidote." + System.Environment.NewLine + System.Environment.NewLine +
			// 		"During this scenario, this item is equipped" + System.Environment.NewLine +
			// 		$"without it occupying an {Icons.Inline(Icons.GetItem(ItemType.Small))} item slot.");
			character = (Character)await AbilityCmd.SelectFigure(authority: null,
				figures => figures.AddRange(map.Figures.Where(figure => figure is Character)),
				mandatory: true, autoSelectIfOne: true, hintText: () =>
					$"Select a character to receive a Pox Antidote.");

			GameController.Instance.EndEvent += (scenarioResult, savedScenarioProgress) =>
			{
				GameController.Instance.SavedScenarioProgress.CustomValues.Add("PoxAntidoteGiven", true);
			};
		}

		if(character != null)
		{
			await AbilityCmd.PermanentlyGiveItem(character, itemModel);
		}
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(!_roomRevealed)
		{
			AddScenarioRule(
				$"""
				 City Archers and City Guards suffer from {Icons.Inline(Icons.GetCondition(Conditions.Infect))}. They are considered allies to you and do not act during their turn. If you perform a heal ability targeting an infected warrior, you have successfully cured them, and they will join your for the remainder of the scenario.
				 """);
			AddScenarioRule(
				$"""
				 City Guards and City Archers draw from the monster attack modifier deck, are one level lower than the scenario level and have a maximum hit point value of 4.
				 """);

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
		int guardLevel = GameController.Instance.SavedScenario.ScenarioLevel > 0 ? GameController.Instance.SavedScenario.ScenarioLevel - 1 : 0;

		Monster monster = await AbilityCmd.SpawnMonster(monsterModel, MonsterType.Normal, marker.Hex, guardLevel, Alignment.Characters);

		monster.SetHealth(4);
		monster.SetMaxHealth(4);
		await AbilityCmd.AddCondition(null, monster, Conditions.Infect);

		InfectedWarrior infectedWarrior = new InfectedWarrior();
		await infectedWarrior.Init(monster, _infectedWarriors, _cureGoal);
	}

	public class InfectedWarrior
	{
		public async GDTask Init(Monster monster, List<InfectedWarrior> infectedWarriors, CustomScenarioGoal cureGoal)
		{
			infectedWarriors.Add(this);

			ScenarioCheckEvents.CanTakeTurnCheckEvent.Subscribe(monster, this,
				parameters => parameters.Figure == monster,
				parameters =>
				{
					parameters.SetCannotTakeTurn();
				}
			);

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
					(parameters.Performer is not Character ||
					 (parameters.PotentialAbilityState != null && parameters.PotentialAbilityState is not HealAbility.State)),
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
				parameters => parameters.Figure == monster && parameters.ConditionModel == Conditions.Infect,
				async parameters =>
				{
					await Unsubscribe(monster);

					await cureGoal.AdjustProgress(1);
					infectedWarriors.Remove(this);
				}
			);

			ScenarioEvents.AfterHealPerformedEvent.Subscribe(monster, this,
				parameters => parameters.AbilityState.Target == monster,
				async parameters =>
				{
					await Unsubscribe(monster);

					infectedWarriors.Remove(this);
				}
			);

			ScenarioEvents.FigureKilledEvent.Subscribe(monster, this,
				parameters => parameters.Figure == monster,
				async parameters =>
				{
					await Unsubscribe(monster);

					infectedWarriors.Remove(this);
				}
			);

			await GDTask.CompletedTask;
		}

		private async GDTask Unsubscribe(Monster monster)
		{
			ScenarioCheckEvents.CanTakeTurnCheckEvent.Unsubscribe(monster, this);
			ScenarioEvents.InflictConditionEvent.Unsubscribe(monster, this);
			ScenarioCheckEvents.CanBeTargetedCheckEvent.Unsubscribe(monster, this);
			ScenarioCheckEvents.CanBeFocusedCheckEvent.Unsubscribe(monster, this);
			ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Unsubscribe(monster, this);
			ScenarioEvents.SufferDamageEvent.Unsubscribe(monster, this);
			ScenarioEvents.RemoveConditionEvent.Unsubscribe(monster, this);
			ScenarioEvents.AfterHealPerformedEvent.Unsubscribe(monster, this);
			ScenarioEvents.FigureKilledEvent.Unsubscribe(monster, this);

			await GDTask.CompletedTask;
		}
	}
}