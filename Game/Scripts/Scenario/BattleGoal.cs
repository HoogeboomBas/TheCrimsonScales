using System;
using Fractural.Tasks;
using Godot;

public class BattleGoal
{
	public static readonly Color CompletedColor = Color.FromHtml("548d57");
	public static readonly Color FailedColor = Color.FromHtml("d0483c");

	private readonly Character _character;

	public BattleGoalModel Model { get; }

	public int Progress { get; private set; }

	public float NormalizedProgress => (float)Progress / Model.MaxProgress;
	public bool ProgressFull => Progress >= Model.MaxProgress;
	public bool GivesCheckmark => ProgressFull != Model.FailIfProgressFull;

	public event Action<BattleGoal> ProgressChangedEvent;

	public BattleGoal(Character character, BattleGoalModel model)
	{
		_character = character;
		Model = model;
		Progress = 0;
	}

	public void AdjustProgress(int value)
	{
		bool previouslyFullProgress = ProgressFull;

		Progress += value;

		if(_character.IsLocal && !previouslyFullProgress && GameController.Instance != null && !GameController.FastForward)
		{
			AppController.Instance.BattleGoalProgressUpdateView.AddItem(this);
		}

		ProgressChangedEvent?.Invoke(this);
	}

	public async GDTask OnScenarioSetupPhaseCompleted()
	{
		await Model.OnScenarioSetupPhaseCompleted(_character, this);
	}
}