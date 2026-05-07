using System.Collections.Generic;
using Fractural.Tasks;

public class Road47 : RoadEventModel<Road47.ChoiceA, Road47.ChoiceB>
{
	public override int Number => 47;

	public override string Text =>
		"""
		"Thank you! Thank you!" the Vermling falls to your feet and begins kissing your toes. You've helped pull his carriage out of a deep pile of mud, and the green-robed Vermling couldn't be more grateful.

		"Alas!" he exclaims. "A gift, for your troubles." The Vermling ruses into the carriage and pulls out two glowing rods. "Please take one. It will help you rest on your journey."
		""";

	public class ChoiceAOnScenarioStartedReward : OnScenarioStartedReward, IEventSubscriber
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"Whenever a character takes a short rest during the scenario, they may perform “{Icons.Inline(Icons.Heal, textParameters)}1, self”.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			ScenarioEvents.ShortRestStartedEvent.Subscribe(this,
				parameters => true,
				async parameters =>
				{
					ActionState actionState = new ActionState(parameters.Character,
					[
						HealAbility.Builder()
							.WithHealValue(1)
							.WithTarget(Target.Self)
							.Build()
					]);
					await actionState.Perform();
				}
			);
		}
	}

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Take the shorter rod on the left.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			"Ah! The shorter rod will serve you well if you ever decide to quickly rest," the Vermling glees as he hands you the shorter rod.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceAOnScenarioStartedReward()
		];
	}

	public class ChoiceBOnScenarioStartedReward : OnScenarioStartedReward, IEventSubscriber
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"Whenever a character takes a long rest during the scenario, add +1{Icons.Inline(Icons.Heal, textParameters)} to the heal ability.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			ScenarioEvents.DuringHealEvent.Subscribe(this,
				parameters =>
					parameters.Performer is Character character &&
					parameters.AbilityState.Target == character &&
					character.LongResting &&
					parameters.AbilityState.ActionState.ActionSource == null,
				async parameters =>
				{
					parameters.AbilityState.AbilityAdjustHealValue(1);

					await GDTask.CompletedTask;
				}
			);
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Take the longer rod on the right.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			"The longer rod, very well! This rod will ensure that you get the most fulfilment out of your resting," the Vermling smiles while gleefully handing out the longer rod.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceBOnScenarioStartedReward()
		];
	}
}