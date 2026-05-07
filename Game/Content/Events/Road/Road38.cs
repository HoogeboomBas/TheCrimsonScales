using System.Collections.Generic;
using Fractural.Tasks;

public class Road38 : RoadEventModel<Road38.ChoiceA, Road38.ChoiceB>
{
	public override int Number => 38;

	public override string Text =>
		"""
		As nightfall approaches, you seek a place to set up camp for the night when you come across an abandoned campsite. As you peek into the tent, you find a small group of Vermlings staring back at you. They seem to have settled in and don't look interested in leaving.
		""";

	public class ChoiceAOnScenarioStartedReward : OnScenarioStartedReward, IEventSubscriber
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"Once during the next scenario, one character may forgo a top action to perform “{Icons.Inline(Icons.Attack, textParameters)}4, {Icons.Inline(Icons.Range, textParameters)}3”.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			Figure figure = await AbilityCmd.SelectFigure(GameController.Instance.CharacterManager.GetCharacter(0),
				list =>
				{
					list.AddRange(GameController.Instance.CharacterManager.Characters);
				}, hintText: () => "Choose a character to receive the Road Event benefit"
			);

			ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
				parameters =>
					parameters.Performer == figure &&
					parameters.AbilityCardSide.AbilityCardSideType is AbilityCardSideType.Top or AbilityCardSideType.BasicTop &&
					!parameters.ForgoneAction,
				async parameters =>
				{
					ScenarioEvents.AbilityCardSideStartedEvent.Unsubscribe(this);

					parameters.ForgoAction();

					ActionState actionState = new ActionState(figure,
					[
						AttackAbility.Builder()
							.WithDamage(4)
							.WithRange(3)
							.Build()
					]);
					await actionState.Perform();
				},
				EffectType.Selectable,
				effectButtonParameters: new IconEffectButton.Parameters(Icons.Attack),
				effectInfoViewParameters: new TextEffectInfoView.Parameters(
					$"Forgo a top action to perform “{Icons.Inline(Icons.Attack)}4, {Icons.Inline(Icons.Range)}3”.")
			);
		}
	}

	public class ChoiceA : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Fight the Vermlings off. You deserve a place of rest more than they do.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You draw your weapon, ready to fight the Vermlings off. They immediately scatter at the sight of your blade, leaving behind a small backpack filled with supplies.

			Within the backpack you find a crossbow and a single bolt. This will come in handy on the battlefield.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceAOnScenarioStartedReward()
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Leave and continue searching for a place to rest. It's not worth the fight.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			Not wanting to pick a fight, you depart from the tent. A few minutes later you happen across an empty caravan. There are no Vermlings, but you do find a few random items that might fetch you a few coins at the market.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveGoldReward(10)
		];
	}
}