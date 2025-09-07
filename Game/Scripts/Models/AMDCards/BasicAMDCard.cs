public class BasicAMDCard : AMDCard
{
	public override int? Value => _value;

	private readonly int _value;

	public BasicAMDCard(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount, int value)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
		_value = value;
	}
}