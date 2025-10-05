public class PromptAnswer
{
	public bool Skipped { get; init; }
	public bool ImmediateCompletion { get; set; }
	public int SelectedEffectIndex { get; init; } = -1;
	public SyncedAction SyncedAction { get; init; } = null;
}