using System;
using Godot;

public class Mastery
{
	public static readonly Color CompletedColor = Color.FromHtml("548d57");
	public static readonly Color FailedColor = Color.FromHtml("d0483c");

	public int Progress { get; private set; }
	public bool TemporarilyFailed { get; private set; }
	public bool Completed { get; set; }
	public MasteryModel Model { get; }

	public float NormalizedProgress => (float)Progress / Model.MaxProgress;
	public bool ProgressFull => Progress >= Model.MaxProgress;

	public event Action<Mastery> ProgressChangedEvent;

	public Mastery(MasteryModel model)
	{
		Model = model;
	}

	public void AdjustProgress(int num = 1)
	{
		SetProgress(Progress + num);
	}

	public void SetProgress(int num)
	{
		if(!TemporarilyFailed)
		{
			Progress = Math.Max(Model.MaxProgress, num);

			ProgressChangedEvent?.Invoke(this);
		}
	}

	public void ResetProgress()
	{
		SetProgress(0);
	}

	public void SetTemporarilyFailed(bool failed)
	{
		TemporarilyFailed = failed;
		ProgressChangedEvent?.Invoke(this);
	}

	public void Complete(SavedCharacter savedCharacter)
	{
		if(!Completed)
		{
			Completed = true;
			savedCharacter.AddAvailablePerk();
		}
	}

	public void OnScenarioCompleted(SavedCharacter savedCharacter, ScenarioResult scenarioResult)
	{
		if(scenarioResult is ScenarioResult.Win && !TemporarilyFailed && ProgressFull)
		{
			Complete(savedCharacter);
		}

		if(Model.SingleScenario)
		{
			ResetProgress();
			SetTemporarilyFailed(false);
		}
	}
}