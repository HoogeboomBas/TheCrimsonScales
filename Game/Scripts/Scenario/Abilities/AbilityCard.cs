using System;
using System.Collections.Generic;
using Fractural.Tasks;

public class AbilityCard : IReferenced
{
	public SavedAbilityCard SavedAbilityCard { get; }

	public AbilityCardModel Model => SavedAbilityCard.Model;

	public Character OriginalOwner { get; private set; }
	public Character Owner { get; private set; }
	public CardState CardState { get; private set; }

	public AbilityCardSide Top { get; }
	public AbilityCardSide Bottom { get; }
	public AbilityCardSide BasicTop { get; }
	public AbilityCardSide BasicBottom { get; }

	public List<ActionState> ActiveActionStates { get; } = new List<ActionState>();

	public int ReferenceId { get; set; }

	public event Action<AbilityCard> CardStateChangedEvent;

	public AbilityCard(SavedAbilityCard savedAbilityCard, Character owner)
	{
		this.InitReference();

		SavedAbilityCard = savedAbilityCard;

		GameController.Instance.CardManager.Register(this);

		OriginalOwner = owner;
		SetOwner(owner);

		CardState = CardState.Hand;

		Top = Model.CreateTopSide(this);
		Bottom = Model.CreateBottomSide(this);
		BasicTop = Model.CreateBasicTopSide(this);
		BasicBottom = Model.CreateBasicBottomSide(this);
	}

	public async GDTask SetCardState(CardState cardState)
	{
		CardState = cardState;

		await ScenarioEvents.AbilityCardStateChangedEvent.CreatePrompt(new ScenarioEvents.AbilityCardStateChanged.Parameters(this));

		CardStateChangedEvent?.Invoke(this);
	}

	public void SetOwner(Character character)
	{
		Owner = character;
	}

	public void SetActionStateActive(ActionState actionState)
	{
		ActiveActionStates.Add(actionState);
	}

	public async GDTask RemoveFromActive()
	{
		for(int i = ActiveActionStates.Count - 1; i >= 0; i--)
		{
			ActionState activeActionState = ActiveActionStates[i];
			await RemoveFromActive(activeActionState);
		}
	}

	public async GDTask RemoveFromActive(ActionState activeActionState)
	{
		ActiveActionStates.Remove(activeActionState);
		await activeActionState.RemoveFromActive();
	}
}