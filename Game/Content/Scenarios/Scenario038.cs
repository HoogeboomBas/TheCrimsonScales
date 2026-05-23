using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario038 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario038.tscn";

	public override int ScenarioNumber => 38;
	public override string Name => "Altars of Confusion";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<PersonalQuestScenarioChain>();

	public override string IntroductionText =>
		"""
		Following the twin entrances down from the Burning Stones, you find yourself in a complex, hand-dug series of tunnels. Although the tunnel network is mainly lit by torches, there is the odd flash and crackle of elemental and transformative energy in the air. With a certain amount of caution, both as to what you’ll find and the stability of the tunnels, you venture deeper underground.
		""";

	public override string ConclusionText =>
		"""
		As you destroy the last altar, the atmosphere returns to something like normal, and you dispatch the remaining creatures. The Night Demons had certainly demonstrated some impressive skills here, no matter how misguided.

		As you leave the caves, you find yourself wondering, how much did the Night Demons manage to change the creatures—and how permanent are the changes likely to be?
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BlackImp>(),
		ModelDB.Monster<LivingSpirit>(),
		ModelDB.Monster<NightDemon>(),
		ModelDB.Monster<StoneGolem>()
	];

	public override List<SavedReward> Rewards =>
	[
		new OpenEnvelopeReward(ModelDB.PersonalQuest<NaturalSelection>())
	];

	private Door _door2;
	private Door _door3;
	private Objective _altarOfMystification;
	private Objective _altarOfDisorientation;
	private Objective _altarOfPerplexity;

	private CustomScenarioGoal _altarGoal;

	private ScenarioRule _altarOfMystificationRule;
	private ScenarioRule _altarOfMystificationDoorRule;
	private ScenarioRule _livingSpiritsKilledRule;

	private ScenarioRule _altarOfDisorientationRule;
	private ScenarioRule _altarOfPerplexityRule;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal(countObjectives: false, revealedOnly: true));
		_altarGoal = await AddGoal(new CustomScenarioGoal(textParameters => "Destroy 2 altars.",
			hasProgress: true, maxProgress: 2));

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<WovenPlateArmor>());

		_door2 = GameController.Instance.Map.GetMarker(Marker.Type._2).GetHexObject<Door>();
		_door3 = GameController.Instance.Map.GetMarker(Marker.Type._3).GetHexObject<Door>();
		_altarOfMystification = GameController.Instance.Map.GetMarker(Marker.Type.a).GetHexObject<Objective>();
		_altarOfDisorientation = GameController.Instance.Map.GetMarker(Marker.Type.b).GetHexObject<Objective>();
		_altarOfPerplexity = GameController.Instance.Map.GetMarker(Marker.Type.c).GetHexObject<Objective>();

		int firstThirdAltarHealth =
			(GameController.Instance.SavedScenario.ScenarioLevel + 3) * GameController.Instance.SavedCampaign.Characters.Count;
		int secondAltarHealth = (GameController.Instance.SavedScenario.ScenarioLevel + 4) * GameController.Instance.SavedCampaign.Characters.Count;
		_altarOfMystification.Init(firstThirdAltarHealth, "Altar of Mystification");
		_altarOfPerplexity.Init(firstThirdAltarHealth, "Altar of Perplexity");
		_altarOfDisorientation.Init(secondAltarHealth, "Altar of Disorientation");
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(parameters.Room == GameController.Instance.Map.Rooms[1])
		{
			foreach(Figure livingSpirit in GameController.Instance.Map.Figures.Where(figure =>
				        figure is Monster monster && monster.MonsterModel is LivingSpirit))
			{
				await AbilityCmd.AddCondition(null, livingSpirit, Conditions.Invisible);
			}

			ScenarioEvents.AfterRemoveConditionEvent.Subscribe(this,
				conditionParameters =>
					conditionParameters.Figure is Monster monster && monster.MonsterModel is LivingSpirit &&
					conditionParameters.Condition == Conditions.Invisible,
				async conditionParameters =>
				{
					await AbilityCmd.AddCondition(null, conditionParameters.Figure, Conditions.Invisible);
				}
			);

			ScenarioEvents.FigureKilledEvent.Subscribe(this, _altarOfMystification,
				figureKilledParameters => figureKilledParameters.Figure == _altarOfMystification,
				async figureKilledParameters =>
				{
					ScenarioEvents.AfterRemoveConditionEvent.Unsubscribe(this);
					_altarOfMystificationRule.Remove();
					_altarOfMystificationDoorRule.Remove();

					await _altarGoal.AdjustProgress(1);

					await _door2.Unlock();
				}
			);

			ScenarioEvents.FigureKilledEvent.Subscribe(this,
				figureKilledParameters =>
					figureKilledParameters.Figure is Monster monster &&
					monster.MonsterModel is LivingSpirit &&
					!GameController.Instance.Map.Figures.Any(figure =>
						figure is Monster otherMonster && otherMonster.MonsterModel is LivingSpirit && !otherMonster.IsDead),
				async figureKilledParameters =>
				{
					_livingSpiritsKilledRule.Remove();

					await _door3.Unlock();
				}
			);

			_altarOfMystificationRule = AddScenarioRule(textParameters =>
				$"Until the Altar of Mystification is destroyed, all Living Spirits are permanently {Icons.Inline(Icons.GetCondition(Conditions.Invisible), textParameters)}.");

			_altarOfMystificationDoorRule = AddScenarioRule(textParameters =>
				$"When the Altar of Mystification is destroyed, unlock door {Icons.InlineMarker(Marker.Type._2, textParameters)}.");

			_livingSpiritsKilledRule = AddScenarioRule(textParameters =>
				$"When all Living Spirits have been killed, unlock door {Icons.InlineMarker(Marker.Type._3, textParameters)}.");

			await ShowText(
				"""
				Opening the door, you are greeted by a strange sensation of being surrounded by beings, yet you can’t see anyone. There is however, a large altar in the middle of the room, which looks as though some kind of ritual was midway through being performed when you appeared. You wonder if this ritual is behind the excess elemental activity at the Burning Stones, and the strange sense you have in here.

				Suddenly, a group of Living Spirits appear, firing off an attack before disappearing again. These must be the creatures responsible, and you know you must stop them. If only you could see where they were…
				""");
		}
		else if(parameters.Room == GameController.Instance.Map.Rooms[2])
		{
			ScenarioEvents.DuringAttackEvent.Subscribe(this, _altarOfDisorientation,
				attackParameters => RangeHelper.Distance(attackParameters.Performer.Hex, _altarOfDisorientation.Hex) == 1,
				async attackParameters =>
				{
					attackParameters.AbilityState.SingleTargetAdjustAttackValue(1);
					await GDTask.CompletedTask;
				}
			);

			ScenarioEvents.FigureKilledEvent.Subscribe(this, _altarOfDisorientation,
				figureKilledParameters => figureKilledParameters.Figure == _altarOfDisorientation,
				async figureKilledParameters =>
				{
					_altarOfDisorientationRule.Remove();
					await _altarGoal.AdjustProgress(1);
				}
			);

			_altarOfDisorientationRule =
				AddScenarioRule(textParameters =>
					$"All figures add -1{Icons.Inline(Icons.Attack, textParameters)} to all attacks performed while adjacent to the Altar of Disorientation.");

			await ShowText(
				"""
				Forcing open another door, you find another Night Demon chanting altercations at the altar. Surrounded by fiendish imps, you shudder as you sense a strange feeling in the room. You all feel slow, confused and unsteady on your feet. The feeling intensifies as you get closer to the altar, but you must destroy it somehow.
				""");
		}
		else if(parameters.Room == GameController.Instance.Map.Rooms[3])
		{
			ScenarioEvents.DuringAttackEvent.Subscribe(this, _altarOfPerplexity,
				attackParameters => RangeHelper.Distance(attackParameters.Performer.Hex, _altarOfPerplexity.Hex) == 1,
				async attackParameters =>
				{
					attackParameters.AbilityState.SingleTargetSetHasAdvantage();
					await GDTask.CompletedTask;
				}
			);

			ScenarioEvents.FigureKilledEvent.Subscribe(this, _altarOfPerplexity,
				figureKilledParameters => figureKilledParameters.Figure == _altarOfPerplexity,
				async figureKilledParameters =>
				{
					_altarOfPerplexityRule.Remove();
					await _altarGoal.AdjustProgress(1);
				}
			);

			_altarOfPerplexityRule =
				AddScenarioRule(textParameters =>
					"All figures gain advantage on all attacks performed while adjacent to the Altar of Perplexity.");

			await ShowText(
				"""
				This final room is much like the last — a Night Demon chanting over an altar, with assorted other creatures crowded in there too.

				Unlike the last room though, you feel sharp, nimble and especially focused. Unfortunately, it seems the other creatures do too.
				""");
		}
	}
}