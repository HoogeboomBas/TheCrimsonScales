using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class MonsterGroup
{
	private readonly List<int> _availableStandeeNumbers = new List<int>();

	public MonsterModel MonsterModel { get; }
	public int GroupIndex { get; }

	public MonsterAbilityCardDeck MonsterAbilityCardDeck { get; private set; }

	public List<Monster> Monsters { get; } = new List<Monster>();

	public List<Element> AbilityCardInfusedElements { get; } = new List<Element>();
	public List<Element> AbilityCardConsumedElements { get; } = new List<Element>();

	public Initiative Initiative { get; private set; }
	public bool InitiativeUpdated { get; private set; }

	public Texture2D PortraitTexture => ResourceLoader.Load<Texture2D>(MonsterModel.PortraitTexturePath);

	public event Action<MonsterGroup> InitiativeChangedEvent;

	public MonsterGroup(MonsterModel monsterModel, int groupIndex, MonsterAbilityCardDeck existingDeckToUse = null)
	{
		MonsterModel = monsterModel;
		GroupIndex = groupIndex;

		for(int i = 0; i < MonsterModel.MaxStandeeCount; i++)
		{
			_availableStandeeNumbers.Add(i + 1);
		}

		//MonsterAbilityCard[] abilityCards = monsterModel.Deck.Select(model => new MonsterAbilityCard(model)).ToArray();
		// foreach(MonsterAbilityCard monsterAbilityCard in abilityCards)
		// {
		// 	monsterAbilityCard.Init(MonsterModel);
		// }

		if(existingDeckToUse == null)
		{
			IEnumerable<MonsterAbilityCard> abilityCards = monsterModel.Deck.Select(model => new MonsterAbilityCard(model));

			MonsterAbilityCardDeck = new MonsterAbilityCardDeck(abilityCards);
		}
		else
		{
			MonsterAbilityCardDeck = existingDeckToUse;
		}

		Initiative = new Initiative()
		{
			Null = true,
			SortingInitiative = 999 * 1000000 + (GroupIndex + 1) * 100000
		};
	}

	public bool TryGetAvailableStandeeNumber(out int number)
	{
		if(_availableStandeeNumbers.Count > 0)
		{
			number = _availableStandeeNumbers.PickRandom(GameController.Instance.StateRNG);
			return true;
		}

		number = -1;
		return false;
	}

	public void RegisterMonster(Monster monster)
	{
		Monsters.Add(monster);

		_availableStandeeNumbers.Remove(monster.StandeeNumber);

		// Check if this spawn came in during a round, if so, potentially draw a card to make the monster take a turn
		if(Monsters.Count == 1 && GameController.Instance.ScenarioPhaseManager.ActivePhase is RoundPhase)
		{
			TryDrawCard();
		}
		else if(MonsterAbilityCardDeck.ActiveCard != null)
		{
			monster.UpdateInitiative();
		}
	}

	public void DeregisterMonster(Monster monster)
	{
		_availableStandeeNumbers.Add(monster.StandeeNumber);

		Monsters.Remove(monster);
	}

	public void TryDrawCard()
	{
		if(Monsters.Count > 0)
		{
			if(MonsterAbilityCardDeck.ActiveCard == null)
			{
				MonsterAbilityCardDeck.ActiveCard = MonsterAbilityCardDeck.DrawCard();
			}

			if(!InitiativeUpdated)
			{
				Initiative = new Initiative()
				{
					MainInitiative = MonsterAbilityCardDeck.ActiveCard.Model.Initiative,
					SortingInitiative = MonsterAbilityCardDeck.ActiveCard.Model.Initiative * 10000000 + 9000000 + GroupIndex * 100000
				};

				foreach(Monster monster in Monsters)
				{
					monster.UpdateInitiative();
				}

				InitiativeChangedEvent?.Invoke(this);
				InitiativeUpdated = true;
			}
		}
	}

	public async GDTask RemoveCard()
	{
		if(MonsterAbilityCardDeck.ActiveCard != null)
		{
			await MonsterAbilityCardDeck.ActiveCard.RemoveFromActive();

			MonsterAbilityCardDeck.ActiveCard = null;
		}

		Initiative = new Initiative()
		{
			Null = true,
			SortingInitiative = 999 * 1000000 + (GroupIndex + 1) * 100000
		};

		InitiativeChangedEvent?.Invoke(this);
		InitiativeUpdated = false;
	}
}