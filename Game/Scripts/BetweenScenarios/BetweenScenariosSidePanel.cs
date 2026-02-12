using Godot;

public partial class BetweenScenariosSidePanel : Control
{
	[Export]
	private ResizingLabel _partyNameLabel;
	[Export]
	private Control _newCharacterButtonContainer;
	[Export]
	private BetterButton _newCharacterButton;

	[Export]
	private BetweenScenariosPartyStat _prosperityStat;
	[Export]
	private BetweenScenariosPartyStat _reputationStat;

	public override void _Ready()
	{
		base._Ready();

		this.DelayedCall(() =>
		{
			_partyNameLabel.SetText(BetweenScenariosController.Instance.SavedCampaign.PartyName);
		});

		_newCharacterButton.Pressed += OnNewCharacterPressed;

		BetweenScenariosController.Instance.SavedCampaign.CharactersChangedEvent += OnCharactersChanged;
		BetweenScenariosController.Instance.SavedCampaign.ProsperityChangedEvent += OnProsperityChanged;
		BetweenScenariosController.Instance.SavedCampaign.ReputationChangedEvent += OnReputationChanged;

		this.DelayedCall(() =>
		{
			OnProsperityChanged();
			OnReputationChanged();
		});
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		BetweenScenariosController.Instance.SavedCampaign.CharactersChangedEvent -= OnCharactersChanged;
		BetweenScenariosController.Instance.SavedCampaign.ProsperityChangedEvent -= OnProsperityChanged;
		BetweenScenariosController.Instance.SavedCampaign.ReputationChangedEvent -= OnReputationChanged;
	}

	private void OnNewCharacterPressed()
	{
		AppController.Instance.PopupManager.RequestPopup(new CreateCharacterPopup.Request()
		{
			SavedCampaign = BetweenScenariosController.Instance.SavedCampaign
		});
	}

	private void OnCharactersChanged()
	{
		_newCharacterButtonContainer.SetVisible(BetweenScenariosController.Instance.SavedCampaign.Characters.Count < 4);
	}

	private void OnProsperityChanged()
	{
		SavedCampaign savedCampaign = BetweenScenariosController.Instance.SavedCampaign;
		int prosperityLevel = savedCampaign.GetProsperityLevel();
		int oldThresholdProsperityAmount =
			SavedCampaign.ProsperityLevelThresholds[Mathf.Min(prosperityLevel - 1, SavedCampaign.ProsperityLevelThresholds.Length - 1)];
		int newThresholdProsperityAmount =
			SavedCampaign.ProsperityLevelThresholds[Mathf.Min(prosperityLevel, SavedCampaign.ProsperityLevelThresholds.Length - 1)];

		float normalizedProgress =
			oldThresholdProsperityAmount == newThresholdProsperityAmount
				? 1f
				: Mathf.InverseLerp(oldThresholdProsperityAmount, newThresholdProsperityAmount, savedCampaign.Prosperity);

		_prosperityStat.Update(normalizedProgress, prosperityLevel.ToString());
	}

	private void OnReputationChanged()
	{
		SavedCampaign savedCampaign = BetweenScenariosController.Instance.SavedCampaign;
		int thresholdIndex = savedCampaign.GetReputationThresholdIndex();
		int oldThresholdReputationAmount =
			SavedCampaign.ReputationPriceCostThresholds[Mathf.Clamp(thresholdIndex - 1, 0, SavedCampaign.ReputationPriceCostThresholds.Length - 1)];
		int newThresholdReputationAmount =
			SavedCampaign.ReputationPriceCostThresholds[Mathf.Min(thresholdIndex, SavedCampaign.ReputationPriceCostThresholds.Length - 1)];

		float normalizedProgress =
			oldThresholdReputationAmount == newThresholdReputationAmount
				? 1f
				: Mathf.InverseLerp(oldThresholdReputationAmount, newThresholdReputationAmount, savedCampaign.Reputation);

		_reputationStat.Update(normalizedProgress, (-savedCampaign.GetItemPriceChange()).ToString());
	}
}