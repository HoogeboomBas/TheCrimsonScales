using System.Collections.Generic;
using Fractural.Tasks;

public class Road46 : RoadEventModel<Road46.ChoiceA, Road46.ChoiceB>
{
	public override int Number => 46;

	public override string Text =>
		"""
		You're walking along a shoreline when you take notice of glowing lights emanating from beneath the sand. You dig into the sand and pull out several detached Lurker claws, each glowing in a different color.

		On one side of your feet you see one claw radiating a soft blue glow, while another looks and feels like a hot coal. On the other side of your feet you find a claw radiating a strong yellow, and it's sitting aside a darkened claw with a dim black glimmer.

		You have a feeling the glowing lights will help you on your journey, but only have room for two.
		""";

	public class ChoiceAOnScenarioStartedReward : OnScenarioStartedReward
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"At the start of the next scenario, {Icons.Inline(Icons.GetElement(Element.Ice), textParameters)}, {Icons.Inline(Icons.GetElement(Element.Fire), textParameters)}.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			await AbilityCmd.InfuseElement(null, Element.Ice, immediately: true);
			await AbilityCmd.InfuseElement(null, Element.Fire, immediately: true);
		}
	}

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Take the soft blue and glowing red claws.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You take the soft blue and glowing red claws and continue on with your journey. As you reach your destination, each claw emits one last burst before their glow fades away.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceAOnScenarioStartedReward()
		];
	}

	public class ChoiceBOnScenarioStartedReward : OnScenarioStartedReward
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"At the start of the next scenario, {Icons.Inline(Icons.GetElement(Element.Light), textParameters)}, {Icons.Inline(Icons.GetElement(Element.Dark), textParameters)}.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			await AbilityCmd.InfuseElement(null, Element.Light, immediately: true);
			await AbilityCmd.InfuseElement(null, Element.Dark, immediately: true);
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Take the bright yellow and dim black claws.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You take the bright yellow and dim black claws and continue on with your journey. As you reach your destination, each claw emits one last burst of light before their glow fades away.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceBOnScenarioStartedReward()
		];
	}
}