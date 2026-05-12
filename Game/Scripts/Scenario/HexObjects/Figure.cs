using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public abstract partial class Figure : HexObject, IActionSource
{
	protected Sprite2D _outline;

	private int _shield;
	private bool _shieldExtraValue;

	private int _retaliate;

	private bool _flying;

	private GTween _shieldTween;
	private GTween _retaliateTween;

	private readonly List<ActionState> _otherRoundActionStates = new List<ActionState>();

	public FigureViewComponent FigureViewComponent { get; private set; }

	public int Health { get; private set; }
	public int MaxHealth { get; private set; }

	public List<HexObjectEffectViewBase> Effects { get; } = new List<HexObjectEffectViewBase>();
	public List<Condition> Conditions { get; } = new List<Condition>();
	public List<FigureTrait> Traits { get; } = new List<FigureTrait>();

	public Alignment Alignment { get; private set; }

	public bool TakingTurn { get; private set; }

	public Initiative Initiative { get; private set; }

	public bool CanTakeTurn { get; protected set; }

	public List<Hex> TurnMovedHexes { get; private set; } = new List<Hex>();
	public List<ActionState> TurnPerformedActionStates { get; } = new List<ActionState>();
	public List<ActionState> RoundPerformedActionStates { get; } = new List<ActionState>();

	public abstract string DisplayName { get; }
	public abstract string DebugName { get; }
	public virtual AMDCardDeck AMDCardDeck { get; }
	public abstract Texture2D MapIconTexture { get; }
	public abstract Node2D Visual { get; }

	public Color OutlineColor => _outline.SelfModulate;

	public bool IsDead => IsDestroyed;

	public virtual bool IsFigure => true;

	public event Action<Figure> HealthChangedEvent;
	public event Action<Figure> MaxHealthChangedEvent;
	public event Action<Figure> InitiativeChangedEvent;
	public event Action<Figure> ConditionsChangedEvent;
	public event Action<Figure> DestroyedEvent;

	public override void _Ready()
	{
		base._Ready();

		_outline = GetNode<Sprite2D>("Outline");
		FigureViewComponent = GetViewComponent<FigureViewComponent>();
	}

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);

		FigureViewComponent.Shield.Scale = Vector2.Zero;

		FigureViewComponent.Retaliate.Scale = Vector2.Zero;

		_flying = false;
		FigureViewComponent.Flying.Scale = Vector2.Zero;

		FigureViewComponent.ActivePS.Hide();

		CanTakeTurn = true;

		SetCrackedShield(false);

		object figureEnteredHexEventSubscriber = new object();
		ScenarioEvents.FigureEnteredHexEvent.Subscribe(this, figureEnteredHexEventSubscriber,
			enteredHexParameters => enteredHexParameters.PotentialAbilityState is MoveAbility.State or PullSelfAbility.State,
			async enteredHexParameters =>
			{
				TurnMovedHexes.Add(enteredHexParameters.Hex);

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.ShieldCheckEvent.SubscribersChangedEvent += OnShieldSubscriptionsChanged;
		ScenarioCheckEvents.RetaliateCheckEvent.SubscribersChangedEvent += OnRetaliateSubscriptionsChanged;
		ScenarioCheckEvents.FlyingCheckEvent.SubscribersChangedEvent += OnFlyingSubscriptionsChanged;
		ScenarioCheckEvents.InitiativeCheckEvent.SubscribersChangedEvent += OnInitiativeSubscriptionsChanged;

		OnShieldSubscriptionsChanged();
		OnRetaliateSubscriptionsChanged();
		OnFlyingSubscriptionsChanged();

		await ScenarioEvents.FigureEnteredHexEvent.CreatePrompt(new ScenarioEvents.FigureEnteredHex.Parameters(null, this), this);
		//await AbilityCmd.EnterHex(null, this, this, Hex, false, false);
	}

	public override async GDTask Destroy(bool immediately = false, bool forceDestroy = false)
	{
		await base.Destroy(immediately, forceDestroy);

		await DeactivateOtherRoundActionStates();

		foreach(FigureTrait trait in Traits)
		{
			await trait.Deactivate(this);
		}

		GameController.Instance.Map.DeregisterFigure(this);

		DestroyedEvent?.Invoke(this);

		ScenarioCheckEvents.ShieldCheckEvent.SubscribersChangedEvent -= OnShieldSubscriptionsChanged;
		ScenarioCheckEvents.RetaliateCheckEvent.SubscribersChangedEvent -= OnRetaliateSubscriptionsChanged;
		ScenarioCheckEvents.FlyingCheckEvent.SubscribersChangedEvent -= OnFlyingSubscriptionsChanged;
		ScenarioCheckEvents.InitiativeCheckEvent.SubscribersChangedEvent -= OnInitiativeSubscriptionsChanged;
		//ScenarioCheckEvents.IsMountedCheckEvent.SubscribersChangedEvent -= OnIsMountedSubscriptionsChanged;
	}

	public void SetMaxHealth(int maxHealth)
	{
		if(maxHealth == MaxHealth)
		{
			return;
		}

		MaxHealth = maxHealth;

		FigureViewComponent.Health.TweenPulse(1.4f, 0.2f).PlayFastForwardable();

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

		FigureViewComponent.Health.TweenPulse(1.4f, 0.2f).PlayFastForwardable();
		FigureViewComponent.HealthLabel.Text = health.ToString();

		UpdateHealthProgressBar();

		HealthChangedEvent?.Invoke(this);
	}

	public bool IsDamaged()
	{
		return Health < MaxHealth;
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

	public async GDTask TakeFullTurn()
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
			FigureViewComponent.TurnStartPS.SetEmitting(true);

			await GDTask.DelayFastForwardable(0.5f);
		}

		TakingTurn = true;
		TurnMovedHexes.Clear();
		TurnPerformedActionStates.Clear();

		FigureViewComponent.ActivePS.Show();
		FigureViewComponent.ActivePS.TweenModulateAlpha(0f, 0f).Play(true);
		FigureViewComponent.ActivePS.TweenModulateAlpha(1f, 0.2f).PlayFastForwardable();

		await ScenarioEvents.FigureTurnStartedEvent.CreatePrompt(
			new ScenarioEvents.FigureTurnStarted.Parameters(this), this);
	}

	protected virtual async GDTask TakeTurn()
	{
		await GDTask.CompletedTask;
	}

	protected virtual async GDTask EndTurn()
	{
		await ScenarioEvents.FigureTurnEndingEvent.CreatePrompt(
			new ScenarioEvents.FigureTurnEnding.Parameters(this), this);

		// Little hack here to make sure looting is performed at the right time
		if(Hex != null)
		{
			await EndOfTurnLooting();
		}

		await ScenarioEvents.FigureTurnEndedConditionsFallOffEvent.CreatePrompt(
			new ScenarioEvents.FigureTurnEndedConditionsFallOff.Parameters(this), this);

		await ScenarioEvents.FigureTurnEndedEvent.CreatePrompt(
			new ScenarioEvents.FigureTurnEnded.Parameters(this), this);

		TakingTurn = false;
		CanTakeTurn = false;

		await GameController.Instance.ElementManager.FinishInfusing();

		FigureViewComponent.ActivePS.TweenModulateAlpha(0f, 0.2f).OnComplete(FigureViewComponent.ActivePS.Hide).PlayFastForwardable();
	}

	protected virtual async GDTask EndOfTurnLooting()
	{
		await GDTask.CompletedTask;
	}

	public void AddOtherRoundActionState(ActionState actionState)
	{
		_otherRoundActionStates.Add(actionState);
	}

	public async GDTask DeactivateOtherRoundActionState(ActionState actionState)
	{
		await actionState.RemoveFromActive();
	}

	public async GDTask DeactivateOtherRoundActionStates()
	{
		for(int i = _otherRoundActionStates.Count - 1; i >= 0; i--)
		{
			ActionState actionState = _otherRoundActionStates[i];
			await DeactivateOtherRoundActionState(actionState);
		}
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

	public Condition GetCondition(ConditionModel conditionModel)
	{
		foreach(Condition condition in Conditions)
		{
			if(condition.ConditionModel == conditionModel)
			{
				return condition;
			}
		}

		return null;
	}

	public bool TryGetCondition(ConditionModel conditionModel, out Condition condition)
	{
		condition = GetCondition(conditionModel);
		return condition != null;
	}

	public async GDTask<Condition> AddCondition(ConditionModel conditionModel, Figure potentialCauser)
	{
		ConditionsChangedEvent?.Invoke(this);

		Condition condition = new Condition(conditionModel, this, potentialCauser);
		Conditions.Add(condition);
		await condition.OnAdded();

		return condition;
	}

	public async GDTask<Condition> AddConditionStack(ConditionModel conditionModel)
	{
		foreach(Condition condition in Conditions)
		{
			if(condition.ConditionModel == conditionModel)
			{
				condition.AdjustStackCount(1);
				return condition;
			}
		}

		await GDTask.CompletedTask;
		return null;
	}

	public async GDTask RemoveCondition(Condition condition)
	{
		ConditionsChangedEvent?.Invoke(this);

		await condition.OnRemoved();
		Conditions.Remove(condition);
	}

	public async GDTask AddTrait(FigureTrait trait)
	{
		FigureTrait mutableTrait = trait.ToMutable();
		Traits.Add(mutableTrait);
		await mutableTrait.Activate(this);
	}

	public T AddEffectView<T>(HexObjectEffectViewParameters parameters)
		where T : HexObjectEffectViewBase
	{
		HexObjectEffectViewBase effectView = ResourceLoader.Load<PackedScene>(parameters.ScenePath).Instantiate<HexObjectEffectViewBase>();
		FigureViewComponent.EffectParent.AddChild(effectView);
		effectView.Init(parameters);
		Effects.Add(effectView);

		ReorderEffects();

		return (T)effectView;
	}

	public void RemoveEffectView(HexObjectEffectViewBase effectView)
	{
		Effects.Remove(effectView);
		effectView.Destroy();

		ReorderEffects();
	}

	protected void SetAlignment(Alignment alignment)
	{
		Alignment = alignment;
	}

	public bool AlliedWith(Figure figure, bool canBeSelf = false)
	{
		FigureRelationship relationship = GetRelationship(figure);
		return relationship == FigureRelationship.AlliedWith || (relationship == FigureRelationship.Self && canBeSelf);
	}

	public bool EnemiesWith(Figure figure)
	{
		return GetRelationship(figure) == FigureRelationship.EnemiesWith;
	}

	public FigureRelationship GetRelationship(Figure figure)
	{
		if(figure == null)
		{
			return FigureRelationship.Undefined;
		}

		if(figure == this)
		{
			return FigureRelationship.Self;
		}

		ScenarioCheckEvents.FigureRelationshipCheck.Parameters relationshipCheckParameters =
			ScenarioCheckEvents.FigureRelationshipCheckEvent.Fire(
				new ScenarioCheckEvents.FigureRelationshipCheck.Parameters(this, figure));

		return relationshipCheckParameters.FigureRelationship;
	}

	public virtual void AddCoin()
	{
	}

	public virtual void RemoveCoin()
	{
	}

	protected abstract Initiative GetInitiative();

	public virtual async GDTask RoundEnd()
	{
		CanTakeTurn = true;
		RoundPerformedActionStates.Clear();

		await DeactivateOtherRoundActionStates();
	}

	public void SetCrackedShield(bool crackedShield)
	{
		FigureViewComponent.ShieldIcon.SetVisible(!crackedShield);
		FigureViewComponent.CrackedShieldIcon.SetVisible(crackedShield);
	}

	private void UpdateHealthProgressBar()
	{
		float t = (float)Health / MaxHealth;
		float fill = FigureViewComponent.HealthProgressBarCurve.Sample(t);
		FigureViewComponent.HealthProgressBar.SetValue(fill);
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

	private void OnInitiativeSubscriptionsChanged()
	{
		UpdateInitiative();
	}

	private void SetShield(int shield, bool extraValue)
	{
		if(shield == _shield && extraValue == _shieldExtraValue)
		{
			return;
		}

		string plus = extraValue ? "+" : string.Empty;
		FigureViewComponent.ShieldLabel.Text = $"{shield}{plus}";

		bool wasVisible = _shield != 0 || _shieldExtraValue;
		bool shouldBeVisible = shield != 0 || extraValue;

		_shieldTween?.Complete();
		if(!wasVisible && shouldBeVisible)
		{
			FigureViewComponent.Shield.Show();
			_shieldTween = FigureViewComponent.Shield
				.TweenScale(1f, 0.2f)
				.SetEasing(Easing.OutBack)
				.PlayFastForwardable();
		}
		else if(wasVisible && !shouldBeVisible)
		{
			_shieldTween = FigureViewComponent.Shield
				.TweenScale(0f, 0.2f)
				.OnComplete(FigureViewComponent.Shield.Hide)
				.SetEasing(Easing.InBack)
				.PlayFastForwardable();
		}
		else
		{
			_shieldTween = FigureViewComponent.Shield.TweenPulse(1.4f, 0.2f).PlayFastForwardable();
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

		FigureViewComponent.RetaliateLabel.Text = $"{retaliate}";

		bool wasVisible = _retaliate != 0;
		bool shouldBeVisible = retaliate != 0;

		_retaliateTween?.Complete();
		if(!wasVisible && shouldBeVisible)
		{
			FigureViewComponent.Retaliate.Show();
			_retaliateTween = FigureViewComponent.Retaliate.TweenScale(1f, 0.2f).SetEasing(Easing.OutBack).PlayFastForwardable();
		}
		else if(wasVisible && !shouldBeVisible)
		{
			_retaliateTween = FigureViewComponent.Retaliate.TweenScale(0f, 0.2f).OnComplete(FigureViewComponent.Retaliate.Hide)
				.SetEasing(Easing.InBack).PlayFastForwardable();
		}
		else
		{
			_retaliateTween = FigureViewComponent.Retaliate.TweenPulse(1.4f, 0.2f).PlayFastForwardable();
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
			FigureViewComponent.Flying.TweenScale(1f, 0.2f).SetEasing(Easing.OutBack).PlayFastForwardable();
		}
		else if(wasVisible && !shouldBeVisible)
		{
			FigureViewComponent.Flying.TweenScale(0f, 0.2f).SetEasing(Easing.InBack).PlayFastForwardable();
		}
		else
		{
			FigureViewComponent.Flying.TweenPulse(1.4f, 0.2f).PlayFastForwardable();
		}

		_flying = flying;
	}

	private void ReorderEffects()
	{
		int effectCount = Effects.Count;
		int index = 0;
		const float maxOffset = 50f;
		foreach(HexObjectEffectViewBase effect in Effects)
		{
			float progress = (index + 1f) / (effectCount + 1);
			float posY = Mathf.Lerp(-maxOffset, maxOffset, progress);
			effect.Move(new Vector2(0f, posY));
			FigureViewComponent.EffectParent.MoveChild(effect, index);

			index++;
		}
	}

	public void SetTakingTurn(bool takingTurn)
	{
		TakingTurn = takingTurn;
	}
}