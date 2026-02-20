using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class MonsterAbilityCard : IDeckCard
{
	private readonly List<ActionState> _actionStates = new List<ActionState>();

	public MonsterAbilityCardModel Model { get; private set; }

	public bool Reshuffles => Model.Reshuffles;
	public bool RemoveAfterDraw => Model.RemoveAfterDraw;

	public MonsterAbilityCard(MonsterAbilityCardModel model)
	{
		Model = model;
	}

	public async GDTask Perform(Monster performer)
	{
		// ScenarioEvents.AbilityStartedEvent.Subscribe(performer, this,
		// 	canApply: parameters => !parameters.IsBlocked && parameters.Performer == performer,
		// 	apply: async parameters =>
		// 	{
		// 		// An ability is not blocked and will be performed; the monster group infuses and consumes all elements on their card now
		// 		ScenarioEvents.AbilityStartedEvent.Unsubscribe(performer, this);
		//
		// 		foreach(MonsterAbilityCardElementConsumption elementConsumption in Model.ElementConsumptions)
		// 		{
		// 			await TryConsume(performer, elementConsumption.ConsumableElements);
		// 		}
		//
		// 		foreach(MonsterAbilityCardElementInfusion elementInfusion in Model.ElementInfusions)
		// 		{
		// 			if(elementInfusion.ConsumableElements == null || await TryConsume(performer, elementInfusion.ConsumableElements))
		// 			{
		// 				await Infuse(performer, elementInfusion.InfusedElement);
		// 			}
		// 		}
		// 	}, EffectType.MandatoryAfterOptionals
		// );

		IEnumerable<MonsterAbilityCardAbility> abilities = Model.GetAbilities(performer);
		ActionState actionState = new ActionState(performer, abilities.Select(ability => ability.Ability).ToList());
		_actionStates.Add(actionState);

		// Ordering is important here, since GetAbilities registers element consumptions, so this needs to be done after creating the abilities, but before actually performing them
		bool hasValidAbility = false;
		if(!performer.HasCondition(Conditions.Stun))
		{
			if(actionState.Abilities.Any(ability => ability is not MoveAbility && ability is not AttackAbility))
			{
				hasValidAbility = true;
			}
			else
			{
				foreach(Ability ability in actionState.Abilities)
				{
					if(ability is MoveAbility && !performer.HasCondition(Conditions.Immobilize))
					{
						hasValidAbility = true;
						break;
					}

					if(ability is AttackAbility && !performer.HasCondition(Conditions.Disarm))
					{
						hasValidAbility = true;
						break;
					}
				}
			}
		}

		if(hasValidAbility)
		{
			foreach(CardElementConsumption elementConsumption in Model.ElementConsumptions)
			{
				await TryConsume(performer, elementConsumption.ConsumableElements);
			}

			foreach(CardElementInfusion elementInfusion in Model.ElementInfusions)
			{
				if(elementInfusion.ConsumableElements == null || await TryConsume(performer, elementInfusion.ConsumableElements))
				{
					await Infuse(performer, elementInfusion.PossibleInfusedElements);
				}
			}
		}

		await actionState.Perform();

		// ScenarioEvents.AbilityStartedEvent.Unsubscribe(performer, this);
	}

	public async GDTask RemoveFromActive()
	{
		foreach(ActionState actionState in _actionStates)
		{
			await actionState.RemoveFromActive();
		}

		_actionStates.Clear();
	}

	public async GDTask RemoveFromActive(Monster monster)
	{
		for(int i = _actionStates.Count - 1; i >= 0; i--)
		{
			ActionState actionState = _actionStates[i];
			if(actionState.Performer == monster)
			{
				await actionState.RemoveFromActive();
				_actionStates.RemoveAt(i);
			}
		}
	}

	protected async GDTask Infuse(Monster performer, IReadOnlyCollection<Element> possibleElements)
	{
		//TODO: Support multiple infusions for a single monster ability card
		//if(!performer.MonsterGroup.AbilityCardInfusedElements.Contains(element))
		if(!performer.MonsterGroup.AbilityCardInfusedElements.Any())
		{
			Element? element = await AbilityCmd.InfuseElement(null, possibleElements, performer);
			if(element.HasValue)
			{
				performer.MonsterGroup.AbilityCardInfusedElements.Add(element.Value);
			}
		}
	}

	protected async GDTask<bool> TryConsume(Monster performer, IReadOnlyCollection<Element> possibleElements)
	{
		foreach(Element element in possibleElements)
		{
			if(performer.MonsterGroup.AbilityCardConsumedElements.Contains(element))
			{
				return true;
			}
		}

		Element? consumedElement = await AbilityCmd.AskConsumeElement(performer, possibleElements, true);
		if(consumedElement.HasValue)
		{
			performer.MonsterGroup.AbilityCardConsumedElements.Add(consumedElement.Value);
			return true;
		}

		return false;
	}

	public Texture2D GetTexture()
	{
		return Model.GetTexture();
	}

	public void Drawn()
	{
	}
}