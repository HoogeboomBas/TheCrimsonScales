using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class CreateCharacterPopup : Popup<CreateCharacterPopup.Request>
{
	public class Request : PopupRequest
	{
		public SavedCampaign SavedCampaign { get; init; }
	}

	[Export]
	private PackedScene _classButtonScene;
	[Export]
	private Control _classButtonParent;

	[Export]
	private TextureRect _matFrontTexture;

	[Export]
	private LineEdit _nameLineEdit;

	[Export]
	private BetterButton _cancelButton;
	[Export]
	private BetterButton _confirmButton;

	private readonly List<CreateCharacterClassButton> _buttons = new List<CreateCharacterClassButton>();

	private CreateCharacterClassButton _selectedButton;

	public override void _Ready()
	{
		base._Ready();

		_nameLineEdit.TextChanged += OnNameChanged;

		_cancelButton.Pressed += OnCancelPressed;
		_confirmButton.Pressed += OnConfirmPressed;

		OnNameChanged(_nameLineEdit.Text);
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		ClassModel[] classModels =
		[
			ModelDB.Class<MirefootModel>(),
			ModelDB.Class<BombardModel>(),
			ModelDB.Class<HierophantModel>(),
			ModelDB.Class<FireKnightModel>(),
			ModelDB.Class<ChainguardModel>(),
		];

		IEnumerable<ClassModel> usableClassModels = classModels.Where(classModel => PopupRequest.SavedCampaign.Characters.All(character => character.ClassModel != classModel));

		foreach(ClassModel classModel in usableClassModels)
		{
			CreateCharacterClassButton classButton = _classButtonScene.Instantiate<CreateCharacterClassButton>();
			_classButtonParent.AddChild(classButton);
			classButton.Init(classModel);
			_buttons.Add(classButton);

			classButton.PressedEvent += OnClassButtonPressed;
		}

		SelectButton(_buttons[0]);

		_nameLineEdit.SetText(string.Empty);
		OnNameChanged(_nameLineEdit.Text);
	}

	protected override void OnClosed()
	{
		base.OnClosed();

		foreach(CreateCharacterClassButton button in _buttons)
		{
			button.QueueFree();
		}

		_buttons.Clear();
	}

	public void SelectButton(CreateCharacterClassButton button)
	{
		if(button == _selectedButton)
		{
			return;
		}

		_selectedButton = button;

		foreach(CreateCharacterClassButton otherButton in _buttons)
		{
			otherButton.SetSelected(false, true);
		}

		_selectedButton?.SetSelected(true, true);

		_matFrontTexture.Texture = button.ClassModel.MatFrontTexture;
	}

	private void OnClassButtonPressed(CreateCharacterClassButton button)
	{
		SelectButton(button);
	}

	private void OnNameChanged(string newText)
	{
		_confirmButton.SetEnabled(!string.IsNullOrEmpty(newText));
	}

	private void OnCancelPressed()
	{
		Close();
	}

	private void OnConfirmPressed()
	{
		// Create a new character and add it to the party
		PopupRequest.SavedCampaign.AddCharacter(_selectedButton.ClassModel, _nameLineEdit.Text);

		AppController.Instance.SaveFile.Save();

		Close();
	}
}