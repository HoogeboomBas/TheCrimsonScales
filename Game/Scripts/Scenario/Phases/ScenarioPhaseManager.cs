using System;
using System.Threading;
using System.Threading.Tasks;
using Fractural.Tasks;
using Godot;

/// <summary>
/// Runs the setup phases and then cycles through card selection and scenario rounds.
/// </summary>
public class ScenarioPhaseManager
{
	public ScenarioPhase ActivePhase { get; private set; }

	public int RoundIndex { get; private set; }

	public async GDTaskVoid Play()
	{
		try
		{
			await PlayTask(GameController.Instance.DestroyCancellationToken);
		}
		catch(TaskCanceledException e)
		{
			Log.Write(e.ToString());
		}
		catch(OperationCanceledException e)
		{
			Log.Write(e.ToString());
		}
	}

	private async GDTask PlayTask(CancellationToken cancellationToken)
	{
		await ActivatePhase(new ScenarioInitializationPhase(), cancellationToken);
		await ActivatePhase(new ScenarioSetupPhase(), cancellationToken);

		while(true)
		{
			await ActivatePhase(new CardSelectionPhase(), cancellationToken);
			await ActivatePhase(new RoundPhase(), cancellationToken);

			RoundIndex++;
		}
	}

	private async GDTask ActivatePhase(ScenarioPhase phase, CancellationToken cancellationToken)
	{
		ActivePhase = phase;

		await phase.Activate();
	}
}