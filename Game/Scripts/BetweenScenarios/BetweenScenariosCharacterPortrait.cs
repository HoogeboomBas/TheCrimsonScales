﻿using Godot;
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
	private BetterButton _infoButton;
	[Export]
	private BetterButton _equipmentButton;
	[Export]
	private BetterButton _cardsButton;

	[Export]
	private Control _levelUpScaleContainer;
	[Export]
	private BetterButton _levelUpButton;

	private bool _active;
	private GTween _scaleTween;
	private GTween _levelUpTween;

	public SavedCharacter SavedCharacter { get; private set; }

	public void Init(SavedCharacter savedCharacter)
	{
		SavedCharacter = savedCharacter;

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

		BetterButton.SetEnabled(false, false);

		SavedCharacter.GoldChangedEvent += OnGoldChanged;
		SavedCharacter.XPChangedEvent += OnXPChanged;
		SavedCharacter.LevelChangedEvent += OnLevelCHanged;
		SavedCharacter.NameChangedEvent += OnNameChanged;
		SavedCharacter.CardsChangedEvent += OnCardsChanged;

		_infoButton.Pressed += OnInfoPressed;
		_equipmentButton.Pressed += OnEquipmentPressed;
		_cardsButton.Pressed += OnCardsPressed;
		_levelUpButton.Pressed += OnLevelUpPressed;
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

	private void OnLevelUpPressed()
	{
		if(SavedCharacter.LevelUpInProgress || SavedCharacter.CheckCanLevelUp())
		{
			SavedCharacter.TryLevelUp();

			AppController.Instance.PopupManager.RequestPopup(new LevelUpCardSelectionPopup.Request
			{
				SavedCharacter = SavedCharacter
			});
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		if(SavedCharacter != null)
		{
			SavedCharacter.GoldChangedEvent -= OnGoldChanged;
			SavedCharacter.XPChangedEvent -= OnXPChanged;
			SavedCharacter.LevelChangedEvent -= OnLevelCHanged;
			SavedCharacter.NameChangedEvent -= OnNameChanged;
			SavedCharacter.CardsChangedEvent -= OnCardsChanged;
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

		_levelUpTween?.Complete();
		if(SavedCharacter.LevelUpInProgress || SavedCharacter.CheckCanLevelUp())
		{
			_levelUpButton.SetEnabled(true, false);

			_levelUpTween = GTweenSequenceBuilder.New()
				.Append(_levelUpScaleContainer.TweenScale(1.2f, 0.3f))
				.Append(_levelUpScaleContainer.TweenScale(1f, 0.3f))
				.Build().SetMaxLoops().Play();
		}
		else
		{
			_levelUpButton.SetEnabled(false, false);
		}
	}

	private void OnGoldChanged(SavedCharacter savedCharacter)
	{
		UpdateVisuals();
	}

	private void OnXPChanged(SavedCharacter savedCharacter)
	{
		UpdateVisuals();
	}

	private void OnLevelCHanged(SavedCharacter savedCharacter)
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
}