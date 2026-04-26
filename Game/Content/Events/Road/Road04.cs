using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Road04 : RoadEventModel<Road04.ChoiceA, Road04.ChoiceB>
{
	public override int Number => 04;

	public override string Text =>
		"""
		After having accidentally stumbled into some thorn-bushes along the path, you find yourself with a deep cut that looks to be getting infected. You tend to the wound to the best of your ability and carry on with little time to lose. You begin to accept the fact that the cut will have to heal over time when you come across a Harrower dressed in red garb standing outside a medical tent.

		The Harrower chitters cheerfully as it introduces itself as a Vimthreader, and offers to tend to your wounds - for a small fee of five gold.
		""";

	public class ChoiceAOnScenarioStartedReward : OnScenarioStartedReward, IEventSubscriber
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"Once, during the next scenario, a character may perform a “{Icons.Inline(Icons.Heal, textParameters)}3, self” ability during their turn.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			AbilityCmd.SubscribeDuringCharacterTurn(this, EffectType.Selectable,
				character => true,
				async character =>
				{
					ActionState actionState = new ActionState(character,
						[
							HealAbility.Builder().WithHealValue(3).WithTarget(Target.Self).Build()
						]
					);
					await actionState.Perform();

					AbilityCmd.UnsubscribeDuringTurn(this);
				}, new IconEffectButton.Parameters(Icons.Heal),
				new TextEffectInfoView.Parameters(
					$"Perform “{Icons.Inline(Icons.Heal)}3, self” as a reward from the last Road Event.")
			);
		}
	}

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Pay the Vimthreader to tend to your cut.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Sum(character => character.Gold) >= 5;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					You pay the Vimthreader and it swiftly stitches up the laceration, chittering and hissing as it weaves each stitch. It thanks you for your payment and offers you a bizarre herbal concoction as a parting gift.
					""";
			}
			else
			{
				return
					"""
					You don't seem to have enough gold, so you ask the Vimthreader to experiment on your wound for free, but it stands idle with no response. As you turn to leave, it grabs your arm and sits you down as it takes out a toolkit.

					The Harrower seems happy to have had the opportunity to stitch up the wound, but it hastily ushers you out of the tent after finishing in anticipation for the next passerby.
					""";
			}
		}

		public override List<SavedReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new LoseCollectiveGoldReward(5),
					new ChoiceAOnScenarioStartedReward()
				];
			}
			else
			{
				return [];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Ask the Vimthreader to experiment on your wound for free.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You ask the Vimthreader to experiment on your wound for free but it stands idle with no response. As you turn to leave, it grabs your arm and sits you down as it takes out a toolkit.

			The Harrower seems happy to have had the opportunity to stitch up the wound, but it hastily ushers you out of the tent after finishing in anticipation for the next passerby.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) => [];
	}
}