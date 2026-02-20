using Godot;

public partial class RetirementPopup : Popup<RetirementPopup.Request>
{
	public class Request : PopupRequest
	{
		public SavedCharacter Character { get; init; }
		public SavedCampaign SavedCampaign { get; init; }
		public ClassModel UnlockedClass { get; init; }
	}

	[Export]
	private TextureRect _portraitTexture;
	[Export]
	private PersonalQuestProgressView _personalQuestProgressView;
	[Export]
	private RichTextLabel _richTextLabel;
	[Export]
	private BetterButton _continueButton;

	protected override void OnOpen()
	{
		base.OnOpen();

		_portraitTexture.SetTexture(PopupRequest.Character.ClassModel.PortraitTexture);
		_personalQuestProgressView.Init(PopupRequest.Character);
		_richTextLabel.SetText($"{PopupRequest.Character.GetNameAndIcon(50)} retires from the mercenary lifestyle!");

		_continueButton.Pressed += OnContinuePressed;
	}

	protected override void OnClosed()
	{
		base.OnClosed();

		if(PopupRequest.UnlockedClass != null)
		{
			BetweenScenariosController.Instance?.UnlockOverlay.Open(PopupRequest.UnlockedClass);
		}
	}

	private void OnContinuePressed()
	{
		Close();
	}
}