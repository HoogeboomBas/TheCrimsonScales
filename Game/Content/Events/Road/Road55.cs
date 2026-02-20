using System.Collections.Generic;
using System.Linq;

public class Road55 : RoadEventModel<Road55.ChoiceA, Road55.ChoiceB>
{
	public override int Number => 55;

	public override string Text =>
		"""
		As you travel along the path, you hear strange noises from just out of sight ahead, away from the road. As you approach, a blinding bolt of lightning fills the sky followed shortly by ear-splitting thunder. Undeterred, you press forward.

		As you continue, the sounds of battle end abruptly. You reach the site of the struggle and find two corpses. The first is a battered Savvas whose rocky skin has been partially dissolved into course black sand around the edges of several vicious-looking sword cuts. The second corpse is a charred and frozen Aesther whose shadowy ashblades lay fallen on the ground by its sides alongside an ominously glowing amethyst.
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		private static readonly ClassModel[] ClassModels =
		[
			ModelDB.Class<HollowpactModel>(),
		];

		public override string ChoiceText => "Loot the bodies. They don't need it, we're not picky, and that gem looks valuable.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Any(character => ClassModels.Contains(character.ClassModel));
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state) =>
			"""
			As you touch the glowing stones and ashen blades, you feel a corrupt energy coarse through your body. You immediately drop the items, realizing in horror that these items must have been tainted by the Void. Those of you who are not fortified against the Void feel their life essence drained by the contact. You abandon these accursed items with all haste.
			""";

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new AllStartScenarioWithConditionEventReward(Conditions.Strengthen)
				];
			}
			else
			{
				return
				[
					new AllStartScenarioWithConditionEventReward(Conditions.Curse)
				];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Leave the area. Who knows what lingering effect the powers used may have had.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			Seeing the coarse black sand disfiguring the wounds of the Savvas reminds you of seeing similar damage among victims of the Void in Gloomhaven. That black sand is a telltale sign of corruption and danger. You think better of looting something so suspicious.

			Carefully, without touching the bodies or equipment, you take time to hide the site. Perhaps someday you can return and safely cleanse or destroy these items, but for now, it's better if no one stumbles across them.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioDiscardingEventReward(1)
		];
	}
}