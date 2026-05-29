using Fractural.Tasks;

public class Prohibitionist : TheCrimsonScalesBattleGoal
{
	public override string Title => "Prohibitionist";
	public override string Description => "Never use a potion.";

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.ItemUseStartedEvent.Subscribe(this,
			parameters => 
				parameters.Performer == character &&
					(parameters.Item == ModelDB.Item<MinorHealingPotion>() ||
					 parameters.Item == ModelDB.Item<MinorManaPotion>() ||
					 parameters.Item == ModelDB.Item<MinorPowerPotion>() ||
					 parameters.Item == ModelDB.Item<MinorStaminaPotion>() ||
					 parameters.Item == ModelDB.Item<MinorCurePotion>() ||
					 parameters.Item == ModelDB.Item<MajorHealingPotion>() ||
					 parameters.Item == ModelDB.Item<MajorManaPotion>() ||
					 parameters.Item == ModelDB.Item<MajorPowerPotion>() ||
					 parameters.Item == ModelDB.Item<MajorStaminaPotion>() ||
					 parameters.Item == ModelDB.Item<MajorCurePotion>() ||
					 parameters.Item == ModelDB.Item<SuperHealingPotion>() ||
					 parameters.Item == ModelDB.Item<IntoxicatingPotion>() ||
					 parameters.Item == ModelDB.Item<AlchemyPotion>()),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			});

		await GDTask.CompletedTask;
	}
}