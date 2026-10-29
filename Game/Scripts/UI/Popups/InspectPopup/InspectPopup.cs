using System.Collections.Generic;
using Godot;

public partial class InspectPopup : Popup<InspectPopup.Request>
{
	public class Request : PopupRequest
	{
		public AbilityCardModel Model { get; init; }
		public SavedAbilityCard SavedAbilityCard { get; init; }
	}

	[Export]
	private AbilityCardView _abilityCardView;

	[Export]
	private PackedScene _tooltipScene;
	[Export]
	private Control _tooltipParent;

	private readonly List<InspectPopupTooltip> _tooltips = new List<InspectPopupTooltip>();

	protected override void OnOpen()
	{
		base.OnOpen();

		if(PopupRequest.SavedAbilityCard != null)
		{
			_abilityCardView.SetCard(PopupRequest.SavedAbilityCard);
		}
		else
		{
			_abilityCardView.SetCard(PopupRequest.Model);
		}

		AbilityCardModel model = PopupRequest.Model ?? PopupRequest.SavedAbilityCard.Model;

		TooltipCollection tooltips = new TooltipCollection();

		model.Top.GetTooltips(tooltips);
		model.Bottom.GetTooltips(tooltips);

		foreach(Tooltip tooltip in tooltips.Tooltips)
		{
			InspectPopupTooltip inspectPopupTooltip = _tooltipScene.Instantiate<InspectPopupTooltip>();
			_tooltipParent.AddChild(inspectPopupTooltip);
			inspectPopupTooltip.Init(tooltip);
			_tooltips.Add(inspectPopupTooltip);
		}
	}

	protected override void OnClosed()
	{
		base.OnClosed();

		foreach(InspectPopupTooltip tooltip in _tooltips)
		{
			tooltip.QueueFree();
		}

		_tooltips.Clear();
	}
}