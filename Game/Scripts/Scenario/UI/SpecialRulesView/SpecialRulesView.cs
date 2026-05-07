using System.Collections.Generic;
using Godot;

public partial class SpecialRulesView : Control
{
	[Export]
	private PackedScene _ruleScene;
	[Export]
	private Control _ruleParent;
	[Export]
	private ScrollContainer _scrollContainer;
	[Export]
	private int _maxScrollContainerSize;

	private readonly List<SpecialRulesViewRule> _rules = new List<SpecialRulesViewRule>();

	public override void _Ready()
	{
		base._Ready();

		GameController.Instance.ScenarioModel.RuleAddedEvent += OnRuleAdded;
		GameController.Instance.ScenarioModel.RuleRemovedEvent += OnRuleRemoved;

		UpdateView();
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		if(GameController.Instance != null && GameController.Instance.ScenarioModel != null)
		{
			GameController.Instance.ScenarioModel.RuleAddedEvent -= OnRuleAdded;
			GameController.Instance.ScenarioModel.RuleRemovedEvent -= OnRuleRemoved;
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		float sizeY = Mathf.Min(_ruleParent.Size.Y, _maxScrollContainerSize);
		_scrollContainer.SetCustomMinimumSize(new Vector2(_scrollContainer.Size.X, sizeY));
	}

	private void UpdateView()
	{
		SetVisible(_rules.Count > 0);

		_rules.Sort((ruleA, ruleB) => ruleA.Rule.Order.CompareTo(ruleB.Rule.Order));
		for(int i = 0; i < _rules.Count; i++)
		{
			SpecialRulesViewRule rule = _rules[i];
			_ruleParent.MoveChild(rule, i);
		}
	}

	private void OnRuleAdded(ScenarioRule scenarioRule)
	{
		SpecialRulesViewRule rule = _ruleScene.Instantiate<SpecialRulesViewRule>();
		_ruleParent.AddChild(rule);
		rule.Init(scenarioRule);
		_rules.Add(rule);

		UpdateView();
	}

	private void OnRuleRemoved(ScenarioRule scenarioRule)
	{
		for(int i = _rules.Count - 1; i >= 0; i--)
		{
			SpecialRulesViewRule rule = _rules[i];
			if(rule.Rule == scenarioRule)
			{
				rule.QueueFree();
				_rules.RemoveAt(i);
				break;
			}
		}

		UpdateView();
	}
}