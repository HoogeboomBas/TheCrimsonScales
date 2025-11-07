using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario006 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario006.tscn";
	public override int ScenarioNumber => 6;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<InfectiousScenarioChain>();

	protected override ScenarioGoals CreateScenarioGoals() => new CustomScenarioGoals($"Purify the poisoned water supply to win this scenario. Place {GameController.Instance.CharacterManager.Characters.Count} bottles of antidote in the fountain.");

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		UpdateScenarioText(
			"The crate and cabinet obstacles contain the bottles of antidote and cannot be destroyed." +
			"Any character may sacrifice the top or bottom action of their turn while adjacent to an antidote to it pick up." + 
			"Any character may sacrifice the top or bottom action of their turn while adjacent to the fountain to place the antidote in the fountain." +
			"Each character may only hold one antidote at a time, and if a character exhausts while holding an antidote, the scenario is immediately lost.");

		Dictionary<Figure, bool> characterHasAntidote = [];
		int antidoteBottlesPicked = 0;
		int antidoteBottlesPlaced = 0;

		foreach(Figure character in GameController.Instance.CharacterManager.Characters)
		{
			characterHasAntidote.Add(character, false);
		}

		//TODO: Scenario effect
		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			await AbilityCmd.AddCondition(null, character, Conditions.Poison1);
		}

		// Allow picking up the antidote
		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
			parameters => !parameters.ForgoneAction && 
			RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Where(hex => hex.HasHexObjectOfType<Obstacle>() && !characterHasAntidote[parameters.Performer],
			async parameters =>
			{
				parameters.ForgoAction();
				characterHasAntidote[parameters.Performer] = true;
				// OBSTACLE.DESTROY(FORCED)
				antidoteBottlesPicked++;

				TriggerMonsterSpawn(antidoteBottlesPicked);
			},
			EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.StartHexMove),
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Pick up a bottle of antidote")
		);

		// Allow placing the antidote into the fountain
		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
			parameters => !parameters.ForgoneAction && 
			RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Where(hex => hex.HasHexObjectOfType<Fountain>() && characterHasAntidote[parameters.Performer],
			async parameters =>
			{
				parameters.ForgoAction();
				characterHasAntidote[parameters.Performer] = false;
				antidoteBottlesPlaced++;

				if(antidoteBottlesPlaced == GameController.Instance.CharacterManager.Characters.Count)
				{
					ScenarioGoals.Win();
				}
			},
			EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.StartHexMove),
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Place the bottle of antidote in the fountain")
		);

		// If a character exhausts while holding an antidote, the scenario is immediately lost
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => characterHasAntidote[parameters.Figure],
			async parameters =>
			{
				await AbilityCmd.Lose();
			}
		);
	}

	private async GDTask TriggerMonsterSpawn(int antidoteBottlesPicked)
	{
		switch(antidoteBottlesPicked)
		{
			case 1: // 6G
			{
				// elite Blood Ooze closest to A
				// normal Water Spirit closest to B
				break;
			}
			case 2: // 6D
			{
				// normal Flaming Drake closest to A
				// normal Flaming Drake closest to B
				break;
			}
			case 3: // 6F
			{
				// Normal and Elite toxic imp B
				// Normal and Elite toxic imp B
				break;
			}
			case 4: // 6E
			{
				// Elite Water Spirit A
				// Elite Water Spirit B
				break;
			}
		}
	}
}
