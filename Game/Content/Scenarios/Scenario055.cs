using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario055 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario055.tscn";

	public override int ScenarioNumber => 55;
	public override string Name => "Catacomb Plunder";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SideScenarioChain>();

	public override string IntroductionText =>
		"""
		You are walking near the city walls with Shiela, the potion maker, on a rare day that is both sunny and peaceful, when the Captain of the Guard approaches you. You greet him with mild caution—many of your exploits are technically outside the city’s laws, but he is a good man and generally very good at turning a blind eye to some of your more dubious activities. Still, you are relieved when he asks for your help.

		“Some of my men have reported strange activity around an old crypt just outside the city walls. They are even talking about, well it’s best that you investigate for yourself, I don’t want to put thoughts in your head. Here’s the location.”

		He virtually throws a scrap of paper at you before practically running off. Strange. Still, it’s never a bad thing to have the Captain of the Guard owe you a favour. Shiela looks at the note and breathes in deeply. “Be careful. There is more there than the Captain is letting on.” She rummages in her cloak and thrusts a small jar of strange powder at each of you which seems to be moving inside as if it is somehow alive. “Take this. It will allow you to re-animate and control those at rest.” She catches your look of surprise.

		“Don’t look at me like that! This isn’t necromancy! It’s just... something to give you some allies in there. Believe me, from what I’ve heard, you’ll need them.” You leave Shiela in Gloomhaven and travel out to the crypt. Long before you see it, the stench of death hits your nostrils and as you descend the crumbling steps, you begin to see why Shiela was concerned. There are earth demons and archers facing you in the crypt and, behind them, a sarcophagus with the lid slightly ajar.

		Maybe Shiela’s strange powder will be useful after all.
		""";

	public override string ConclusionText =>
		"""
		The Tomb Protector crumbles and falls, and the rest of his army are quickly dispatched.

		You travel back to Gloomhaven, thinking that you probably owe Shiela a drink for her reviving powder—and the Captain of the Guard definitely owes you one.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BanditArcher>(),
		ModelDB.Monster<EarthDemon>(),
		ModelDB.Monster<HarrowerInfester>(),
		ModelDB.Monster<LivingBonesScenario55>(),
		ModelDB.Monster<LivingCorpse>(),
		ModelDB.Monster<TombProtector>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainProsperityReward(1),
		new GainReputationReward(1),
		new GainXPReward(10)
	];

	private readonly List<SarcophagusObstacle> _openedSarcophagi = [];
	private List<Objective> _springGuns;
	private Door _door1;
	private List<Door> _doors2;

	private CustomScenarioGoal _coffinGoal;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal(countObjectives: true));
		_coffinGoal = await AddGoal(new CustomScenarioGoal(textParameters => "Open 4 coffins.", hasProgress: true, maxProgress: 4));

		GameController.Instance.Map.Treasures[0].SetItemDesignLoot(ModelDB.Item<DrakesBlood>());
		GameController.Instance.Map.Treasures[1].SetItemDesignLoot(ModelDB.Item<ManaMedicine>());

		_door1 = GameController.Instance.Map.GetMarker(Marker.Type._1).GetHexObject<Door>();
		_doors2 = GameController.Instance.Map.GetMarkers(Marker.Type._2).Select(hex => hex.GetHexObject<Door>()).ToList();

		_springGuns = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.GetHexObject<Objective>()).ToList();
		int springGunHealth = GameController.Instance.SavedCampaign.Characters.Count + GameController.Instance.SavedScenario.ScenarioLevel;
		foreach(Objective springGun in _springGuns)
		{
			springGun.Init(springGunHealth, "Spring Gun");
			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, springGun,
				parameters => parameters.Figure is Character or Summon && RangeHelper.Distance(parameters.Figure.Hex, springGun.Hex) <= 2,
				async parameters =>
				{
					ActionState actionState = new ActionState(springGun,
					[
						AttackAbility.Builder()
							.WithDamage(2)
							.WithRange(2)
							.WithPierce(2)
							.WithFilterTargets((_, figure) => figure == parameters.Figure)
							.Build()
					]);
					await actionState.Perform();
				}
			);
		}

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
			parameters =>
				parameters.Figure is Character &&
				RangeHelper.GetHexesInRange(parameters.Figure.Hex, 1).Any(hex =>
					hex.TryGetHexObjectOfType(out SarcophagusObstacle sarcophagus) && !_openedSarcophagi.Contains(sarcophagus)),
			async parameters =>
			{
				SarcophagusObstacle sarcophagusObstacle = RangeHelper.GetHexesInRange(parameters.Figure.Hex, 1)
					.First(hex => hex.HasHexObjectOfType<SarcophagusObstacle>()).GetHexObjectOfType<SarcophagusObstacle>();
				Monster monster = await SpawnMonster(parameters.Figure, ModelDB.Monster<LivingBonesScenario55>(), MonsterType.Normal,
					sarcophagusObstacle.Hexes,
					alignment: Alignment.Characters, enemies: Alignment.Monsters);
				monster.SetAMDCardDeck(parameters.Figure.AMDCardDeck);
				_openedSarcophagi.Add(sarcophagusObstacle);
				await _coffinGoal.AdjustProgress(1);

				Character character;
				if(parameters.Figure is Character parametersFigure)
				{
					character = parametersFigure;
				}
				else
				{
					character = ((Summon)parameters.Figure).CharacterOwner;
				}

				await AbilityCmd.AddCharacterToken(character, monster,
					textParameters => $"{character.SavedCharacter.GetNameAndIcon(textParameters)} controls all this figure's abilities.");

				ScenarioEvents.AbilityStartedEvent.Subscribe(this, monster,
					abilityStartedParameters => abilityStartedParameters.Authority == monster,
					async abilityStartedParameters =>
					{
						abilityStartedParameters.SetAuthority(parameters.Figure);
						await GDTask.CompletedTask;
					}
				);
			}
		);

		AddScenarioRule("The sarcophagus obstacles represent coffins and cannot be moved or destroyed.");
		AddScenarioRule(textParameters =>
			$"If a character ends their turn adjacent to a coffin, it is considered opened. Spawn one normal Living Bones in a hex closest to the coffin, and the character who opened the coffin places one of their character tokens on the Living Bones. This Living Bones is an ally to you and your allies and an enemy to all other monsters. It has only {Icons.Inline(Icons.Targets, textParameters)}1. It acts on Initiative 50 each round, performing “{Icons.Inline(Icons.Move, textParameters)}+0, {Icons.Inline(Icons.Attack, textParameters)}+0” on each of its turns. The character who marked the Living Bones with their character token controls its abilities, and the Living Bones draws from the character’s attack modifier deck. Only one Living Bones can be spawned per coffin.");
		AddScenarioRule(textParameters =>
			$"The stone pillar objectives represent Spring Guns. If any character or character summon ends their turn within {Icons.Inline(Icons.Range, textParameters)}2 of a Spring Gun, the Spring Gun performs “{Icons.Inline(Icons.Attack, textParameters)}2, {Icons.Inline(Icons.Range, textParameters)}2, {Icons.Inline(Icons.Pierce, textParameters, ignoreParametersColor: true)}2” on the figure, drawing from the monster ability deck.");
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters roomRevealedParameters)
	{
		await base.OnRoomRevealed(roomRevealedParameters);
		if(roomRevealedParameters.OpenedDoor == _door1)
		{
			AddScenarioRule(textParameters =>
				$"If any character or character summon ends their turn on a pressure plate, all figures within {Icons.Inline(Icons.Range, textParameters)}1 of the pressure plate suffer {Icons.Inline(Icons.Damage, textParameters)}2 and gain {Icons.InlineCondition(Conditions.Wound1, textParameters)}.");

			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _door1,
				parameters => parameters.Figure is Character or Summon && parameters.Figure.Hex.HasHexObjectOfType<PressurePlate>(),
				async parameters =>
				{
					foreach(Figure figure in RangeHelper.GetFiguresInRange(parameters.Figure, 1))
					{
						await AbilityCmd.SufferDamage(figure, 2, parameters.Figure);
						await AbilityCmd.AddCondition(null, figure, Conditions.Wound1);
					}
				}
			);

			await ShowText(
				"""
				You delve deeper into the catacombs, your creepy skeletal companion following behind. As you enter the next corridor, you are faced with a whole range of horrific creatures, and the intensifying stench of decay. You begin to wonder whether this favor for the Captain is worth it.
				""");
		}
		else if(_doors2.Contains(roomRevealedParameters.OpenedDoor))
		{
			foreach(Door door in _doors2)
			{
				if(!door.Opened)
				{
					await door.Open(roomRevealedParameters.PotentialOpener);
				}
			}

			Figure tombProtector =
				GameController.Instance.Map.Figures.First(figure => figure is Monster monster && monster.MonsterModel is TombProtector);

			int quarterHealth = tombProtector.MaxHealth / 4;

			ScenarioEvents.RoundStartBeforeCardSelectionEvent.Subscribe(this,
				parameters => !tombProtector.IsDead,
				async parameters =>
				{
					await AbilityCmd.SufferDamage(tombProtector, quarterHealth, tombProtector);
				}
			);

			ScenarioEvents.SufferDamageEvent.Subscribe(this,
				parameters => parameters.Figure == tombProtector && parameters.WouldSufferDamage &&
				              parameters.PotentialDamageDealer != tombProtector &&
				              GameController.Instance.ScenarioPhaseManager.ActivePhase is not CardSelectionPhase,
				async parameters =>
				{
					parameters.SetDamagePrevented();

					await GDTask.CompletedTask;
				}
			);

			ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(this,
				parameters => parameters.PotentialTarget == tombProtector,
				parameters =>
				{
					parameters.SetCannotBeTargeted();
				}
			);

			//TODO: Draws two cards at the beginning of each round

			ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(this,
				parameters => parameters.Figure == tombProtector,
				parameters =>
				{
					parameters.Add(new InfoTextExtraEffect.Parameters(textParameters =>
						"This figure cannot suffer damage other than at the beginning of each round. This figure cannot be targeted by any abilities."));
				}
			);

			AddScenarioRule(textParameters =>
				$"The Tomb Protector suffers {Icons.Inline(Icons.Damage, textParameters)}{quarterHealth} at the beginning of each round. It cannot be targeted by any abilities or suffer {Icons.Inline(Icons.Damage, textParameters)} any other way."); // It draws two monster ability cards instead of one each round and performs each action at the initiative listed on each card.");

			await ShowText(
				"""
				Battling through the catacombs, you acquire a couple more resurrected skeletons on the way. Your strange group forces its way into the next chamber, where you come face to face with a huge Stone Golem. In a voice that shakes the whole chamber he cries “I AM THE TOMB PROTECTOR! YOU DESECRATED MY GRAVES? NOW YOU WILL FILL THEM YOURSELVES!”

				The beast looks terrifying, but as he raises a giant fist, you can see that he is ancient and crumbling. He looks too powerful to be beaten, but you hope he can tear himself to pieces before he kills you.
				""");
		}
	}
}