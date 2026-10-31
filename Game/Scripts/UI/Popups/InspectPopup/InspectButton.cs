using Godot;

public partial class InspectButton : Button
{
	[Export]
	private AbilityCardView _abilityCardView;

	public override void _Ready()
	{
		base._Ready();

		Pressed += OnPressed;
	}

	private void OnPressed()
	{
		if(_abilityCardView == null || (_abilityCardView.SavedAbilityCard == null && _abilityCardView.Model == null))
		{
			return;
		}

		if(AppController.Instance.PopupManager.IsPopupOpen<InspectPopup.Request>())
		{
			return;
		}

		AppController.Instance.PopupManager.OpenPopupOnTop(new InspectPopup.Request()
		{
			SavedAbilityCard = _abilityCardView.SavedAbilityCard,
			Model = _abilityCardView.Model,
		});
	}
}