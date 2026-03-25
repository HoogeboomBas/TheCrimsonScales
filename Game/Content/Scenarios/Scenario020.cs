using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario020 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario020.tscn";
	public override int ScenarioNumber => 20;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario022>()];

	protected override ScenarioGoals CreateScenarioGoals() =>
		new KillSpecificEnemiesTypeGoals(ModelDB.Monster<CultLeader>(), "Kill the Cult Leader to win the scenario");

	private bool _summonElite;
	private List<Objective> _altars = [];

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
			parameters => parameters.Performer is Monster monster && monster.MonsterModel is CultLeader &&
			              parameters.AbilityState is MonsterSummonAbility.State,
			async parameters =>
			{
				MonsterSummonAbility.State abilityState = (MonsterSummonAbility.State)parameters.AbilityState;
				abilityState.SetMonsterModel(ModelDB.Monster<LivingSpirit>());
				abilityState.SetMonsterType(CalculateMonsterType());
				_summonElite = !_summonElite;
				await GDTask.CompletedTask;
			});

		object subscriber = new object();
		ScenarioEvents.AbilityStartedEvent.Subscribe(this, subscriber,
			parameters => parameters.Performer is Monster monster && monster.MonsterModel is CultLeader &&
			              parameters.AbilityState is MoveAbility.State,
			async parameters =>
			{
				//TODO Teleport ability
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