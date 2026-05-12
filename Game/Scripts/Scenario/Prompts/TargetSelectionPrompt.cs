using System;
using System.Collections.Generic;
using System.Linq;

public class TargetSelectionPrompt(
	Action<List<Figure>> getValidTargets, bool autoSelectIfOne, bool autoSkipIfNone, bool mandatory, EffectCollection effectCollection,
	Func<string> getHintText)
	: Prompt<TargetSelectionPrompt.Answer>(effectCollection, getHintText)
{
	public class Answer : PromptAnswer
	{
		public int FigureReferenceId { get; init; }
	}

	private readonly List<Figure> _validTargets = new List<Figure>();

	private Figure _selectedFigure;

	protected override bool CanConfirm => _selectedFigure != null;
	protected override bool CanSkip => _validTargets.Count == 0 || !mandatory;

	protected override void Enable()
	{
		base.Enable();

		_validTargets.Clear();
		getValidTargets(_validTargets);

		if(autoSelectIfOne && _validTargets.Count == 1)
		{
			_selectedFigure = _validTargets[0];

			if(mandatory)
			{
				Complete(true);
			}
		}

		if(autoSkipIfNone && _validTargets.Count == 0)
		{
			Skip();
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