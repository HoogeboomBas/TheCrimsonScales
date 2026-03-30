using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario021 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario021.tscn";
	public override int ScenarioNumber => 21;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario027>()];

	protected override ScenarioGoals CreateScenarioGoals() =>
		new KillSpecificEnemiesTypeGoals(ModelDB.Monster<Lavalite>(), "Kill the Lavalite to win this scenario.");

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 4).SetObtainLootFunction(async character =>
		{
			await AbilityCmd.GainGold(character, 15);
			AbilityCard selectedAbilityCard = await AbilityCmd.SelectAbilityCard(character, CardState.Lost,
				hintText: $"Select a lost card to {Icons.Inline(Icons.RecoverCard)}");
			if(selectedAbilityCard != null)
			{
				await AbilityCmd.ReturnToHand(selectedAbilityCard);
			}
		});
		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 36).SetObtainLootFunction(async character =>
		{
			await AbilityCmd.GainXP(character, 10);
			await AbilityCmd.AddCondition(null, character, Conditions.Invisible);
		});

		UpdateScenarioText("At the start of the scenario, nominate one character to carry the Frosted Crystal for the duration of the scenario");

		Figure frostedCrystalCharacter = await AbilityCmd.SelectFigure(GameController.Instance.CharacterManager.FirstAlive(), figures =>
		{
			figures.AddRange(GameController.Instance.CharacterManager.Characters);
		}, true, hintText: () => "Select a character to gain the Frosted Crystal");

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(this,
			parameters => parameters.Performer == frostedCrystalCharacter && parameters.AbilityState.Target is Monster monster &&
			              monster.MonsterModel is Lavalite,
			async parameters =>
			{
				parameters.AbilityState.SingleTargetAdjustAttackValue(1);
				await GDTask.CompletedTask;
			});

		if(GameController.Instance.SavedCampaign.Characters.Count >= 3)
		{
			ScenarioEvents.AbilityStartedEvent.Subscribe(this,
				parameters => parameters.Performer is Monster monster && monster.MonsterModel is Lavalite &&
				              parameters.AbilityState is MonsterSummonAbility.State summonAbilityState &&
				              (summonAbilityState.MonsterModel is EarthDemon || (summonAbilityState.MonsterModel is FlameDemon &&
				                                                                 GameController.Instance.SavedCampaign.Characters.Count >= 4)),
				async parameters =>
				{
					((MonsterSummonAbility.State)parameters.AbilityState).SetMonsterType(MonsterType.Elite);
					await GDTask.CompletedTask;
				});
		}
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		UpdateScenarioText($"""
		                    The elite Savvas Lavaflow is the Lavalite and immune to {Icons.Inline(Icons.GetCondition(Conditions.Stun))}, {Icons.Inline(Icons.GetCondition(Conditions.Disarm))}, and {Icons.Inline(Icons.GetCondition(Conditions.Muddle))}. Its hit point value is multiplied by C+1.
		                    For three characters, every Earth Demon summoned is elite. For four characters, all summons are elite. The character holding the Frosted Crystal adds +1 {Icons.Inline(Icons.Attack)} to all attacks targeting the Lavalite.
		                    """);
	}
}