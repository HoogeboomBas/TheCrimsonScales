using System;
using System.Collections.Generic;
using Fractural.Tasks;

public class TestScenario : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/TestScenario.tscn";

	public override int ScenarioNumber => 1;
	public override string Name => "Test Scenario";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();

	public override string IntroductionText =>
		"""
		TODO
		""";

	public override string ConclusionText =>
		"""
		TODO
		""";

	public override List<MonsterModel> MonsterModels { get; } = [];
	// [
	// 	ModelDB.Monster<SpittingDrake>(),
	// 	ModelDB.Monster<VermlingScout>(),
	// 	ModelDB.Monster<WaterSpirit>(),
	// ];

	public override List<SavedReward> Rewards { get; } =
	[
		new GainGoldEachReward(15)
	];

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());

		GameController.Instance.Map.Treasures[0].SetItemDesignLoot(ModelDB.Item<VipertoothDagger>());

		List<Objective> objectives = GameController.Instance.Map.GetChildrenOfType<Objective>();
		int objectiveHealth = 1;
		foreach(Objective objective in objectives)
		{
			objective.Init(objectiveHealth, "Look at this test objective");
		}

		foreach(Element element in Enum.GetValues<Element>())
		{
			await AbilityCmd.InfuseElement(null, element, immediately: true);
		}

		NPC brightspark = await SpawnNPC(GameController.Instance.Map.GetMarker(Marker.Type.b).Hex, CharacterCount + ScenarioLevel * 3, "Brightspark",
			"res://Content/Scenarios/NPCs/Brightspark", 50, [
				MoveAbility.Builder().WithDistance(2).Build(),
				AttackAbility.Builder().WithDamage(1).Build()
			],
			textParameters => $"{Icons.Inline(Icons.Move, textParameters)}2\n{Icons.Inline(Icons.Attack, textParameters)}1");

		ScenarioEvents.FigureFoundFocusEvent.Subscribe(this,
			parameters =>
				parameters.Performer == brightspark &&
				parameters.AbilityState is MoveAbility.State &&
				parameters.Focus == null,
			async parameters =>
			{
				parameters.SetFocusHex(GameController.Instance.Map.GetMarker(Marker.Type.a).Hex);

				ScenarioCheckEvents.AIMoveParametersCheckEvent.Subscribe(this,
					moveParameters => moveParameters.Performer == brightspark,
					moveParameters =>
					{
						moveParameters.SetRange(0);
						moveParameters.SetRangeType(RangeType.Melee);
						moveParameters.SetTargets(1);
						moveParameters.SetAOEPattern(null);
					}
				);

				ScenarioEvents.AbilityEndedEvent.Subscribe(this,
					abilityEndedParameters => abilityEndedParameters.Performer == brightspark,
					async _ =>
					{
						ScenarioEvents.AbilityEndedEvent.Unsubscribe(this);
						ScenarioCheckEvents.AIMoveParametersCheckEvent.Unsubscribe(this);

						await GDTask.CompletedTask;
					}
				);

				await GDTask.CompletedTask;
			});

		ScenarioEvents.FigureTurnEndingEvent.Subscribe(this,
			ScenarioEvents.FigureTurnEnding.Subscription.ConsumeElement([CardElementConsumption.ConsumeWild()],
				canApplyFunction: applyParameters => applyParameters.Figure == brightspark,
				applyFunction: async applyParameters =>
				{
					await new ActionState(brightspark, [HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()]).Perform();
				}, EffectType.Selectable,
				effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Heal)}2, self")));
	}
}