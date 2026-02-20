using System.Collections.Generic;
using Godot;

public partial class PerksPopupPerk : Control
{
	[Export]
	private PackedScene _perkBoxesScene;
	[Export]
	private Control _perkBoxContainer;

	[Export]
	private RichTextLabel _description;

	private SavedCharacter _savedCharacter;

	private readonly List<PerksPopupPerkBoxes> _perkBoxes = new List<PerksPopupPerkBoxes>();

	public PerkModel PerkModel { get; private set; }
	public int StartingPerkIndex { get; private set; }
	public int PerkCount { get; private set; }
	public bool Acquired { get; private set; }

	public void Init(PerkModel perkModel, int startingPerkIndex, int perkCount, SavedCharacter savedCharacter)
	{
		PerkModel = perkModel;
		StartingPerkIndex = startingPerkIndex;
		PerkCount = perkCount;
		_savedCharacter = savedCharacter;

		for(int i = 0; i < perkCount; i++)
		{
			int perkIndex = startingPerkIndex + i;
			PerksPopupPerkBoxes perkBoxes = _perkBoxesScene.Instantiate<PerksPopupPerkBoxes>();
			_perkBoxContainer.AddChild(perkBoxes);
			perkBoxes.Init(perkIndex, perkModel.PerkBoxCount);
			perkBoxes.PressedEvent += OnPerkBoxPressed;
			_perkBoxes.Add(perkBoxes);
		}

		UpdatePerks();

		_description.SetText(perkModel.ToString(_description.GetRichTextParameters()));

		_savedCharacter.PerksChangedEvent += OnPerksChangedEvent;
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		if(_savedCharacter != null)
		{
			_savedCharacter.PerksChangedEvent -= OnPerksChangedEvent;
		}
	}

	private void UpdatePerks()
	{
		for(int i = 0; i < PerkCount; i++)
		{
			int perkIndex = StartingPerkIndex + i;
			PerksPopupPerkBoxes perkBoxes = _perkBoxes[i];
			bool acquired = _savedCharacter.GetPerkAcquired(perkIndex);
			perkBoxes.SetAcquired(acquired);
			perkBoxes.SetCanPress(_savedCharacter.GetAvailablePerkCount() >= perkBoxes.BoxCount && !acquired);
		}
	}

	private void OnPerksChangedEvent(SavedCharacter savedCharacter)
	{
		UpdatePerks();
	}

	private void OnPerkBoxPressed(PerksPopupPerkBoxes perkBoxes)
	{
		AppController.Instance.PopupManager.OpenPopupOnTop(new PerkConfirmationPopup.Request()
		{
			SavedCharacter = _savedCharacter,
			PerkModel = PerkModel,
			PerkIndex = perkBoxes.PerkIndex
		});
	}
}