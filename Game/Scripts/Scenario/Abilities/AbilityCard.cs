using System;
using System.Collections.Generic;
using Fractural.Tasks;

public class AbilityCard : IReferenced
{
	public int ReferenceId { get; set; }

	public SavedAbilityCard SavedAbilityCard { get; }

	public AbilityCardSide Top { get; }
	public AbilityCardSide Bottom { get; }
	public AbilityCardSide BasicTop { get; }
	public AbilityCardSide BasicBottom { get; }

	public Character OriginalOwner { get; }

	public Character Owner { get; private set; }
	public CardState CardState { get; private set; }
	public bool Unrecoverable { get; private set; }

	public List<ActionState> ActiveActionStates { get; } = new List<ActionState>();

	public AbilityCardModel Model => SavedAbilityCard.Model;

	public event Action<AbilityCard> CardStateChangedEvent;

	public AbilityCard(SavedAbilityCard savedAbilityCard, Character owner)
	{
		this.InitReference();

		SavedAbilityCard = savedAbilityCard;

		Top = new AbilityCardSide(this, Model.Top);
		Bottom = new AbilityCardSide(this, Model.Bottom);
		BasicTop = new AbilityCardSide(this, Model.BasicTop);
		BasicBottom = new AbilityCardSide(this, Model.BasicBottom);

		GameController.Instance.CardManager.Register(this);

		OriginalOwner = owner;
		SetOwner(owner);

		CardState = CardState.Hand;
	}

	public async GDTask SetCardState(CardState cardState)
	{
		CardState = cardState;

		await ScenarioEvents.AbilityCardStateChangedEvent.CreatePrompt(new ScenarioEvents.AbilityCardStateChanged.Parameters(this));

		CardStateChangedEvent?.Invoke(this);
	}

	public void SetUnrecoverable(bool unrecoverable)
	{
		Unrecoverable = unrecoverable;
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

	private async GDTask RemoveFromActive(ActionState activeActionState)
	{
		ActiveActionStates.Remove(activeActionState);
		await activeActionState.RemoveFromActive();
	}
}