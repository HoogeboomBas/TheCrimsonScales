using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class BetweenScenariosCharacterPortraitManager : Control
{
	[Export]
	private Control _portraitParent;
	[Export]
	private PackedScene _portraitScene;

	private bool _selectionMode;

	private List<BetweenScenariosCharacterPortrait> Portraits { get; } = new List<BetweenScenariosCharacterPortrait>();

	public BetweenScenariosCharacterPortrait SelectedPortrait { get; private set; }

	public event Action<BetweenScenariosCharacterPortrait> SelectedPortraitChangedEvent;

	public override void _Ready()
	{
		base._Ready();

		_selectionMode = false;

		BetweenScenariosController.Instance.SavedCampaign.CharactersChangedEvent += OnCharactersChanged;

		OnCharactersChanged();
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		BetweenScenariosController.Instance.SavedCampaign.CharactersChangedEvent -= OnCharactersChanged;
	}

	public void SetSelectionMode(bool selectionMode)
	{
		if(selectionMode == _selectionMode)
		{
			return;
		}

		_selectionMode = selectionMode;

		if(_selectionMode)
		{
			foreach(BetweenScenariosCharacterPortrait portrait in Portraits)
			{
				portrait.SetActive(portrait == SelectedPortrait, true);
			}
		}
		else
		{
			foreach(BetweenScenariosCharacterPortrait portrait in Portraits)
			{
				portrait.SetActive(true, false);
			}
		}
	}

	public void SelectPortrait(BetweenScenariosCharacterPortrait portrait)
	{
		if(portrait == SelectedPortrait)
		{
			return;
		}

		SelectedPortrait = portrait;
		SelectedPortraitChangedEvent?.Invoke(SelectedPortrait);

		if(!_selectionMode)
		{
			return;
		}

		foreach(BetweenScenariosCharacterPortrait otherPortrait in Portraits)
		{
			otherPortrait.SetActive(false, true);
		}

		SelectedPortrait?.SetActive(true, true);
	}

	private void OnPressed(BetweenScenariosCharacterPortrait portrait)
	{
		SelectPortrait(portrait);
	}

	private void OnCharactersChanged()
	{
		foreach(BetweenScenariosCharacterPortrait portrait in Portraits)
		{
			portrait.QueueFree();
		}

		Portraits.Clear();

		foreach(SavedCharacter savedCharacter in BetweenScenariosController.Instance.SavedCampaign.Characters)
		{
			BetweenScenariosCharacterPortrait portrait = _portraitScene.Instantiate<BetweenScenariosCharacterPortrait>();
			_portraitParent.AddChild(portrait);
			portrait.Init(BetweenScenariosController.Instance.SavedCampaign, savedCharacter);
			Portraits.Add(portrait);

			portrait.BetterButton.Pressed += () => OnPressed(portrait);
		}

		SelectPortrait(Portraits.FirstOrDefault());
	}
}