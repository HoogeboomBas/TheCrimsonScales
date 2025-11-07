using Godot;

public partial class MonsterInfoItem : FigureInfoItem<MonsterInfoItem.Parameters>
{
	public class Parameters(Monster hexObject) : FigureInfoItemParameters(hexObject)
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/InfoView/MonsterInfoItem.tscn";

		public Monster Monster { get; } = hexObject;
	}

	[Export]
	private Control _abilityCardContainer;
	[Export]
	private TextureRect _abilityCardTexture;

	[Export]
	private Label _moveLabel;
	[Export]
	private Label _attackLabel;
	[Export]
	private Label _rangeLabel;

	private Monster _monster;

	public override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_monster = parameters.Monster;

		_portraitTexture.SetTexture(_monster.MonsterGroup.PortraitTexture);
		_portraitBorder.SetSelfModulate(_monster.TypeColor);

		_moveLabel.SetText(_monster.Stats.Move?.ToString() ?? "-");
		_attackLabel.SetText(_monster.Stats.Attack.ToString());
		_rangeLabel.SetText(_monster.Stats.Range?.ToString() ?? "-");

		UpdateAbilityCard();

		_monster.InitiativeChangedEvent += OnInitiativeChanged;
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		if(_monster != null)
		{
			_monster.InitiativeChangedEvent -= OnInitiativeChanged;
		}
	}

	private void UpdateAbilityCard()
	{
		_abilityCardTexture.SetTexture(_monster.MonsterGroup.ActiveMonsterAbilityCard?.GetTexture());
		_abilityCardContainer.SetVisible(_monster.MonsterGroup.ActiveMonsterAbilityCard != null);
	}

	private void OnInitiativeChanged(Figure figure)
	{
		UpdateAbilityCard();
	}
}