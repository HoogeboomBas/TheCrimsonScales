using System.Collections.Generic;
using Godot;

public partial class GoldDistributionPopup : Popup<GoldDistributionPopup.Request>
{
	public class Request : PopupRequest
	{
		public int Gold { get; init; }
		public bool LoseGold { get; init; }
		public List<SavedCharacter> Characters { get; init; }
	}

	[Export]
	private Label _distributionAmountLabel;

	[Export]
	private PackedScene _characterScene;
	[Export]
	private Control _characterParent;

	[Export]
	private ChoiceButton _confirmButton;

	private readonly List<GoldDistributionPopupCharacter> _characters = new List<GoldDistributionPopupCharacter>();

	public override void _Ready()
	{
		base._Ready();

		_confirmButton.BetterButton.Pressed += OnConfirmPressed;
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		SetCanClose(false);

		// Create "fair" distribution
		int[] goldDistribution = new int[PopupRequest.Characters.Count];
		int goldRemaining = PopupRequest.Gold;
		while(goldRemaining > 0)
		{
			for(int i = 0; i < PopupRequest.Characters.Count && goldRemaining > 0; i++)
			{
				if(!PopupRequest.LoseGold || PopupRequest.Characters[i].Gold >= goldDistribution[i] + 1)
				{
					goldDistribution[i]++;
					goldRemaining--;
				}
			}
		}

		for(int i = 0; i < PopupRequest.Characters.Count; i++)
		{
			SavedCharacter character = PopupRequest.Characters[i];
			GoldDistributionPopupCharacter goldDistributionPopupCharacter = _characterScene.Instantiate<GoldDistributionPopupCharacter>();
			_characterParent.AddChild(goldDistributionPopupCharacter);
			goldDistributionPopupCharacter.Init(character, goldDistribution[i], PopupRequest.LoseGold);
			goldDistributionPopupCharacter.DistributionAmountChangedEvent += OnDistributionAmountChanged;
			_characters.Add(goldDistributionPopupCharacter);
		}

		UpdateVisuals();
	}

	protected override void OnClosed()
	{
		base.OnClosed();

		foreach(GoldDistributionPopupCharacter character in _characters)
		{
			character.QueueFree();
		}

		_characters.Clear();
	}

	private void UpdateVisuals()
	{
		int remainingAmount = PopupRequest.Gold;
		foreach(GoldDistributionPopupCharacter character in _characters)
		{
			remainingAmount -= character.DistributionAmount;
		}

		foreach(GoldDistributionPopupCharacter character in _characters)
		{
			character.UpdateRemainingGold(remainingAmount);
		}

		_distributionAmountLabel.SetText($"{remainingAmount}/{PopupRequest.Gold}");

		_confirmButton.SetActive(remainingAmount == 0);
	}

	private void OnDistributionAmountChanged(GoldDistributionPopupCharacter character)
	{
		UpdateVisuals();
	}

	private void OnConfirmPressed()
	{
		SetCanClose(true);

		foreach(GoldDistributionPopupCharacter character in _characters)
		{
			if(PopupRequest.LoseGold)
			{
				character.SavedCharacter.RemoveGold(character.DistributionAmount);
			}
			else
			{
				character.SavedCharacter.AddGold(character.DistributionAmount);
			}
		}

		Close();
	}
}