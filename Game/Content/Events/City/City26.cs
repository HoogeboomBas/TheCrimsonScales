using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class City26 : CityEventModel<City26.ChoiceA, City26.ChoiceB>
{
	public override int Number => 26;

	public override string Text =>
		"""
		During a visit to the Old Scales one morning, you overhear an argument between a Harrower and a Human outside a shop. The Human is wearing exterminator apparel and is yelling various slurs at the Harrower.

		Although you can't quite understand the Harrower's response, the chitters and hisses are amplifying and the Harrower is growing angrier as the Human continues on his rant.
		""";

	public class ChoiceA : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Intervene on behalf of the Human. Harrowers have no place here.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You intervene on behalf of the Human and demand the Harrower leave the premises. Outnumbered, the Harrower lets out a shrill noise before storming away.

			The human gratefully hands you a canister of poison gas used in his line of work and proceeds to enter the shop.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(this,
						parameters =>
							parameters.Performer is Character &&
							GameController.Instance.ScenarioPhaseManager.RoundIndex == 0,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Poison1);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				},
				color =>
					$"During the first round of the next scenario, all characters add {Icons.Inline(Icons.GetCondition(Conditions.Poison1))} to all their attacks."
			)
		];
	}

	public class ChoiceB : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Intervene on behalf of the Harrower. Discrimination will not be tolerated.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You intervene on behalf of the Harrower and demand the Human leave it alone. Outnumbered, the Human curses under his breath as he turns around and enters the shop. The Harrower chitters gleefully and explains that it belongs to a sect of medically inclined Harrowers and is willing to aid you on your next journey.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					ScenarioEvents.InflictConditionEvent.Subscribe(this,
						parameters =>
							parameters.Target is Character &&
							parameters.Condition.ImmunityCompareBaseConditions.Any(conditionModel => conditionModel == Conditions.Muddle),
						async parameters =>
						{
							parameters.SetPrevented(true);

							await GDTask.CompletedTask;
						}
					);

					ScenarioCheckEvents.ImmunitiesVisualCheckEvent.Subscribe(this,
						parameters => parameters.Figure is Character,
						parameters =>
						{
							parameters.AddImmunity(Conditions.Muddle);
						}
					);

					await GDTask.CompletedTask;
				},
				color =>
					$"During the next scenario, all characters are immune to {Icons.Inline(Icons.GetCondition(Conditions.Muddle))}."
			)
		];
	}
}