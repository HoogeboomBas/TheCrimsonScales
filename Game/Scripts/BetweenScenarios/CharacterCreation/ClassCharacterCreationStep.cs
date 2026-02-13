using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class ClassCharacterCreationStep : CharacterCreationStep
{
	[Export]
	private PackedScene _classScene;
	[Export]
	private Control _classParent;

	[Export]
	private TextureRect _matFrontTexture;

	private readonly List<CharacterCreationClass> _characterCreationClasses = new List<CharacterCreationClass>();

	private CharacterCreationClass _selectedClass;

	public override bool ConfirmButtonActive => true;

	public override void Activate()
	{
		base.Activate();

		foreach(CharacterCreationClass characterCreationClass in _characterCreationClasses)
		{
			characterCreationClass.QueueFree();
		}

		_characterCreationClasses.Clear();

		List<ClassModel> unlockedClasses =
			_characterCreationOverlay.SavedCampaign.SavedClasses
				.Where(savedClass => savedClass.Value.Unlocked)
				.Select(keyValuePair => ModelDB.GetById<ClassModel>(keyValuePair.Key))
				.ToList();

		IEnumerable<ClassModel> usableClassModels = unlockedClasses.Where(classModel =>
			_characterCreationOverlay.SavedCampaign.Characters.All(character => character.ClassModel != classModel));

		foreach(ClassModel classModel in usableClassModels)
		{
			CharacterCreationClass characterCreationClass = _classScene.Instantiate<CharacterCreationClass>();
			_classParent.AddChild(characterCreationClass);
			characterCreationClass.Init(classModel);
			_characterCreationClasses.Add(characterCreationClass);

			characterCreationClass.PressedEvent += OnClassPressed;
		}

		SelectButton(_characterCreationClasses.FirstOrDefault(characterCreationClass =>
			characterCreationClass.ClassModel == _characterCreationOverlay.ClassModel) ?? _characterCreationClasses[0]);
	}

	private void SelectButton(CharacterCreationClass characterCreationClass)
	{
		if(characterCreationClass == _selectedClass)
		{
			return;
		}

		_selectedClass = characterCreationClass;

		foreach(CharacterCreationClass otherClass in _characterCreationClasses)
		{
			otherClass.SetSelected(false, true);
		}

		_selectedClass.SetSelected(true, true);

		_matFrontTexture.SetTexture(_selectedClass.ClassModel.MatFrontTexture);

		_characterCreationOverlay.SetClassModel(_selectedClass.ClassModel);
	}

	private void OnClassPressed(CharacterCreationClass characterCreationClass)
	{
		SelectButton(characterCreationClass);
	}
}