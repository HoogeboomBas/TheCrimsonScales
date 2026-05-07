using System;
using Fractural.Tasks;

public class CustomScenarioGoal : ScenarioGoal
{
	private readonly TextHelper.LabelTextDelegate _getLabelText;
	private readonly Func<CustomScenarioGoal, GDTask> _onStart;

	private bool _markedCompleted;

	public override bool HasProgress { get; }

	public override bool Completed => _markedCompleted || Progress >= MaxProgress;

	public CustomScenarioGoal(TextHelper.LabelTextDelegate getLabelText, Func<CustomScenarioGoal, GDTask> onStart = null,
		bool hasProgress = true, int? maxProgress = null,
		int order = 1)
		: base(order)
	{
		_getLabelText = getLabelText;
		_onStart = onStart;

		HasProgress = hasProgress;
		MaxProgress = maxProgress;
	}

	public override string GetLabelText(RichTextParameters textParameters) => _getLabelText(textParameters);

	public override async GDTask Start()
	{
		await base.Start();

		if(_onStart != null)
		{
			await _onStart.Invoke(this);
		}
	}

	public async GDTask Complete()
	{
		_markedCompleted = true;
		//await base.Complete();

		await GDTask.CompletedTask;
	}

	public new async GDTask AdjustProgress(int progress)
	{
		await base.AdjustProgress(progress);
	}

	public new async GDTask SetProgress(int progress)
	{
		await base.SetProgress(progress);
	}

	public new async GDTask SetMaxProgress(int? maxProgress)
	{
		await base.SetMaxProgress(maxProgress);
	}
}