using System.Collections.Generic;
using System.Linq;

public class Road51 : RoadEventModel<Road51.ChoiceA, Road51.ChoiceB>
{
	public override int Number => 51;

	public override string Text =>
		"""
		On the road to your next adventure, you spot a trade caravan with a broken wheel off to the side of the path. As you approach, you notice multiple human bodies lying motionless on the ground near the wagon as an Inox and Savvas are rummaging through the chests that fell from the wagon.

		At first, you suspect that you are witnessing a robbery in progress, but after further inspection, you realize they are placing the chests back onto the wagon, taking particular care with an especially ornate chest. You keep your distance for now, but can overhear the pair conversing. "Those bandits came out of nowhere," the Savvas says to its companion. "Yeah," the Inox replies in between heavy breaths. "Keep watch. Could be more on the way." They take a moment to catch their breath before getting back to work. "I'll grab some tools and try to repair the wheel," the Savvas states matter-of-factly.
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		private static readonly ClassModel[] ClassModels =
		[
			ModelDB.Class<FireKnightModel>(),
			ModelDB.Class<ChainguardModel>(),
			ModelDB.Class<HollowpactModel>(),
		];

		public override string ChoiceText => "Help the traders get their wagon back on the road.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Any(character => ClassModels.Contains(character.ClassModel));
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override EventResolveType GetEventResolveType(SavedEventState state) =>
			state.GetCustomValue<bool>(ConditionsMetKey) ? EventResolveType.Lost : EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					While the Savvas watches on the wagon, your strength makes short work of reloading the fallen chests. The Inox standing guard gruffs, "Thanks for helping. Take this schematic as our thanks."
					""";
			}
			else
			{
				return
					"""
					You offer to stand guard while the traders fix their wagon. Unfortunately, this takes much longer than anticipated, but at least they pay your wage as promised before leaving.
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new GainItemDesignEventReward(ModelDB.Item<FlamingAxe>())
				];
			}
			else
			{
				return
				[
					new AllStartScenarioDiscardingEventReward(2),
					new GainGoldEachEventReward(5)
				];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		private static readonly ClassModel[] ClassModels =
		[
			ModelDB.Class<MirefootModel>(),
			ModelDB.Class<SpiritCallerModel>(),
		];

		public override string ChoiceText => "Create a distraction and attempt to steal the ornate chest.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Any(character => ClassModels.Contains(character.ClassModel));
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override EventResolveType GetEventResolveType(SavedEventState state) =>
			state.GetCustomValue<bool>(ConditionsMetKey) ? EventResolveType.Lost : EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					Your allies provide an effective distraction and you are able to sneak away unnoticed with the ornate chest in hand.
					""";
			}
			else
			{
				return
					"""
					Distracting the guards is easy enough, but escaping unscathed is another matter entirely. Fortunately, your ally was quick enough to avoid capture, but you are disappointed to leave empty-handed.
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new GainItemDesignEventReward(ModelDB.Item<FlamingAxe>())
				];
			}
			else
			{
				return
				[
					new AllStartScenarioWithDamageEventReward(2)
				];
			}
		}
	}
}