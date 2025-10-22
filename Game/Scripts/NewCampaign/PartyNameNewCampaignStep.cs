using Godot;

public partial class PartyNameNewCampaignStep : NewCampaignStep
{
	[Export]
	private LineEdit _nameLineEdit;

	public override bool ConfirmButtonActive => !string.IsNullOrEmpty(_nameLineEdit.Text);

	public override void _Ready()
	{
		base._Ready();

		_nameLineEdit.TextChanged += OnNameChanged;

		OnNameChanged(_nameLineEdit.Text);
	}

	public override void Activate()
	{
		base.Activate();

		OnNameChanged(_nameLineEdit.Text);
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