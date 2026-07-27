using System.Collections.Generic;
using Fractural.Tasks;

public partial class Chainguard : Character
{
	public static Shackle Shackle { get; } = ModelDB.Condition<Shackle>();

	// public async GDTask SetMaximumShackles(int maximumShackles)
	// {
	// 	_maximumShackles = maximumShackles;
	//
	// 	await PromptAndRemoveAllButXShackles(_maximumShackles);
	// }

	public override async GDTask OnScenarioSetupCompleted()
	{
		await base.OnScenarioSetupCompleted();

		object subscriber = new object();

		ScenarioEvents.InflictConditionDuplicatesCheckEvent.Subscribe(this, subscriber,
			canApply: parameters => parameters.ConditionModel is Shackle && !parameters.Prevented,
			apply: async parameters =>
			{
				Figure shackler = parameters.PotentialAbilityState?.Performer;
				int shacklesToKeep = GetMaxShackleCount(shackler) - 1;

				await RemoveAllExtraShackles(parameters.PotentialAbilityState?.Performer, shacklesToKeep);
			},
			order: int.MaxValue
		);
	}

	public static int GetMaxShackleCount(Figure shackler)
	{
		return ScenarioCheckEvents.MaxShackleCountCheckEvent.Fire(
			new ScenarioCheckEvents.MaxShackleCountCheck.Parameters(shackler)).MaxShackleCount;
	}

	public static async GDTask RemoveAllExtraShackles(Figure shackler, int shacklesToKeep)
	{
		List<Figure> shackledFigures = GameController.Instance.Map.Figures.FindAll(
			figure => figure.TryGetCondition(Shackle, out Condition condition) && condition.PotentialGiver == shackler);

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

				Figure figure = await AbilityCmd.SelectFigure(shackler,
					figures => figures.AddRange(shackledFigures),
					true,
					hintText: () => $"Select an enemy to lose {Icons.Inline(Icons.GetCondition(Shackle))}, {index}/{shacklesToRemove}");

				await AbilityCmd.RemoveCondition(figure, Shackle);
			}
		}
	}
}
