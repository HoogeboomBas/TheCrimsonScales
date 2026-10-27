using System;

public class AMDCardSelectionPrompt(EffectCollection effectCollection, Func<string> getHintText)
	: Prompt<AMDCardSelectionPrompt.Answer>(effectCollection, getHintText)
{
	public class Answer : PromptAnswer
	{
		public bool FirstTerminalSelected { get; init; }
	}

	protected override bool CanConfirm => false;
	protected override bool CanSkip => false;

	protected override void Enable()
	{
		base.Enable();

		GameController.Instance.AMDDrawView.FirstTerminalButton.SetEnabled(true, false);
		GameController.Instance.AMDDrawView.SecondTerminalButton.SetEnabled(true, false);

		GameController.Instance.AMDDrawView.FirstTerminalButton.Pressed += OnFirstTerminalPressed;
		GameController.Instance.AMDDrawView.SecondTerminalButton.Pressed += OnSecondTerminalPressed;
	}

	protected override void Disable()
	{
		base.Disable();

		GameController.Instance.AMDDrawView.FirstTerminalButton.SetEnabled(false, false);
		GameController.Instance.AMDDrawView.SecondTerminalButton.SetEnabled(false, false);

		GameController.Instance.AMDDrawView.FirstTerminalButton.Pressed -= OnFirstTerminalPressed;
		GameController.Instance.AMDDrawView.SecondTerminalButton.Pressed -= OnSecondTerminalPressed;
	}

	private void OnFirstTerminalPressed()
	{
		Complete(new Answer()
		{
			FirstTerminalSelected = true
		});
	}

	private void OnSecondTerminalPressed()
	{
		Complete(new Answer()
		{
			FirstTerminalSelected = false
		});
	}
}