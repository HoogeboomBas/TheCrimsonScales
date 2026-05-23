using System.Collections.Generic;
using Fractural.Tasks;
using System.Linq;

public class Scenario023 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario023.tscn";

	public override int ScenarioNumber => 23;
	public override string Name => "Icicle Chambers";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<ChillyScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario026>(true)];

	public override string IntroductionText =>
		"""
		The abandoned Aesther’s lab contained details of some kind of new, or maybe mutated, demons, who she has discovered in a cave in the heights of the Copperneck Mountains. The directions are very thorough, and you find the cave easily. The details of what is inside however, are very sketchy, and you don’t really know what to expect as you creep into the long, gloomy ice cave. Suddenly, a deeply unsettling noise, half wheeze, half growl, comes from the cave, very close to you. Whatever made the noise, you’re about to find out…
		""";

	public override string ConclusionText =>
		"""
		There are many strange and terrifying creatures in and around Gloomhaven, and you figure the city doesn’t need any more, so you perform your civic duty and slaughter them all. The Society of Zoological wonders might disagree—but then the frozen horrors weren’t trying to kill them, were they?
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<FrozenCadaver>(),
		ModelDB.Monster<HailDemon>(),
		ModelDB.Monster<HarrowerIcecrawlers>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainXPReward(15),
		new GainPartyAchievementReward(PartyAchievement.FrozenWarrior),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario026>())
	];

	public override string BGSPath => "res://Audio/BGS/Cave.ogg";

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());

		AddScenarioRule(textParameters =>
			$"Any character may forgo the top or bottom action of their turn to remove all {Icons.InlineCondition(Conditions.Chill, textParameters)} tokens from self or one summon they own within {Icons.Inline(Icons.Range, textParameters)}2.");

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<IronSnare>());

		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
			parameters =>
				!parameters.ForgoneAction && RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 2)
					.Any(figure =>
						figure.HasCondition(Conditions.Chill) &&
						((figure is Summon summon && summon.Owner == parameters.Performer) || parameters.Performer == figure)),
			async parameters =>
			{
				parameters.ForgoAction();

				ActionState actionState = new ActionState(parameters.Performer,
				[
					OtherAbility.Builder()
						.WithPerformAbility(async state =>
						{
							Figure figure = await AbilityCmd.SelectFigure(state, list =>
							{
								list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 2)
									.Where(figure =>
										(figure is Summon summon && summon.Owner == parameters.Performer) || parameters.Performer == figure));
							});

							if(figure == null)
							{
								return;
							}

							await AbilityCmd.RemoveAllChill(figure);
						})
						.Build()
				]);
				await actionState.Perform();
			},
			EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Chill)),
			effectInfoViewParameters: new TextEffectInfoView.Parameters(
				$"Remove all {Icons.Inline(Icons.GetCondition(Conditions.Chill))} from self or one of your summons within {Icons.Inline(Icons.Range)}2.")
		);
	}
}