using System;

public class ConfirmPrompt(EffectCollection effectCollection, Func<string> getHintText, bool mandatory = false)
	: Prompt<ConfirmPrompt.Answer>(effectCollection, getHintText)
{
	public class Answer : PromptAnswer
	{
		public bool Confirmed { get; init; }
	}

	protected override bool CanSkip => !mandatory;

	protected override void Enable()
	{
		base.Enable();

		if(_authority is not Character)
		{
			Complete(true);
		}
	}

	protected override Answer CreateAnswer()
	{
		return new Answer()
		{
			Confirmed = true
		};
	}
}