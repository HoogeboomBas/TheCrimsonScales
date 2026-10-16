using Godot;

public partial class CharacterInfoItem : FigureInfoItem<CharacterInfoItem.Parameters>
{
	public class Parameters(Character hexObject) : FigureInfoItemParameters(hexObject)
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/InfoView/CharacterInfoItem.tscn";

		public Character Character { get; } = hexObject;
	}

	[Export]
	private Label _coinsLabel;
	[Export]
	private Label _xpLabel;

	[Export]
	private Control _battleGoalContainer;
	[Export]
	private RichTextLabel _battleGoalTextLabel;
	[Export]
	private ProgressBar _battleGoalProgressBar;

	[Export]
	private Label _masteriesLabel;
	[Export]
	private Control[] _masteryContainers;
	[Export]
	private RichTextLabel[] _masteryTextLabels;
	[Export]
	private ProgressBar[] _masteryProgressBars;

	private Character _character;

	public override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_character = parameters.Character;

		_portraitTexture.SetTexture(_character.PortraitTexture);
		_portraitBorder.SetSelfModulate(_character.OutlineColor);

		UpdateCoins();
		UpdateXP();
		UpdateBattleGoal();

		_character.CoinsChangedEvent += OnCoinsChanged;
		_character.XPChangedEvent += OnXPChanged;
		_character.BattleGoalChangedEvent += OnBattleGoalChangedEvent;
		_character.BattleGoalProgressChangedEvent += OnBattleGoalProgressChangedEvent;
		if(!_character.SavedCharacter.Masteries[0].Completed)
		{
			UpdateMastery(0);
			_character.Mastery1ProgressChangedEvent += OnMastery1ProgressChangedEvent;
		}

		if(!_character.SavedCharacter.Masteries[1].Completed)
		{
			UpdateMastery(1);
			_character.Mastery2ProgressChangedEvent += OnMastery2ProgressChangedEvent;
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		if(_character != null)
		{
			_character.CoinsChangedEvent -= OnCoinsChanged;
			_character.XPChangedEvent -= OnXPChanged;
			_character.BattleGoalChangedEvent -= OnBattleGoalChangedEvent;
			_character.BattleGoalProgressChangedEvent -= OnBattleGoalProgressChangedEvent;
			_character.Mastery1ProgressChangedEvent -= OnMastery1ProgressChangedEvent;
			_character.Mastery2ProgressChangedEvent -= OnMastery2ProgressChangedEvent;
		}
	}

	private void UpdateCoins()
	{
		_coinsLabel.SetText(_character.ObtainedCoins.ToString());
	}

	private void UpdateXP()
	{
		_xpLabel.SetText(_character.ObtainedXP.ToString());
	}

	private void UpdateBattleGoal()
	{
		_battleGoalContainer.SetVisible(_character.BattleGoal != null);

		BattleGoal battleGoal = _character.BattleGoal;
		if(battleGoal != null)
		{
			_battleGoalTextLabel.SetText(battleGoal.Model.Description);

			this.DelayedCall(() =>
			{
				_battleGoalProgressBar.Update(battleGoal.NormalizedProgress, $"{battleGoal.Progress}/{battleGoal.Model.MaxProgress}");
			});

			if(battleGoal.ProgressFull)
			{
				_battleGoalProgressBar.ProgressBarFill.SetSelfModulate(battleGoal.Model.FailIfProgressFull
					? BattleGoal.FailedColor
					: BattleGoal.CompletedColor);
			}
		}
	}

	private void UpdateMastery(int index)
	{
		_masteriesLabel.Show();
		_masteryContainers[index].Show();

		Mastery mastery = _character.SavedCharacter.Masteries[index];
		_masteryTextLabels[index].SetText(mastery.Model.Description);

		this.DelayedCall(() =>
		{
			_masteryProgressBars[index].Update(mastery.NormalizedProgress, $"{mastery.Progress}/{mastery.Model.MaxProgress}");
		});

		if(mastery.TemporarilyFailed)
		{
			_masteryProgressBars[index].ProgressBarFill.SetSelfModulate(Mastery.FailedColor);
		}
		else if(mastery.ProgressFull)
		{
			_masteryProgressBars[index].ProgressBarFill.SetSelfModulate(Mastery.CompletedColor);
		}
	}

	private void OnCoinsChanged(Character character)
	{
		UpdateCoins();
	}

	private void OnXPChanged(Character character)
	{
		UpdateXP();
	}

	private void OnBattleGoalChangedEvent(Character character)
	{
		UpdateBattleGoal();
	}

	private void OnBattleGoalProgressChangedEvent(Character character)
	{
		UpdateBattleGoal();
	}

	private void OnMastery1ProgressChangedEvent(Character character)
	{
		UpdateMastery(0);
	}

	private void OnMastery2ProgressChangedEvent(Character character)
	{
		UpdateMastery(1);
	}
}