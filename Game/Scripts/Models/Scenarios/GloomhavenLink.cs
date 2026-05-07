public class GloomhavenLink : ScenarioLink
{
	public static readonly GloomhavenLink Instance = new GloomhavenLink();

	protected GloomhavenLink()
		: base(null)
	{
	}

	public override bool ToGloomhaven => false;
}