using Godot;

public partial class PartyNameNewCampaignStep : NewCampaignStep
{
	[Export]
	private LineEdit _nameLineEdit;

	public override bool ConfirmButtonActive => !string.IsNullOrEmpty(_nameLineEdit.Text);

	public override void _Ready()
	{
		base._Ready();

		_nameLineEdit.SetText(NewCampaignController.Instance.PartyName);
		OnNameChanged(NewCampaignController.Instance.PartyName);

		_nameLineEdit.TextChanged += OnNameChanged;
	}

	public override void Activate()
	{
		base.Activate();

		_nameLineEdit.SetText(NewCampaignController.Instance.PartyName);
		OnNameChanged(NewCampaignController.Instance.PartyName);
	}

	private void OnNameChanged(string newText)
	{
		if(Active)
		{
			NewCampaignController.Instance.SetPartyName(newText);
			NewCampaignController.Instance.UpdateConfirmVisible();
		}
	}
}