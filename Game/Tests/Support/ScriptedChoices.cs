using System;
using System.Collections.Generic;
using Godot;
using Fractural.Tasks;

namespace CrimsonScales.Tests;

public sealed class ScriptedChoices : IPromptDecisionSource
{
	private readonly Queue<(Type PromptType, Func<object, PromptAnswer> Answer)> _answers = new();
	private readonly TestGame _game;

	internal ScriptedChoices(TestGame game) => _game = game;

	public ScriptedChoices SelectTarget(string name) => Answer<TargetSelectionPrompt>(prompt => prompt.SelectTarget(_game.Figure(name)));
	public ScriptedChoices SelectTarget(UnitHandle unit) => Answer<TargetSelectionPrompt>(prompt => prompt.SelectTarget(_game.Resolve(unit)));
	public ScriptedChoices Confirm() => Answer<ConfirmPrompt>(_ => new ConfirmPrompt.Answer { Confirmed = true });

	public ScriptedChoices MoveTo(Vector2I position) => Answer<MovePrompt>(prompt =>
		prompt.SelectDestination(GameController.Instance.Map.GetHex(position)));

	/// <summary>Escape hatch for additional prompt types; prefer validated selection helpers where available.</summary>
	public ScriptedChoices Answer<TPrompt>(Func<TPrompt, PromptAnswer> answer)
	{
		_answers.Enqueue((typeof(TPrompt), prompt => answer((TPrompt)prompt)));
		return this;
	}

	public GDTask<TAnswer> Decide<TAnswer>(Prompt<TAnswer> prompt, Figure authority)
		where TAnswer : PromptAnswer, new()
	{
		if(!_answers.TryPeek(out var next) || !next.PromptType.IsInstanceOfType(prompt))
		{
			throw new InvalidOperationException($"Unexpected {prompt.GetType().Name} for {authority?.DebugName}. " +
			                                    $"Next scripted choice: {(_answers.Count == 0 ? "none" : next.PromptType.Name)}.");
		}

		_answers.Dequeue();
		TAnswer answer = next.Answer(prompt) as TAnswer
			?? throw new InvalidOperationException($"Invalid scripted answer for {prompt.GetType().Name}.");
		return GDTask.FromResult(answer);
	}

	internal void Clear() => _answers.Clear();

	internal void AssertConsumed()
	{
		if(_answers.Count != 0)
		{
			throw new InvalidOperationException($"{_answers.Count} scripted choice(s) were not used. Next: {_answers.Peek().PromptType.Name}.");
		}
	}
}