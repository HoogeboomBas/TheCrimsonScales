using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario016 : ScenarioModel
{
	public class Scenario016DowntimeEnhancementCostReward : DowntimeEnhancementCostReward
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"The next {Icons.Inline(Icons.PlusOneEnhancement, textParameters)} enhancement for a level 1/X card purchased during the next City Phase will be free.";

		public Scenario016DowntimeEnhancementCostReward()
		{
		}

		protected override void CalculateCostApplyFunction(BetweenScenariosEvents.CalculateEnhancementCost.Parameters parameters)
		{
			if(parameters.EnhancementModel is IPlusOneEnhancement && parameters.SavedAbilityCard.Model.Level == 1)
			{
				parameters.AdjustCost(-parameters.Cost);
			}
		}

		protected override void EnhancementBoughtApplyFunction(BetweenScenariosEvents.EnhancementBought.Parameters parameters)
		{
			if(parameters.EnhancementModel is IPlusOneEnhancement && parameters.SavedAbilityCard.Model.Level == 1)
			{
				Complete();
			}
		}
	}

	public override string ScenePath => "res://Content/Scenarios/Scenario016.tscn";

	public override int ScenarioNumber => 16;
	public override string Name => "Preto Krisanta";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SailScenarioChain>();

	public override string IntroductionText =>
		"""
		You return to the docks, seeking the Apex Demon’s ship. You don’t quite know what you will encounter, but you do want to know why he tried to prevent you from travelling to the island that Sankas told you about. Although you are careful to avoid the Harbour Master after your disastrous attempt to land the ship you took to the island, you find the Preto Krisanta easily enough.

		Not the largest ship on the docks, it is however, the most imposing. Made entirely out of a very dark wood, rather than the usual oak, it also has black sails raised, and seems to suck the light itself into it. It certainly appears unnatural, an impression reinforced by the demons patrolling the deck.

		“We have come to see the Apex Demon” you call, as you approach the gang plank. The demons on deck do not answer, but merely towards you, ready to fight. Looks like you’re going to have to do it the hard way.
		""";

	public override string ConclusionText =>
		"""
		After a lengthy battle, the Apex Demon lies mortally wounded on the floor. As you prepare to finish him off, he uses the last of his strength to croak a series of questions at you: “Have you considered why I tried to stop you? Do you know what Selandre is planning with that thing? Have you even stopped to consider whether what you are doing is right?”

		You weigh the gold in your pocket, shrug and run your sword through him one last time.

		However, you decide to return to The Sleeping Lion rather than the Crimson Scale, and the demon’s questions run round your head for a while.

		Eventually though, you realize that morality has never really bothered you in the past, and Selandre is a well paying client. Whatever she’s up to, she’s keeping the money flowing straight into your pocket—which you then responsibly invest behind the many bars of Gloomhaven.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<ApexDemon>(),
		ModelDB.Monster<BlackImp>(),
		ModelDB.Monster<FlameDemon>(),
		ModelDB.Monster<NightDemon>(),
		ModelDB.Monster<SunDemon>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainGoldEachReward(10),
		new Scenario016DowntimeEnhancementCostReward()
	];

	private Door _door1;
	private bool _treasureRoom3Looted;
	private bool _treasureRoom4Looted;

	private ScenarioRule _treasureTileRule;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		//await AddGoal(new LootGoalTreasuresGoal());
		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<ApexDemon>()));

		_treasureTileRule = AddScenarioRule(textParameters =>
			$"The door marked {Icons.InlineMarker(Marker.Type.a, textParameters)} is locked and becomes unlocked once both Goal treasure tiles have been looted.");

		_door1 = GameController.Instance.Map.GetMarker(Marker.Type.a).GetHexObject<Door>();

		GameController.Instance.Map.Treasures[0].SetObtainLootFunction(async character =>
		{
			await AbilityCmd.SufferDamage(character, HazardousTerrain.DamageAmount, character);
			await AbilityCmd.AddCondition(null, character, Conditions.Invisible);
		});

		GameController.Instance.Map.Treasures[1].SetObtainLootFunction(async character =>
		{
			_treasureRoom3Looted = true;
			if(_treasureRoom4Looted)
			{
				await UnlockDoor1();
			}
		});

		GameController.Instance.Map.Treasures[2].SetObtainLootFunction(async character =>
		{
			_treasureRoom4Looted = true;
			if(_treasureRoom3Looted)
			{
				await UnlockDoor1();
			}
		});

		ScenarioEvents.DoorOpenedEvent.Subscribe(this,
			parameters => parameters.OpenedDoor == _door1,
			async parameters =>
			{
				await ShowText(
					"""
					Using the two keys found in the chests, you unlock the cabin door with questions running through your mind.

					As soon as you swing the door open, a bellowing roar knocks you backwards. The biggest demon you’ve ever seen stands before you. “I am the Apex Demon, guardian of relics!” the demon thunders. “You dared loot the protected island, and now you have invaded my ship? You shall not leave here alive!”
					""");
			}
		);
	}

	private async GDTask UnlockDoor1()
	{
		await _door1.Unlock();
		_treasureTileRule.Remove();
	}
}