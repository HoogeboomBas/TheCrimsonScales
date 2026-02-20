using System;
using Godot;

public partial class CardPlayCard : Control
{
	[Export]
	private AbilityCardView[] _abilityCardViews;

	[Export]
	private BetterButton _topButton;
	[Export]
	private BetterButton _bottomButton;

	[Export]
	private ChoiceButton _basicTopButton;
	[Export]
	private ChoiceButton _basicBottomButton;

	public AbilityCard AbilityCard { get; private set; }

	public event Action<CardPlayCard, AbilityCardSection> CardSectionPressedEvent;
	public event Action<CardPlayCard> TopPressedEvent;
	public event Action<CardPlayCard> BottomPressedEvent;
	public event Action<CardPlayCard> BasicTopPressedEvent;
	public event Action<CardPlayCard> BasicBottomPressedEvent;

	public override void _Ready()
	{
		base._Ready();

		_topButton.Pressed += OnTopPressed;
		_bottomButton.Pressed += OnBottomPressed;
		_basicTopButton.BetterButton.Pressed += OnBasicTopPressed;
		_basicBottomButton.BetterButton.Pressed += OnBasicBottomPressed;
	}

	public void SetCardData(CardPlayCardData cardData)
	{
		if(cardData == null)
		{
			SetVisible(false);
		}
		else
		{
			AbilityCard = cardData.AbilityCard;

			foreach(AbilityCardView cardView in _abilityCardViews)
			{
				cardView.SetCard(AbilityCard.SavedAbilityCard);
			}

			_topButton.SetEnabled(cardData.CanPlayTop);
			_bottomButton.SetEnabled(cardData.CanPlayBottom);

			_basicTopButton.SetVisible(cardData.CanPlayBasicTop);
			_basicBottomButton.SetVisible(cardData.CanPlayBasicBottom);
		}
	}

	private void OnTopPressed()
	{
		CardSectionPressedEvent?.Invoke(this, AbilityCardSection.Top);
		TopPressedEvent?.Invoke(this);
	}

	private void OnBottomPressed()
	{
		CardSectionPressedEvent?.Invoke(this, AbilityCardSection.Bottom);
		BottomPressedEvent?.Invoke(this);
	}

	private void OnBasicTopPressed()
	{
		CardSectionPressedEvent?.Invoke(this, AbilityCardSection.BasicTop);
		BasicTopPressedEvent?.Invoke(this);
	}

	private void OnBasicBottomPressed()
	{
		CardSectionPressedEvent?.Invoke(this, AbilityCardSection.BasicBottom);
		BasicBottomPressedEvent?.Invoke(this);
	}
}