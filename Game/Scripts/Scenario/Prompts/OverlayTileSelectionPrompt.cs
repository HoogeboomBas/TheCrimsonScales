using System;
using System.Collections.Generic;
using System.Linq;

public class OverlayTileSelectionPrompt(
	Action<List<OverlayTile>> getValidOverlayTiles, bool autoSelectIfMaxCountIsValidCount, EffectCollection effectCollection,
	Func<string> getHintText,
	int minSelectionCount = 1, int maxSelectionCount = 1)
	: Prompt<OverlayTileSelectionPrompt.Answer>(effectCollection, getHintText)
{
	public class Answer : PromptAnswer
	{
		public List<int> OverlayTileReferenceIds { get; init; }
	}

	private readonly List<OverlayTile> _validOverlayTiles = [];

	private readonly List<OverlayTile> _selectedOverlayTiles = [];

	protected override bool CanConfirm => _selectedOverlayTiles.Count > 0 &&
	                                      (_selectedOverlayTiles.Count >= minSelectionCount ||
	                                       _selectedOverlayTiles.Count == _validOverlayTiles.Count) &&
	                                      _selectedOverlayTiles.Count <= maxSelectionCount;

	protected override bool CanSkip => minSelectionCount == 0 || _validOverlayTiles.Count == 0;

	protected override void Enable()
	{
		base.Enable();

		_validOverlayTiles.Clear();
		getValidOverlayTiles(_validOverlayTiles);

		_selectedOverlayTiles.Clear();

		if(autoSelectIfMaxCountIsValidCount && _validOverlayTiles.Count == maxSelectionCount)
		{
			_selectedOverlayTiles.AddRange(_validOverlayTiles);
		}

		if(_authority is not Character)
		{
			if(_validOverlayTiles.Count == 0)
			{
				Skip();
			}

			if(_validOverlayTiles.Count == 1)
			{
				_selectedOverlayTiles.Add(_validOverlayTiles[0]);
				Complete(true);
			}
		}
	}

	protected override void UpdateState()
	{
		base.UpdateState();

		GameController.Instance.HexIndicatorManager.StartSettingIndicators();

		HashSet<Hex> hexes = _validOverlayTiles.SelectMany(overlayTile => overlayTile.Hexes).ToHashSet();
		foreach(Hex hex in hexes)
		{
			GameController.Instance.HexIndicatorManager.SetIndicator(hex,
				_selectedOverlayTiles?.SelectMany(overlayTile => overlayTile.Hexes).Contains(hex) ?? false
					? HexIndicatorType.Selected
					: HexIndicatorType.Normal,
				OnIndicatorPressed);
		}

		GameController.Instance.HexIndicatorManager.EndSettingIndicators();
	}

	protected override void Disable()
	{
		base.Disable();

		GameController.Instance.HexIndicatorManager.ClearIndicators();
	}

	protected override Answer CreateAnswer()
	{
		return new Answer()
		{
			OverlayTileReferenceIds = _selectedOverlayTiles.Select(hex => hex.ReferenceId).ToList()
		};
	}

	private void OnIndicatorPressed(HexIndicator hexIndicator)
	{
		OverlayTile overlayTile = _selectedOverlayTiles.FirstOrDefault(overlayTile => overlayTile.Hexes.Contains(hexIndicator.Hex));
		if(overlayTile != null)
		{
			_selectedOverlayTiles.Remove(overlayTile);
		}
		else
		{
			if(_selectedOverlayTiles.Count < maxSelectionCount)
			{
				overlayTile = _validOverlayTiles.FirstOrDefault(tile => tile.Hexes.Contains(hexIndicator.Hex));
				if(overlayTile != null)
				{
					_selectedOverlayTiles.Add(overlayTile);
				}
			}
		}

		FullUpdateState();
	}
}