using System;
using System.Collections.Generic;
using Godot;

public class AOEPrompt(
	Figure performer, AOEPattern pattern, Hex forcedOriginHex, EffectCollection effectCollection, Func<string> getHintText, int range)
	: Prompt<AOEPrompt.Answer>(effectCollection, getHintText)
{
	public class Answer : PromptAnswer
	{
		public List<AOEHex> AOEHexes { get; init; }
	}

	protected override void Enable()
	{
		base.Enable();

		GameController.Instance.AOEView.AOEChangedEvent += OnAOEChanged;
		GameController.Instance.AOEView.Open(pattern, forcedOriginHex, performer, range);
	}

	protected override void Disable()
	{
		base.Disable();

		GameController.Instance.AOEView.AOEChangedEvent -= OnAOEChanged;
		GameController.Instance.AOEView.Close();
	}

	protected override Answer CreateAnswer()
	{
		List<AOEHex> aoeHexes = [];

		foreach(AOEHexView hexView in GameController.Instance.AOEView.Hexes)
		{
			aoeHexes.Add(new AOEHex(hexView.GlobalCoords, hexView.Type, hexView.CustomMark));
		}

		return new Answer()
		{
			AOEHexes = aoeHexes
		};
	}

	private void OnAOEChanged()
	{
		FullUpdateState();
	}
}