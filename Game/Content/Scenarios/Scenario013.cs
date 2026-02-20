using Fractural.Tasks;

public class Scenario013 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario013.tscn";
	public override int ScenarioNumber => 13;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();

	protected override ScenarioGoals CreateScenarioGoals() => new KillAllEnemiesScenarioGoals();

	public override string BGSPath => "res://Audio/BGS/Cave.ogg";

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		UpdateScenarioText($"All Living Corpses add {Icons.Inline(Icons.Targets)} 1 on all their attacks.");

		GameController.Instance.Map.Treasures[0].SetItemLoot(AbilityCmd.GetRandomAvailableStone());

		//TODO: Scenario effect
		// foreach(Character character in GameController.Instance.CharacterManager.Characters)
		// {
		// 	await AbilityCmd.AddCondition(null, character, Conditions.Poison1);
		// }

		ScenarioEvents.AbilityStartedEvent.Subscribe(this,
			parameters =>
				parameters.AbilityState is AttackAbility.State &&
				parameters.Performer is Monster monster &&
				monster.MonsterModel == ModelDB.Monster<LivingCorpse>(),
			async parameters =>
			{
				AttackAbility.State attackAbilityState = (AttackAbility.State)parameters.AbilityState;
				attackAbilityState.AdjustTargets(1);

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.AIMoveParametersCheckEvent.Subscribe(this,
			parameters =>
				parameters.Performer is Monster monster &&
				monster.MonsterModel == ModelDB.Monster<LivingCorpse>(),
			parameters =>
			{
				parameters.AdjustTargets(1);
			}
		);

		ScenarioCheckEvents.TargetsCheckEvent.Subscribe(this,
			parameters =>
				parameters.Figure is Monster monster &&
				monster.MonsterModel == ModelDB.Monster<LivingCorpse>(),
			parameters =>
			{
				parameters.AdjustTargets(1);
			}
		);
	}
}