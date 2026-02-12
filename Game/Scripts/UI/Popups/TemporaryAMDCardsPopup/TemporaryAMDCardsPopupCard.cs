using Godot;

public partial class TemporaryAMDCardsPopupCard : Control
{
	[Export]
	private AMDCardView _amdCardView;

	public void Init(AMDCardModel amdCardModel, AMDCardOwner owner)
	{
		_amdCardView.SetCard(amdCardModel, owner);
	}
}