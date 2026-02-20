public partial class InfoExtraEffect<T> : InfoExtraEffectBase
	where T : InfoExtraEffectParameters
{
	public sealed override void Init(InfoExtraEffectParameters parameters)
	{
		Init((T)parameters);
	}

	protected virtual void Init(T parameters)
	{
	}
}