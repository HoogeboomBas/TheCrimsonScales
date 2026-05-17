using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Scenario020 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario020.tscn";

	public override int ScenarioNumber => 20;
	public override string Name => "Midnight Ritual";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario022>()];

	public override string IntroductionText =>
		"""
		You take the Orb of Embers straight to Athan (knocking cautiously on the office door this time). Rather than his slightly grumpy call of “Enter!,” instead he sounds weary as he invites you in.

		As you enter the office, an atmosphere of gloom envelopes his study, and he barely raises his head as you enter. Learning from past mistakes, you say to him, “We have something to show you, but is everything ok?

		At this, he raises his head, shaking it slowly. “On the contrary, there is a serious problem affecting my congregation.” He continues “I generally leave the rumor-mongers of Gloomhaven to their tales, and rise above the various goings-on of this city to focus on my work. On this occasion though, I cannot pull my thoughts away from my fellow worshippers who have been led down a dangerous path.”

		“As you know, The Keeper of The Great Oak is a responsible position, and one that I am deeply privileged to hold. Although maintaining the Oak and communicating its incredible properties is a big part of my calling, caring for the less fortunate souls of Gloomhaven through the donations we receive, is something I take equally seriously.”

		“Recently however, a corrupting and dangerous cult has sprung up, deliberately seeking to recruit those most vulnerable to their evil. We have lost many people, who desperately need help from The Great Oak. This is not a power struggle, it is a humanitarian issue, but we do not have the ability to penetrate this cult and cannot help those who need us now more than ever.”

		Sensing an opportunity, but still a little wary, you reply “We have recovered the Orb of Embers. Would you like us to infiltrate the cult and free the victims, while you examine the Orb and the book we brought to you previously?”

		Athan’s eyes light up briefly, before a flicker of concern crosses his face. “You are kind and good people, thank you. And I will, of course, examine the Orb and the detailed writings you have brought for me.

		“One thing though, please—no unnecessary violence. The Oak abhors it.”

		You nod understandingly and take your leave; after all ‘unnecessary’ is open to interpretation…

		Athan provides with the details he has of the cult’s hideout, and that their rituals tend to take place at night. You find the chamber late in the afternoon and settle down to watch and wait.

		As midnight begins to approach, you begin to observe activity from your hiding place, followed by a good deal of ceremony centering around a hooded figure. You are sure you have just seen the cult leader, and experience tells you that if you can reach him, the cult should collapse. Taking out the cult leader probably doesn’t count as ‘unnecessary’ either, though you may keep quiet about any collateral damage.
		""";

	public override string ConclusionText =>
		"""
		There is little question that you underestimated the cultist. Whatever dark skills he possessed were impressive, just not quite impressive enough.

		As you strike the final blow, he screams in agony, before his cloak blows open and his body transforms into swirling purple smoke before you.

		Taken aback by this for a moment, you initially fail to notice that the cavern has somehow lightened and feels less oppressive. Also, from a small, roughly-hewn corridor you had not noticed before, there emerges a steady trail of disorientated and scruffy drifters and beggars, who seem to have come straight from the Sinking Market. You realize that they must have been the people that Athan was helping, and lead them out of the cavern.

		While shaking off the effects of the cultist’s indoctrination, they are still very dazed and compliant, and you have little trouble in herding the confused crowd back to Gloomhaven. You make a strange sight as you parade through Gloomhaven and Athan emerges from the Sanctuary just in time to see them.

		“By the light!” he cries, “You did it!” He sidles a little closer before saying quietly “No-one got hurt… too badly, I hope?”

		You assure him that the cultist was the only person killed. He looks at you sideways, but says nothing further for a moment.

		Still beaming at the sight of his flock, he cries out as if remembering something. “Oh! I have something to share with you!” ushering you back into his study. In your absence, he has examined both the Orb of Embers and The Book of Naiqa and has written extensive notes, much of which mean little to you.

		“As I told you, the twin Orbs are extremely powerful and great care must be used in their handling and operation. They will however, come in very useful if you come in contact with either the Icebound or the Lavalite.”

		“Once again, I must express my gratitude, you have made an enormous difference to those who are often overlooked by others. I cannot offer you much by way of thanks, but you may want to take a look at the old Imp Temple on the edge of the Dagger Forest. A good soul will find their reward there.

		Thanking the old man once more, you leave his office, quietly proud that you have won him over and softened the grumpy exterior. As you walk down the corridor however, you hear him bellow “Shut the door, you fools—my cuttings! And look at these dirty footprints! Weevils and bark beetles, the lot of you!”
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<CultLeader>(),
		ModelDB.Monster<DeepTerror>(),
		ModelDB.Monster<LivingSpirit>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainReputationReward(1),
		new GainProsperityReward(1),
		new GainRandomOrbEachReward(),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario022>()),
	];

	private bool _summonElite;
	private List<Objective> _altars = [];
	private int _currentAltarIndex = 0;
	private ScenarioRule _scenarioTeleportRule;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<CultLeader>(), specificCount: 1));

		int characterCount = GameController.Instance.SavedCampaign.Characters.Count;
		string summonInfo = characterCount == 3 ? """Every other Living Spirit summoned is elite.""" : characterCount == 4 ? """The Living Spirits summoned are elite.""" : "";

		AddScenarioRule(textParameters =>
			$"""
			 The Cult Leader is a special Cultist. It does not suffer damage when summoning. Instead of summoning Living Bones, the Cult leader summons Living Spirits. {summonInfo}
			 """);

		_scenarioTeleportRule = AddScenarioRule(textParameters =>
			$"""
			 If there is a Move ability listed on the Cultist ability card, it first starts its turn by {Icons.Inline(Icons.Teleport)} to the closest hex adjacent to an altar which is also closest to an enemy. The order in which it teleports is first the hex marked {Icons.InlineMarker(Marker.Type.a, textParameters)}, {Icons.InlineMarker(Marker.Type.b, textParameters)}, then {Icons.InlineMarker(Marker.Type.c, textParameters)}.

			 If an altar is destroyed the Cultist can no longer teleport near it and skips the teleport ability if it would otherwise teleport to the marked hex. When there is only one altar remaining, the Cultist no longer teleports.
			 """);

		_altars.Add(GameController.Instance.Map.GetMarker(Marker.Type.a).GetHexObject<Objective>());
		_altars.Add(GameController.Instance.Map.GetMarker(Marker.Type.b).GetHexObject<Objective>());
		_altars.Add(GameController.Instance.Map.GetMarker(Marker.Type.c).GetHexObject<Objective>());

		foreach(Objective altar in _altars)
		{
			altar.Init((GameController.Instance.SavedCampaign.Characters.Count + GameController.Instance.SavedScenario.ScenarioLevel) * 3,
				"Altar");
		}

		ScenarioEvents.FigureKilledEvent.Subscribe(this, 
			parameters =>
				parameters.Figure is Objective && _altars.Count(altar => altar.IsDestroyed) == 2,
			async parameters =>
			{
				ScenarioEvents.FigureKilledEvent.Unsubscribe(this);
				ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(this);

				_scenarioTeleportRule.Remove();
			}
		);

		ScenarioEvents.FigureTurnStartedEvent.Subscribe(this, 
			parameters => 
				parameters.Figure is Monster monster && 
				monster.MonsterModel is CultLeader && 
				monster.MonsterGroup.ActiveMonsterAbilityCard.Model.GetAbilities(monster).Any(monsterAbility => monsterAbility.Ability is MoveAbility),
			async parameters =>
			{
				if(!_altars[_currentAltarIndex].IsDestroyed)
				{
					ActionState actionState = new ActionState(parameters.Figure, [
						TeleportAbility.Builder()
							.WithCustomGetHexes((state, hexes) =>
							{
								// First find hexes around the altar that are closest to the Cultist
								int closestAltarHexRange = int.MaxValue;

								List<Hex> closestAltarHexes = [];

								foreach(Hex neighbourHex in _altars[_currentAltarIndex].Hex.Neighbours)
								{
									if(!neighbourHex.IsEmpty())
									{
										continue;
									}

									// Teleporting so calculating direct distance
									int range = Map.SimpleDistance(neighbourHex.Coords, parameters.Figure.Hex.Coords);

									if(range == closestAltarHexRange)
									{
										closestAltarHexes.Add(neighbourHex);
									}
									else if(range < closestAltarHexRange)
									{
										closestAltarHexRange = range;
										closestAltarHexes.Clear();
										closestAltarHexes.Add(neighbourHex);
									}
								}

								// From these hexes find those that are also closest to his enemy
								int closestEnemyRange = int.MaxValue;

								foreach(Hex altarHex in closestAltarHexes)
								{
									foreach(Figure figure in GameController.Instance.Map.Figures)
									{
										if(state.Performer.EnemiesWith(figure))
										{
											// Moving to the enemy after the teleport
											int range = RangeHelper.Distance(altarHex, figure.Hex);
											
											if(range == closestEnemyRange)
											{
												hexes.AddIfNew(altarHex);
											}
											else if(range < closestEnemyRange)
											{
												closestEnemyRange = range;
												hexes.Clear();
												hexes.Add(altarHex);
											}
										}
									}
								}
							})
							.Build()
					]);
					await actionState.Perform();
				}
				_currentAltarIndex++;
				_currentAltarIndex %= _altars.Count;
			}, order: 100

		);

		ScenarioEvents.AbilityStartedEvent.Subscribe(this,
			parameters => parameters.Performer is Monster monster && monster.MonsterModel is CultLeader,
			async parameters =>
			{
				switch (parameters.AbilityState)
				{
					case MonsterSummonAbility.State abilityState:
						abilityState.SetMonsterModel(ModelDB.Monster<LivingSpirit>());
						abilityState.SetMonsterType(CalculateMonsterType());
						_summonElite = !_summonElite;
						break;
					case SufferDamageAbility.State abilityState:
						if(abilityState.AbilityTarget == Target.Self)
						{
							abilityState.SetBlocked();
						}
						break;
				}
				
				await GDTask.CompletedTask;
			});
	}

	private MonsterType CalculateMonsterType()
	{
		int characterCount = GameController.Instance.SavedCampaign.Characters.Count;
		if(characterCount >= 4 || (characterCount >= 3 && _summonElite))
		{
			return MonsterType.Elite;
		}

		return MonsterType.Normal;
	}
}
