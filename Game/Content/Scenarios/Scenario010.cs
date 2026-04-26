using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario010 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario010.tscn";

	public override int ScenarioNumber => 10;
	public override string Name => "Den of Monstrosity";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<WondrousScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario017>(), new ScenarioConnection<Scenario019>()];

	public override string IntroductionText =>
		"""
		You remain fascinated by what Selandre called the Frosted Crystal and, having recovered it (somewhat reluctantly) from her, you take it, carefully wrapped, to see her suggested contact, Athan Tredan— Head Keeper of the Gloomhaven Great Oak.

		You know he has a suite of rooms in the Sanctuary, the ancient building that lies in the shade of the Great Oak itself. Part monastery, part university, the building itself is relatively humble, but nevertheless, you feel a certain amount of trepidation due to the air of religious sanctity and academia that pervades the air.

		After some direction, you find Athan’s rooms, and knock firmly on the door. “Enter!” comes a slightly impatient, but not unpleasant voice, and you push open the door to his office.

		The room is part study, part… greenhouse — with a chaos of books and papers piled around watering cans, detailed diagrams of root formations and along the window ledge, pots of seedlings in various stages of growth. The far wall is dominated by a huge tapestry of the Great Oak—its winding boughs and leaves picked out in shimmering golden thread, with the lower branches curling inwards to almost hug a row of supplicant worshippers kneeling at its trunk. This sight is so arresting, that you do not immediately notice the elderly man behind the crowded desk, until he lowers his glasses with an assertive “Yes?”

		Despite the huge monsters you have fought and killed, the self-assured aura of this academic gentleman leaves you quite wrong-footed, and you all begin to stammer a response, when he interjects with a sigh. “I am right in the middle of my research, and there is much to be done here, as always. Can you kindly explain what you are doing in my office, or else please leave, so I can return to my work in peace.”

		You are very much on the back foot now, but manage to explain that you have been told he may be able to shed some light on your discovery, which you unwrap and place on the clearest part of his desk.

		“By the light!” he cries, kicking back his chair. “Where in the world did you find that? And cover it up quickly, before my seedlings get frostburn!”

		You cover it up, and Athan settles down again. You give him a deliberately vague outline of how you acquired it, and ask what its purpose is, and what else he knows about it.

		“It is indeed the Frosted Crystal, which is an ancient stone containing mystical properties. As you are aware, it is permanently cold, but it also holds power against certain elements of evil. Aside from that, and the fact that it has a twin, the so-called Orb of Embers, I know very little, but I can research it—though my own work comes first,” he lectures. “My investigations will also require a rare book. I believe that there is a copy — here” he says, unrolling a map of Gloomhaven across the jumbled desk and stabbing a finger at a building near the docks.

		“Generally speaking, as long as nothing interferes with the Great Oak and her aura of protection, my fellow Keepers and I resist interfering in others’ actions. However, both the Frosted Crystal and the Orb of Embers have the potential to damage the Great Oak, both physically and spiritually, so we have been monitoring this place carefully.”

		“Go there, find the book and bring it to me. I’ll see what I can find—although I am rather busy,” Athan reminds you again. “Also, be careful—there are strange experiments taking place out there using elemental magic—the dockers do not call it the Den of Monstrosity for nothing. Now, can I please return to my writings?” Taking this as a not so-subtle hint to leave, you proceed to the docks at once.

		Approaching the building that Athan had pointed out, you hear strange noises, and see bursts of light from under the door. You kick the door open to be greeted by a range of freakish creatures—and a stack of crates. If the book’s here, it must be in one of those.
		""";

	public override string ConclusionText =>
		"""
		The creatures keep appearing as if out of thin air, slowly snapping their claws and gritting their teeth. You have destroyed enough crates to subdue them though, and they look weary and tired. In the final crate, you find a book. Although it’s written in an ancient language you can’t understand, the cover seems to say ‘The Book of Naiqa’. You firmly clasp the book to your chest to protect it from any flames. Hopefully this was the book Athan Tredan was talking about, or at least something that will help you find out more about the Frosted Crystal and its powers. You are also intrigued by how these monsters have been manipulated and the overwhelming presence of elemental powers.

		In the back room, there are a series of notes on how to modify the creatures. Little of it makes sense, but there is reference to removing the creatures ‘core elemental powers’ before working on them, and a reference to another building nearby. Although you want to return the book to Athan as soon as possible, the idea of removing powers from these creatures that seem to plague you at every turn would be extremely useful…
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BlackImp>(),
		ModelDB.Monster<Lurker>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainCollectiveItemReward(ModelDB.Item<MinorHealingPotion>()),
		new GainCollectiveItemReward(ModelDB.Item<MinorManaPotion>()),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario017>()),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario019>()),
	];

	public override string BGSPath => null;

	private bool _fireConsumed;
	private bool _airConsumed;
	private bool _iceConsumed;
	private bool _earthConsumed;

	private int _crateKillCount = 0;

	private ScenarioRule _impInfuseRule;
	private ScenarioRule _lurkerInfuseRule;
	private ScenarioRule _impConsumeRule;
	private ScenarioRule _lurkerConsumeRule;
	private ScenarioRule _progressRule;

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		AbilityCard card = await AbilityCmd.SelectAbilityCard(character, CardState.Hand, true, hintText: "Select a card to discard");

		if(card != null)
		{
			await card.SetCardState(CardState.Discarded);
		}
	}

	public override async GDTask InitializeBeforeFirstRoomRevealed()
	{
		await base.InitializeBeforeFirstRoomRevealed();

		List<Objective> objectives = GameController.Instance.Map.GetChildrenOfType<Objective>();
		int objectiveHealth =
			2 * (GameController.Instance.SavedCampaign.Characters.Count + GameController.Instance.SavedScenario.ScenarioLevel + 1);
		foreach(Objective objective in objectives)
		{
			objective.Init(objectiveHealth, "Supply Crate");
		}

		await AddGoal(new CustomScenarioGoal(textParameters => "Destroy 5 supply crates", async goal =>
		{
			ScenarioEvents.FigureKilledEvent.Subscribe(goal,
				parameters => objectives.Contains(parameters.Figure as Objective),
				async parameters =>
				{
					await goal.AdjustProgress(1);
				}
			);

			await GDTask.CompletedTask;
		}, hasProgress: true, maxProgress: 5));

		_impInfuseRule = AddScenarioRule(textParameters =>
			$"At the end of every odd round, if there is at least 1 Black Imp present, {Icons.InlineElement(Element.Fire, textParameters)}, {Icons.InlineElement(Element.Air, textParameters)}.");
		_lurkerInfuseRule = AddScenarioRule(textParameters =>
			$"At the end of every even round, if there is at least 1 Lurker present, {Icons.InlineElement(Element.Ice, textParameters)}, {Icons.InlineElement(Element.Earth, textParameters)}.");
		_impConsumeRule = AddScenarioRule(textParameters =>
			$"During their turn, Black Imps can consume {Icons.InlineElement(Element.Fire, textParameters)} to add +1{Icons.Inline(Icons.Attack, textParameters)} to all attack abilities for the round and consume {Icons.InlineElement(Element.Air, textParameters)} to add +2{Icons.Inline(Icons.Range, textParameters)} to their ranged abilities for the round.");
		_lurkerConsumeRule = AddScenarioRule(textParameters =>
			$"During their turn, Lurkers can consume {Icons.InlineElement(Element.Ice, textParameters)} to add +1{Icons.Inline(Icons.Attack, textParameters)} to all attack abilities for the round and consume {Icons.InlineElement(Element.Earth, textParameters)} to perform “{Icons.Inline(Icons.Heal, textParameters)}1, self”.");
		_progressRule = AddScenarioRule(textParameters =>
			$"As you destroy supply crates, {Icons.InlineElement(Element.Fire, textParameters)}, {Icons.InlineElement(Element.Ice, textParameters)}, {Icons.InlineElement(Element.Air, textParameters)} and {Icons.InlineElement(Element.Earth, textParameters)} will no longer be consumed or infused as per the special rules above (in that order).");

		GameController.Instance.Map.Treasures[0].SetItemLoot(AbilityCmd.GetRandomAvailableStone());

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				if(parameters.RoundNumber % 2 == 1)
				{
					// Odd round

					bool blackImpPresent = false;
					foreach(Figure figure in GameController.Instance.Map.Figures)
					{
						if(figure is Monster monster && monster.MonsterModel == ModelDB.Monster<BlackImp>())
						{
							blackImpPresent = true;
						}
					}

					if(blackImpPresent)
					{
						if(_crateKillCount < 1)
						{
							await AbilityCmd.InfuseElement(null, Element.Fire, immediately: true);
						}

						if(_crateKillCount < 3)
						{
							await AbilityCmd.InfuseElement(null, Element.Air, immediately: true);
						}
					}
				}
				else
				{
					// Even round

					bool lurkerPresent = false;
					foreach(Figure figure in GameController.Instance.Map.Figures)
					{
						if(figure is Monster monster && monster.MonsterModel == ModelDB.Monster<Lurker>())
						{
							lurkerPresent = true;
						}
					}

					if(lurkerPresent)
					{
						if(_crateKillCount < 2)
						{
							await AbilityCmd.InfuseElement(null, Element.Ice, immediately: true);
						}

						if(_crateKillCount < 4)
						{
							await AbilityCmd.InfuseElement(null, Element.Earth, immediately: true);
						}
					}
				}

				_fireConsumed = false;
				_airConsumed = false;
				_iceConsumed = false;
				_earthConsumed = false;
			}
		);

		ScenarioEvents.FigureTurnStartedEvent.Subscribe(this,
			parameters => parameters.Figure is Monster,
			async parameters =>
			{
				Monster monster = (Monster)parameters.Figure;
				if(monster.MonsterModel == ModelDB.Monster<BlackImp>())
				{
					if(_crateKillCount < 1 && await AbilityCmd.TryConsumeElement(Element.Fire))
					{
						_fireConsumed = true;
					}

					if(_crateKillCount < 3 && await AbilityCmd.TryConsumeElement(Element.Air))
					{
						_airConsumed = true;
					}
				}

				if(monster.MonsterModel == ModelDB.Monster<Lurker>())
				{
					if(_crateKillCount < 2 && await AbilityCmd.TryConsumeElement(Element.Ice))
					{
						_iceConsumed = true;
					}

					if(_crateKillCount < 4 && await AbilityCmd.TryConsumeElement(Element.Earth))
					{
						_earthConsumed = true;
					}

					if(_earthConsumed)
					{
						ActionState actionState = new ActionState(monster,
						[
							HealAbility.Builder()
								.WithHealValue(1)
								.WithTarget(Target.Self)
								.Build()
						]);
						await actionState.Perform();
					}
				}
			}, order: 99
		);

		ScenarioEvents.AbilityStartedEvent.Subscribe(this,
			parameters => parameters.AbilityState is AttackAbility.State && parameters.Performer is Monster,
			async parameters =>
			{
				AttackAbility.State attackAbilityState = (AttackAbility.State)parameters.AbilityState;
				Monster monster = (Monster)parameters.Performer;
				if(monster.MonsterModel == ModelDB.Monster<BlackImp>())
				{
					if(_fireConsumed)
					{
						attackAbilityState.AbilityAdjustAttackValue(1);
					}
				}

				if(monster.MonsterModel == ModelDB.Monster<Lurker>())
				{
					if(_iceConsumed)
					{
						attackAbilityState.AbilityAdjustAttackValue(1);
					}
				}

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.AbilityStartedEvent.Subscribe(this,
			parameters => parameters.AbilityState is TargetedAbilityState && parameters.Performer is Monster,
			async parameters =>
			{
				TargetedAbilityState targetedAbilityState = (TargetedAbilityState)parameters.AbilityState;
				Monster monster = (Monster)parameters.Performer;
				if(monster.MonsterModel == ModelDB.Monster<BlackImp>())
				{
					if(_airConsumed)
					{
						targetedAbilityState.AbilityAdjustRange(2);
					}
				}

				await GDTask.CompletedTask;
			}, checkDuplicates: false
		);

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure is Objective,
			async parameters =>
			{
				_crateKillCount++;

				switch(_crateKillCount)
				{
					case 1:
						_impInfuseRule.SetText(textParameters =>
							$"At the end of every odd round, if there is at least 1 Black Imp present, {Icons.InlineElement(Element.Air, textParameters)}.");
						_impConsumeRule.SetText(textParameters =>
							$"During their turn, Black Imps can consume {Icons.InlineElement(Element.Air, textParameters)} to add +2{Icons.Inline(Icons.Range)} to their ranged abilities for the round.");
						_progressRule.SetText(textParameters =>
							$"As you destroy more supply crates, {Icons.InlineElement(Element.Ice, textParameters)}, {Icons.InlineElement(Element.Air, textParameters)} and {Icons.InlineElement(Element.Earth, textParameters)} will no longer be consumed or infused as per the special rules above (in that order).");
						break;
					case 2:
						_lurkerInfuseRule.SetText(textParameters =>
							$"At the end of every even round, if there is at least 1 Lurker present, {Icons.InlineElement(Element.Earth, textParameters)}.");
						_lurkerConsumeRule.SetText(textParameters =>
							$"During their turn, Lurkers can consume {Icons.InlineElement(Element.Earth, textParameters)} to perform “{Icons.Inline(Icons.Heal)}1, self”.");
						_progressRule.SetText(textParameters =>
							$"As you destroy more supply crates, {Icons.InlineElement(Element.Air, textParameters)} and {Icons.InlineElement(Element.Earth, textParameters)} will no longer be consumed or infused as per the special rules above (in that order).");
						break;
					case 3:
						_impInfuseRule.Remove();
						_impConsumeRule.Remove();
						_progressRule.SetText(textParameters =>
							$"As you destroy more supply crates, {Icons.InlineElement(Element.Earth, textParameters)} will no longer be consumed or infused as per the special rules above (in that order).");
						break;
					case 4:
						_lurkerInfuseRule.Remove();
						_lurkerConsumeRule.Remove();
						_progressRule.Remove();
						break;
					case 5:
						break;
				}

				await GDTask.CompletedTask;
			}
		);
	}
}