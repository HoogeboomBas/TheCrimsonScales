using Godot;

public partial class StartingGroupNewCampaignStep : NewCampaignStep
{
	[Export]
	private NewCampaignStartingGroup[] _startingGroups;

	public override bool ConfirmButtonActive => false;

	public override void _Ready()
	{
		base._Ready();

		foreach(NewCampaignStartingGroup startingParty in _startingGroups)
		{
			startingParty.PressedEvent += OnStartingPartyPressed;
		}
	}

	private void OnStartingPartyPressed(NewCampaignStartingGroup startingGroup)
	{
		if(Active)
		{
			NewCampaignController.Instance.SetStartingParty(startingGroup.StartingGroup);
			NewCampaignController.Instance.NextStep();
		}
	}
}