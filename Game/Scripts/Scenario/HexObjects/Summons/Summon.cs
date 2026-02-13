using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public partial class Summon : Figure
{
	private SummonViewComponent _summonViewComponent;
	private string _name;
	private readonly List<Ability> _abilities = new List<Ability>();

	private ActionState _turnActionState;

	public SummonStats Stats { get; private set; }
	public Character CharacterOwner { get; private set; }
	public Texture2D Texture { get; private set; }
	public int SummonIndex { get; private set; }

	public override string DisplayName => _name;
	public override string DebugName => _name;
	public override AMDCardDeck AMDCardDeck => CharacterOwner.AMDCardDeck;
	public override Texture2D MapIconTexture => _summonViewComponent.Sprite.Texture;
	public override Node2D Visual => _summonViewComponent.Sprite;

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);

		_summonViewComponent = GetViewComponent<SummonViewComponent>();
	}

	public async GDTask Spawn(SummonStats stats, Character characterOwner, string name, string texturePath, string mapIconTexturePath)
	{
		Stats = stats;
		CharacterOwner = characterOwner;
		_name = name;

		_outline.SetSelfModulate(CharacterOwner.OutlineColor);
		_figureViewComponent.TurnStartPS.SetSelfModulate(OutlineColor);
		_figureViewComponent.ActivePS.SetModulate(OutlineColor);

		_summonViewComponent.StandeeNumberCircle.SetSelfModulate(OutlineColor);

		Texture = ResourceLoader.Load<Texture2D>(texturePath);
		Texture2D mapIconTexture = ResourceLoader.Load<Texture2D>(mapIconTexturePath);
		_summonViewComponent.Sprite.SetTexture(mapIconTexture);
		float textureWidth = mapIconTexture.GetWidth();
		_summonViewComponent.Sprite.SetScale((250f / textureWidth) * Vector2.One);

		SetMaxHealth(Stats.Health);
		SetHealth(Stats.Health);

		SetAlignment(characterOwner.Alignment);
		SetEnemies(characterOwner.Enemies);

		if(Stats.Traits != null)
		{
			foreach(FigureTrait trait in Stats.Traits)
			{
				await trait.Activate(this);
			}
		}

		CharacterOwner.RegisterSummon(this);

		GameController.Instance.Map.RegisterFigure(this);

		UpdateInitiative();

		CanTakeTurn = false;

		if(Stats.Move.HasValue)
		{
			MoveAbility moveAbility = MoveAbility.Builder().WithDistance(Stats.Move.Value).Build();
			_abilities.Add(moveAbility);
		}

		if(Stats.Attack.HasValue)
		{
			AttackAbility attackAbility = AttackAbility.Builder()
				.WithDamage(Stats.Attack.Value)
				.WithRange(Stats.Range ?? 1)
				.WithRangeType(Stats.RangeType)
				.Build();
			_abilities.Add(attackAbility);
		}

		ScenarioEvents.FigureFoundFocusEvent.Subscribe(this, characterOwner,
			parameters =>
				parameters.Performer == this &&
				parameters.AbilityState is MoveAbility.State &&
				parameters.Focus == null,
			async parameters =>
			{
				parameters.SetNewFocus(CharacterOwner);

				ScenarioCheckEvents.AIMoveParametersCheckEvent.Subscribe(this, characterOwner,
					parameters => parameters.Performer == this,
					parameters =>
					{
						parameters.SetRange(1);
						parameters.SetRangeType(RangeType.Melee);
						parameters.SetTargets(1);
						parameters.SetAOEPattern(null);
					}
				);

				ScenarioEvents.AbilityEndedEvent.Subscribe(this, characterOwner,
					parameters => parameters.Performer == this,
					async parameters =>
					{
						ScenarioEvents.AbilityEndedEvent.Unsubscribe(this, characterOwner);
						ScenarioCheckEvents.AIMoveParametersCheckEvent.Unsubscribe(this, characterOwner);

						await GDTask.CompletedTask;
					}
				);

				await GDTask.CompletedTask;
			},
			effectType: EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.Move),
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Choose for the summon to move towards the summoner")
		);
	}

	public void SetSummonIndex(int summonIndex)
	{
		SummonIndex = summonIndex;

		UpdateInitiative();

		_summonViewComponent.StandeeNumberLabel.SetText((SummonIndex + 1).ToString());
	}

	protected override async GDTask TakeTurn()
	{
		await base.TakeTurn();

		ScenarioCheckEvents.IsSummonControlledCheck.Parameters isSummonControlledCheckParameters =
			ScenarioCheckEvents.IsSummonControlledCheckEvent.Fire(
				new ScenarioCheckEvents.IsSummonControlledCheck.Parameters(this));

		Figure authority = this;
		if(isSummonControlledCheckParameters.IsControlled)
		{
			authority = CharacterOwner;
		}

		_turnActionState = new ActionState(this, this, authority, _abilities);
		await _turnActionState.Perform();
	}

	public async GDTask RemoveTurnActionFromActive()
	{
		if(_turnActionState != null)
		{
			await _turnActionState.RemoveFromActive();
		}
	}

	public override async GDTask Destroy(bool immediately = false, bool forceDestroy = false)
	{
		if(Stats.Traits != null)
		{
			foreach(FigureTrait trait in Stats.Traits)
			{
				await trait.Deactivate(this);
			}
		}

		await RemoveTurnActionFromActive();

		ScenarioEvents.FigureFoundFocusEvent.Unsubscribe(this, CharacterOwner);

		CharacterOwner.DeregisterSummon(this);

		await base.Destroy(immediately, forceDestroy);
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
			SortingInitiative = ownerInitiative.SortingInitiative - 100 + SummonIndex
		};
	}

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new SummonInfoItem.Parameters(this));
	}
}