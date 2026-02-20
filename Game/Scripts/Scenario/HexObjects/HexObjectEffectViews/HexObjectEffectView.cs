public abstract partial class HexObjectEffectView<T> : HexObjectEffectViewBase
	where T : HexObjectEffectViewParameters
{
	public T ViewParameters { get; private set; }

	public sealed override void Init(HexObjectEffectViewParameters parameters)
	{
		base.Init(parameters);

		Init((T)parameters);
	}

	protected virtual void Init(T parameters)
	{
		ViewParameters = parameters;
	}
}