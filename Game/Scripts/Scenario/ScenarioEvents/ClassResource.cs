public abstract class ClassResource
{
	//public abstract void Gain<T>(T parameters, int quantity);
	public abstract bool CheckAvailability(Figure figure, int quantity);
	public abstract bool TryConsume(Figure figure, int quantity);
	public abstract string GetIcon();
	public abstract string GetText(int quantity);
}