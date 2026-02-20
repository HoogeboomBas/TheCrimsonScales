using Godot;

public partial class AbilityCardSideView : Control
{
	[Export]
	private AbilityCardView[] _abilityCardViews;

	[Export]
	private Control _topContainer;
	[Export]
	private Control _bottomContainer;

	public void SetCard(AbilityCardSide abilityCardSide)
	{
		foreach(AbilityCardView abilityCardView in _abilityCardViews)
		{
			abilityCardView.SetCard(abilityCardSide.AbilityCard.SavedAbilityCard);
		}

		bool showTop = abilityCardSide.AbilityCardSideType == AbilityCardSideType.Top;
		_topContainer.SetVisible(showTop);
		_bottomContainer.SetVisible(!showTop);

		CustomMinimumSize = showTop ? _topContainer.Size : _bottomContainer.Size;
	}
}