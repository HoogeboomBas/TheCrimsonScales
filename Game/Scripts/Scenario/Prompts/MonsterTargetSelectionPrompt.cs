using System;
using System.Collections.Generic;
using System.Linq;

public class MonsterTargetSelectionPrompt(
	Action<List<Figure>> getValidTargets, bool autoSelectIfOne, Figure focus, EffectCollection effectCollection, Func<string> getHintText)
	: Prompt<MonsterTargetSelectionPrompt.Answer>(effectCollection, getHintText)
{
	public class Answer : PromptAnswer
	{
		public int FigureReferenceId { get; init; }
	}

	private readonly List<Figure> _allTargets = new List<Figure>();
	private readonly List<Figure> _validTargets = new List<Figure>();

	private Figure _selectedFigure;

	protected override bool CanConfirm => _selectedFigure != null;
	protected override bool CanSkip => _validTargets.Count == 0;

	protected override void Enable()
	{
		base.Enable();

		_allTargets.Clear();
		getValidTargets(_allTargets);

		_validTargets.Clear();
		if(_allTargets.Contains(focus))
		{
			_validTargets.Add(focus);
		}
		else
		{
			//TODO: Filter to give priority to attacks without disadvantage
			//TODO: If focus hasn't and cannot be targeted, one target should still be wasted I think?
			_validTargets.AddRange(_allTargets);
		}

		if(autoSelectIfOne && _validTargets.Count == 1)
		{
			_selectedFigure = _validTargets[0];
		}

		if(_validTargets.Count == 0)
		{
			Skip();
		}

		if(_validTargets.Count == 1)
		{
			_selectedFigure = _validTargets[0];
			Complete(true);
		}
	}

	protected override void UpdateState()
	{
		base.UpdateState();

		GameController.Instance.HexIndicatorManager.StartSettingIndicators();

		HashSet<Hex> hexes = _validTargets.SelectMany(figure => figure.Hexes).ToHashSet();
		foreach(Hex hex in hexes)
		{
			GameController.Instance.HexIndicatorManager.SetIndicator(hex,
				_selectedFigure?.Hexes.Contains(hex) ?? false ? HexIndicatorType.Selected : HexIndicatorType.Normal,
				OnIndicatorPressed);
		}

		GameController.Instance.HexIndicatorManager.EndSettingIndicators();
	}

	protected override void Disable()
	{
		base.Disable();

		GameController.Instance.HexIndicatorManager.ClearIndicators();
		GameController.Instance.SelectFigureView.Close();
	}

	protected override Answer CreateAnswer()
	{
		return new Answer()
		{
			FigureReferenceId = _selectedFigure.ReferenceId
		};
	}

	private void OnIndicatorPressed(HexIndicator hexIndicator)
	{
		if(_selectedFigure != null && hexIndicator.Hex == _selectedFigure.Hex)
		{
			_selectedFigure = null;
		}
		else
		{
			List<Figure> figures = hexIndicator.Hex.GetHexObjectsOfType<Figure>().Where(_validTargets.Contains).ToList();
			if(figures.Count > 1)
			{
				GameController.Instance.SelectFigureView.Open(figures, OnFigurePressed);
			}
			else
			{
				_selectedFigure = figures.FirstOrDefault();
			}
		}

		FullUpdateState();
	}

	private void OnFigurePressed(Figure figure)
	{
		GameController.Instance.SelectFigureView.Close();

		_selectedFigure = figure;

		FullUpdateState();
	}
}