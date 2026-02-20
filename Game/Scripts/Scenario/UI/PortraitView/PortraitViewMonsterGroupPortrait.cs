public partial class PortraitViewMonsterGroupPortrait : PortraitViewPortrait
{
	public MonsterGroup MonsterGroup { get; private set; }

	public override Initiative Initiative => MonsterGroup.Initiative;

	public void Init(MonsterGroup monsterGroup)
	{
		base.Init();

		MonsterGroup = monsterGroup;

		_portraitTexture.Texture = MonsterGroup.PortraitTexture;

		MonsterGroup.InitiativeChangedEvent += OnInitiativeChanged;

		OnInitiativeChanged(MonsterGroup);
	}

	public override void Destroy()
	{
		base.Destroy();

		if(MonsterGroup != null)
		{
			MonsterGroup.InitiativeChangedEvent -= OnInitiativeChanged;
		}
	}

	protected override void OnTurnTakerChanged(Figure figure)
	{
		base.OnTurnTakerChanged(figure);

		SetSelected(figure is Monster monster && monster.MonsterGroup == MonsterGroup);
	}

	private void OnInitiativeChanged(MonsterGroup monsterGroup)
	{
		_initiativeLabel.Text = MonsterGroup.Initiative.ToString();

		GameController.Instance.PortraitView.Reorder();
	}
}