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
				((Shackle)parameters.Condition).SetShackler(parameters.PotentialAbilityState.Performer);

				int shacklesToKeep = _maximumShackles - 1;

				await PromptAndRemoveAllButXShackles(shacklesToKeep);
			}
		);
	}

	private async GDTask PromptAndRemoveAllButXShackles(int shacklesToKeep)
	{
		List<Figure> shackledFigures = GameController.Instance.Map.Figures.FindAll(figure => figure.HasCondition(Shackle));
		int shacklesToRemove = shackledFigures.Count - shacklesToKeep;

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
				int index = extraShacklesIndex;

				Figure figure = await AbilityCmd.SelectFigure(this,
					figures => figures.AddRange(shackledFigures),
					true,
					hintText: () => $"Select an enemy to lose {Icons.Inline(Icons.GetCondition(Shackle))}, {index}/{shacklesToRemove}");

				await AbilityCmd.RemoveCondition(figure, Shackle);
			}
		}
	}
}