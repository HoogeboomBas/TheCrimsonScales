using Godot;

public abstract class EnhancementModel<TState> : EnhancementModel
	where TState : class
{
	public override void Enhance(AbilityState abilityState, EnhancementMark enhancementMark)
	{
		if(abilityState is TState castState)
		{
			_Enhance(castState, enhancementMark);
		}
	}

	protected abstract void _Enhance(TState state, EnhancementMark enhancementMark);
}

public abstract class EnhancementModel : AbstractModel
{
	public abstract string TexturePath { get; }
	public abstract int BaseCost { get; }

	public virtual bool DefaultDoubleCostOnDoubleTarget => false;
	public virtual bool DefaultTripleCostOnPersistent => false;

	public Texture2D GetTexture()
	{
		return ResourceLoader.Load<Texture2D>(TexturePath);
	}

	public abstract void Enhance(AbilityState abilityState, EnhancementMark enhancementMark);
}