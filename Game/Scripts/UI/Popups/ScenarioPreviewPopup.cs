using System.Collections.Generic;
using Godot;

public partial class ScenarioPreviewPopup : Popup<ScenarioPreviewPopup.Request>
{
	public class Request : PopupRequest
	{
		public string HeaderText { get; }
		public TextHelper.LabelTextDelegate GetText { get; }

		public TextButton.Parameters[] ButtonParameters { get; }

		public List<MonsterModel> MonsterModels { get; }

		public Request(string headerText, string text)
			: this(headerText, text, new TextButton.Parameters("Confirm", null))
		{
		}

		public Request(string headerText, TextHelper.LabelTextDelegate getText)
			: this(headerText, getText, new TextButton.Parameters("Confirm", null))
		{
		}

		public Request(string headerText, string getText, params TextButton.Parameters[] buttonParameters)
			: this(headerText, parameters => getText, buttonParameters)
		{
		}

		public Request(string headerText, TextHelper.LabelTextDelegate getText, params TextButton.Parameters[] buttonParameters)
		{
			HeaderText = headerText;
			GetText = getText;
			ButtonParameters = buttonParameters;
		}

		public Request(
			string headerText,
			string getText,
			List<MonsterModel> monsterModels,
			params TextButton.Parameters[] buttonParameters)
			: this(
				headerText,
				parameters => getText,
				monsterModels,
				buttonParameters)
		{
					GD.Print("got to the request constructor!");
		}
		// for monsters
		public Request(
			string headerText,
			TextHelper.LabelTextDelegate getText,
			List<MonsterModel> monsterModels,
			params TextButton.Parameters[] buttonParameters)
		{
			HeaderText = headerText;
			GetText = getText;
			MonsterModels = monsterModels;
			ButtonParameters = buttonParameters;
		}
	}
	
	[Export]
	private Label _headerLabel;
	[Export]
	private RichTextLabel _bodyLabel;
	[Export]
	private PackedScene _enemyPreview;
	[Export]
	private HBoxContainer _enemyPreviewParent;

	[Export]
	private PackedScene _textButtonScene;
	[Export]
	private Control _textButtonParent;

	private readonly List<TextButton> _buttons = new List<TextButton>();

	protected override void OnOpen()
	{
		base.OnOpen();
		
		_headerLabel.SetText(PopupRequest.HeaderText);
		RichTextParameters textParameters = _bodyLabel.GetRichTextParameters();
		_bodyLabel.SetText(PopupRequest.GetText(textParameters));

		foreach(MonsterModel monster in PopupRequest.MonsterModels)
		{
			EnemyPreview enemyPreview = _enemyPreview.Instantiate<EnemyPreview>();
			_enemyPreviewParent.AddChild(enemyPreview);
			enemyPreview.Init(monster);
		}

		foreach(TextButton.Parameters buttonParameters in PopupRequest.ButtonParameters)
		{
			TextButton textButton = _textButtonScene.Instantiate<TextButton>();
			_textButtonParent.AddChild(textButton);
			textButton.Init(buttonParameters);
			textButton.PressedEvent += OnButtonPressed;
			_buttons.Add(textButton);
		}
	}

	protected override void OnClosed()
	{
		base.OnClosed();

		foreach (EnemyPreview enemy in _enemyPreviewParent.GetChildren())
		{
			enemy.QueueFree();
		}

		foreach(TextButton button in _buttons)
		{
			button.QueueFree();
		}

		_buttons.Clear();
	}

	private void OnButtonPressed()
	{
		Close();
	}
}
