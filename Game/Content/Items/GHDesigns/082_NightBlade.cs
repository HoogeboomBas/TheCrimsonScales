public class NightBlade : ElementalBlade
{
	public override string Name => "Night Blade";
	public override int ItemNumber => 82;

	protected override int AtlasIndex => 11;

	protected override Element Element => Element.Dark;
}