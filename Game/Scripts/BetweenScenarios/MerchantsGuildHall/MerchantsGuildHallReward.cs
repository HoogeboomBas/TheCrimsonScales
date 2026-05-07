using System.Collections.Generic;
using System.Threading;
using Fractural.Tasks;
using Godot;

public partial class MerchantsGuildHallReward : Control
{
	[Export]
	private BetterButton _button;
	[Export]
	private TextureRect _checkmark;
	[Export]
	private RichTextLabel _description;

	private SavedMerchantsGuildHallReward _savedReward;

	public bool CanUnlock =>
		BetweenScenariosController.Instance.SavedCampaign.SavedMerchantsGuildHall.CompletedScenarioCount >= 5 &&
		!_savedReward.Unlocked;

	public override void _Ready()
	{
		base._Ready();

		_button.Pressed += OnPressed;
	}

	public void Init(SavedMerchantsGuildHallReward savedReward)
	{
		_savedReward = savedReward;

		RichTextParameters parameters = _description.GetRichTextParameters();
		_description.SetText(savedReward.Model.GetDescription(parameters));

		_savedReward.UnlockedEvent += OnUnlocked;

		BetweenScenariosController.Instance.SavedCampaign.SavedMerchantsGuildHall.CompletedScenarioCountChanged += OnCompletedScenarioCountChanged;

		UpdateVisuals();
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		if(_savedReward != null)
		{
			_savedReward.UnlockedEvent -= OnUnlocked;
		}

		if(BetweenScenariosController.Instance != null)
		{
			BetweenScenariosController.Instance.SavedCampaign.SavedMerchantsGuildHall.CompletedScenarioCountChanged -=
				OnCompletedScenarioCountChanged;
		}
	}

	private void UpdateVisuals()
	{
		_checkmark.SetVisible(_savedReward.Unlocked);

		_button.SetEnabled(CanUnlock, false);
	}

	private async GDTaskVoid GiveRewards()
	{
		if(!CanUnlock)
		{
			return;
		}

		CancellationToken cancellationToken = BetweenScenariosController.Instance.DestroyCancellationToken;

		List<SavedReward> rewards = _savedReward.Model.GetRewards();
		await AppController.Instance.GiveRewards(BetweenScenariosController.Instance.SavedCampaign, rewards, cancellationToken: cancellationToken);

		_savedReward.SetUnlocked();
		BetweenScenariosController.Instance.SavedCampaign.SavedMerchantsGuildHall.RemoveFiveCompletedScenarios();

		AppController.Instance.SaveGame();
	}

	private void OnPressed()
	{
		AppController.Instance.PopupManager.OpenPopupOnTop(new TextPopup.Request("Donation",
			$"Unlock this reward?",
			new TextButton.Parameters("Cancel",
				() =>
				{
				}
			),
			new TextButton.Parameters("Confirm",
				() =>
				{
					GiveRewards().Forget();
				},
				TextButton.ColorType.Green
			)
		));
	}

	private void OnUnlocked(SavedMerchantsGuildHallReward savedMerchantsGuildHallReward)
	{
		UpdateVisuals();
	}

	private void OnCompletedScenarioCountChanged()
	{
		UpdateVisuals();
	}
}