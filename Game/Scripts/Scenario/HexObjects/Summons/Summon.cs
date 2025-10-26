using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public partial class Summon : Figure
{
	private SummonViewComponent _summonViewComponent;
	private string _name;
	private List<Ability> _abilities = new List<Ability>();

	private ActionState _turnActionState;

	public SummonStats Stats { get; private set; }
	public Character CharacterOwner { get; private set; }
	public int SummonIndex { get; private set; }

	public override string DisplayName => _name;
	public override string DebugName => _name;

	public override AMDCardDeck AMDCardDeck => CharacterOwner.AMDCardDeck;

	public Texture2D Texture => _summonViewComponent.Sprite.Texture;

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);

		_summonViewComponent = GetViewComponent<SummonViewComponent>();
	}

	public void Spawn(SummonStats stats, Character characterOwner, string name, string texturePath)
	{
		Stats = stats;
		CharacterOwner = characterOwner;
		_name = name;

		_figureViewComponent.Outline.SelfModulate = CharacterOwner.OutlineColor;
		_figureViewComponent.TurnStartPS.SelfModulate = CharacterOwner.OutlineColor;
		_figureViewComponent.ActivePS.Modulate = _figureViewComponent.Outline.SelfModulate;

		_summonViewComponent.StandeeNumberCircle.SelfModulate = CharacterOwner.OutlineColor;

		Texture2D texture = ResourceLoader.Load<Texture2D>(texturePath);
		_summonViewComponent.Sprite.Texture = texture;
		float textureWidth = texture.GetWidth();
		_summonViewComponent.Sprite.Scale = (330f / textureWidth) * Vector2.One;

		SetMaxHealth(Stats.Health);
		SetHealth(Stats.Health);

		SetAlignment(characterOwner.Alignment);
		SetEnemies(characterOwner.Enemies);

		if(Stats.Traits != null)
		{
			foreach(FigureTrait trait in Stats.Traits)
			{
				trait.Activate(this);
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
	}

	public void SetSummonIndex(int summonIndex)
	{
		SummonIndex = summonIndex;

		UpdateInitiative();

		_summonViewComponent.StandeeNumberLabel.Text = (SummonIndex + 1).ToString();
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

		_turnActionState = new ActionState(this, authority, _abilities);
		await _turnActionState.Perform();
	}

	public async GDTask RemoveActionFromActive()
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
				trait.Deactivate(this);
			}
		}

		await RemoveActionFromActive();

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