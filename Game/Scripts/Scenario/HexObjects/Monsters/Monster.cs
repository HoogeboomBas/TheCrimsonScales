using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;

public partial class Monster : Figure
{
	private static readonly Color NormalColor = Colors.White;
	private static readonly Color EliteColor = Color.FromHtml("#edc916");
	private static readonly Color BossColor = Color.FromHtml("#bc1515");

	private Sprite2D _staticSprite;
	private MonsterViewComponent _monsterViewComponent;
	private AMDCardDeck _amdCardDeckOverride;

	public MonsterModel MonsterModel { get; private set; }
	public MonsterGroup MonsterGroup { get; private set; }
	public MonsterType MonsterType { get; private set; }
	public int StandeeNumber { get; private set; }
	public int MonsterLevel { get; private set; }
	public MonsterStats Stats { get; private set; }
	public bool IsSummon { get; private set; }

	public Color TypeColor { get; private set; }

	public override string DisplayName => $"{(MonsterType == MonsterType.Elite ? $"{MonsterType} " : string.Empty)}{MonsterGroup.MonsterModel.Name}";
	public override string DebugName => $"{MonsterGroup.MonsterModel.Name} {StandeeNumber}";
	public override AMDCardDeck AMDCardDeck => _amdCardDeckOverride ?? GameController.Instance.MonsterAMDCardDeck;
	public override Texture2D MapIconTexture => _staticSprite.Texture;
	public override Node2D Visual => _staticSprite;

	public override void _Ready()
	{
		base._Ready();

		_staticSprite = GetNode<Sprite2D>("Mask/Sprite2D");
	}

	public void SetMonsterModel(MonsterModel monsterModel)
	{
		MonsterModel = monsterModel;
	}

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);

		_monsterViewComponent = GetViewComponent<MonsterViewComponent>();
	}

	public async GDTask Spawn(MonsterGroup monsterGroup, MonsterType monsterType, int standeeNumber, bool summon,
		int? monsterLevel, Alignment alignment, Alignment enemies)
	{
		MonsterGroup = monsterGroup;
		MonsterType = monsterType;

		StandeeNumber = standeeNumber;
		_monsterViewComponent.StandeeNumberLabel.Text = StandeeNumber.ToString();

		MonsterStats[] levelStats;
		switch(MonsterType)
		{
			case MonsterType.Normal:
				TypeColor = NormalColor;
				levelStats = MonsterModel.NormalLevelStats;
				break;
			case MonsterType.Elite:
				TypeColor = EliteColor;
				levelStats = MonsterModel.EliteLevelStats;
				break;
			case MonsterType.Boss:
				TypeColor = BossColor;
				levelStats = MonsterModel.BossLevelStats;
				break;
			case MonsterType.Named:
				TypeColor = BossColor;
				levelStats = MonsterModel.NamedLevelStats;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(monsterType), monsterType, null);
		}

		_outline.SelfModulate = TypeColor;
		_figureViewComponent.TurnStartPS.SelfModulate = TypeColor;
		_figureViewComponent.ActivePS.Modulate = OutlineColor;
		_monsterViewComponent.StandeeNumberCircle.SelfModulate = TypeColor;
		_monsterViewComponent.StandeeNumberCircle.Visible = MonsterType != MonsterType.Boss;

		Texture2D mapIconTexture = ResourceLoader.Load<Texture2D>(MonsterModel.MapIconTexturePath);
		_staticSprite.SetTexture(mapIconTexture);

		if(mapIconTexture != null)
		{
			float textureWidth = mapIconTexture.GetWidth();
			_staticSprite.SetScale((250f / textureWidth) * Vector2.One);
		}

		MonsterLevel = Math.Clamp(monsterLevel ?? GameController.Instance.SavedScenario.ScenarioLevel, 0, 7);
		Stats = levelStats[MonsterLevel];

		SetMaxHealth(Stats.Health);
		SetHealth(Stats.Health);

		SetAlignment(alignment);
		SetEnemies(enemies);

		if(Stats.Traits != null)
		{
			foreach(FigureTrait trait in Stats.Traits)
			{
				await trait.Activate(this);
			}
		}

		IsSummon = summon;

		if(IsSummon)
		{
			CanTakeTurn = false;
		}

		MonsterGroup.RegisterMonster(this);
		GameController.Instance.Map.RegisterFigure(this);

		Scale = Vector2.Zero;
		this.TweenScale(1f, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardable();
	}

	protected override async GDTask TakeTurn()
	{
		await base.TakeTurn();

		if(MonsterGroup.ActiveMonsterAbilityCard != null)
		{
			await MonsterGroup.ActiveMonsterAbilityCard.Perform(this);
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

		// Unsubscribe from any events that the monster subscribed to using abilities this turn
		if(MonsterGroup.ActiveMonsterAbilityCard != null)
		{
			await MonsterGroup.ActiveMonsterAbilityCard.RemoveFromActive(this);
		}

		MonsterGroup.DeregisterMonster(this);

		await base.Destroy(immediately, forceDestroy);

		await AbilityCmd.SpawnCoin(Hex, this);
	}

	protected override Initiative GetInitiative()
	{
		Initiative monsterGroupInitiative = MonsterGroup.Initiative;

		if(monsterGroupInitiative.Null)
		{
			return new Initiative()
			{
				Null = true
			};
		}

		return new Initiative()
		{
			MainInitiative = monsterGroupInitiative.MainInitiative,
			SortingInitiative = monsterGroupInitiative.SortingInitiative + (MonsterType == MonsterType.Normal ? 10000 : 0) + 100 * StandeeNumber
		};
	}

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new MonsterInfoItem.Parameters(this));
	}

	public void SetAMDCardDeck(AMDCardDeck amdCardDeck)
	{
		_amdCardDeckOverride = amdCardDeck;
	}
}