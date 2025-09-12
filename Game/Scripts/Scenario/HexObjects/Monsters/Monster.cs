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

	private MonsterViewComponent _monsterViewComponent;

	public override string DisplayName => $"{(MonsterType == MonsterType.Elite ? $"{MonsterType} " : string.Empty)}{MonsterGroup.MonsterModel.Name}";
	public override string DebugName => $"{MonsterGroup.MonsterModel.Name} {StandeeNumber}";

	public MonsterModel MonsterModel { get; private set; }
	public MonsterGroup MonsterGroup { get; private set; }
	public MonsterType MonsterType { get; private set; }
	public int StandeeNumber { get; private set; }
	public MonsterStats Stats { get; private set; }
	public Color TypeColor { get; private set; }

	public override AMDCardDeck AMDCardDeck => GameController.Instance.MonsterAMDCardDeck;

	public void SetMonsterModel(MonsterModel monsterModel)
	{
		MonsterModel = monsterModel;
	}

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);

		_monsterViewComponent = GetViewComponent<MonsterViewComponent>();
	}

	public void Spawn(MonsterGroup monsterGroup, MonsterType monsterType, int standeeNumber, bool summon)
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
			default:
				throw new ArgumentOutOfRangeException(nameof(monsterType), monsterType, null);
		}

		_figureViewComponent.Outline.SelfModulate = TypeColor;
		_figureViewComponent.TurnStartPS.SelfModulate = TypeColor;
		_figureViewComponent.ActivePS.Modulate = _figureViewComponent.Outline.SelfModulate;
		_monsterViewComponent.StandeeNumberCircle.SelfModulate = TypeColor;
		_monsterViewComponent.StandeeNumberCircle.Visible = MonsterType != MonsterType.Boss;

		Stats = levelStats[GameController.Instance.SavedScenario.ScenarioLevel];

		SetMaxHealth(Stats.Health);
		SetHealth(Stats.Health);

		SetAlignment(Alignment.Enemies);
		SetEnemies(Alignment.Characters);

		if(Stats.Traits != null)
		{
			foreach(FigureTrait trait in Stats.Traits)
			{
				trait.Activate(this);
			}
		}

		if(summon)
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

		await MonsterGroup.ActiveMonsterAbilityCard.Perform(this);
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

		// Unsubscribe from any events that the monster subscribed to using abilities this turn
		if(MonsterGroup.ActiveMonsterAbilityCard != null)
		{
			await MonsterGroup.ActiveMonsterAbilityCard.RemoveFromActive(this);
		}

		MonsterGroup.DeregisterMonster(this);

		await base.Destroy(immediately, forceDestroy);

		await AbilityCmd.SpawnCoin(Hex);
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
}