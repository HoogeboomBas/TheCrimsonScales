using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class Spirit : Figure
{
	[Export]
	private Node2D _container;
	[Export]
	private Node2D _mask;

	private SpiritViewComponent _spiritViewComponent;
	private string _name;
	private readonly List<Ability> _abilities = new List<Ability>();

	private ActionState _turnActionState;

	private bool _inCorner;

	public int HealthStat { get; private set; }
	public int? Move { get; private set; }
	public int? Attack { get; private set; }
	public int? Range { get; private set; }
	public Character CharacterOwner { get; private set; }
	public Texture2D PortraitTexture { get; private set; }
	public int SpiritIndex { get; private set; }

	public override string DisplayName => _name;
	public override string DebugName => _name;
	public override AMDCardDeck AMDCardDeck => CharacterOwner.AMDCardDeck;
	public override Texture2D MapIconTexture => _spiritViewComponent.Sprite.Texture;
	public override Node2D Visual => _spiritViewComponent.Sprite;

	public override bool IsFigure => false;

	public RangeType RangeType => Range.HasValue ? RangeType.Range : RangeType.Melee;

	public override void _Ready()
	{
		base._Ready();

		_spiritViewComponent = GetViewComponent<SpiritViewComponent>();

		_mask.Reparent(_container, false);
		_outline.Reparent(_container, false);
		FigureViewComponent.Reparent(_container, false);
		_spiritViewComponent.Reparent(_container, false);
	}

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);
	}

	public async GDTask Spawn(int health, int? move, int? attack, int? range, FigureTrait[] traits, Character characterOwner, string name,
		string portraitTexturePath, string mapIconTexturePath)
	{
		HealthStat = health;
		Move = move;
		Attack = attack;
		Range = range;

		CharacterOwner = characterOwner;
		_name = name;

		_outline.SetSelfModulate(CharacterOwner.OutlineColor);
		FigureViewComponent.TurnStartPS.SetSelfModulate(OutlineColor);
		FigureViewComponent.ActivePS.SetModulate(OutlineColor);

		_spiritViewComponent.StandeeNumberCircle.SetSelfModulate(OutlineColor);

		PortraitTexture = ResourceLoader.Load<Texture2D>(portraitTexturePath);
		Texture2D mapIconTexture = ResourceLoader.Load<Texture2D>(mapIconTexturePath);
		_spiritViewComponent.Sprite.SetTexture(mapIconTexture);
		float textureWidth = mapIconTexture.GetWidth();
		_spiritViewComponent.Sprite.SetScale((250f / textureWidth) * Vector2.One);

		SetMaxHealth(HealthStat);
		SetHealth(HealthStat);

		SetAlignment(Alignment.Custom);
		ScenarioCheckEvents.FigureRelationshipCheckEvent.Subscribe(this, CharacterOwner,
			parameters => parameters.Figure == this || parameters.OtherFigure == this,
			parameters =>
			{
				if(parameters.Figure == this)
				{
					if(parameters.OtherFigure.Alignment == Alignment.Characters)
					{
						parameters.SetAlliedWith();
						return;
					}
					else if(parameters.OtherFigure.Alignment is Alignment.Monsters or Alignment.Other)
					{
						parameters.SetEnemiesWith();
						return;
					}
				}

				if(parameters.OtherFigure == this)
				{
					parameters.SetFigureRelationship(FigureRelationship.Undefined);
				}
			}
		);

		if(traits != null)
		{
			foreach(FigureTrait trait in traits)
			{
				await AddTrait(trait);
			}
		}

		RegisterSpirit();

		await GameController.Instance.Map.RegisterFigure(this);

		UpdateInitiative();

		if(Move.HasValue)
		{
			MoveAbility moveAbility = MoveAbility.Builder().WithDistance(Move.Value).Build();
			_abilities.Add(moveAbility);
		}

		if(Attack.HasValue)
		{
			AttackAbility attackAbility = AttackAbility.Builder()
				.WithDamage(Attack.Value)
				.WithRange(Range ?? 1)
				.WithMinRange(1)
				.WithRangeType(RangeType)
				.Build();
			_abilities.Add(attackAbility);
		}

		ScenarioEvents.HexObjectDestroyedEvent.Subscribe(this, CharacterOwner,
			parameters =>
				parameters.HexObject == CharacterOwner ||
				parameters.HexObject.Hex == Hex,
			async parameters =>
			{
				if(parameters.HexObject == CharacterOwner)
				{
					await Destroy(parameters.Immediately, parameters.ForceDestroy);
				}
				else
				{
					await UpdateInCorner();
				}
			}
		);

		ScenarioEvents.FigureEnteredHexEvent.Subscribe(this, CharacterOwner,
			parameters =>
				parameters.Hex == Hex ||
				parameters.Figure == this,
			async parameters =>
			{
				await UpdateInCorner();
			}
		);

		ScenarioEvents.FigureExitingHexEvent.Subscribe(this, CharacterOwner,
			parameters =>
				parameters.Hex == Hex,
			async parameters =>
			{
				await UpdateInCorner(parameters.Figure);
			}
		);

		ScenarioEvents.RetaliateEvent.Subscribe(this, CharacterOwner,
			parameters => parameters.AbilityState.Performer == this,
			async parameters =>
			{
				parameters.SetRetaliateBlocked();
				await GDTask.CompletedTask;
			});

		ScenarioCheckEvents.FlyingCheckEvent.Subscribe(this, CharacterOwner,
			parameters => parameters.Figure == this,
			parameters => parameters.SetFlying(true)
		);

		ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(this, CharacterOwner,
			canApplyParameters =>
				canApplyParameters.PotentialTarget == this,
			applyParameters =>
			{
				applyParameters.SetCannotBeFocused();
			}
		);

		// ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(this, CharacterOwner,
		// 	canApplyParameters =>
		// 		canApplyParameters.PotentialTarget == this,
		// 	applyParameters =>
		// 	{
		// 		applyParameters.SetCannotBeTargeted();
		// 	}
		// );

		// Allow stopping movement in the same hex
		ScenarioCheckEvents.CanStopMoveAtHexWithFigureCheckEvent.Subscribe(this, CharacterOwner,
			parameters =>
				(parameters.Figure == this &&
				 parameters.OtherFigure is not Spirit) ||
				(parameters.OtherFigure == this &&
				 parameters.Figure is not Spirit),
			parameters =>
			{
				parameters.SetCanStopAt();
			}
		);

		ScenarioCheckEvents.CanPassEnemyCheckEvent.Subscribe(this, CharacterOwner,
			parameters =>
				parameters.EnemyFigure == this,
			parameters =>
			{
				parameters.SetCanPass();
			}
		);

		ScenarioCheckEvents.ChangeAuthorityCheckEvent.Subscribe(this, CharacterOwner,
			parameters =>
				parameters.Authority == this,
			parameters =>
			{
				parameters.SetAuthority(CharacterOwner);
			}
		);

		CharacterOwner.InitiativeChangedEvent += OnOwnerInitiativeChanged;
	}

	protected override async GDTask TakeTurn()
	{
		await base.TakeTurn();

		_turnActionState = new ActionState(this, this, this, _abilities);
		await _turnActionState.Perform();
	}

	protected override async GDTask EndTurn()
	{
		await base.EndTurn();

		// Spirits suffer 1 damage at the end of their turns
		await AddDamageCounters(this, 1);
	}

	public async GDTask RemoveTurnActionFromActive()
	{
		// TODO: Call this at the end of the round?
		if(_turnActionState != null)
		{
			await _turnActionState.RemoveFromActive();
		}
	}

	public override async GDTask Destroy(bool immediately = false, bool forceDestroy = false)
	{
		await RemoveTurnActionFromActive();

		ScenarioEvents.HexObjectDestroyedEvent.Unsubscribe(this, CharacterOwner);
		ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(this, CharacterOwner);
		ScenarioEvents.FigureExitingHexEvent.Unsubscribe(this, CharacterOwner);
		ScenarioEvents.RetaliateEvent.Unsubscribe(this, CharacterOwner);
		ScenarioCheckEvents.FlyingCheckEvent.Unsubscribe(this, CharacterOwner);
		ScenarioCheckEvents.CanBeFocusedCheckEvent.Unsubscribe(this, CharacterOwner);
		//ScenarioCheckEvents.CanBeTargetedCheckEvent.Unsubscribe(this, CharacterOwner);
		ScenarioCheckEvents.CanStopMoveAtHexWithFigureCheckEvent.Unsubscribe(this, CharacterOwner);
		ScenarioCheckEvents.CanPassEnemyCheckEvent.Unsubscribe(this, CharacterOwner);
		ScenarioCheckEvents.FigureRelationshipCheckEvent.Unsubscribe(this, CharacterOwner);

		if(CharacterOwner != null)
		{
			CharacterOwner.InitiativeChangedEvent -= OnOwnerInitiativeChanged;
		}

		DeregisterSpirit();

		await base.Destroy(immediately, forceDestroy);
	}

	public void SetAttack(int? attack)
	{
		Attack = attack;
	}

	public void SetMove(int? move)
	{
		Move = move;
	}

	public void SetRange(int? range)
	{
		Range = range;
	}

	public void SetAsFirstSpirit()
	{
		List<Spirit> spirits = GetOwnedSpirits(CharacterOwner);
		spirits.Remove(this);
		spirits.Insert(0, this);

		for(int i = 0; i < spirits.Count; i++)
		{
			Spirit spirit = spirits[i];
			spirit.SetSpiritIndex(i);
		}
	}

	protected override Initiative GetInitiative()
	{
		Initiative ownerInitiative = CharacterOwner.Initiative;
		if(ownerInitiative.Null)
		{
			return new Initiative()
			{
				Null = true
			};
		}

		return new Initiative()
		{
			MainInitiative = ownerInitiative.MainInitiative,
			SortingInitiative = ownerInitiative.SortingInitiative + (SpiritIndex + 1) * 10
		};
	}

	private async GDTask UpdateInCorner(Figure figureToIgnore = null, float initialDelay = 0f)
	{
		bool inCorner = Hex.GetFigures().Any(figure => figure != figureToIgnore);
		if(inCorner != _inCorner)
		{
			if(inCorner)
			{
				FigureViewComponent.SetCanAdjustViewPosition(false);
				await GTweenSequenceBuilder.New()
					.AppendTime(initialDelay)
					.Append(_container.TweenScale(0.7f, 0.3f).SetEasing(Easing.InOutBack))
					.Join(_container.TweenPosition(new Vector2(-70f, -70f), 0.3f).SetEasing(Easing.OutBack))
					.Build().PlayFastForwardableAsync();
			}
			else
			{
				FigureViewComponent.SetCanAdjustViewPosition(true);
				_container.TweenScale(1f, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardable();
				_container.TweenPosition(Vector2.Zero, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardable();
				await GDTask.DelayFastForwardable(0.3f);
			}

			_inCorner = inCorner;
		}
	}

	private void SetSpiritIndex(int spiritIndex)
	{
		SpiritIndex = spiritIndex;

		UpdateInitiative();

		_spiritViewComponent.StandeeNumberLabel.SetText((SpiritIndex + 1).ToString());
	}

	private void RegisterSpirit()
	{
		List<Spirit> spirits = GetOwnedSpirits(CharacterOwner);

		spirits.Add(this);
		SetSpiritIndex(spirits.Count - 1);
	}

	private void DeregisterSpirit()
	{
		List<Spirit> spirits = GetOwnedSpirits(CharacterOwner);

		for(int i = 0; i < spirits.Count; i++)
		{
			Spirit spirit = spirits[i];
			spirit.SetSpiritIndex(i);
		}
	}

	private static List<Spirit> GetOwnedSpirits(Character characterOwner)
	{
		const string spiritsKey = "Spirits";
		if(!characterOwner.TryGetCustomValue(spiritsKey, out List<Spirit> spirits))
		{
			spirits = new List<Spirit>();
			characterOwner.SetCustomValue(spiritsKey, spirits);
		}

		return spirits;
	}

	public static List<Figure> GetAllSpirits()
	{
		List<Figure> spirits = new List<Figure>();
		foreach(Figure figure in GameController.Instance.Map.Figures)
		{
			if(CountsAsSpirit(figure))
			{
				spirits.Add(figure);
			}
		}

		return spirits;
	}

	public static async GDTask<Figure> SelectSpirit(AbilityState state, EffectCollection effectCollection = null)
	{
		Figure figure = await AbilityCmd.SelectFigure(state, list =>
		{
			list.AddRange(GetAllSpirits());
		}, effectCollection: effectCollection, hintText: () => $"Select a Spirit");

		return figure;
	}

	public static bool CountsAsSpirit(Figure figure)
	{
		return
			ScenarioCheckEvents.CountsAsSpiritCheckEvent.Fire(
				new ScenarioCheckEvents.CountsAsSpiritCheck.Parameters(figure, figure is Spirit)).CountsAsSpirit;
	}

	public static async GDTask AddDamageCounters(Figure spirit, int amount)
	{
		await AbilityCmd.SufferDamage(spirit, 1, spirit);
	}

	public static async GDTask RemoveDamageCounters(Figure spirit, int amount)
	{
		if(spirit.Health < spirit.MaxHealth)
		{
			int targetHealth = spirit.Health + amount;
			targetHealth = Mathf.Min(targetHealth, spirit.MaxHealth);
			spirit.SetHealth(targetHealth);
		}

		await GDTask.CompletedTask;
	}

	public static bool HasSpirit(Hex hex)
	{
		return hex.GetFigures(true).Any(figure => CountsAsSpirit(figure));
	}

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new SpiritInfoItem.Parameters(this));
	}

	private void OnOwnerInitiativeChanged(Figure owner)
	{
		UpdateInitiative();
	}
}