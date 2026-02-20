public class RedHexEnhancement : EnhancementModel<TargetedAbilityState>
{
	public override string TexturePath => Icons.RedAOEHex;
	public override int BaseCost => 200;

	protected override void _Enhance(TargetedAbilityState state, EnhancementMark enhancementMark)
	{
		if(enhancementMark is AOEHexMark aoeHexMark)
		{
			state.AbilityAddAOEHex(new AOEHex(aoeHexMark.LocalCoords, AOEHexType.Red));
		}
	}
}