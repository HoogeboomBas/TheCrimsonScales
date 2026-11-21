using Godot;

public partial class InspectPopupTooltip : Control
{
	[Export]
	private TooltipView _tooltipView;

	public void Init(Tooltip tooltip)
	{
		_tooltipView.Init(tooltip);
	}
}