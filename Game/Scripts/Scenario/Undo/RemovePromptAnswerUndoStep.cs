public class RemovePromptAnswerUndoStep(PromptAnswer promptAnswer) : UndoStep
{
	public override bool Silent => promptAnswer.ImmediateCompletion;

	public override void Undo(SavedScenario savedScenario)
	{
		savedScenario.PromptAnswers.RemoveLast();
	}
}