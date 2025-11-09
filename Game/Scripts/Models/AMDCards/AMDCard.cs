using System;
using Fractural.Tasks;
using Godot;
using System.Collections.Generic;

public abstract class AMDCard : IDeckCard
{
	public virtual bool Reshuffles => false;
	public virtual bool RemoveAfterDraw => false;

	public virtual bool Rolling(AttackAbility.State state) => false;

	public virtual AMDCardType Type => AMDCardType.Value;
	public virtual int? Value(AttackAbility.State state) => null;
	public virtual int? Pierce => null;
	public virtual int? Push => null;
	public virtual int? Pull => null;
	public virtual int? Swing => null;
	public virtual bool IgnoreRetaliate => false;

	public virtual List<Element> Elements => [];
	public virtual List<ConditionModel> ConditionModels(AttackAbility.State state) => [];
	public virtual List<Ability> Abilities => [];

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
				
		return new AMDCardValue(Rolling(attackAbilityState), amdCardDrawnParameters.Type, amdCardDrawnParameters.Value, Pierce, Push, Pull, Swing, IgnoreRetaliate, Elements, ConditionModels(attackAbilityState), Abilities);
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