using System;
using System.Collections.Generic;
using Fractural.Tasks;

public class City22 : CityEventModel<City22.ChoiceA, City22.ChoiceB>
{
	public override int Number => 22;

	public override string Text =>
		"""
		A new Quatryl magic shop has opened up in the Mixed District and has been advertised all over town. You decide to visit the shop on opening day, and as you approach the magic store you see a sign that reads 'GLOOMAGIC'.

		You wonder if this is a novelty store as you're greeted by a Quatryl with a black magic top hat holding a wand. "Welcome! We're having a magic trick giveaway to the first 50 costumers, and well, you're the first!" It's half-past noon, the shop has been open for several hours. You begin to wonder if you're wasting your time when the Quatryl pulls out several small boxes.

		"Would you like to try the Disappearing Act trick? Or perhaps our Spinning Wheel trick?"
		""";

	public class ChoiceAOnScenarioStartedReward : OnScenarioStartedReward, IEventSubscriber
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"Once, during the next scenario, a character may perform a “{Icons.Inline(Icons.GetCondition(Conditions.Invisible))}, self” ability during their turn.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			AbilityCmd.SubscribeDuringCharacterTurn(this, EffectType.Selectable,
				character => true,
				async character =>
				{
					ActionState actionState = new ActionState(character,
						[
							ConditionAbility.Builder().WithConditions(Conditions.Invisible).WithTarget(Target.Self).Build()
						]
					);
					await actionState.Perform();

					AbilityCmd.UnsubscribeDuringCharacterTurn(this);
				}, new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Invisible)),
				new TextEffectInfoView.Parameters(
					$"Perform “{Icons.Inline(Icons.GetCondition(Conditions.Invisible))}, self” as a reward from the last Road Event.")
			);
		}
	}

	public class ChoiceA : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Take the Disappearing Act trick.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			"The Disappearing Act trick! It's quite simple to use," the Quatryl glees as he proceeds to show you how to use the trick. "Wishing you the best and please, tell your friends about us!"
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceAOnScenarioStartedReward()
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Select the Spinning Wheel trick.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			"The Spinning Wheel! Great trick for causing confusion," the Quatryl joyfully exclaims as he proceeds to show you how to use the trick. Don't forget to spread the word about the shop opening!"
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new AllMonstersStartScenarioWithConditionReward(Conditions.Muddle)
		];
	}
}