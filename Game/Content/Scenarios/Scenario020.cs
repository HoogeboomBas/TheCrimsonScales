using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario020 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario020.tscn";

	public override int ScenarioNumber => 20;
	public override string Name => "Midnight Ritual";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario022>()];

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
		ModelDB.Monster<Cultist>(),
		ModelDB.Monster<DeepTerror>(),
		ModelDB.Monster<LivingSpirit>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainReputationReward(),
		new GainProsperityReward(),
		new GainRandomOrbEachReward(),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario022>()),
	];

	protected override ScenarioGoals CreateScenarioGoals() =>
		new KillSpecificEnemiesTypeGoals(ModelDB.Monster<CultLeader>(), "Kill the Cult Leader to win the scenario");

	private bool _summonElite;
	private List<Objective> _altars = [];
	private int _currentAltarIndex = 0;

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		_altars.Add(GameController.Instance.Map.GetMarker(Marker.Type.a).GetHexObject<Objective>());
		_altars.Add(GameController.Instance.Map.GetMarker(Marker.Type.b).GetHexObject<Objective>());
		_altars.Add(GameController.Instance.Map.GetMarker(Marker.Type.c).GetHexObject<Objective>());

		foreach(Objective altar in _altars)
		{
			altar.Init((GameController.Instance.SavedCampaign.Characters.Count + GameController.Instance.SavedScenario.ScenarioLevel) * 3,
				"Altar");
		}

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
					case OtherAbility.State abilityState:
						abilityState.SetBlocked();
						break;
					case MoveAbility.State abilityState:
						if(!_altars[_currentAltarIndex].IsDestroyed)
						{
							abilityState.SetBlocked();
							/*
							ActionState actionState = new ActionState(abilityState.ActionState, abilityState.Performer, [
								TeleportAbility.Builder()
									.WithDistance(999)
									.With
									.WithGetTargetingHintText(grantAbilityState =>
										$"Select an ally to grant {Icons.HintText(Icons.Attack)}3, {Icons.HintText(Icons.Range)}3"
									)
									.Build()
							]);
							await actionState.Perform();
							
							List<Hex> selectedHexes = await AbilityCmd.SelectHexes(abilityState,
								list =>
								{
									foreach(Hex possibleHex in RangeHelper.GetHexesInRange(abilityState.Performer.Hex, 3, true))
									{
										if(possibleHex != null && possibleHex.IsFeatureless())
										{
											list.Add(possibleHex);
										}
									}
								},
								0, 1, false, "Place difficult terrain in a featureless hex"
							);
							*/
							//TODO Teleport ability
						}

						_currentAltarIndex++;
						_currentAltarIndex %= _altars.Count;
						break;
				}
				
				await GDTask.CompletedTask;
			});

		UpdateScenarioText($"""
		                    The Cultist is the Cult Leader. It does not suffer damage when summoning. Instead of summoning Living Bones, the Cultist summons Living Spirits. The cultist is immune to {Icons.Inline(Icons.GetCondition(Conditions.Stun))}, {Icons.Inline(Icons.GetCondition(Conditions.Disarm))}, and {Icons.Inline(Icons.GetCondition(Conditions.Immobilize))}. For three characters, every other Living Spirit summoned is elite. For four characters, every Living Spirit summoned is elite.
		                    If there is a Move ability listed on the Cultist ability card, it first starts its turn by {Icons.Inline(Icons.Teleport)} to the closest hex adjacent to an altar marked hex which is also closest to an enemy. The order in which it teleports is first the hex marked {Icons.InlineMarker(Marker.Type.a)}, {Icons.InlineMarker(Marker.Type.b)}, then {Icons.InlineMarker(Marker.Type.c)} in that order.

		                    The altars have (C+L)x3 hit points, and if an altar is destroyed the Cultist can no longer teleport near it and skips the teleport ability if it would otherwise teleport to the marked hex. When there is only one altar remaining, the Cultist no longer teleports.
		                    """);
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
