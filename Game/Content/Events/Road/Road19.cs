using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Road19 : RoadEventModel<Road19.ChoiceA, Road19.ChoiceB>
{
	public override int Number => 19;

	public override string Text =>
		"""
		You come across a booth set up on the side of the road, where an Inox bearing articles of clothing resembling tribal gear sits aside a collection of totems in different shapes and sizes.

		"These spiritual totems are used by our tribes in battle," the Inox explains. "For the small price of ten gold, one of them could be yours."

		The Inox places two totems on the table of his booth. "Today, I can sell you the Kangaroo Totem of Balance or the Camel Totem of Endurance. Which would you care to buy?"
		""";

	public class ChoiceA : EventChoiceModel, IEventSubscriber
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Offer to buy the Kangaroo Totem.";

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
					"The Kangaroo Totem!" the Inox smiles as he places a totem resembling a kangaroo on the table. "Used to instill a sense of balance within those who gaze at it. Brilliant choice."
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
									RangeHelper.Distance(parameters.Performer.Hex, obstacle.Hex) <= 1,
								async parameters =>
								{
									parameters.AbilityState.SingleTargetSetHasAdvantage();

									await GDTask.CompletedTask;
								}
							);
						},
						obstacle =>
						{
							ScenarioEvents.DuringAttackEvent.Unsubscribe(this);
						},
						"Kangaroo",
						color =>
							$"All characters adjacent to this obstacle gain Advantage to all their attacks."
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

		public override string ChoiceText => "Offer to buy the Camel Totem.";

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
					"The Camel Totem!" the Inox nods in agreement as he places a totem resembling an camel on the table. "Thought to aid any to endure the harshest of conditions. Perfect choice."
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
							ScenarioEvents.InflictConditionEvent.Subscribe(this,
								parameters =>
									parameters.Target is Character &&
									RangeHelper.Distance(parameters.Target.Hex, obstacle.Hex) <= 1 &&
									parameters.ConditionModel?.ImmunityCompareBaseConditions != null &&
									parameters.ConditionModel.ImmunityCompareBaseConditions
										.Any(c1 => Conditions.NegativeBaseConditionModels.Contains(c1)),
								async parameters =>
								{
									parameters.SetPrevented(true);

									await GDTask.CompletedTask;
								}
							);

							ScenarioCheckEvents.ImmunitiesVisualCheckEvent.Subscribe(this,
								parameters =>
									parameters.Figure is Character &&
									RangeHelper.Distance(parameters.Figure.Hex, obstacle.Hex) <= 1,
								parameters =>
								{
									foreach(ConditionModel conditionModel in Conditions.NegativeBaseConditionModels)
									{
										parameters.AddImmunity(conditionModel);
									}
								}
							);

							ScenarioEvents.FigureEnteredHexEvent.Subscribe(this,
								parameters => parameters.Figure is Character,
								async parameters =>
								{
									ScenarioCheckEvents.ImmunitiesVisualCheckEvent.FireChangedEvent();
									await GDTask.CompletedTask;
								}
							);
						},
						obstacle =>
						{
							ScenarioEvents.InflictConditionEvent.Unsubscribe(this);
							ScenarioCheckEvents.ImmunitiesVisualCheckEvent.Unsubscribe(this);
							ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(this);
						},
						"Camel",
						color =>
							$"All characters adjacent to this obstacle are immune to negative conditions."
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