using System.Collections.Generic;
using Godot;
using GTweens.Builders;
using GTweensGodot.Extensions;

public partial class ItemShop : BetweenScenariosAction
{
	private const int ItemsPerPage = 9;

	[Export]
	private AnimationPlayer _animationPlayer;
	[Export]
	private StringName _forwardAnimationName;
	[Export]
	private StringName _backwardAnimationName;
	[Export]
	private StringName _openAnimationName;
	[Export]
	private StringName _closeAnimationName;
	[Export]
	private SubViewport _frontSubViewport;
	[Export]
	private SubViewport _backSubViewport;
	[Export]
	private SubViewportContainer _bookSubViewportContainer;

	[Export]
	private Control _leftPageParent;
	[Export]
	private Control _rightPageParent;

	[Export]
	private Control _frontPageParent;
	[Export]
	private Control _backPageParent;

	[Export]
	private PackedScene _itemShopPageScene;

	[Export]
	private ChoiceButton _flipLeftButton;
	[Export]
	private ChoiceButton _flipRightButton;

	[Export]
	private Control _bookContainer;
	[Export]
	private Control _bookCover;
	[Export]
	private Control _frontBookCover;

	[Export]
	private Control _leftPageCoverInside;
	[Export]
	private Control _backPageCoverInside;

	[Export]
	private Node3D _3dRoot;

	private int _leftPageIndex;

	private ItemShopPage _leftPage;
	private ItemShopPage _rightPage;

	private ItemShopPage _flipFrontPage;
	private ItemShopPage _flipBackPage;

	private readonly List<ItemModel> _allAvailableItems = new List<ItemModel>();

	public bool IsFlipping { get; private set; }

	protected override bool SelectCharacter => true;

	private bool CanFlipLeft => Active && _leftPageIndex >= 2;
	private bool CanFlipRight => Active && (_leftPageIndex + 2) * ItemsPerPage < _allAvailableItems.Count;

	public override void _Ready()
	{
		base._Ready();

		foreach((string modelId, SavedItem savedItem) in BetweenScenariosController.Instance.SavedCampaign.SavedItems)
		{
			if(savedItem.UnlockedCount > 0)
			{
				_allAvailableItems.Add(ModelDB.GetById<ItemModel>(modelId));
			}
		}

		_animationPlayer.AnimationFinished += OnAnimationFinished;

		_leftPageIndex = 0;

		_flipLeftButton.BetterButton.Pressed += OnFlipLeftPressed;
		_flipRightButton.BetterButton.Pressed += OnFlipRightPressed;

		UpdateButtons();
	}

	protected override void AnimateIn(GTweenSequenceBuilder sequenceBuilder)
	{
		_3dRoot.SetVisible(true);

		_leftPage?.QueueFree();
		_leftPage = null;
		_rightPage?.QueueFree();
		_rightPage = null;
		_flipFrontPage?.QueueFree();
		_flipFrontPage = null;
		_flipBackPage?.QueueFree();
		_flipBackPage = null;

		_bookContainer.Position = new Vector2(-100, -2000);
		_bookContainer.RotationDegrees = 40;

		base.AnimateIn(sequenceBuilder);

		sequenceBuilder
			.Append(_bookContainer.TweenPosition(new Vector2(0f, 10f), 0.6f))
			.Join(_bookContainer.TweenRotationDegrees(0f, 0.6f))
			.Append(_bookContainer.TweenPosition(Vector2.Zero, 0.05f))
			.AppendTime(0.1f)
			.AppendCallback(() =>
			{
				_flipBackPage = CreatePage(_leftPageIndex);
				ReparentPage(_flipBackPage, _backPageParent);

				_rightPage = CreatePage(_leftPageIndex + 1);
				ReparentPage(_rightPage, _rightPageParent);

				_frontBookCover.SetVisible(true);
				_backPageCoverInside.SetVisible(true);

				this.DelayedCall(() =>
				{
					_bookSubViewportContainer.SetVisible(true);
					_bookCover.SetVisible(false);
				}, 0.01f);

				_frontSubViewport.SetUpdateMode(SubViewport.UpdateMode.Once);
				_backSubViewport.SetUpdateMode(SubViewport.UpdateMode.Once);

				_animationPlayer.Play(_openAnimationName);

				AppController.Instance.AudioController.Play("res://Audio/SFX/ItemShop/BookOpen.wav", delay: 0.0f);
			})
			.AppendTime(0.7f);
	}

	protected override void AfterAnimateIn()
	{
		base.AfterAnimateIn();

		UpdateButtons();
	}

