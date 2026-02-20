using Godot;

public partial class AMDCardView : Control
{
	[Export]
	public TextureRect TextureRect;

	public AMDCardModel AMDCardModel { get; private set; }

	public void SetCard(AMDCardModel amdCardModel, AMDCardOwner owner)
	{
		AMDCardModel = amdCardModel;

		if(AMDCardModel == null)
		{
			TextureRect.SetVisible(false);
			return;
		}

		TextureRect.SetVisible(true);
		TextureRect.SetTexture(AMDCardModel.GetTexture(owner));
	}
}