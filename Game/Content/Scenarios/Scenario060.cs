using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;

public class Scenario060 : SoloScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario060.tscn";

	public override int ScenarioNumber => 60;
	public override string Name => "Through the Fire and the Flames";
	public override ClassModel ClassModel => ModelDB.Class<FireKnightModel>();
	protected override List<ScenarioRequirement> Requirements { get; } = [new SoloScenarioRequirement(ModelDB.Class<FireKnightModel>())];

	public override string IntroductionText =>
		"""
		Why do these creatures have to live in such remote mountain peaks? At least you enjoy the challenge of scaling the rocks, but you question yourself for taking on the task that lies ahead of you. All this effort for some shiny armor? Not just any armor, you remind yourself. It’s the signature armor of the Fire Knights that have been crafted and worn with pride since the sect was formed after the Demon War. Plus, a Valrath is raised to never back down from a challenge, and you don’t plan to be the first Fire Knight to break tradition.

		When a Fire Knight begins to develop their fire-manipulating abilities, they are tasked by the chiefs to kill an elite fire-wielding beast as a rite of passage, signifying their mastery over the flames that ravaged their homeland years ago. The memory of the fallen will never be forgotten. This rite of passage also serves to keep the Fire Knights’ abilities fresh. After all, there’s no better way to keep your skills sharp than in combat with a beast who is trying to burn you alive! And those who survive share their newfound skills with the rest of the clan. That’s how it’s always been, and who are you to turn down a good fight anyway?

		So here you are hunting a fire-wielding beast in hopes of bringing back some valuable new skills to the clan, but this peak is so remote that you are starting to lose all hope of find ing any creature at all. Just as you are about to give up, the rocky crags that you have been scaling for the last hour flatten to reveal the mouth of a cave that descends into the depths of the mountain. Now you’re getting somewhere. After a few steps into the cave, the boulder in front of you begins to move. It appears this Savvas Lavaflow is keen to prevent you from discovering what ever lies inside this mountain.
		""";

	public override string ConclusionText =>
		"""
		The Drakefiend falls to the cavern floor once more and the cave suddenly falls eerily silent. The other beasts in the cavern drop to the ground lifeless, apparently sharing some sort of bond with the Drakefiend’s life-form.

		You cautiously approach to find it solidified into a proper corpse this time. No ash pile. No wind. Just stillness. Not wanting to take any chances, you grab your axe and hack at the creatures hollow neck until the head is severed. “Let’s see you come back from that,” you think to yourself.

		But there is no time to revel in victory in the heart of a mystic mountain whose arcane power has already brought one mythical creature back from the ashes. As you exit the cave, you can’t help but reminisce about the battle that just took place. You are certainly lucky to have survived the encounter, but that was exactly the novel type of experience you were hoping for.

		The sheer power of covering the battlefield in flames and the ability to raise a creature from the ashes are skills that will be immensely difficult to learn, and even harder to master. It’s time now to return to the Fire Knights, recount your experiences, and resume your training. Plus, that cold drink with the crew is long overdue by now.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<Drakefiend>(),
		ModelDB.Monster<FlameDemon>(),
		ModelDB.Monster<RendingDrake>(),
		ModelDB.Monster<SavvasLavaflow>(),
		ModelDB.Monster<SpittingDrake>()
	];

	public override List<SavedReward> Rewards =>
	[
		new SoloScenarioReward(ModelDB.Item<FlamingArmor>())
	];

	public override IEnumerable<AbilityCardModel> UnpickableCardModels { get; } = [ModelDB.AbilityCard<LoyalCompanion>()];

	private ScenarioRule _lavaflowRule;

	public override async GDTask OnSetupCompleted()
	{
		await base.OnSetupCompleted();

		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<Drakefiend>()));

		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 48).SetObtainLootFunction(async lootingCharacter =>
		{
			await AbilityCmd.GainGold(lootingCharacter, 15);
		});

		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 49).SetItemLoot(ModelDB.Item<MagmaWaders>());

		await ShowText("Summon Spotted Hound", textParameters =>
			$"At the beginning of the scenario, before choosing cards for the first round, place your “Loyal Companion” ability card in your active area and summon the Spotted Hound in any hex adjacent to the Fire Knight. Do not add a {Icons.InlineCondition(Conditions.Bless, textParameters)} card to your deck. This card does not count toward your hand limit for this scenario. The Spotted Hound has modified stats.");

		FireKnight fireKnight = (FireKnight)GameController.Instance.CharacterManager.FirstAlive();

		AbilityCard abilityCard = new AbilityCard(fireKnight.SavedCharacter.AvailableAbilityCards.First(card => card.Model is LoyalCompanion),
			fireKnight);

		fireKnight.AddCard(abilityCard);

		if(!GameController.FastForward)
		{
			Log.Write($"Playing {abilityCard.Model.Name} Top.");
		}

		ScenarioEvents.AbilityStartedEvent.Subscribe(this,
			_ => true,
			async parameters =>
			{
				if(parameters.AbilityState is SummonAbility.State summonState)
				{
					summonState.AdjustMove(1);
					summonState.AdjustHealth(ScenarioLevel * 3 + 2 - summonState.Health);
				}
				else
				{
					parameters.AbilityState.SetBlocked();
				}

				await GDTask.CompletedTask;
			});
		await abilityCard.Top.Perform(fireKnight);

		ScenarioEvents.AbilityStartedEvent.Unsubscribe(this);

		Summon spottedHound = (Summon)GameController.Instance.Map.Figures.FirstOrDefault(figure => figure is Summon);

		if(spottedHound != null)
		{
			spottedHound.SetCanTakeTurn(true);
			AddScenarioRule(textParameters =>
				$"You may lose cards as normal to negate damage done to the Fire Knight or the Spotted Hound. The Spotted Hound’s turn is directly after the Fire Knight. At the start of the Spotted Hound’s turn, if there are no enemies on which to focus and it is already adjacent to the Fire Knight, it performs {Icons.Inline(Icons.Heal, textParameters)}2, self.");

			ScenarioCheckEvents.InitiativeCheckEvent.Subscribe(this,
				initiativeCheckParameters => initiativeCheckParameters.Figure == spottedHound && !fireKnight.Initiative.Null,
				initiativeCheckParameters => initiativeCheckParameters.SetSortingInitiative(fireKnight.Initiative.SortingInitiative + 1));

			object loseHandCardToCancelDamageSubscriber = new object();
			ScenarioEvents.SufferDamageEvent.Subscribe(this, loseHandCardToCancelDamageSubscriber,
				parameters => parameters.Figure == spottedHound && parameters.WouldSufferDamage &&
				              fireKnight.Cards.Any(card => card.CardState == CardState.Hand && card.OriginalOwner == fireKnight),
				fireKnight.LoseCardToCancelDamage, EffectType.Selectable,
				effectButtonParameters: new IconEffectButton.Parameters(Icons.LoseCard),
				effectInfoViewParameters: new TextEffectInfoView.Parameters("Lose a card from hand to negate the damage"));

			object loseDiscardedCardsToCancelDamageSubscriber = new object();
			ScenarioEvents.SufferDamageEvent.Subscribe(this, loseDiscardedCardsToCancelDamageSubscriber,
				parameters => parameters.Figure == spottedHound && parameters.WouldSufferDamage &&
				              fireKnight.Cards.Count(card => card.CardState == CardState.Discarded && card.OriginalOwner == fireKnight) >= 2,
				fireKnight.LoseDiscardedCardsToCancelDamage, EffectType.Selectable,
				effectButtonParameters: new IconEffectButton.Parameters(Icons.LoseDiscardedCards),
				effectInfoViewParameters: new TextEffectInfoView.Parameters("Lose two cards from your discard pile to negate the damage"));

			ScenarioEvents.FigureFoundFocusEvent.Subscribe(this,
				focusParameters => focusParameters.Performer == spottedHound &&
				                   focusParameters.AbilityState is MoveAbility.State &&
				                   (focusParameters.Focus == fireKnight || focusParameters.Focus == null) &&
				                   RangeHelper.Distance(fireKnight.Hex, spottedHound.Hex) <= 1,
				async _ =>
				{
					await new ActionState(spottedHound, [HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()]).Perform();

					await GDTask.CompletedTask;
				}, order: 1000);
		}

		_lavaflowRule = AddScenarioRule(textParameters =>
			$"Whenever the Savvas Lavaflow would summon an Earth Demon and generate {Icons.InlineElement(Element.Earth, textParameters)}, it summons one normal Flame Demon and generate {Icons.InlineElement(Element.Fire, textParameters)} instead.");

		ScenarioEvents.AbilityPerformedEvent.Subscribe(this,
			parameters => parameters.AbilityState is MonsterSummonAbility.State monsterSummonState && monsterSummonState.MonsterModel is EarthDemon,
			async parameters =>
			{
				((MonsterSummonAbility.State)parameters.AbilityState).SetMonsterModel(ModelDB.Monster<FlameDemon>());

				ScenarioEvents.InfuseElementEvent.Subscribe(this,
					elementParameters => elementParameters.Authority == parameters.Authority && elementParameters.Element == Element.Earth,
					async elementParameters =>
					{
						ScenarioEvents.InfuseElementEvent.Unsubscribe(this);
						elementParameters.SetCanInfuse(false);
						await AbilityCmd.InfuseElement(elementParameters.PotentialAbilityState, Element.Fire,
							elementParameters.Authority);
					});

				await GDTask.CompletedTask;
			});

		AddScenarioRule(textParameters =>
			$"Whenever the Fire Knight or the Spotted Hound would gain {Icons.InlineCondition(Conditions.Poison1, textParameters)}, they gain {Icons.InlineCondition(Conditions.Wound1, textParameters)} instead. If they already have {Icons.InlineCondition(Conditions.Wound1, textParameters)}, they gain {Icons.InlineCondition(Conditions.Muddle, textParameters)}.");

		ScenarioEvents.InflictConditionEvent.Subscribe(this,
			parameters => (parameters.Target == fireKnight || parameters.Target == spottedHound) && parameters.ConditionModel is Poison,
			async parameters =>
			{
				parameters.SetPrevented(true);
				if(parameters.Target.HasWound())
				{
					await AbilityCmd.AddCondition(parameters.PotentialAbilityState, parameters.Target, Conditions.Muddle);
				}
				else
				{
					await AbilityCmd.AddCondition(parameters.PotentialAbilityState, parameters.Target, Conditions.Wound1);
				}
			});
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters roomRevealedParameters)
	{
		await base.OnRoomRevealed(roomRevealedParameters);

		if(roomRevealedParameters.Room == GameController.Instance.Map.Rooms[1])
		{
			Monster monster = (Monster)GameController.Instance.Map.Figures.First(figure =>
				figure is Monster monster && monster.MonsterModel is RendingDrake);
			monster.SetMaxHealth(monster.MaxHealth * 3 / 2);
			monster.SetHealth(monster.MaxHealth);

			await ShowText(
				"As you continue through the cave deeper into the mountain, you find yourself face to face with a fire-breathing reptile basking in the warmth of a few hot coals. The glow from the flames surrounds the Drake’s body even as it steps away from the coals, but there is little time to ponder this mystery before it advances on your position.");

			_lavaflowRule.Remove();
		}
		else if(roomRevealedParameters.Room == GameController.Instance.Map.Rooms[2])
		{
			await ShowText("""
			               You are considering turning back from this tiresome adventure when the cave tunnel opens up into a huge cavern. Awestruck by the sheer size, you squint into the darkness, trying to make out the opposite cavern wall, when you notice the cold mountain air suddenly getting warmer. A heart-stopping roar bellows from the darkness, and the dark cave walls are suddenly glowing bright with the reflections of dancing flames.

			               You find yourself simultaneously excited and terrified as a demon-like creature unfurls its fiery wings, taking flight in a dazzling blaze of glorious flame. A Drakefiend! You’ve heard about these legendary creatures in the tales of the Demon Wars, but never thought you would see one with your own eyes!
			               """);

			//For now at least, going to make this end the round at the of the current turn and then spawn at the end of round, its quite messy otherwise

			ScenarioRule drakefiendRule = AddScenarioRule(
				"When the Drakefiend would be killed for the first time, end the current round at the end of the turn and something will happen at the end of round.");

			ScenarioEvents.BeforeFigureKilledEvent.Subscribe(this,
				parameters => parameters.Figure is Monster monster && monster.MonsterModel is Drakefiend,
				async parameters =>
				{
					foreach(Figure figure in GameController.Instance.Map.Figures)
					{
						figure.SetCanTakeTurn(false);
					}

					ScenarioEvents.BeforeFigureKilledEvent.Unsubscribe(this);

					parameters.SetPrevented();

					Hex drakefiendHex = parameters.Figure.Hex;

					parameters.Figure.RemoveFromMap();
					parameters.Figure.TweenScale(0f, 0.3f).SetEasing(Easing.InBack).PlayFastForwardable();

					drakefiendRule.Remove();

					ScenarioRule respawnRule = AddScenarioRule(
						$"At the end of the round, remove all Hot Coals overlay tiles from the N1a map tile. Place the Drakefiend in the hex in which it died and reset its current hit point value to {parameters.Figure.MaxHealth / 2}.");

					ScenarioEvents.RoundEndedEvent.Subscribe(this,
						_ => true,
						async _ =>
						{
							ScenarioEvents.RoundEndedEvent.Unsubscribe(this);

							respawnRule.Remove();

							await ShowText("""
							               Fight fire with fire they say, and that’s exactly what you’ve done. With the final blow dealt to the raging beast, it disintegrates into a heap of smoldering ash. This adventure has turned out to be more than you bargained for, so you figure it’s time to get away from these foul creatures and return home to enjoy a cold drink with the rest of the Fire Knight clan. But just as you turn to make for the exit, a gust of wind rushes in from the mouth of the cave, swirling through the cavern and whipping up the pile of ash in a frenzy.

							               Either your eyes are playing tricks on you, or the ash is being formed into the very beast you just slayed! Flames reignite to surround the Drakefiend once more as it bellows forth a vicious roar that echoes throughout the cavern! Dumbfounded, you quickly grab your tools and make ready for battle... again.
							               """);

							foreach(HotCoals hotCoals in GameController.Instance.Map.Rooms[2].Hexes
								        .SelectMany(hex => hex.GetHexObjectsOfType<HotCoals>()))
							{
								await hotCoals.Destroy();
							}

							parameters.Figure.SetHealth(parameters.Figure.MaxHealth / 2);
							parameters.Figure.SetScale(Vector2.One);

							await AbilityCmd.EnterHex(null, parameters.Figure, parameters.Figure, drakefiendHex, true, true);
						});

					await GDTask.CompletedTask;
				});
		}
	}
}