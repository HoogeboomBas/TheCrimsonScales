using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

public class Scenario022 : ScenarioModel
{
	[Serializable, JsonObject(MemberSerialization.OptIn)]
	public class Scenario022Reward : SavedReward
	{
		[JsonProperty]
		private Dictionary<string, int> _tokenCounts;

		public override RewardType Type => RewardType.Immediate;

		public Scenario022Reward()
		{
		}

		public Scenario022Reward(Dictionary<string, int> tokenCounts)
		{
			_tokenCounts = tokenCounts;
		}

		public override string GetLabelText(RichTextParameters textParameters) =>
			$"Each Character gains {Icons.Inline(Icons.Coins, textParameters)}{2} and {Icons.Inline(Icons.XP, textParameters)}{2} for each money token they looted.";

		public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
		{
			await base.ImmediateResolve(savedCampaign, cancellationToken);

			foreach(SavedCharacter savedCharacter in savedCampaign.Characters)
			{
				if(_tokenCounts.TryGetValue(savedCharacter.Guid.ToString(), out int tokenCount))
				{
					savedCharacter.AddGold(tokenCount * 2);
					savedCharacter.AddXP(tokenCount * 2);
				}
			}
		}
	}

	public override string ScenePath => "res://Content/Scenarios/Scenario022.tscn";

	public override int ScenarioNumber => 22;
	public override string Name => "Imp Temple";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();

	public override string IntroductionText =>
		"""
		Following Athan Tredan’s information on the Imp Temple, you head towards the Dagger Forest, albeit a little dubiously as imps do not strike you as the religious type. As you approach the clearing in which the temple is set however, you begin to understand.

		Whenever this temple was built, it was truly spectacular. However, this was clearly long in the past and the forest is quickly reclaiming it. Creepers and vines snake up the walls, the roof is partially collapsed, and there are holes where, you assume, once precious decorations have been looted.

		Stepping into the temple, you realize that the forest has reclaimed the inside too. The temple has obviously gotten its name from its new occupants, who have fully colonized the old temple.

		You see imps of various descriptions, and also the odd Harrower. Someone has also been using this as a hiding place for their ill-gotten gains—there are piles of gold, trinkets and fine clothes dotted around. Admiring the treasure, you fail to notice just how close to collapse the temple is. The walls loom at slightly alarming angles and much of the floor is covered in fallen masonry.

		Suddenly, with an enormous noise, a huge beam crashes into the middle of the floor you are standing in, sending up great clouds of dust. You stagger back, thoroughly disorientated as the imps move in to defend their home.
		""";

	public override string ConclusionText =>
		"""
		Having grabbed as much of the treasure as you can, you decide to escape before the whole temple comes crashing down around you.

		Counting your loot, you say a silent prayer of thanks to Athan and decide to make a large donation to the Great Oak the next time you’re in town.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BlackImp>(),
		ModelDB.Monster<ForestImp>(),
		ModelDB.Monster<HarrowerInfester>(),
	];

	public override List<SavedReward> Rewards
	{
		get
		{
			Dictionary<string, int> tokenCounts = new Dictionary<string, int>();
			foreach(Character character in GameController.Instance.CharacterManager.Characters)
			{
				tokenCounts.Add(character.SavedCharacter.Guid.ToString(), character.ObtainedCoins);
			}

			return [new Scenario022Reward(tokenCounts)];
		}
	}

	private CustomScenarioGoal _goal;

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		await AbilityCmd.AddCondition(null, character, Conditions.Muddle);
	}

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		int impCount = GameController.Instance.SavedCampaign.Characters.Count * 4;

		_goal = await AddGoal(new CustomScenarioGoal(textParameters => $"Kill {impCount} Imps.", hasProgress: true, maxProgress: impCount));

		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 15).SetItemLoot(ModelDB.Item<ShiftingCompass>());
		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 32).SetItemLoot(ModelDB.Item<CuriousPendant>());

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure is Monster monster && monster.MonsterModel is Imp,
			async parameters =>
			{
				await _goal.AdjustProgress(1);
			}
		);
	}
}