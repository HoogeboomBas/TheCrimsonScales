using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Scenario026 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario026.tscn";

	public override int ScenarioNumber => 26;
	public override string Name => "Thermal Stone Peak";

	protected override List<ScenarioRequirement> Requirements => [new PartyAchievementRequirement(PartyAchievement.FrozenWarrior, true)];
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<ChillyScenarioChain>();

	public override string IntroductionText =>
		"""
		You consult the directions you found in the Aesther’s lab one final time, and haul yourself to the top of a nearby peak. There is significant geo-thermal energy here, creating hot springs and bubbling lava amongst the ice and snow. There is steam, or possibly mist, surrounding the peak, so it is initially hard to see what is going on. You check the brief notes, which seem to refer to harnessing “thermal stones” to create, influence or somehow modify, the strange creatures. You hope that destroying them will halt the mysterious Aesther’s plans, at least for now.

		Suddenly, the mist clears in front of you, and you see 2 ice-blue glowing stones, along with several of the strange creatures. Your destruction of these thermal stones will not be straightforward—but then again, nothing ever is.
		""";

	public override string ConclusionText =>
		"""
		As you destroy the last stone, the strange creatures melt away into the mist—a little too quickly. Almost immediately, a glowing purple dot appears in mid-air, swirling and expanding until it is large enough for the Aesther to step through.

		“I am Helena, and I believe those are my notes you’ve been using to destroy my work,” she says calmly. “All I can say is, firstly those who can manipulate time and space will not be held back by the actions of mere land-dwellers. Secondly, I hope you know what you are doing. I have seen your path, and it is fraught with all kinds of dangers. At the very least, you will be needing this,” she says, dropping a strange artefact before opening the portal again, stepping through it and disappearing again.

		You approach the device extremely carefully, sure it must be a trap—but it appears that Helena has witnessed you destroying the natural sources she spent months preparing, and decided to reward you.

		Baffled, not for the first time, by the actions of Aesthers, you take the item and return to Gloomhaven before the snow sets in.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<FrozenCadaver>(),
		ModelDB.Monster<HailDemon>(),
		ModelDB.Monster<HarrowerIcecrawlers>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainXPReward(20),
		new GainCheckmarkReward()
	];

	public override string BGSPath => "res://Audio/BGS/Cave.ogg";

	private CustomScenarioGoal _goal;

	private ScenarioRule _coldThermalStoneRule1;
	private ScenarioRule _coldThermalStoneRule2;
	private ScenarioRule _hotThermalStoneRule1;
	private ScenarioRule _hotThermalStoneRule2;
	private ScenarioRule _icyFireThermalStoneRule1;
	private ScenarioRule _icyFireThermalStoneRule2;

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		await AbilityCmd.AddCondition(null, character, Conditions.Chill);
	}

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		_goal = await AddGoal(new CustomScenarioGoal(textParameters => "Destroy 5 thermal stones.",
			hasProgress: true, maxProgress: 5));

		AddScenarioRule(textParameters =>
			$"Any character may forgo the top or bottom action of their turn to remove all {Icons.InlineCondition(Conditions.Chill, textParameters)} tokens from self or one summon they own within {Icons.Inline(Icons.Range, textParameters)}2.");

		List<Objective> coldThermalStones = GameController.Instance.Map.Rooms[0].GetChildrenOfType<Objective>();
		List<Objective> hotThermalStones = GameController.Instance.Map.Rooms[1].GetChildrenOfType<Objective>();
		Objective icyFireThermalStone = GameController.Instance.Map.Rooms[2].GetChildrenOfType<Objective>()[0];
		int thermalStoneHealth = GameController.Instance.SavedCampaign.Characters.Count + 3;
		int icyFireThermalStoneHealth = GameController.Instance.SavedCampaign.Characters.Count * 6;

		int coldThermalStonesRemaining = coldThermalStones.Count;
		int hotThermalStonesRemaining = coldThermalStones.Count;

		_coldThermalStoneRule1 = AddScenarioRule(textParameters =>
			$"Each time a character or character summons attacks a Cold thermal stone, they gain {Icons.Inline(Icons.GetCondition(Conditions.Chill), textParameters)} immediately following the attack.");

		_coldThermalStoneRule2 = AddScenarioRule(textParameters =>
			$"When a character or character summon destroys a Cold thermal stone, they immediately remove all {Icons.Inline(Icons.GetCondition(Conditions.Chill), textParameters)} from self and place a water tile in the hex it was occupying.");

		foreach(Objective objective in coldThermalStones)
		{
			objective.Init(thermalStoneHealth, "Cold Thermal Stone");

			ScenarioEvents.AfterAttackPerformedEvent.Subscribe(this, objective,
				canApplyParameters =>
					canApplyParameters.AbilityState.Target == objective &&
					canApplyParameters.AbilityState.Performer is Character or Summon,
				async applyParameters =>
				{
					await AbilityCmd.AddCondition(null, applyParameters.AbilityState.Performer, Conditions.Chill);
				}
			);

			ScenarioEvents.FigureKilledEvent.Subscribe(this, objective,
				canApplyParameters =>
					canApplyParameters.Figure == objective &&
					canApplyParameters.PotentialAbilityState.Performer is Character or Summon,
				async applyParameters =>
				{
					await _goal.AdjustProgress(1);

					coldThermalStonesRemaining--;

					if(coldThermalStonesRemaining == 0)
					{
						_coldThermalStoneRule1.Remove();
						_coldThermalStoneRule2.Remove();
					}

					await AbilityCmd.RemoveAllChill(applyParameters.PotentialAbilityState.Performer);
					await AbilityCmd.CreateDifficultTerrain(objective.Hex,
						ResourceLoader.Load<PackedScene>("res://Content/OverlayTiles/DifficultTerrain/Water1H.tscn"));
					ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(this, objective);
					ScenarioEvents.FigureKilledEvent.Unsubscribe(this, objective);
				}
			);
		}

		foreach(Objective objective in hotThermalStones)
		{
			objective.Init(thermalStoneHealth, "Hot Thermal Stone");

			ScenarioEvents.AfterAttackPerformedEvent.Subscribe(this, objective,
				canApplyParameters =>
					canApplyParameters.AbilityState.Target == objective &&
					canApplyParameters.AbilityState.Performer is Character or Summon,
				async applyParameters =>
				{
					await AbilityCmd.SufferDamage(applyParameters.AbilityState.Performer, 1, objective);
				}
			);

			ScenarioEvents.FigureKilledEvent.Subscribe(this, objective,
				canApplyParameters =>
					canApplyParameters.Figure == objective &&
					canApplyParameters.PotentialAbilityState.Performer is Character or Summon,
				async applyParameters =>
				{
					await _goal.AdjustProgress(1);

					hotThermalStonesRemaining--;

					if(hotThermalStonesRemaining == 0)
					{
						_hotThermalStoneRule1.Remove();
						_hotThermalStoneRule2.Remove();
					}

					HealAbility heal = HealAbility.Builder()
						.WithHealValue(3)
						.WithTarget(Target.Self)
						.Build();
					ActionState actionState = new ActionState(applyParameters.PotentialAbilityState.Performer, [heal]);
					await actionState.Perform();
					await AbilityCmd.CreateOverlayTile<HazardousTerrain>(objective.Hex,
						ResourceLoader.Load<PackedScene>("res://Content/OverlayTiles/HazardousTerrain/HotCoals1H.tscn"));
					ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(this, objective);
					ScenarioEvents.FigureKilledEvent.Unsubscribe(this, objective);
				}
			);
		}

		icyFireThermalStone.Init(icyFireThermalStoneHealth, "Icy Fire Thermal Stone");

		ScenarioEvents.AfterAttackPerformedEvent.Subscribe(this, icyFireThermalStone,
			canApplyParameters =>
				canApplyParameters.AbilityState.Target == icyFireThermalStone &&
				canApplyParameters.AbilityState.Performer is Character or Summon,
			async applyParameters =>
			{
				await AbilityCmd.AddConditions(null, applyParameters.AbilityState.Performer, [Conditions.Chill, Conditions.Wound1]);
			}
		);

		ScenarioEvents.FigureKilledEvent.Subscribe(this, icyFireThermalStone,
			canApplyParameters =>
				canApplyParameters.Figure == icyFireThermalStone &&
				canApplyParameters.PotentialAbilityState.Performer is Character or Summon,
			async applyParameters =>
			{
				await _goal.AdjustProgress(1);

				_icyFireThermalStoneRule1.Remove();
				_icyFireThermalStoneRule2.Remove();

				Figure figure = applyParameters.PotentialAbilityState.Performer;
				await AbilityCmd.RemoveAllNegativeConditions(figure);
				ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(this, icyFireThermalStone);
				ScenarioEvents.FigureKilledEvent.Unsubscribe(this, icyFireThermalStone);
			}
		);

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<OrbOfDespair>());

		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
			parameters => !parameters.ForgoneAction && RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 2)
				.Any(figure => figure.HasCondition(Conditions.Chill) &&
				               ((figure is Summon summon && summon.Owner == parameters.Performer) || parameters.Performer == figure)),
			async parameters =>
			{
				parameters.ForgoAction();

				ActionState actionState = new ActionState(parameters.Performer, [
					OtherAbility.Builder()
						.WithPerformAbility(async state =>
						{
							Figure figure = await AbilityCmd.SelectFigure(state, list =>
							{
								list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 2)
									.Where(figure =>
										(figure is Summon summon && summon.Owner == parameters.Performer) || parameters.Performer == figure));
							});

							if(figure == null)
							{
								return;
							}

							await AbilityCmd.RemoveAllChill(figure);
						})
						.Build()
				]);
				await actionState.Perform();
			},
			EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Chill)),
			effectInfoViewParameters: new TextEffectInfoView.Parameters(
				$"Remove all {Icons.Inline(Icons.GetCondition(Conditions.Chill))} from self or one of your summons within {Icons.Inline(Icons.Range)} 2.")
		);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(parameters.Room == GameController.Instance.Map.Rooms[1])
		{
			_hotThermalStoneRule1 = AddScenarioRule(textParameters =>
				$"Each time a character or character summon attacks a Hot thermal stone, they immediately suffer {Icons.Inline(Icons.Damage, textParameters)}1 following the attack.");

			_hotThermalStoneRule2 = AddScenarioRule(textParameters =>
				$"When a character or character summon destroys a Hot thermal stone, they immediately perform {Icons.Inline(Icons.Heal, textParameters)}3, Self and place a hot coal tile in the hex it was occupying.");

			await ShowText(
				"""
				You move further across the peak, where the steam is rising thickly. The stones in this area are glowing red and are emitting great heat. Again, you attempt to destroy the stones, while fending off the attention of the beasts emerging from the smoky atmosphere.
				""");
		}
		else if(parameters.Room == GameController.Instance.Map.Rooms[2])
		{
			_icyFireThermalStoneRule1 = AddScenarioRule(textParameters =>
				$"Each time a character or character summon attacks an Icy Flame thermal stone, they immediately gain {Icons.Inline(Icons.GetCondition(Conditions.Wound1), textParameters)} and {Icons.Inline(Icons.GetCondition(Conditions.Chill), textParameters)}.");

			_icyFireThermalStoneRule2 = AddScenarioRule(textParameters =>
				$"When a character or character summon destroys the Icy Flame thermal stone, they immediately remove all negative conditions from self.");

			await ShowText(
				"""
				The final stone lays on a small rise ahead. Here, the mist has cleared slightly and you can see that there are barely visible blue flames rising from its edges. This is the final stone, and is the largest and seems to exert the most energy. It is also surrounded by more of the frozen creatures.
				""");
		}
	}
}