using System.Collections.Generic;
using Fractural.Tasks;

public abstract class ScenarioModel : AbstractModel<ScenarioModel>, IEventSubscriber
{
	public ScenarioGoals ScenarioGoals { get; private set; }

	public abstract string ScenePath { get; }
	public abstract int ScenarioNumber { get; }
	public abstract ScenarioChain ScenarioChain { get; }
	public virtual IEnumerable<ScenarioConnection> Connections { get; } = [];
	public virtual int[] TreasureNumbers { get; } = [];

	protected virtual IEnumerable<ScenarioRequirement> ScenarioRequirements { get; } = [];

	public virtual string BGMPath => "res://Audio/BGM/Floral-Woods.ogg";
	public virtual string BGSPath => null;

	public virtual async GDTask StartBeforeFirstRoomRevealed()
	{
		ScenarioGoals = CreateScenarioGoals();
		UpdateScenarioText(null);

		await GDTask.CompletedTask;
	}

	public virtual async GDTask StartAfterFirstRoomRevealed()
	{
		ScenarioGoals.Start();

		ScenarioEvents.RoomRevealedEvent.Subscribe(this, parameters => true, OnRoomRevealed);

		await GDTask.CompletedTask;
	}

	protected virtual async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await GDTask.CompletedTask;
	}

	protected abstract ScenarioGoals CreateScenarioGoals();

	protected virtual void UpdateScenarioText(string text)
	{
		string displayText;
		if(text != null)
		{
			displayText = $"{ScenarioGoals.Text}\n\n{text}";
		}
		else
		{
			displayText = ScenarioGoals.Text;
		}

		GameController.Instance.SpecialRulesView.SetText(displayText);
	}
}