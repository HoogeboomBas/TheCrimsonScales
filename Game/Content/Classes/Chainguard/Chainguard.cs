using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public partial class Chainguard : Character
{
	public static Shackle Shackle { get; } = ModelDB.Condition<Shackle>();

	private int _maximumShackles = 1;

	public async GDTask SetMaximumShackles(int maximumShackles)
	{
		_maximumShackles = maximumShackles;

		await PromptAndRemoveAllButXShackles(_maximumShackles);
	}

	public override async GDTask OnScenarioSetupCompleted()
	{
		await base.OnScenarioSetupCompleted();

		object subscriber = new();

		ScenarioEvents.InflictConditionEvent.Subscribe(this, subscriber,
			canApply: parameters => parameters.Condition is Shackle,
			apply: async parameters =>
			{
				((Shackle)parameters.Condition).AddShackler(parameters.PotentialAbilityState.Performer);

				int shacklesToKeep = _maximumShackles - 1;

				await PromptAndRemoveAllButXShackles(shacklesToKeep);
			}
		);
	}

	private async GDTask PromptAndRemoveAllButXShackles(int shacklesToKeep)
	{
		IEnumerable<Figure> shackledFigures = GameController.Instance.Map.Figures.FindAll(figure => figure.HasCondition(Shackle));
		int shacklesToRemove = shackledFigures.Count() - shacklesToKeep;

		if(shacklesToKeep == 0)
		{
			foreach(Figure figure in shackledFigures)
			{
				await AbilityCmd.RemoveCondition(figure, Shackle);
			}
		}
		else
		{
			for(int extraShacklesIndex = 1; extraShacklesIndex <= shacklesToRemove; extraShacklesIndex++)
			{
				TargetSelectionPrompt.Answer targetAnswer = await PromptManager.Prompt(
					new TargetSelectionPrompt(figures => figures.AddRange(shackledFigures), 
						true, true, null, 
						() => $"Select an enemy to lose {Icons.Inline(Icons.GetCondition(Shackle))}, {extraShacklesIndex}/{shacklesToRemove}"), 
					this);

				await AbilityCmd.RemoveCondition(GameController.Instance.ReferenceManager.Get<Figure>(targetAnswer.FigureReferenceId), Shackle);

				extraShacklesIndex++;
			}
		}
	}
}