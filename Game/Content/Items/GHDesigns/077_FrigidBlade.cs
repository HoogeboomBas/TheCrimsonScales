public class FrigidBlade : ElementalBlade
{
	public override string Name => "Frigid Blade";
	public override int ItemNumber => 77;

	protected override int AtlasIndex => 6;

	protected override Element Element => Element.Ice;
}