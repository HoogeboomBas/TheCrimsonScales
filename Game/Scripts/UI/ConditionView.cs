using Godot;

public partial class ConditionView : Control
{
	[Export]
	private TextureRect _textureRect;
	[Export]
	private Label _stackLabel;

	public ConditionModel ConditionModel { get; private set; }

	public void SetCondition(ConditionModel conditionModel)
	{
		ConditionModel = conditionModel;

		_textureRect.SetTexture(ResourceLoader.Load<Texture2D>(ConditionModel.IconPath));

		SetStackText(null);
	}

	public void SetStackText(string text)
	{
		_stackLabel.Visible = text != null;
		_stackLabel.Text = text;
	}
}