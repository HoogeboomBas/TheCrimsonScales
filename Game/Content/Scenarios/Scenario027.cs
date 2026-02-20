using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario027 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario027.tscn";
	public override int ScenarioNumber => 27;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario028>()];

	protected override ScenarioGoals CreateScenarioGoals() =>
		new KillSpecificEnemiesTypeGoals(ModelDB.Monster<Icebound>(), "Kill the Icebound to win this scenario.");

	private Door _door2;

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		//Scenario Effect

		_door2 = GameController.Instance.Map.GetMarker(Marker.Type._2).GetHexObject<Door>();

		Figure orbOfEmbersCharacter = await AbilityCmd.SelectFigure(GameController.Instance.CharacterManager.FirstAlive(), figures =>
		{
			figures.AddRange(GameController.Instance.CharacterManager.Characters);
		}, true, hintText: () => "Select a character to hold the Orb of Embers");

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(this,
			parameters => parameters.Performer == orbOfEmbersCharacter && parameters.AbilityState.Target is Monster monster &&
			              monster.MonsterModel is Icebound,
			async parameters =>
			{
				parameters.AbilityState.SingleTargetAdjustAttackValue(1);
				await GDTask.CompletedTask;
			});

		UpdateScenarioText($"""
		                    At the beginning of the scenario, nominate one character to hold the Orb of Embers. The Orb of Embers cannot be transferred to another character, and it becomes inactive if the nominated character becomes exhausted.

		                    Door {Icons.InlineMarker(Marker.Type._2)} is locked and cannot be opened until instructed.
		                    """);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(parameters.Room == GameController.Instance.Map.Rooms[1])
		{
			await _door2.Unlock();
			await _door2.Open(parameters.PotentialOpener);
			UpdateScenarioText($"""
			                    Door {Icons.InlineMarker(Marker.Type._2)} becomes immediately open.

			                    The character holding the Orb of Embers adds +1{Icons.Inline(Icons.Attack)} to all attacks targeting the Icebound.

			                    The Savvas Icestorm is the Icebound. The Icebound uses the Boss monster ability deck, performing the following specials:

			                    Special 1: If the Icebound is occupying Room 1 (A2b tile), summon one Wind Demon in an empty hex closest to the hex marked {Icons.InlineMarker(Marker.Type.c)}. If the Icebound is occupying Room 2 (A3a tile), summon one Frost Demon in an empty hex closest to the hex marked {Icons.InlineMarker(Marker.Type.a)}. If the Icebound is occupying the G2a tile, summon one Stone Golem in an empty hex closest to the hex marked {Icons.InlineMarker(Marker.Type.b)}. Summons are normal for two characters, every other summon is elite for three characters, and all summons are elite for four characters.
			                    The Icebound then performs ”{Icons.Inline(Icons.Heal)}3, Self”.

			                    Special 2: {Icons.Inline(Icons.Attack)}+0, {Icons.Inline(Icons.Targets)} all, {Icons.Inline(Icons.Range)}3.
			                    If the Icebound is occupying Room 1 (A2b tile), it immediately jumps into the nearest unoccupied hex adjacent to {Icons.InlineMarker(Marker.Type.e)}. If the Icebound is occupying Room 2 (A3a tile), it immediately jumps into the nearest unoccupied hex adjacent to {Icons.InlineMarker(Marker.Type.d)}.
			                    {Icons.Inline(Icons.Attack)}+0, {Icons.Inline(Icons.Targets)} all, {Icons.Inline(Icons.Range)}3
			                    """);
		}
	}
}