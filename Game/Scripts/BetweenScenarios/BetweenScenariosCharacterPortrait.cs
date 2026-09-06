using System;
using Fractural.Tasks;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public partial class BetweenScenariosCharacterPortrait : Control
{
	[Export]
	public BetterButton BetterButton { get; private set; }

	[Export]
	private Control _container;
	[Export]
	private TextureRect _textureRect;
	[Export]
	private Control _colorOutline;
	[Export]
	private Control _inactiveOverlay;

	[Export]
	private TextureRect _levelBackground;

	[Export]
	private Label _levelLabel;
	[Export]
	private Label _xpLabel;
	[Export]
	private Control _xpProgressBar;
	[Export]
	private Label _goldLabel;

	[Export]
	private Control _buttonContainer;
	[Export]
	private BetterButton _infoButton;
	[Export]
	private BetterButton _equipmentButton;
	[Export]
	private BetterButton _cardsButton;
	[Export]
	private BetterButton _perksButton;

	[Export]
	private ExclamationMark _cardsExclamationMark;
	[Export]
	private ExclamationMark _perksExclamationMark;

	[Export]
	private Texture2D _normalInfoTexture;
	[Export]
	private Texture2D _retireInfoTexture;
	[Export]
	private TextureRect _infoButtonTextureRect;

	[Export]
	private ExclamationMark _levelUpExclamationMark;
	[Export]
	private BetterButton _levelUpButton;

	private SavedCampaign _savedCampaign;
	private PersonalQuestData _personalQuestData;

	private bool _active;
	private GTween _scaleTween;

	public SavedCharacter SavedCharacter { get; private set; }

	public void Init(SavedCampaign savedCampaign, SavedCharacter savedCharacter)
	{
		_savedCampaign = savedCampaign;
		SavedCharacter = savedCharacter;
		_personalQuestData = SavedCharacter.SavedPersonalQuest?.PersonalQuestData;

		_textureRect.Texture = SavedCharacter.ClassModel.PortraitTexture;
		_colorOutline.Modulate = SavedCharacter.ClassModel.PrimaryColor;
		_levelBackground.SelfModulate = SavedCharacter.ClassModel.PrimaryColor;

		this.DelayedCall(() =>
		{
			_container.PivotOffset = _container.Size * 0.5f;
		}, 0.05f);

		_active = true;
		_inactiveOverlay.TweenModulateAlpha(0f, 0f).Play();

		if(BetweenScenariosController.Instance.SavedCampaign.Characters.Count < 3)
		{
			CustomMinimumSize = new Vector2(0f, 390f);
			SizeFlagsVertical = SizeFlags.Fill;
		}

		UpdateVisuals();
		UpdateScaling();

		BetterButton.SetEnabled(false, false);

		SavedCharacter.GoldChangedEvent += OnGoldChanged;
		SavedCharacter.XPChangedEvent += OnXPChanged;
		SavedCharacter.LevelChangedEvent += OnLevelChanged;
		SavedCharacter.NameChangedEvent += OnNameChanged;
		SavedCharacter.CardsChangedEvent += OnCardsChanged;
		SavedCharacter.CheckmarkCountChangedEvent += OnCheckmarkCountChanged;
		SavedCharacter.PerksChangedEvent += OnPerksChanged;

		if(_personalQuestData != null)
		{
			_personalQuestData.ProgressChangedEvent += OnPersonalQuestProgressChanged;
		}

		_infoButton.Pressed += OnInfoPressed;
		_equipmentButton.Pressed += OnEquipmentPressed;
		_cardsButton.Pressed += OnCardsPressed;
		_perksButton.Pressed += OnPerksPressed;
		_levelUpButton.Pressed += () => OnLevelUpPressed().Forget();
		GetViewport().SizeChanged += OnViewportSizeChanged;
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		if(SavedCharacter != null)
		{
			SavedCharacter.GoldChangedEvent -= OnGoldChanged;
			SavedCharacter.XPChangedEvent -= OnXPChanged;
			SavedCharacter.LevelChangedEvent -= OnLevelChanged;
			SavedCharacter.NameChangedEvent -= OnNameChanged;
			SavedCharacter.CardsChangedEvent -= OnCardsChanged;
			SavedCharacter.CheckmarkCountChangedEvent -= OnCheckmarkCountChanged;
			SavedCharacter.PerksChangedEvent -= OnPerksChanged;
		}

		if(_personalQuestData != null)
		{
			_personalQuestData.ProgressChangedEvent -= OnPersonalQuestProgressChanged;
		}

		Viewport viewport = GetViewport();
		if(viewport != null)
		{
			viewport.SizeChanged -= OnViewportSizeChanged;
		}
	}

	public void SetActive(bool active, bool canPress)
	{
		BetterButton.SetEnabled(canPress, false);

		if(_active == active)
		{
			return;
		}

		_active = active;

		_scaleTween?.Kill();
		if(_active)
		{
			_scaleTween = GTweenSequenceBuilder.New()
				.AppendTime(0.05f)
				.Append(_container.TweenScale(1f, 0.15f).SetEasing(Easing.OutBack))
				.Join(_inactiveOverlay.TweenModulateAlpha(0f, 0.15f))
				.Build().Play();
		}
		else
		{
			_scaleTween = GTweenSequenceBuilder.New()
				.AppendTime(0.05f)
				.Append(_container.TweenScale(0.9f, 0.15f).SetEasing(Easing.InBack))
				.Join(_inactiveOverlay.TweenModulateAlpha(1f, 0.15f))
				.Build().Play();
		}
	}

	private void UpdateVisuals()
	{
		_levelLabel.Text = SavedCharacter.Level.ToString();
		int currentLevelXP = SavedCharacter.Level == 1 ? 0 : SavedCharacter.ClassModel.XPLevelValues.Values[SavedCharacter.Level - 2];
		int nextLevelXP = SavedCharacter.ClassModel.XPLevelValues.Values[
			Mathf.Min(SavedCharacter.Level - 1, SavedCharacter.ClassModel.XPLevelValues.Values.Length - 1)];
		_xpLabel.Text = $"{SavedCharacter.XP}/{nextLevelXP}";
		_xpProgressBar.Scale = new Vector2(Mathf.Clamp(Mathf.InverseLerp(currentLevelXP, nextLevelXP, SavedCharacter.XP), 0f, 1f), 1f);
		_goldLabel.Text = SavedCharacter.Gold.ToString();

		_infoButtonTextureRect.SetTexture(SavedCharacter.GetCanRetire(_savedCampaign) ? _retireInfoTexture : _normalInfoTexture);

		bool canLevelUp = SavedCharacter.LevelUpInProgress || SavedCharacter.CheckCanLevelUp();
		_levelUpExclamationMark.SetActive(canLevelUp);
		_levelUpButton.SetEnabled(canLevelUp, false);

		bool canAcquirePerk = false;
		for(int i = 0; i < SavedCharacter.ClassModel.Perks.Count; i++)
		{
			if(SavedCharacter.CanAcquirePerk(i))
			{
				canAcquirePerk = true;
				break;
			}
		}

		_perksExclamationMark.SetActive(canAcquirePerk);

		UpdateScaling();
	}

	private void UpdateScaling()
	{
		this.DelayedCall(() =>
		{
			float buttonsScale = Mathf.Min(1f, Size.Y / _buttonContainer.Size.Y);
			_buttonContainer.SetScale(buttonsScale * Vector2.One);
		});
	}

	private void OnGoldChanged(SavedCharacter savedCharacter)
	{
		UpdateVisuals();
	}

	private void OnXPChanged(SavedCharacter savedCharacter)
	{
		UpdateVisuals();
	}

	private void OnLevelChanged(SavedCharacter savedCharacter)
	{
		UpdateVisuals();
	}

	private void OnNameChanged(SavedCharacter savedCharacter)
	{
		UpdateVisuals();
	}

	private void OnCardsChanged(SavedCharacter savedCharacter)
	{
		UpdateVisuals();
	}

	private void OnCheckmarkCountChanged(SavedCharacter savedCharacter)
	{
		UpdateVisuals();
	}

	private void OnPerksChanged(SavedCharacter savedCharacter)
	{
		UpdateVisuals();
	}

	private void OnPersonalQuestProgressChanged(PersonalQuestData personalQuestData)
	{
		UpdateVisuals();
	}

	private void OnInfoPressed()
	{
		AppController.Instance.PopupManager.RequestPopup(new CharacterInfoPopup.Request
		{
			SavedCampaign = BetweenScenariosController.Instance.SavedCampaign,
			SavedCharacter = SavedCharacter
		});
	}

	private void OnEquipmentPressed()
	{
		AppController.Instance.PopupManager.RequestPopup(new EquipmentPopup.Request
		{
			SavedCampaign = BetweenScenariosController.Instance.SavedCampaign,
			SavedCharacter = SavedCharacter
		});
	}

	private void OnCardsPressed()
	{
		AppController.Instance.PopupManager.RequestPopup(new CardSelectionPopup.Request
		{
			SavedCharacter = SavedCharacter
		});
	}

	private void OnPerksPressed()
	{
		AppController.Instance.PopupManager.RequestPopup(new PerksPopup.Request
		{
			SavedCharacter = SavedCharacter
		});
	}

	private async GDTask OnLevelUpPressed()
	{
		if(SavedCharacter.LevelUpInProgress || SavedCharacter.CheckCanLevelUp())
		{
			if(await SavedCharacter.TryLevelUp(_savedCampaign))
			{
				AppController.Instance.PopupManager.RequestPopup(new LevelUpCardSelectionPopup.Request
				{
					SavedCharacter = SavedCharacter
				});
			}
		}
	}

	private void OnViewportSizeChanged()
	{
		UpdateScaling();
	}
}