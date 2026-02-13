using System.Collections.Generic;
using System.Linq;

public class City07 : CityEventModel<City07.ChoiceA, City07.ChoiceB>
{
	public override int Number => 07;

	public override string Text =>
		"""
		Strolling through the Sinking Market, you are surprised when you find yourself suddenly surrounded by wild turkeys. A short Vermling steps out from behind the flock and introduces himself as Gables the Turkey Wrangler and proceeds to explain how he trains the turkeys for combat.

		Gables lifts one of the turkeys up as it gobbles ferociously. "I usually charge twenty gold, but today I'll sell you one for ten. What do you say?"
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Inquire about their flavor.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You inquire about their flavor and Gables looks around nervously as he places the turkey down behind him. "These are specially trained turkeys! How dare you suggest eating one!" he exclaims in a loud voice. His voice softens as he says, "Do you know what it would do to my reputation if people started eating these?"

			Gables turns to walk away, but not before flashing you a wink and whispering in your ear, "Quite delicious."
			""";

		public override List<EventReward> GetRewards(SavedEventState state) => [];
	}

	public class ChoiceB : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Offer to buy a battle turkey.";

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
					You offer to buy the battle turkey and Gables' eyes light up with joy. "First sale of the day! Here's Sally, best take good care of her."
					""";
			}
			else
			{
				return
					"""
					You turn down the offer because you do not have enough gold. "A pity!" Gables replies as he turns to walk away.
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
					new SummonEventReward(
						SummonAbility.Builder()
							.WithName("Battle Turkey")
							.WithTexturePath("res://Content/Classes/Chieftain/Summons/speedy_ostrich_AI.png") //TODO: Generic or AI summon visual?
							.WithHealth(5)
							.WithMove(2)
							.WithAttack(2)
							.WithTraits(new PierceTrait(2))
							.Build()
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