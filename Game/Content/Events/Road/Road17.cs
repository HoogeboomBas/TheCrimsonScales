using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Road17 : RoadEventModel<Road17.ChoiceA, Road17.ChoiceB>
{
	public override int Number => 17;

	public override string Text =>
		"""
		You come across a booth set up on the side of the road, where an Inox bearing articles of clothing resembling tribal gear sits aside a collection of totems in different shapes and sizes.

		"These spiritual totems are used by our tribes in battle," the Inox explains. "For the small price of ten gold, one of them could be yours."

		The Inox places two totems on the table of his booth. "Today, I can sell you the Drake Totem of Confusion or the Eagle Totem of Watchfulness. Which would you care to buy?"
		""";

	public class ChoiceA : EventChoiceModel, IEventSubscriber
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Offer to buy the Drake Totem.";

		public override EventResolveType GetEventResolveType(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return EventResolveType.Lost;
			}
			else
			{
				return EventResolveType.ReturnCardToBottom;
			}
		}

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Sum(character => character.Gold) >= 10;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					"The Drake Totem!" the Inox smiles as he places a totem resembling a drake on the table. "Used throughout the ages as an emblem to confuse enemies. Excellent choice."
					""";
			}
			else
			{
				return
					"""
					"Not today, huh?" the Inox frowns. "May the animal spirits be with you."
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new LoseCollectiveGoldEventReward(10),
					new TotemEventReward(
						obstacle =>
						{
							ScenarioEvents.DuringAttackEvent.Subscribe(this,
								parameters =>
									parameters.Performer is Character &&
									parameters.AbilityState.SingleTargetRangeType == RangeType.Melee &&
									RangeHelper.Distance(parameters.Performer.Hex, obstacle.Hex) <= 1,
								async parameters =>
								{
									parameters.AbilityState.SingleTargetAddCondition(Conditions.Muddle);

									await GDTask.CompletedTask;
								}
							);
						},
						obstacle =>
						{
							ScenarioEvents.DuringAttackEvent.Unsubscribe(this);
						},
						"Drake",
						color =>
							$"All characters adjacent to this obstacle add {Icons.Inline(Icons.GetCondition(Conditions.Muddle), color: color)} to all their melee attacks."
					)
				];
			}
			else
			{
				return [];
			}
		}
	}

	public class ChoiceB : EventChoiceModel, IEventSubscriber
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Offer to buy the Eagle Totem.";

		public override EventResolveType GetEventResolveType(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return EventResolveType.Lost;
			}
			else
			{
				return EventResolveType.ReturnCardToBottom;
			}
		}

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Sum(character => character.Gold) >= 10;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					"The Eagle Totem!" the Inox nods in agreement as he places a totem resembling an eagle on the table. "Sought after by many healers as a sign of watchfulness. Wise choice."
					""";
			}
			else
			{
				return
					"""
					"Maybe a different day then."
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new LoseCollectiveGoldEventReward(10),
					new TotemEventReward(
						obstacle =>
						{
							ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
								parameters =>
									parameters.Figure is Character &&
									RangeHelper.Distance(parameters.Figure.Hex, obstacle.Hex) <= 1,
								async parameters =>
								{
									ActionState actionState = new ActionState(parameters.Figure,
										[
											HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build()
										]
									);
									await actionState.Perform();
								}
							);
						},
						obstacle =>
						{
							ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(this);
						},
						"Eagle",
						color =>
							$"Whenever a character ends their turn adjacent to this obstacle, they may perform “{Icons.Inline(Icons.Heal, color: color)}1, self”."
					)
				];
			}
			else
			{
				return [];
			}
		}
	}
}