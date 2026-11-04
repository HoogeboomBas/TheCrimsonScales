using System;
using Fractural.Tasks;

public interface IPromptDecisionSource
{
	GDTask<TAnswer> Decide<TAnswer>(Prompt<TAnswer> prompt, Figure authority)
		where TAnswer : PromptAnswer, new();
}

public sealed class InteractiveDecisionSource : IPromptDecisionSource
{
	public async GDTask<TAnswer> Decide<TAnswer>(Prompt<TAnswer> prompt, Figure authority)
		where TAnswer : PromptAnswer, new()
	{
		GameController controller = GameController.Instance;
		GameController.SetFastForward(false);
		try
		{
			return await prompt.Decide(authority);
		}
		finally
		{
			// A canceled prompt must not clean up UI belonging to a replacement scene.
			if(ReferenceEquals(GameController.Instance, controller))
			{
				prompt.Cleanup();
			}
		}
	}
}

public class PromptManager
{
	private int _promptIndex = 0;

	private readonly IPromptDecisionSource _decisions;

	public PromptManager(IPromptDecisionSource decisions)
	{
		_decisions = decisions ?? throw new ArgumentNullException(nameof(decisions));
	}

	public event Action<Character> PromptStartedEvent;
	public event Action<Character> PromptEndedEvent;

	public static GDTask<TAnswer> Prompt<TAnswer>(Prompt<TAnswer> prompt, Figure authority)
		where TAnswer : PromptAnswer, new()
	{
		return GameController.Instance.PromptManager._Prompt(prompt, authority);
	}

	public static GDTask<TAnswer> Prompt<TAnswer>(Prompt<TAnswer> prompt, Character authority)
		where TAnswer : PromptAnswer, new()
	{
		return GameController.Instance.PromptManager._Prompt(prompt, authority);
	}

	private async GDTask<TAnswer> _Prompt<TAnswer>(Prompt<TAnswer> prompt, Figure authority)
		where TAnswer : PromptAnswer, new()
	{
		TAnswer answer = null;
		Character characterDecider = authority as Character;

		if(characterDecider == null)
		{
			// Monster authority, let a player decide
			characterDecider = GameController.Instance.CharacterManager.GetCharacter(0);
		}

		PromptStartedEvent?.Invoke(characterDecider);

		bool localDecider = characterDecider != null && characterDecider.IsLocal;

		EffectCollection effectCollection = prompt.EffectCollection;

		if(effectCollection != null)
		{
			await effectCollection.PerformBeforePrompt();
		}

		bool promptValid = !(prompt is ScenarioEventPrompt && effectCollection != null && !effectCollection.HasSelectableEffects);

		if(promptValid)
		{
			while(true)
			{
				if(_promptIndex < GameController.Instance.SavedScenario.PromptAnswers.Count)
				{
					//GameController.SetFastForward(true);

					// Answer has already been decided
					answer = (TAnswer)GameController.Instance.SavedScenario.PromptAnswers[_promptIndex];
				}
				else if(localDecider)
				{
					// Local player needs to make a decision

					answer = await _decisions.Decide(prompt, authority);

					//TODO: Multiplayer sync resign
					await GameController.Instance.CheckEarlyEnd();

					//answer.AuthorityReferenceId = authority.ReferenceId;

					//string serializedData = JsonConvert.SerializeObject(answer, JsonSerializerSettings);

					// if(ScenarioController.Instance.SavedGame.IsOnline)
					// {
					// 	NetworkResult result = await AppController.Instance.NetworkController.AddPromptAnswer(GameController.Instance.SavedGame.Id, promptKey, serializedData, cancellationToken);
					// 	if(!result.Success)
					// 	{
					// 		AppController.Instance.SceneLoader.RequestSceneChange(new MainMenuSceneRequest());
					// 		return answer;
					// 	}
					// }

					GameController.Instance.SavedScenario.PromptAnswers.Add(answer);
				}
				else
				{
					//Log.Error("Whoopsie, online issue");
					throw new Exception("Whoopsie, online issue");
					// Wait for an answer to become available
					// while(!cancellationToken.IsCancellationRequested)
					// {
					// 	SetFastForward(false);
					//
					// 	NetworkResult<SharedLibrary.PromptAnswer> promptAnswerResult = await AppController.Instance.NetworkController.GetPromptAnswer(GameController.Instance.SavedGame.Id, promptKey, cancellationToken);
					//
					// 	if(!promptAnswerResult.Success || promptAnswerResult.Value == null || promptAnswerResult.Value.SerializedData == null)
					// 	{
					// 		await GDTask.Delay(1f, cancellationToken: cancellationToken);
					// 		continue;
					// 	}
					//
					// 	// Answer has been decided
					// 	answer = JsonConvert.DeserializeObject<TAnswer>(promptAnswerResult.Value.SerializedData!, JsonSerializerSettings);
					//
					// 	GameController.Instance.SavedGame.PromptAnswers.Add(promptKey, promptAnswerResult.Value.SerializedData!);
					//
					// 	break;
					// }
				}

				if(!GameController.FastForward)
				{
					AppController.Instance.SaveGame();
				}

				_promptIndex++;

				GameController.Instance.UndoManager.AddStep(new RemovePromptAnswerUndoStep(answer));

				if(answer.SelectedEffectIndex >= 0)
				{
					await prompt.EffectCollection.Effects[answer.SelectedEffectIndex].Apply();

					//TODO: Check if this PerformBeforePrompt call is best placed here.
					await prompt.EffectCollection.PerformBeforePrompt();

					continue;
				}

				if(answer.SyncedAction != null)
				{
					if(answer.SyncedAction.Validate())
					{
						await answer.SyncedAction.Perform();
					}

					continue;
				}

				break;
			}
		}

		if(effectCollection != null)
		{
			await effectCollection.PerformAfterPrompt();
		}

		PromptEndedEvent?.Invoke(characterDecider);

		return answer;
	}
}