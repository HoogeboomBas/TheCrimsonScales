using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario055 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario055.tscn";
	public override int ScenarioNumber => 55;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SideScenarioChain>();

	protected override ScenarioGoals CreateScenarioGoals() =>
		new CustomScenarioGoals("Kill all monsters and open four coffins to win this scenario.");

	private readonly List<SarcophagusObstacle> _openedSarcophagi = [];
	private List<Objective> _springGuns;
	private Door _door1;
	private List<Door> _doors2;

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		//TODO: Sarcophagi can't be moved

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
				});
		}

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
			parameters => parameters.Figure is Character &&
			              RangeHelper.GetHexesInRange(parameters.Figure.Hex, 1).Any(hex =>
				              hex.TryGetHexObjectOfType(out SarcophagusObstacle sarcophagus) && !_openedSarcophagi.Contains(sarcophagus)),
			async parameters =>
			{
				SarcophagusObstacle sarcophagusObstacle = RangeHelper.GetHexesInRange(parameters.Figure.Hex, 1)
					.First(hex => hex.HasHexObjectOfType<SarcophagusObstacle>()).GetHexObjectOfType<SarcophagusObstacle>();
				Monster monster = await SpawnMonster(parameters.Figure, ModelDB.Monster<LivingBonesScenario55>(), MonsterType.Normal,
					sarcophagusObstacle.Hexes,
					alignment: Alignment.Characters, enemies: Alignment.Enemies);
				monster.SetAMDCardDeck(parameters.Figure.AMDCardDeck);
				_openedSarcophagi.Add(sarcophagusObstacle);

				Character character;
				if(parameters.Figure is Character parametersFigure)
				{
					character = parametersFigure;
				}
				else
				{
					character = ((Summon)parameters.Figure).CharacterOwner;
				}

				await AbilityCmd.AddCharacterToken(character, monster, $"{character.Name} controls all this figure's abilities.");

				ScenarioEvents.AbilityStartedEvent.Subscribe(this, monster,
					abilityStartedParameters => abilityStartedParameters.Authority == monster,
					async abilityStartedParameters =>
					{
						abilityStartedParameters.SetAuthority(parameters.Figure);
						await GDTask.CompletedTask;
					});
			});

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => _openedSarcophagi.Count == 4 && KillAllEnemiesScenarioGoals.NoEnemiesRemaining(false),
			async parameters =>
			{
				await ((CustomScenarioGoals)ScenarioGoals).Win();
			});

		UpdateScenarioText(
			$"The stone pillar objectives represent Spring Guns. If any character or character summon ends their turn within {Icons.Inline(Icons.Range)}2 of a Spring Gun, the Spring Gun performs “{Icons.Inline(Icons.Attack)}2, {Icons.Inline(Icons.Range)}2, {Icons.Inline(Icons.Pierce)}2” on the figure, drawing from the monster ability deck.");
	}


	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters roomRevealedParameters)
	{
		await base.OnRoomRevealed(roomRevealedParameters);
		if(roomRevealedParameters.OpenedDoor == _door1)
		{
			UpdateScenarioText(
				$"If any character or character summon ends their turn on a pressure plate, all figures within {Icons.Inline(Icons.Range)}1 of the pressure plate suffer {Icons.Inline(Icons.Damage)}2 and gain {Icons.Inline(Icons.GetCondition(Conditions.Wound1))}.");
			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _door1,
				parameters => parameters.Figure is Character or Summon && parameters.Figure.Hex.HasHexObjectOfType<PressurePlate>(),
				async parameters =>
				{
					foreach(Figure figure in RangeHelper.GetFiguresInRange(parameters.Figure, 1))
					{
						await AbilityCmd.SufferDamage(figure, 2, parameters.Figure);
						await AbilityCmd.AddCondition(null, figure, Conditions.Wound1);
					}
				});
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
				});

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
				});

			//TODO: Draws two cards at the beginning of each round

			ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(this,
				parameters => parameters.Figure == tombProtector,
				parameters =>
				{
					parameters.Add(new InfoTextExtraEffect.Parameters(
						"This figure cannot suffer damage other than at the beginning of each round. This figure cannot be targeted by any abilities."));
				}
			);

			UpdateScenarioText($"""
			                    If any character or character summon ends their turn on a pressure plate, all figures within {Icons.Inline(Icons.Range)}1 of the pressure plate suffer {Icons.Inline(Icons.Damage)}2 and gain {Icons.Inline(Icons.GetCondition(Conditions.Wound1))}.

			                    The elite Stone Golem is the Tomb Protector and suffers {Icons.Inline(Icons.Damage)}{quarterHealth} at the beginning of each round. It cannot be targeted by any abilities or suffer {Icons.Inline(Icons.Damage)} any other way. It draws two monster ability cards instead of one each round and performs each action at the initiative listed on each card.
			                    """);
		}
	}

	protected override void UpdateScenarioText(string text)
	{
		string fullText = $"""
		                   The sarcophagus obstacles represent coffins and cannot be moved or destroyed.

		                   If a character ends their turn adjacent to a coffin, it is considered opened. Spawn one normal Living Bones in a hex closest to the coffin, and the character who opened the coffin places one of their character tokens on the Living Bones. This Living Bones is an ally to you and your allies and an enemy to all other monsters. It has only {Icons.Inline(Icons.Targets)}1. It acts on Initiative 50 each round, performing “{Icons.Inline(Icons.Move)}+0, {Icons.Attack}+0” on each of its turns. The character who marked the Living Bones with their character token controls its abilities, and the Living Bones draws from the character’s attack modifier deck. Only one Living Bones can be spawned per coffin.


		                   """ + text;
		base.UpdateScenarioText(fullText);
	}
}