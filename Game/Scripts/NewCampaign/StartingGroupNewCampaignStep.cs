using Godot;

public partial class StartingGroupNewCampaignStep : NewCampaignStep
{
	[Export]
	private NewCampaignStartingGroupToggleButton[] _startingGroups;

	public override bool ConfirmButtonActive => NewCampaignController.Instance.StartingGroup.HasValue;

	public override void _Ready()
	{
		base._Ready();

		foreach(NewCampaignStartingGroupToggleButton startingGroup in _startingGroups)
		{
			startingGroup.Init();

			startingGroup.PressedEvent += OnStartingPartyPressed;
		}
	}

	private void OnStartingPartyPressed(NewCampaignStartingGroupToggleButton startingGroup)
	{
		if(Active)
		{
			NewCampaignController.Instance.SetStartingGroup(startingGroup.StartingGroup);
			NewCampaignController.Instance.UpdateConfirmVisible();

			foreach(NewCampaignStartingGroupToggleButton toggleButton in _startingGroups)
			{
				toggleButton.SetSelected(toggleButton.StartingGroup == NewCampaignController.Instance.StartingGroup, true);
			}
		}
	}
}