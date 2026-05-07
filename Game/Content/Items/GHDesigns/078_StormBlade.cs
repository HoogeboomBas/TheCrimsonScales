public class StormBlade : ElementalBlade
{
	public override string Name => "Storm Blade";
	public override int ItemNumber => 78;

	protected override int AtlasIndex => 7;

	protected override Element Element => Element.Air;
}