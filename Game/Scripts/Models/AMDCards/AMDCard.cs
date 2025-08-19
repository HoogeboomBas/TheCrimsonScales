using System;
using Fractural.Tasks;
using Godot;

public abstract class AMDCard : IDeckCard
{
	public virtual bool Reshuffles => false;
	public virtual bool Rolling => false;
	public virtual bool RemoveAfterDraw => false;
	public virtual bool IsCrit => false;
	public virtual bool IsNull => false;

	private readonly string _textureAtlasPath;
	private readonly int _atlasIndex;
	private readonly int _textureAtlasColumnCount;
	private readonly int _textureAtlasRowsCount;

	public event Action<AMDCard> DrawnEvent;

	protected AMDCard(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
	{
		_atlasIndex = atlasIndex;
		_textureAtlasPath = textureAtlasPath;
		_textureAtlasColumnCount = textureAtlasColumnCount;
		_textureAtlasRowsCount = textureAtlasRowsCount;
	}

	public async GDTask Apply(AttackAbility.State attackAbilityState)
	{
		int value = GetValue(attackAbilityState);
		ScenarioEvents.AMDCardDrawn.Parameters amdCardDrawnParameters =
			await ScenarioEvents.AMDCardDrawnEvent.CreatePrompt(
				new ScenarioEvents.AMDCardDrawn.Parameters(attackAbilityState, this, value), attackAbilityState);

		attackAbilityState.SingleTargetAdjustAttackValue(amdCardDrawnParameters.Value);
	}

	protected abstract int GetValue(AttackAbility.State attackAbilityState);

	public abstract (int, bool) GetScore(AttackAbility.State attackAbilityState);

	public Texture2D GetTexture()
	{
		return AtlasTextureHelper.CreateAtlasTexture(
			_atlasIndex, _textureAtlasColumnCount, _textureAtlasRowsCount,
			ResourceLoader.Load<Texture2D>(_textureAtlasPath));
	}

	public virtual void Drawn()
	{
		DrawnEvent?.Invoke(this);
	}
}