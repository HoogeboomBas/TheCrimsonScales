using Godot;
using GTweens.Builders;
using GTweensGodot.Extensions;

public partial class CharacterCreationOverlay : Control
{
	[Export]
	private CharacterCreationStep[] _steps;

	[Export]
	private ChoiceButton _backButton;
	[Export]
	private ChoiceButton _confirmButton;

	private int _stepIndex;
	private CharacterCreationStep _currentStep;

	private bool _animating;

	public SavedCampaign SavedCampaign { get; private set; }

	public ClassModel ClassModel { get; private set; }
	public PersonalQuestModel PersonalQuestModel { get; private set; }
	public string CharacterName { get; private set; }

	public override void _Ready()
	{
		base._Ready();

		_backButton.BetterButton.Pressed += OnBackPressed;
		_confirmButton.BetterButton.Pressed += OnConfirmPressed;

		foreach(CharacterCreationStep step in _steps)
		{
			step.Init(this);
		}

		Hide();
	}

	public void Open(SavedCampaign savedCampaign)
	{
		SavedCampaign = savedCampaign;

		Show();
		SetModulate(Colors.Transparent);
		this.TweenModulateAlpha(1f, 0.3f).Play();

		SetClassModel(null);
		SetPersonalQuestModel(null);
		SetCharacterName(string.Empty);

		SetStep(0);
	}

	public void Close()
	{
		_confirmButton.SetActive(false);

		_currentStep?.Deactivate();
		_currentStep = null;
		this.TweenModulateAlpha(0f, 0.3f).OnComplete(Hide).Play();
	}

	public void NextStep()
	{
		if(_stepIndex == _steps.Length - 1)
		{
			// Final step completed, time to create the character!
			FinalizeCharacter();

			return;
		}

		SetStep(_stepIndex + 1);
	}

	public void UpdateConfirmVisible()
	{
		_confirmButton.SetActive(_currentStep?.ConfirmButtonActive ?? false);
	}

	public void SetClassModel(ClassModel classModel)
	{
		ClassModel = classModel;
	}

	public void SetPersonalQuestModel(PersonalQuestModel personalQuestModel)
	{
		PersonalQuestModel = personalQuestModel;
	}

	public void SetCharacterName(string characterName)
	{
		CharacterName = characterName;
	}

	private void SetStep(int newStepIndex)
	{
		CharacterCreationStep oldStep = _currentStep;

		_stepIndex = newStepIndex;
		_currentStep = _steps[_stepIndex];

		if(oldStep == null)
		{
			_currentStep.Activate();
			UpdateConfirmVisible();
		}
		else
		{
			_animating = true;
			GTweenSequenceBuilder.New()
				.AppendCallback(oldStep.Deactivate)
				.AppendCallback(UpdateConfirmVisible)
				.AppendTime(0.3f)
				.AppendCallback(_currentStep.Activate)
				.AppendCallback(UpdateConfirmVisible)
				.AppendCallback(() => _animating = false)
				.Build().Play();
		}
	}

	private void FinalizeCharacter()
	{
		SavedCampaign.SavedPersonalQuests.DrawPersonalQuest(PersonalQuestModel);
		SavedCampaign.AddCharacter(ClassModel, PersonalQuestModel, CharacterName);

		AppController.Instance.SaveFile.Save();

		Close();
	}

	private void OnBackPressed()
	{
		if(_stepIndex == 0)
		{
			Close();
			return;
		}

		SetStep(_stepIndex - 1);
	}

	private void OnConfirmPressed()
	{
		if(_animating)
		{
			return;
		}

		NextStep();
	}
}