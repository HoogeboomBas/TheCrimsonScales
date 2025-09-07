using System;
using Fractural.Tasks;
using Godot;

public abstract class AMDCard : IDeckCard
{
	public virtual bool Reshuffles => false;
	public virtual bool Rolling => false;
	public virtual bool RemoveAfterDraw => false;
	public virtual AMDCardType Type => AMDCardType.Value;
	public virtual int? Value => 0;

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

	public async GDTask<AMDCardValue> Draw(AttackAbility.State attackAbilityState)
	{
		ScenarioEvents.AMDCardDrawn.Parameters amdCardDrawnParameters =
			await ScenarioEvents.AMDCardDrawnEvent.CreatePrompt(
				new ScenarioEvents.AMDCardDrawn.Parameters(attackAbilityState, this));
				
		return new(amdCardDrawnParameters.Type, amdCardDrawnParameters.Value);
	}

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