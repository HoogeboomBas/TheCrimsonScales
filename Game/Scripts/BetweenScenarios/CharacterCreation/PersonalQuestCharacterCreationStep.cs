using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class PersonalQuestCharacterCreationStep : CharacterCreationStep
{
	[Export]
	private PackedScene _questScene;
	[Export]
	private Control _questParent;
	[Export]
	private Label _noneRemainingLabel;

	private readonly List<CharacterCreationPersonalQuest> _quests = new List<CharacterCreationPersonalQuest>();
	private CharacterCreationPersonalQuest _selectedQuest;

	public override bool ConfirmButtonActive =>
		_characterCreationOverlay.SavedCampaign.SavedPersonalQuests.PersonalQuestDeckIds.Count == 0 || _selectedQuest != null;

	public override void Activate()
	{
		base.Activate();

		foreach(CharacterCreationPersonalQuest quest in _quests)
		{
			quest.QueueFree();
		}

		_quests.Clear();

		_noneRemainingLabel.SetVisible(_characterCreationOverlay.SavedCampaign.SavedPersonalQuests.PersonalQuestDeckIds.Count == 0);

		List<PersonalQuestModel> personalQuests = new List<PersonalQuestModel>();
		for(int i = 0; i < 2; i++)
		{
			PersonalQuestModel personalQuestModel = _characterCreationOverlay.SavedCampaign.SavedPersonalQuests.PeekPersonalQuest(i);
			if(personalQuestModel == null)
			{
				break;
			}

			personalQuests.Add(personalQuestModel);
		}

		for(int i = 0; i < personalQuests.Count; i++)
		{
			PersonalQuestModel personalQuestModel = personalQuests[i];
			CharacterCreationPersonalQuest personalQuest = _questScene.Instantiate<CharacterCreationPersonalQuest>();
			_questParent.AddChild(personalQuest);
			personalQuest.Init(personalQuestModel, 0.3f + i * 0.3f);
			personalQuest.Fade(1f, 0.3f);
			_quests.Add(personalQuest);

			personalQuest.PressedEvent += OnQuestPressed;
		}
	}

	public override void Deactivate()
	{
		base.Deactivate();

		_selectedQuest = null;

		foreach(CharacterCreationPersonalQuest quest in _quests)
		{
			quest.Fade(0f, 0.3f);
		}
	}

	private void OnQuestPressed(CharacterCreationPersonalQuest personalQuest)
	{
		if(personalQuest == _selectedQuest)
		{
			return;
		}

		if(_quests.Any(quest => quest.Animating))
		{
			return;
		}

		_selectedQuest = personalQuest;

		foreach(CharacterCreationPersonalQuest otherQuest in _quests)
		{
			otherQuest.SetSelected(false, true);
		}

		_selectedQuest.SetSelected(true, true);

		_characterCreationOverlay.SetPersonalQuestModel(_selectedQuest.PersonalQuestModel);
		_characterCreationOverlay.UpdateConfirmVisible();
	}
}