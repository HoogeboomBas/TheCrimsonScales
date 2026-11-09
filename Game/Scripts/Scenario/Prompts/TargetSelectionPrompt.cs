using System;
using System.Collections.Generic;
using System.Linq;

public class TargetSelectionPrompt(
	Action<List<Figure>> getValidTargets, bool autoSelectIfOne, bool autoSkipIfNone, bool autoConfirmIfOne, bool mandatory, EffectCollection effectCollection,
	Func<string> getHintText)
	: Prompt<TargetSelectionPrompt.Answer>(effectCollection, getHintText)
{
	public class Answer : PromptAnswer
	{
		public int FigureReferenceId { get; init; }
	}

	public TargetSelection Selection { get; } = new TargetSelection(
		getValidTargets, autoSelectIfOne, autoSkipIfNone, autoConfirmIfOne, mandatory);

	protected override bool CanConfirm => Selection.CanConfirm;
	protected override bool CanSkip => Selection.CanSkip;

	protected override void Enable()
	{
		base.Enable();

		Selection.Initialize();
		if(Selection.ShouldAutoConfirm)
		{
			Complete(true);
		}

		if(Selection.ShouldAutoSkip)
		{
			Skip();
		}
	}

	protected override void UpdateState()
	{
		base.UpdateState();

		GameController.Instance.HexIndicatorManager.StartSettingIndicators();

		HashSet<Hex> hexes = Selection.ValidTargets.SelectMany(figure => figure.Hexes).ToHashSet();
		foreach(Hex hex in hexes)
		{
			if(hex == null)
			{
				continue;
			}

			GameController.Instance.HexIndicatorManager.SetIndicator(hex,
				Selection.SelectedFigure?.Hexes.Contains(hex) ?? false ? HexIndicatorType.Selected : HexIndicatorType.Normal,
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
			FigureReferenceId = Selection.SelectedFigure.ReferenceId
		};
	}

	private void OnIndicatorPressed(HexIndicator hexIndicator)
	{
		if(Selection.SelectedFigure != null && hexIndicator.Hex == Selection.SelectedFigure.Hex)
		{
			Selection.Clear();
		}
		else
		{
			List<Figure> figures = hexIndicator.Hex.GetHexObjectsOfType<Figure>().Where(figure => Selection.ValidTargets.Contains(figure)).ToList();
			if(figures.Count > 1)
			{
				GameController.Instance.SelectFigureView.Open(figures, OnFigurePressed);
			}
			else
			{
				Figure figure = figures.FirstOrDefault();
				if(figure == null)
				{
					Selection.Clear();
				}
				else
				{
					Selection.Select(figure);
				}
			}
		}

		FullUpdateState();
	}

	private void OnFigurePressed(Figure figure)
	{
		GameController.Instance.SelectFigureView.Close();

		Selection.Select(figure);

		FullUpdateState();
	}

	/// <summary>Target eligibility and selection state, independent of prompt presentation.</summary>
	public sealed class TargetSelection
	{
		private readonly Action<List<Figure>> _getValidTargets;
		private readonly bool _autoSelectIfOne;
		private readonly bool _autoSkipIfNone;
		private readonly bool _autoConfirmIfOne;
		private readonly bool _mandatory;
		private readonly List<Figure> _validTargets = new List<Figure>();

		public IReadOnlyList<Figure> ValidTargets { get; }
		public Figure SelectedFigure { get; private set; }
		public bool CanConfirm => SelectedFigure != null;
		public bool CanSkip => _validTargets.Count == 0 || !_mandatory;
		public bool ShouldAutoConfirm => (_autoSelectIfOne || _autoConfirmIfOne) &&
			_validTargets.Count == 1 && (_mandatory || _autoConfirmIfOne);
		public bool ShouldAutoSkip => _autoSkipIfNone && _validTargets.Count == 0;

		public TargetSelection(Action<List<Figure>> getValidTargets, bool autoSelectIfOne,
			bool autoSkipIfNone, bool autoConfirmIfOne, bool mandatory)
		{
			_getValidTargets = getValidTargets;
			_autoSelectIfOne = autoSelectIfOne;
			_autoSkipIfNone = autoSkipIfNone;
			_autoConfirmIfOne = autoConfirmIfOne;
			_mandatory = mandatory;
			ValidTargets = _validTargets.AsReadOnly();
		}

		public void Initialize()
		{
			Refresh();
			if((_autoSelectIfOne || _autoConfirmIfOne) && _validTargets.Count == 1)
			{
				Select(_validTargets[0]);
			}
		}

		public void Refresh()
		{
			_validTargets.Clear();
			_getValidTargets(_validTargets);
		}

		public void Select(Figure figure)
		{
			if(figure == null || !_validTargets.Contains(figure))
			{
				throw new InvalidOperationException($"{figure?.DebugName ?? "Null"} is not a valid target for this prompt.");
			}

			SelectedFigure = figure;
		}

		public void Clear() => SelectedFigure = null;
	}
}