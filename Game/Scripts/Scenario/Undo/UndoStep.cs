public abstract class UndoStep
{
	public virtual bool Silent => false;
	public abstract void Undo(SavedScenario savedScenario);
}