using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class PerksPopup : Popup<PerksPopup.Request>
{
	public class Request : PopupRequest
	{
		public SavedCharacter SavedCharacter { get; init; }
	}

	[Export]
	private Label _remainingPerksLabel;
	[Export]
	private CheckmarkBoxSet[] _checkmarkBoxSets;

	[Export]
	private ScrollContainer _scrollContainer;

	[Export]
	private PackedScene _perkScene;
	[Export]
	private Control _perkContainer;

	private readonly List<PerksPopupPerk> _perks = new List<PerksPopupPerk>();

	private SavedCharacter _savedCharacter;

	public override void _Ready()
	{
		base._Ready();

		for(int i = 0; i < _checkmarkBoxSets.Length; i++)
		{
			CheckmarkBoxSet checkmarkBoxSet = _checkmarkBoxSets[i];
			checkmarkBoxSet.Init(i * 3);
		}
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		UnsubscribeSavedCharacter();

		_savedCharacter = PopupRequest.SavedCharacter;

		UpdateAvailablePerks();

		foreach(CheckmarkBoxSet checkmarkBoxSet in _checkmarkBoxSets)
		{
			checkmarkBoxSet.UpdateCheckmarks(_savedCharacter);
		}

		int perkIndex = 0;
		IEnumerable<IGrouping<PerkModel, PerkModel>> perkGroups = _savedCharacter.ClassModel.Perks.GroupBy(perkModel => perkModel);
		foreach(IGrouping<PerkModel, PerkModel> perkGroup in perkGroups)
		{
			int perkCount = perkGroup.Count();

			PerkModel perkModel = perkGroup.Key;
			PerksPopupPerk perk = _perkScene.Instantiate<PerksPopupPerk>();
			_perkContainer.AddChild(perk);
			perk.Init(perkModel, perkIndex, perkCount, _savedCharacter);
			_perks.Add(perk);

			perkIndex += perkCount;
		}

		_savedCharacter.PerksChangedEvent += OnPerksChanged;
	}

	protected override void OnClosed()
	{
		base.OnClosed();

		foreach(PerksPopupPerk perk in _perks)
		{
			perk.QueueFree();
		}

		_perks.Clear();

		_scrollContainer.SetVScroll(0);

		UnsubscribeSavedCharacter();
	}

	private void UpdateAvailablePerks()
	{
		int usedPerkCount = _savedCharacter.GetUsedPerkCount();
		int availablePerkCount = _savedCharacter.TotalAvailablePerkCount - usedPerkCount;
		_remainingPerksLabel.SetText($"{availablePerkCount}/{_savedCharacter.TotalAvailablePerkCount}");
	}

	private void UnsubscribeSavedCharacter()
	{
		if(_savedCharacter != null)
		{
			_savedCharacter.PerksChangedEvent -= OnPerksChanged;
		}

		_savedCharacter = null;
	}

	private void OnPerksChanged(SavedCharacter savedCharacter)
	{
		UpdateAvailablePerks();
	}
}