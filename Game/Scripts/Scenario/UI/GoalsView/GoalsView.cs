using System.Collections.Generic;
using Godot;

public partial class GoalsView : Control
{
	[Export]
	private PackedScene _goalScene;
	[Export]
	private Control _goalParent;

	private readonly List<GoalsViewGoal> _goals = new List<GoalsViewGoal>();

	public override void _Ready()
	{
		base._Ready();

		GameController.Instance.ScenarioModel.GoalAddedEvent += OnGoalAdded;
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		if(GameController.Instance != null && GameController.Instance.ScenarioModel != null)
		{
			GameController.Instance.ScenarioModel.GoalAddedEvent -= OnGoalAdded;
		}
	}

	private void OnGoalAdded(ScenarioGoal scenarioGoal)
	{
		GoalsViewGoal goal = _goalScene.Instantiate<GoalsViewGoal>();
		_goalParent.AddChild(goal);
		goal.Init(scenarioGoal);
		_goals.Add(goal);

		_goals.Sort((goalA, goalB) => goalA.Goal.Order.CompareTo(goalB.Goal.Order));
		for(int i = 0; i < _goals.Count; i++)
		{
			GoalsViewGoal otherGoal = _goals[i];
			_goalParent.MoveChild(otherGoal, i);
		}
	}
}