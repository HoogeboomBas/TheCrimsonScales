using System;
using System.Collections.Generic;
using System.Threading;
using Fractural.Tasks;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class BetweenScenariosPartyGoals : BetweenScenariosAction
{
	[Export]
	private Control _container;
	[Export]
	private PackedScene _partyGoalScene;
	[Export]
	private Control _partyGoalParent;

	[Export]
	private ExclamationMark _exclamationMark;

	private readonly List<PartyGoalsPartyGoal> _partyGoals = new List<PartyGoalsPartyGoal>();

	protected override bool SelectCharacter => false;

	public override void _Ready()
	{
		base._Ready();

		foreach(SavedPartyGoal savedPartyGoal in BetweenScenariosController.Instance.SavedCampaign.SavedPartyGoals.PartyGoals)
		{
			PartyGoalsPartyGoal goal = _partyGoalScene.Instantiate<PartyGoalsPartyGoal>();
			_partyGoalParent.AddChild(goal);
			goal.Init(savedPartyGoal);
			goal.CompletedChangedEvent += OnCompletedChangedEvent;
			_partyGoals.Add(goal);
		}

		Button.SetVisible(!BetweenScenariosController.Instance.SavedCampaign.HasPartyAchievement(PartyAchievement.AccomplishedMercenaries));

		BetweenScenariosController.Instance.SavedCampaign.SavedPartyGoals.CompletedPartyGoalCountChangedEvent += OnCompletedPartyGoalCountChanged;
		BetweenScenariosController.Instance.SavedCampaign.SavedPartyGoals.CompletedEnoughChanged += OnCompletedEnoughChanged;

		UpdateCompleted();
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		if(BetweenScenariosController.Instance != null)
		{
			BetweenScenariosController.Instance.SavedCampaign.SavedPartyGoals.CompletedPartyGoalCountChangedEvent -=
				OnCompletedPartyGoalCountChanged;
			BetweenScenariosController.Instance.SavedCampaign.SavedPartyGoals.CompletedEnoughChanged -= OnCompletedEnoughChanged;
		}
	}

	protected override void AnimateIn(GTweenSequenceBuilder sequenceBuilder, BetweenScenariosAction previousActiveAction)
	{
		base.AnimateIn(sequenceBuilder, previousActiveAction);

		_exclamationMark.SetActive(false);

		_container.SetPosition(new Vector2(0, -1000));

		sequenceBuilder
			.AppendTime(previousActiveAction is ItemShop ? 0.6f : 0.4f)
			.Append(_container.TweenPosition(Vector2.Zero, 0.6f).SetEasing(Easing.OutBack))
			.AppendCallback(() =>
			{
				if(BetweenScenariosController.Instance.SavedCampaign.SavedPartyGoals.CompletedEnough)
				{
					Complete().Forget();
				}
			});
	}

	protected override void AnimateOut(GTweenSequenceBuilder sequenceBuilder)
	{
		sequenceBuilder
			.Append(_container.TweenPosition(new Vector2(0, -1000), 0.4f).SetEasing(Easing.InQuad));

		base.AnimateOut(sequenceBuilder);
	}

	private void UpdateCompleted()
	{
		int completedCount = 0;
		foreach(PartyGoalsPartyGoal partyGoal in _partyGoals)
		{
			if(partyGoal.Completed)
			{
				completedCount++;
			}
		}

		BetweenScenariosController.Instance.SavedCampaign.SavedPartyGoals.UpdateCompletedPartyGoalCount(completedCount);
	}

	private async GDTaskVoid Complete()
	{
		if(BetweenScenariosController.Instance == null)
		{
			return;
		}

		if(BetweenScenariosController.Instance.SavedCampaign.HasPartyAchievement(PartyAchievement.AccomplishedMercenaries))
		{
			return;
		}

		CancellationToken cancellationToken = BetweenScenariosController.Instance.DestroyCancellationToken;

		AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("Party Goals Completed",
			"Congratulations! You have completed 4 out of 5 Party Goals!"));
		await GDTask.WaitWhile(() => AppController.Instance.PopupManager.IsPopupOpen(), cancellationToken: cancellationToken);

		await AppController.Instance.StoryView.OpenAsync("An Invitation for... Tea?", null,
			"""
			Toasting your latest success in the Sleeping Lion, a young Vermling scampers up to your party before shyly dropping off an envelope and scurrying off. The envelope itself is blank but very grand, thick and heavily embossed with a grand wax seal on the back. Tearing it open, you find a handwritten note inside: 

			“My adventurous friends, Your reputation continues to spread, and I would like to share a small token of gratitude from myself and my fellow citizens of Gloomhaven. Please be so gracious as to take tea with me tomorrow at 3pm at the house. Yours, Councilman Raksani (Sir)”. 

			After a brief discussion the next day as to the correct attire for a tea party at the wealthiest merchant in town (you settle on ‘no obvious weapons’), you head up to Councilman Raksani’s huge mansion, which no-one in their right mind could casually refer to as “the house.”

			Making your way up the enormous drive to the main house, there is a shout of “I say!” from your left, and you see the portly Councilman Raksani seated at a large table on the lawn with a magnificent spread of food—and Shiela.

			“Take a seat friends,” he indicates generously before calling a butler over. “How do you take your tea?” Waiting a second to enjoy the slightly perplexed looks on your faces, he bellows with laughter before instructing his butler “Another pot for Shiela and I please, and a tankard of ale for my companions, I think!”

			Still chortling slightly at his joke, Councilman Raksani turns back to you. “Enjoy the food, friends” he says, “but I mainly invited you here to share something I have arranged with a few friends to recognize the work you have done for us.” He carefully hands a large bag over to you, as if it contains live explosives.

			“I have been advised that this may be of some use to you. Furthermore, Shiela has been looking into ways to further your, er, natural abilities. This process cannot be hurried, but she has been preparing for some time, so may be able to give you an example immediately. After that, you will need to aid her with supplies and so on.”

			“I appreciate that these are trifling gifts compared to the dangers you have faced to keep Gloomhaven safe, but I beg that you accept them in the spirit of gratitude that they were intended.”

			“Now then—eat, drink and tell us of your latest adventures!”

			A few hours later, having had maybe one more tankard of ale than was strictly sensible, you bid Councilman Raksani farewell, and arrange to meet Shiela in the morning to discuss her latest discovery. Not too early though...
			""", cancellationToken: cancellationToken);

		AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("Accomplished Mercenaries",
			"The Merchant’s Guild has noticed your efforts and have decided to grant some of the Guild’s services to you—including a special gift constructed by the finest Tinkerers in Gloomhaven, with some guidance from a mysterious, mustachioed fellow, tailored to fit your party’s needs."));
		await GDTask.WaitWhile(() => AppController.Instance.PopupManager.IsPopupOpen(), cancellationToken: cancellationToken);

		BetweenScenariosController.Instance.SavedCampaign.AddPartyAchievement(PartyAchievement.AccomplishedMercenaries);
		BetweenScenariosController.Instance.SavedCampaign.UnlockEnhancements();

		Button.SetVisible(false);

		AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("Enhancements Unlocked",
			"The Enhancer has now opened up their services to you!"));
		await GDTask.WaitWhile(() => AppController.Instance.PopupManager.IsPopupOpen(), cancellationToken: cancellationToken);

		ItemModel itemModel = BetweenScenariosController.Instance.SavedCampaign.StartingGroup switch
		{
			StartingGroup.Militants => ModelDB.Item<SlugCrossbow>(),
			StartingGroup.Protectors => ModelDB.Item<BulwarkBanner>(),
			StartingGroup.Explorers => ModelDB.Item<RemoteBeetle>(),
			StartingGroup.Trailblazers => ModelDB.Item<BlazingBoots>(),
			StartingGroup.Naturalists => ModelDB.Item<ViperBlowgun>(),
			_ => throw new ArgumentOutOfRangeException()
		};

		SavedItem savedItem = BetweenScenariosController.Instance.SavedCampaign.GetSavedItem(itemModel);
		savedItem.AddUnlocked(1);
		savedItem.AddStock(1);
		//TODO: Open rewards popup
		//TODO: Free enhancement
		//TODO: Give custom item as a reward rather than adding it to the shop
		//TODO: New party goal to use custom item 10 times

		AppController.Instance.SaveGame();
	}

	private void OnCompletedChangedEvent(PartyGoalsPartyGoal partyGoal)
	{
		UpdateCompleted();
	}

	private void OnCompletedPartyGoalCountChanged()
	{
		if(!Active)
		{
			_exclamationMark.SetActive(true);
		}
	}

	private void OnCompletedEnoughChanged()
	{
		if(Active)
		{
			Complete().Forget();
		}
		else
		{
			_exclamationMark.SetActive(true);
		}
	}
}