	protected override void AnimateOut(GTweenSequenceBuilder sequenceBuilder)
	{
		AppController.Instance.AudioController.Play("res://Audio/SFX/ItemShop/BookClose.wav", minPitch: 1.1f, maxPitch: 1.2f, delay: 0.0f);

		base.AnimateOut(sequenceBuilder);

		UpdateButtons();

		sequenceBuilder
			.AppendCallback(() =>
			{
				_flipBackPage = _leftPage;
				_leftPage = null;
				ReparentPage(_flipBackPage, _backPageParent);

				_frontBookCover.SetVisible(true);
				_backPageCoverInside.SetVisible(true);

				this.DelayedCall(() =>
				{
					_leftPageCoverInside.SetVisible(false);
					_bookSubViewportContainer.SetVisible(true);
				}, 0.01f);

				_frontSubViewport.SetUpdateMode(SubViewport.UpdateMode.Once);
				_backSubViewport.SetUpdateMode(SubViewport.UpdateMode.Once);

				_animationPlayer.Play(_closeAnimationName);
			})
			.AppendTime(0.5f)
			.Append(_bookContainer.TweenPosition(new Vector2(-100, -2000), 0.6f))
			.Join(_bookContainer.TweenRotationDegrees(40f, 0.6f));
	}

	protected override void AfterAnimateOut()
	{
		base.AfterAnimateOut();

		_3dRoot.SetVisible(true);
	}

	private ItemShopPage CreatePage(int pageIndex)
	{
		ItemShopPage shopPage = _itemShopPageScene.Instantiate<ItemShopPage>();
		int startIndex = pageIndex * ItemsPerPage;
		int endIndex = Mathf.Min(startIndex + ItemsPerPage, _allAvailableItems.Count);
		int itemCount = endIndex - startIndex;
		shopPage.Init(itemCount <= 0 ? [] : _allAvailableItems.GetRange(pageIndex * ItemsPerPage, itemCount));

		return shopPage;
	}

	private void Flip(bool forward, ItemShopPage frontPage, ItemShopPage backPage)
	{
		_flipFrontPage = frontPage;
		ReparentPage(_flipFrontPage, _frontPageParent);

		_flipBackPage = backPage;
		ReparentPage(_flipBackPage, _backPageParent);

		_bookSubViewportContainer.SetVisible(true);

		_frontSubViewport.SetUpdateMode(SubViewport.UpdateMode.Once);
		_backSubViewport.SetUpdateMode(SubViewport.UpdateMode.Once);

		_animationPlayer.Play(forward ? _forwardAnimationName : _backwardAnimationName);

		IsFlipping = true;
	}

	private void ReparentPage(Control page, Node parent)
	{
		page.GetParent()?.RemoveChild(page);
		parent.AddChild(page);
		page.Position = Vector2.Zero;
	}

	private void UpdateButtons()
	{
		_flipLeftButton.SetActive(CanFlipLeft && Active && !Transitioning);
		_flipRightButton.SetActive(CanFlipRight && Active && !Transitioning);
	}

	private void OnFlipLeftPressed()
	{
		if(!CanFlipLeft || IsFlipping)
		{
			return;
		}

		_leftPageIndex -= 2;
		ItemShopPage newRightPage = CreatePage(_leftPageIndex + 1);

		Flip(false, newRightPage, _leftPage);

		_leftPage = CreatePage(_leftPageIndex);
		ReparentPage(_leftPage, _leftPageParent);

		UpdateButtons();
	}

	private void OnFlipRightPressed()
	{
		if(!CanFlipRight || IsFlipping)
		{
			return;
		}

		_leftPageIndex += 2;
		ItemShopPage newLeftPage = CreatePage(_leftPageIndex);

		Flip(true, _rightPage, newLeftPage);

		_rightPage = CreatePage(_leftPageIndex + 1);
		ReparentPage(_rightPage, _rightPageParent);

		UpdateButtons();
	}

	private void OnAnimationFinished(StringName animationName)
	{
		if(animationName == _forwardAnimationName)
		{
			// Forward animation finished
			_leftPage.QueueFree();
			_leftPage = _flipBackPage;
			ReparentPage(_leftPage, _leftPageParent);
			_flipBackPage = null;

			_flipFrontPage.QueueFree();
			_flipFrontPage = null;
		}
		else if(animationName == _backwardAnimationName)
		{
			// Backward animation finished
			_rightPage.QueueFree();
			_rightPage = _flipFrontPage;
			ReparentPage(_rightPage, _rightPageParent);
			_flipFrontPage = null;

			_flipBackPage.QueueFree();
			_flipBackPage = null;
		}
		else if(animationName == _openAnimationName)
		{
			_leftPage = _flipBackPage;
			ReparentPage(_leftPage, _leftPageParent);
			_flipBackPage = null;

			_frontBookCover.SetVisible(false);

			_leftPageCoverInside.SetVisible(true);
			_backPageCoverInside.SetVisible(false);
		}
		else if(animationName == _closeAnimationName)
		{
			_bookCover.SetVisible(true);
			_backPageCoverInside.SetVisible(false);
		}

		_bookSubViewportContainer.SetVisible(false);
		IsFlipping = false;
	}
}