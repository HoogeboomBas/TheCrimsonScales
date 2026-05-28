using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario053 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario053.tscn";

	public override int ScenarioNumber => 53;
	public override string Name => "Cave of Currents";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SideScenarioChain>();

	public override string IntroductionText =>
		"""
		As you rest near a river not far off the road, you notice that the normally calm river has a far stronger current than normal. Fallen trees and even boulders of various sizes are being ripped from the bank and dragged with the current into a small cave entrance not far away. Although your instincts are telling you this is not a good idea, you decide to investigate what’s causing this.

		As you enter the cave the ground unexpectedly starts to tremble and shift under your feet, and you slip and stumble down further into the cave until finally, you come to a stop in a small cavern.

		Above you, there is a gap in the ceiling, which is letting in rays of sunbeams coming from a gap in the ceiling. It would be beautiful; however, the shafts of light also reveal that you are not alone...
		""";

	public override string ConclusionText =>
		"""
		You watch as the watery shell soaks up all the water in the room before collapsing in on itself, soaking up the creature you defeated and leaving behind a small floating watery orb that seems to always have the water surrounding it in motion. With the last drop of water gone you notice some leftovers from the monsters you have slain.

		You grab anything of value, as well as the strange orb and start to climb the fallen down rocks to the surface. As you dry off, you resolve to be a bit less nosy about strange water currents in the future.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<Lurker>(),
		ModelDB.Monster<SpittingDrake>(),
		ModelDB.Monster<WaterSpirit>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainCollectiveItemReward(ModelDB.Item<SphereOfCurrents>()),
		new GainGoldEachReward(5)
	];

	private ScenarioRule _somethingWillHappenRule;

	public override GDTask StartOfScenarioEffects(Character character)
	{
		return base.StartOfScenarioEffects(character);
		//TODO all characters gain muddle
	}

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());

		_somethingWillHappenRule = AddScenarioRule("Something will happen once all enemies in this room are killed.");

		//TODO: Scenario has not been implemented yet!
	}
}