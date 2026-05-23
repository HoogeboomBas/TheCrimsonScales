using System.Collections.Generic;
using System.Threading;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class EventOverlay : Control
{
	[Export]
	private Control _background;
	[Export]
	private BetterButton _skipTextButton;

	[Export]
	private RotatingCardView _rotatingCardView;

	[Export]
	private EventCard _cityEventCard;
	[Export]
	private EventCard _roadEventCard;

	[Export]
	private ChoiceButton _continueButton;

	[Export]
	private PackedScene _eventChoiceButtonScene;
	[Export]
	private Control _eventChoiceButtonParent;

	private readonly List<EventChoiceButton> _choiceButtons = new List<EventChoiceButton>();

	private EventCard _currentEventCard;
	private EventChoiceModel _chosenModel;
	private bool _continuePressed;

	public override void _Ready()
	{
		base._Ready();

		_skipTextButton.Pressed += OnSkipTextPressed;
		_continueButton.BetterButton.Pressed += OnContinuePressed;

		Hide();
	}

	public async GDTask DrawEventCard(EventType eventType, CancellationToken cancellationToken)
	{
		AppController.Instance.SaveManager.BlockSaving(this);

		EventModel eventModel;
		if(eventType == EventType.City)
		{
			eventModel = BetweenScenariosController.Instance.SavedCampaign.SavedEvents.DrawCityEvent();
			_currentEventCard = _cityEventCard;
		}
		else
		{
			eventModel = BetweenScenariosController.Instance.SavedCampaign.SavedEvents.DrawRoadEvent();
			_currentEventCard = _roadEventCard;
		}

		_cityEventCard.SetVisible(false);
		_roadEventCard.SetVisible(false);
		_currentEventCard.SetVisible(true);
		_skipTextButton.Hide();

		Show();

		_chosenModel = null;
		_continuePressed = false;

		_background.SetModulate(Colors.Transparent);
		_background.TweenModulateAlpha(1f, 0.3f).Play();

		foreach(EventChoiceModel choiceModel in eventModel.EventChoiceModels)
		{
			EventChoiceButton choiceButton = _eventChoiceButtonScene.Instantiate<EventChoiceButton>();
			_eventChoiceButtonParent.AddChild(choiceButton);
			choiceButton.Init(choiceModel, OnChoiceButtonPressed);
			_choiceButtons.Add(choiceButton);
		}

		_rotatingCardView.SetPivotOffset(_rotatingCardView.Size * 0.5f);
		_rotatingCardView.SetScale(Vector2.One * 0.001f);

		_currentEventCard.SetupFront(eventModel, false);

		await GDTask.Yield(cancellationToken);
		await GDTask.Delay(0.2f, cancellationToken: cancellationToken);
		await _rotatingCardView.TweenScale(1f, 0.6f).SetEasing(Easing.OutBack).PlayAsync(cancellationToken);

		_skipTextButton.Show();
		await _currentEventCard.AnimateText(_currentEventCard.FrontEventText, cancellationToken: cancellationToken);
		_skipTextButton.Hide();

		foreach(EventChoiceButton choiceButton in _choiceButtons)
		{
			choiceButton.SetActive(true);
			await GDTask.Delay(0.3f, cancellationToken: cancellationToken);
		}

		await GDTask.WaitUntil(() => _chosenModel != null, cancellationToken: cancellationToken);

		foreach(EventChoiceButton choiceButton in _choiceButtons)
		{
			choiceButton.Disable();

			if(choiceButton.Model != _chosenModel)
			{
				choiceButton.SetActive(false);
			}
		}

		// Initialize state
		SavedEventState savedEventState = new SavedEventState(_chosenModel);
		_chosenModel!.InitState(savedEventState, BetweenScenariosController.Instance.SavedCampaign);

		_currentEventCard.SetupBack(savedEventState, false, false);
		await RotateEventCard(cancellationToken);

		_skipTextButton.Show();
		await _currentEventCard.AnimateText(_currentEventCard.BackEventText, cancellationToken: cancellationToken);
		_skipTextButton.Hide();

		foreach(EventChoiceButton choiceButton in _choiceButtons)
		{
			choiceButton.SetActive(false);
		}

		await GDTask.Delay(0.3f, cancellationToken: cancellationToken);

		_continueButton.SetActive(true);

		await GDTask.WaitUntil(() => _continuePressed, cancellationToken: cancellationToken);

		_continueButton.SetActive(false);

		List<SavedReward> rewards = _chosenModel.GetRewards(savedEventState);
		await AppController.Instance.GiveRewards(BetweenScenariosController.Instance.SavedCampaign, rewards, showPopup: false,
			cancellationToken: cancellationToken);

		// foreach(SavedReward reward in rewards)
		// {
		// 	if(reward.Type == RewardType.Immediate)
		// 	{
		// 		await reward.ImmediateResolve(BetweenScenariosController.Instance.SavedCampaign, cancellationToken);
		// 	}
		// 	else
		// 	{
		// 		BetweenScenariosController.Instance.SavedCampaign.SavedRewards.AddReward(reward);
		// 	}
		// }

		EventResolveType eventResolveType = _chosenModel.GetEventResolveType(savedEventState);
		if(eventResolveType == EventResolveType.ReturnCardToBottom)
		{
			if(eventModel.EventType == EventType.City)
			{
				BetweenScenariosController.Instance.SavedCampaign.SavedEvents.ReturnCityEventToBottom(eventModel);
			}
			else
			{
				BetweenScenariosController.Instance.SavedCampaign.SavedEvents.ReturnRoadEventToBottom(eventModel);
			}
		}

		AppController.Instance.SaveManager.UnblockSaving(this);
		AppController.Instance.SaveManager.SaveGame();

		_background.TweenModulateAlpha(0f, 0.3f).Play();
		await _rotatingCardView.TweenScale(0f, 0.3f).SetEasing(Easing.InBack).PlayAsync(cancellationToken: cancellationToken);

		foreach(EventChoiceButton choiceButton in _choiceButtons)
		{
			choiceButton.QueueFree();
		}

		_choiceButtons.Clear();

		Hide();
	}

	private async GDTask RotateEventCard(CancellationToken cancellationToken)
	{
		await GDTask.Yield(cancellationToken);
		await GDTask.Delay(0.2f, cancellationToken: cancellationToken);

		await _rotatingCardView.GetRotationTween(() =>
		{
			_currentEventCard.FrontContainer.SetModulate(Colors.Transparent);
			_currentEventCard.BackContainer.SetModulate(Colors.White);
		}).PlayAsync(cancellationToken);
	}

	private void OnSkipTextPressed()
	{
		_currentEventCard?.SkipText();
		_skipTextButton.Hide();
	}

	private void OnChoiceButtonPressed(EventChoiceButton choiceButton)
	{
		if(_chosenModel != null)
		{
			return;
		}

		_chosenModel = choiceButton.Model;
	}

	private void OnContinuePressed()
	{
		_continuePressed = true;
	}
}