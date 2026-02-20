using System;
using Godot;

public partial class GoldDistributionPopupCharacter : Control
{
	[Export]
	private ClassView _classView;
	[Export]
	private Label _currentGoldLabel;

	[Export]
	private Label _distributionAmountLabel;

	[Export]
	private GoldDistributionPopupButton _increaseButton;
	[Export]
	private GoldDistributionPopupButton _increaseMoreButton;
	[Export]
	private GoldDistributionPopupButton _decreaseButton;
	[Export]
	private GoldDistributionPopupButton _decreaseMoreButton;

	private bool _loseGold;

	public SavedCharacter SavedCharacter { get; private set; }
	public int DistributionAmount { get; private set; }

	public event Action<GoldDistributionPopupCharacter> DistributionAmountChangedEvent;

	public override void _Ready()
	{
		base._Ready();

		_increaseButton.Button.Pressed += OnIncreasePressed;
		_increaseMoreButton.Button.Pressed += OnIncreaseMorePressed;
		_decreaseButton.Button.Pressed += OnDecreasePressed;
		_decreaseMoreButton.Button.Pressed += OnDecreaseMorePressed;
	}

	public void Init(SavedCharacter savedCharacter, int distributionAmount, bool loseGold)
	{
		_loseGold = loseGold;
		SavedCharacter = savedCharacter;

		_classView.Init(savedCharacter.ClassModel);

		UpdateDistributionAmount(distributionAmount);
	}

	public void UpdateRemainingGold(int remainingAmount)
	{
		if(_loseGold)
		{
			int remainingCharacterGold = SavedCharacter.Gold - DistributionAmount;
			_increaseButton.SetActive(remainingAmount >= 1 && remainingCharacterGold >= 1);
			_increaseMoreButton.SetActive(remainingAmount >= 5 && remainingCharacterGold >= 5);
		}
		else
		{
			_increaseButton.SetActive(remainingAmount >= 1);
			_increaseMoreButton.SetActive(remainingAmount >= 5);
		}

		_decreaseButton.SetActive(DistributionAmount >= 1);
		_decreaseMoreButton.SetActive(DistributionAmount >= 5);
	}

	private void UpdateDistributionAmount(int changeAmount)
	{
		DistributionAmount += changeAmount;
		_distributionAmountLabel.SetText(DistributionAmount.ToString());
		if(_loseGold)
		{
			_currentGoldLabel.SetText((SavedCharacter.Gold - DistributionAmount).ToString());
		}
		else
		{
			_currentGoldLabel.SetText((SavedCharacter.Gold + DistributionAmount).ToString());
		}

		DistributionAmountChangedEvent?.Invoke(this);
	}

	private void OnIncreasePressed()
	{
		UpdateDistributionAmount(1);
	}

	private void OnIncreaseMorePressed()
	{
		UpdateDistributionAmount(5);
	}

	private void OnDecreasePressed()
	{
		UpdateDistributionAmount(-1);
	}

	private void OnDecreaseMorePressed()
	{
		UpdateDistributionAmount(-5);
	}
}