using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Road18 : CityEventModel<Road18.ChoiceA, Road18.ChoiceB>
{
	public override int Number => 18;

	public override string Text =>
		"""
		You come across a booth set up on the side of the road, where an Inox bearing articles of clothing resembling tribal gear sits aside a collection of totems in different shapes and sizes.

		"These spiritual totems are used by our tribes in battle," the Inox explains. "For the small price of ten gold, one of them could be yours."

		The Inox places two totems on the table of his booth. "Today, I can sell you the Bull Totem of Aggression or the Dog Totem of Protection. Which would you care to buy?"
		""";

	public class ChoiceA : EventChoiceModel, IEventSubscriber
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Offer to buy the Bull Totem.";

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
					"The Bull Totem!" the Inox smiles as he places a totem resembling a bull on the table. "Channels rage into tactical aggression. Powerful choice."
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
							ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(this,
								parameters =>
									parameters.Figure is Character &&
									RangeHelper.Distance(parameters.Figure.Hex, obstacle.Hex) <= 1,
								parameters =>
								{
									parameters.AddRetaliate(1, 1);
								}
							);

							ScenarioEvents.RetaliateEvent.Subscribe(this,
								parameters =>
									parameters.RetaliatingFigure is Character &&
									RangeHelper.Distance(parameters.RetaliatingFigure.Hex, obstacle.Hex) <= 1 &&
									RangeHelper.Distance(parameters.AbilityState.Performer.Hex, parameters.RetaliatingFigure.Hex) <= 1,
								async parameters =>
								{
									parameters.AdjustRetaliate(1);
									await GDTask.CompletedTask;
								}
							);

							ScenarioEvents.FigureEnteredHexEvent.Subscribe(this,
								parameters => parameters.Figure is Character,
								async parameters =>
								{
									ScenarioCheckEvents.RetaliateCheckEvent.FireChangedEvent();
									await GDTask.CompletedTask;
								}
							);
						},
						obstacle =>
						{
							ScenarioCheckEvents.RetaliateCheckEvent.Unsubscribe(this);
							ScenarioEvents.RetaliateEvent.Unsubscribe(this);
							ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(this);
						},
						"Bull",
						color =>
							$"All characters adjacent to this obstacle gain {Icons.Inline(Icons.Retaliate, color: color)}1."
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

		public override string ChoiceText => "Offer to buy the Dog Totem.";

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
					"The Dog Totem!" the Inox nods in agreement as he places a totem resembling a dog on the table. "Represents the guard dogs used in battle to help protect their owners. Smart choice."
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
							ScenarioCheckEvents.ShieldCheckEvent.Subscribe(this,
								parameters =>
									parameters.Figure is Character &&
									RangeHelper.Distance(parameters.Figure.Hex, obstacle.Hex) <= 1,
								applyParameters =>
								{
									applyParameters.AdjustShield(1);
								}
							);

							ScenarioEvents.SufferDamageEvent.Subscribe(this,
								parameters =>
									parameters.Figure is Character &&
									RangeHelper.Distance(parameters.Figure.Hex, obstacle.Hex) <= 1,
								async parameters =>
								{
									parameters.AdjustShield(1);
									await GDTask.CompletedTask;
								}
							);

							ScenarioEvents.FigureEnteredHexEvent.Subscribe(this,
								parameters => parameters.Figure is Character,
								async parameters =>
								{
									ScenarioCheckEvents.ShieldCheckEvent.FireChangedEvent();
									await GDTask.CompletedTask;
								}
							);
						},
						obstacle =>
						{
							ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(this);
							ScenarioEvents.SufferDamageEvent.Unsubscribe(this);
							ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(this);
						},
						"Dog",
						color =>
							$"All characters adjacent to this obstacle gain {Icons.Inline(Icons.Shield, color: color)}1."
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