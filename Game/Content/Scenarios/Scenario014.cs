using Fractural.Tasks;

public class Scenario014 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario014.tscn";
	public override int ScenarioNumber => 14;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();

	protected override ScenarioGoals CreateScenarioGoals() =>
		new CustomScenarioGoals("Kill three Cultists to win this scenario.");

	public override string BGSPath => "res://Audio/BGS/Forest Day.ogg";

	private bool _firstDoorOpened;
	private int _cultistMurderCount = 0;

	public override async GDTask StartBeforeFirstRoomRevealed()
	{
		await base.StartBeforeFirstRoomRevealed();

		GameController.Instance.Map.Treasures[0].SetItemLoot(8, ModelDB.Item<DizzyingTincture>());
		GameController.Instance.Map.Treasures[1].SetItemLoot(9, ModelDB.Item<OrbOfFortune>());

		UpdateScenarioText("The door is locked.\nSomething will happen once all enemies in this room are killed.");

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => _cultistMurderCount >= 3,
			async parameters =>
			{
				await ((CustomScenarioGoals)ScenarioGoals).Win();
			}
		);

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				if(!_firstDoorOpened)
				{
					foreach(Figure figure in GameController.Instance.Map.Figures)
					{
						if(figure.Alignment == Alignment.Enemies)
						{
							return;
						}
					}

					Door firstDoor = GameController.Instance.Map.GetMarker(Marker.Type._1).Hex.GetHexObjectOfType<Door>();
					await firstDoor.Open();

					UpdateScenarioText("Whenever a cultist performs a summon ability, it summons a Living Corpse instead of a Living Bones.");

					_firstDoorOpened = true;
				}

				if(parameters.Figure is Monster monster && monster.MonsterModel == ModelDB.Monster<Cultist>())
				{
					_cultistMurderCount++;
				}
			}
		);

		ScenarioEvents.AbilityStartedEvent.Subscribe(this,
			parameters =>
				parameters.AbilityState is MonsterSummonAbility.State &&
				parameters.Performer is Monster monster &&
				monster.MonsterModel == ModelDB.Monster<Cultist>(),
			async parameters =>
			{
				MonsterSummonAbility.State monsterSummonAbilityState = (MonsterSummonAbility.State)parameters.AbilityState;
				monsterSummonAbilityState.SetMonsterModel(ModelDB.Monster<LivingCorpse>());

				await GDTask.CompletedTask;
			}
		);
	}
}