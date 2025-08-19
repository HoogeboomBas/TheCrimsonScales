public class BasicAMDCard : AMDCard
{
	private readonly int _value;

	public BasicAMDCard(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount, int value)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
		_value = value;
	}
	
	public BasicAMDCard(AMDCard card, int value)
		: base(card)
	{
		_value = value;
	}

	protected override int GetValue(AttackAbility.State attackAbilityState)
	{
		return _value;
	}

	public override (int, bool) GetScore(AttackAbility.State attackAbilityState)
	{
		return (_value, false);
	}
}