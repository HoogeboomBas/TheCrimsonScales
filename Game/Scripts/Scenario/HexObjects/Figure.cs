using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public abstract partial class Figure : HexObject
{
	protected FigureViewComponent _figureViewComponent;

	private int _shield;
	private bool _shieldExtraValue;

	private int _retaliate;

	private bool _flying;

	private GTween _shieldTween;
	private GTween _retaliateTween;

	public abstract string DisplayName { get; }
	public abstract string DebugName { get; }

	public int Health { get; private set; }
	public int MaxHealth { get; private set; }

	public List<ConditionModel> Conditions { get; } = new List<ConditionModel>();

	public Alignment Alignment { get; private set; }
	public Alignment Enemies { get; private set; }

	public bool TakingTurn { get; private set; }

	public Initiative Initiative { get; private set; }

	public bool CanTakeTurn { get; protected set; }

	public abstract AMDCardDeck AMDCardDeck { get; }

	public int TurnMovedHexCount { get; private set; }
	public List<ActionState> TurnPerformedActionStates { get; } = new List<ActionState>();

	public Color OutlineColor => _figureViewComponent.Outline.SelfModulate;

	public bool IsDead => IsDestroyed;

	public event Action<Figure> HealthChangedEvent;
	public event Action<Figure> MaxHealthChangedEvent;
	public event Action<Figure> InitiativeChangedEvent;
	public event Action<Figure> ConditionsChangedEvent;
	public event Action<Figure> DestroyedEvent;

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);

		_figureViewComponent = GetViewComponent<FigureViewComponent>();

		_figureViewComponent.Shield.Scale = Vector2.Zero;

		_figureViewComponent.Retaliate.Scale = Vector2.Zero;

		_flying = false;
		_figureViewComponent.Flying.Scale = Vector2.Zero;

		_figureViewComponent.ActivePS.Hide();

		CanTakeTurn = true;

		object figureEnteredHexEventSubscriber = new object();
		ScenarioEvents.FigureEnteredHexEvent.Subscribe(this, figureEnteredHexEventSubscriber,
			enteredHexParameters => enteredHexParameters.AbilityState is MoveAbility.State or PullSelfAbility.State,
			async enteredHexParameters =>
			{
				TurnMovedHexCount++;

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.ShieldCheckEvent.SubscribersChangedEvent += OnShieldSubscriptionsChanged;
		ScenarioCheckEvents.RetaliateCheckEvent.SubscribersChangedEvent += OnRetaliateSubscriptionsChanged;
		ScenarioCheckEvents.FlyingCheckEvent.SubscribersChangedEvent += OnFlyingSubscriptionsChanged;

		OnShieldSubscriptionsChanged();
		OnRetaliateSubscriptionsChanged();
		OnFlyingSubscriptionsChanged();
	}

	public override async GDTask Destroy(bool immediately = false, bool forceDestroy = false)
	{
		await base.Destroy(immediately, forceDestroy);

		GameController.Instance.Map.DeregisterFigure(this);

		DestroyedEvent?.Invoke(this);

		ScenarioCheckEvents.ShieldCheckEvent.SubscribersChangedEvent -= OnShieldSubscriptionsChanged;
		ScenarioCheckEvents.RetaliateCheckEvent.SubscribersChangedEvent -= OnRetaliateSubscriptionsChanged;
		ScenarioCheckEvents.FlyingCheckEvent.SubscribersChangedEvent -= OnFlyingSubscriptionsChanged;
	}

	public void SetMaxHealth(int maxHealth)
	{
		if(maxHealth == MaxHealth)
		{
			return;
		}

		MaxHealth = maxHealth;

		_figureViewComponent.Health.TweenPulse(1.4f, 0.2f).PlayFastForwardable();

		UpdateHealthProgressBar();

		MaxHealthChangedEvent?.Invoke(this);
	}

	public void SetHealth(int health)
	{
		if(health == Health)
		{
			return;
		}

		Health = health;

		_figureViewComponent.Health.TweenPulse(1.4f, 0.2f).PlayFastForwardable();
		_figureViewComponent.HealthLabel.Text = health.ToString();

		UpdateHealthProgressBar();

		HealthChangedEvent?.Invoke(this);
	}

	public virtual void UpdateInitiative()
	{
		Initiative oldInitiative = Initiative;

		Initiative = GetInitiative();
		ScenarioCheckEvents.InitiativeCheck.Parameters parameters =
			ScenarioCheckEvents.InitiativeCheckEvent.Fire(new ScenarioCheckEvents.InitiativeCheck.Parameters(this, Initiative));
		Initiative = parameters.Initiative;

		if(!Initiative.Equals(oldInitiative))
		{
			InitiativeChangedEvent?.Invoke(this);
		}
	}

	public virtual async GDTask TakeFullTurn()
	{
		if(!IsDead)
		{
			await StartTurn();
		}

		if(!IsDead)
		{
			await TakeTurn();
		}

		if(!IsDead)
		{
			await EndTurn();
		}

		await GDTask.DelayFastForwardable(0.5f);
	}

	private async GDTask StartTurn()
	{
		if(!GameController.FastForward)
		{
			Log.Write($"Started turn of {DisplayName}.");
		}

		if(!GameController.FastForward)
		{
			_figureViewComponent.TurnStartPS.SetEmitting(true);

			await GDTask.DelayFastForwardable(0.5f);
		}

		TakingTurn = true;
		TurnMovedHexCount = 0;
		TurnPerformedActionStates.Clear();

		_figureViewComponent.ActivePS.Show();
		_figureViewComponent.ActivePS.TweenModulateAlpha(0f, 0f).Play(true);
		_figureViewComponent.ActivePS.TweenModulateAlpha(1f, 0.2f).PlayFastForwardable();

		await ScenarioEvents.FigureTurnStartedEvent.CreatePrompt(
			new ScenarioEvents.FigureTurnStarted.Parameters(this), this);
	}

	protected virtual async GDTask TakeTurn()
	{
		await GDTask.CompletedTask;
	}

	protected async GDTask EndTurn()
	{
		await ScenarioEvents.FigureTurnEndingEvent.CreatePrompt(
			new ScenarioEvents.FigureTurnEnding.Parameters(this), this);

		// Little hack here to make sure looting is performed at the right time
		await EndOfTurnLooting();

		await ScenarioEvents.FigureTurnEndedConditionsFallOffEvent.CreatePrompt(
			new ScenarioEvents.FigureTurnEndedConditionsFallOff.Parameters(this), this);

		await ScenarioEvents.FigureTurnEndedEvent.CreatePrompt(
			new ScenarioEvents.FigureTurnEnded.Parameters(this), this);

		TakingTurn = false;
		CanTakeTurn = false;

		GameController.Instance.ElementManager.FinishInfusing();

		_figureViewComponent.ActivePS.TweenModulateAlpha(0f, 0.2f).OnComplete(_figureViewComponent.ActivePS.Hide).PlayFastForwardable();
	}

	protected virtual async GDTask EndOfTurnLooting()
	{
		await GDTask.CompletedTask;
	}

	public bool HasCondition(ConditionModel conditionModel)
	{
		return GetCondition(conditionModel) != null;
	}

	public bool HasPoison()
	{
		return
			HasCondition(global::Conditions.Poison1) || HasCondition(global::Conditions.Poison2) ||
			HasCondition(global::Conditions.Poison3) || HasCondition(global::Conditions.Poison4);
	}

	public bool HasWound()
	{
		return HasCondition(global::Conditions.Wound1) || HasCondition(global::Conditions.Wound2);
	}

	// public bool HasInvisible()
	// {
	// 	return HasCondition(global::Conditions.Invisible);
	// }

	public ConditionModel GetCondition(ConditionModel conditionModel)
	{
		foreach(ConditionModel condition in Conditions)
		{
			if(condition.ImmutableInstance == conditionModel)
			{
				return condition;
			}
		}

		return null;
	}

	public async GDTask<ConditionNode> AddCondition(ConditionModel condition)
	{
		ConditionNode conditionNode = null;
		if(condition.ShowOnFigure)
		{
			conditionNode = ResourceLoader.Load<PackedScene>("res://Scenes/Scenario/Condition.tscn").Instantiate<ConditionNode>();
			_figureViewComponent.ConditionParent.AddChild(conditionNode);
			conditionNode.Init(condition);
		}

		Conditions.Add(condition);
		//ConditionNodes.Add(condition, conditionNode);

		ConditionsChangedEvent?.Invoke(this);

		await condition.Add(this, conditionNode);

		ReorderConditions();

		return conditionNode;
	}

	public async GDTask RemoveCondition(ConditionModel conditionModel)
	{
		ConditionModel condition = GetCondition(conditionModel);
		ConditionNode node = condition.Node;
		node?.Destroy();
		Conditions.Remove(condition);

		ConditionsChangedEvent?.Invoke(this);

		await condition.Remove();

		ReorderConditions();
	}

	public void SetAlignment(Alignment alignment)
	{
		Alignment = alignment;
	}

	public void SetEnemies(Alignment alignment)
	{
		Enemies = alignment;
	}

	public bool AlliedWith(Figure figure, bool canBeSelf = false)
	{
		if(figure == null)
		{
			return false;
		}

		if(!canBeSelf && figure == this)
		{
			return false;
		}

		return Alignment.HasFlag(figure.Alignment);
	}

	public bool EnemiesWith(Figure figure)
	{
		if(figure == null)
		{
			return false;
		}

		return Enemies.HasFlag(figure.Alignment);
	}

	public virtual void AddCoin()
	{
	}

	public virtual void RemoveCoin()
	{
	}

	protected abstract Initiative GetInitiative();

	public void RoundEnd()
	{
		CanTakeTurn = true;
	}

	private void UpdateHealthProgressBar()
	{
		_figureViewComponent.HealthProgressBar.Value = (float)Health / MaxHealth;
	}

	private void OnShieldSubscriptionsChanged()
	{
		ScenarioCheckEvents.ShieldCheck.Parameters parameters =
			ScenarioCheckEvents.ShieldCheckEvent.Fire(new ScenarioCheckEvents.ShieldCheck.Parameters(this));

		SetShield(parameters.Shield, parameters.ExtraValue);
	}

	private void OnRetaliateSubscriptionsChanged()
	{
		ScenarioCheckEvents.RetaliateCheck.Parameters parameters =
			ScenarioCheckEvents.RetaliateCheckEvent.Fire(new ScenarioCheckEvents.RetaliateCheck.Parameters(this));

		int finalRetaliate = 0;
		foreach((int retaliate, int range) in parameters.RetaliateValues)
		{
			finalRetaliate += retaliate;
		}

		SetRetaliate(finalRetaliate);
	}

	private void OnFlyingSubscriptionsChanged()
	{
		ScenarioCheckEvents.FlyingCheck.Parameters parameters =
			ScenarioCheckEvents.FlyingCheckEvent.Fire(new ScenarioCheckEvents.FlyingCheck.Parameters(this));

		SetFlying(parameters.HasFlying);
	}

	private void SetShield(int shield, bool extraValue)
	{
		if(shield == _shield && extraValue == _shieldExtraValue)
		{
			return;
		}

		string plus = extraValue ? "+" : string.Empty;
		_figureViewComponent.ShieldLabel.Text = $"{shield}{plus}";

		bool wasVisible = _shield != 0 || _shieldExtraValue;
		bool shouldBeVisible = shield != 0 || extraValue;

		_shieldTween?.Complete();
		if(!wasVisible && shouldBeVisible)
		{
			_figureViewComponent.Shield.Show();
			_shieldTween = _figureViewComponent.Shield.TweenScale(1f, 0.2f).SetEasing(Easing.OutBack).PlayFastForwardable();
		}
		else if(wasVisible && !shouldBeVisible)
		{
			_shieldTween = _figureViewComponent.Shield.TweenScale(0f, 0.2f).OnComplete(_figureViewComponent.Shield.Hide).SetEasing(Easing.InBack).PlayFastForwardable();
		}
		else
		{
			_shieldTween = _figureViewComponent.Shield.TweenPulse(1.4f, 0.2f).PlayFastForwardable();
		}

		_shield = shield;
		_shieldExtraValue = extraValue;
	}

	private void SetRetaliate(int retaliate)
	{
		if(retaliate == _retaliate)
		{
			return;
		}

		_figureViewComponent.RetaliateLabel.Text = $"{retaliate}";

		bool wasVisible = _retaliate != 0;
		bool shouldBeVisible = retaliate != 0;

		_retaliateTween?.Complete();
		if(!wasVisible && shouldBeVisible)
		{
			_figureViewComponent.Retaliate.Show();
			_retaliateTween = _figureViewComponent.Retaliate.TweenScale(1f, 0.2f).SetEasing(Easing.OutBack).PlayFastForwardable();
		}
		else if(wasVisible && !shouldBeVisible)
		{
			_retaliateTween = _figureViewComponent.Retaliate.TweenScale(0f, 0.2f).OnComplete(_figureViewComponent.Retaliate.Hide).SetEasing(Easing.InBack).PlayFastForwardable();
		}
		else
		{
			_retaliateTween = _figureViewComponent.Retaliate.TweenPulse(1.4f, 0.2f).PlayFastForwardable();
		}

		_retaliate = retaliate;
	}

	private void SetFlying(bool flying)
	{
		if(flying == _flying)
		{
			return;
		}

		bool wasVisible = _flying;
		bool shouldBeVisible = flying;

		if(!wasVisible && shouldBeVisible)
		{
			_figureViewComponent.Flying.TweenScale(1f, 0.2f).SetEasing(Easing.OutBack).PlayFastForwardable();
		}
		else if(wasVisible && !shouldBeVisible)
		{
			_figureViewComponent.Flying.TweenScale(0f, 0.2f).SetEasing(Easing.InBack).PlayFastForwardable();
		}
		else
		{
			_figureViewComponent.Flying.TweenPulse(1.4f, 0.2f).PlayFastForwardable();
		}

		_flying = flying;
	}

	private void ReorderConditions()
	{
		List<ConditionNode> nodes = Conditions.Where(condition => condition.Node != null).Select(condition => condition.Node).ToList();

		int conditionCount = nodes.Count;
		int index = 0;
		const float maxOffset = 50f;
		foreach(ConditionNode node in nodes)
		{
			float progress = (index + 1f) / (conditionCount + 1);
			float posY = Mathf.Lerp(-maxOffset, maxOffset, progress);
			node.Move(new Vector2(0f, posY));
			_figureViewComponent.ConditionParent.MoveChild(node, index);

			index++;
		}
	}
}