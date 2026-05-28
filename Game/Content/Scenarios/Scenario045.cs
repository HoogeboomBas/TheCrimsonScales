using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario045 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario045.tscn";

	public override int ScenarioNumber => 45;
	public override string Name => "Defend the Swamp";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();

	public override string IntroductionText =>
		"""
		The Lingering Swamp stretches for miles and, particularly near the coast, can often be a relatively calming place, though the wildlife is not always friendly. Today though, the animalistic screams that suddenly cut through the air chill even you to the bone.

		As you approach the swampy grounds, the shrieking grows louder and begins ringing in your ears. Imps being slowly rising out of the marsh, clearly disoriented by the shrill noise coming from further within the swamp. You’ll have to make your way through them if you’re going to find the source of the piercing screeching.
		""";

	public override string ConclusionText =>
		"""
		With a half roar, half scream, the Leviathan shrinks back momentarily as the killing blow is delivered, before bursting apart, showering the area and in a viscous, foul smelling goo. You examine the area carefully, looking for its origins, but find nothing. Finding this unnerving, you mark the area down as an area to approach with care in future, in case there are others.

		Meanwhile, your temporary battle colleagues slink away into the marshes. The truce is over, and they may well be foes of yours again in future.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BlackImp>(),
		ModelDB.Monster<CaveBear>(),
		ModelDB.Monster<ForestImp>(),
		ModelDB.Monster<GiantViper>(),
		ModelDB.Monster<LandLeviathan>(),
		ModelDB.Monster<Lurker>(),
		ModelDB.Monster<RendingDrake>(),
		ModelDB.Monster<SpittingDrake>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainReputationReward(1),
		new GainProsperityReward(1),
		new GainCheckmarkReward()
	];

	private IEnumerable<Hex> _markerAHexes;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<LandLeviathan>()));

		_markerAHexes = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.Hex);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		ScenarioRule tempRule = AddScenarioRule(
			$"Spawn a monsters groups in the hexes marked {Icons.Inline(Icons.GetMarker(Marker.Type.a))}. These monsters are allies to you and each other and enemies to all other monsters, are immune to {Icons.Inline(Icons.GetCondition(Conditions.Curse))}.");

		await ShowText(
			"""
			You quickly see what is making, or rather causing others to make, the ear-splitting cry. A huge, purple creature is scything its many tentacles through the reeds, enveloping any creature it finds before swallowing the squealing victims whole. You have never seen anything like this before, but its appetite appears to be insatiable.

			In a rare fit of environmentalism, you decide to protect the swamp and send the beast back to wherever it came from. You quickly realize the error of your ways as you slice at a tentacle—the beast was not merely large, it was partially submerged and is actually huge. As the beast itself screams and rises up towards where you are gathered, you realize you have definitely bitten off more than you can chew.
			""");

		await ShowText(
			"""
			Miraculously however, it seems that you are not the only being that wants to maintain the natural order of the swamp. Some of the more powerful residents have gathered behind you and, without any form of communication, you acknowledge that traditional rivalries have been temporarily set aside to rid this terror from the land. With an odd mixture of teeth, claws and weapons, you turn to face the creature as one.
			""");

		await AbilityCmd.GenericChoice(GameController.Instance.CharacterManager.FirstAlive(),
		[
			ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
				applyFunction: async applyParameters =>
				{
					await SpawnAlliedMonster(ModelDB.Monster<CaveBear>(), MonsterType.Normal);
					await SpawnAlliedMonster(ModelDB.Monster<CaveBear>(), MonsterType.Normal);
					foreach(Figure figure in GameController.Instance.Map.Figures.Where(figure =>
						        figure is Monster monster && monster.MonsterModel is CaveBear))
					{
						ConditionImmunityTrait curseImmunity = new ConditionImmunityTrait(Conditions.Curse);
						await curseImmunity.Activate(figure);
						ScenarioEvents.FigureKilledEvent.Subscribe(this, figure,
							parameters => parameters.Figure == figure,
							async parameters =>
							{
								await curseImmunity.Deactivate(figure);
								ScenarioEvents.FigureKilledEvent.Unsubscribe(this, figure);
							});
					}
				},
				effectButtonParameters: new IconEffectButton.Parameters("res://Content/Monsters/CaveBear/MapIcon.tres"),
				effectInfoViewParameters: new TextEffectInfoView.Parameters("Spawn two normal Cave Bears"),
				effectType: EffectType.SelectableMandatory
			),
			ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
				applyFunction: async applyParameters =>
				{
					await SpawnAlliedMonster(ModelDB.Monster<RendingDrake>(), MonsterType.Normal);
					await SpawnAlliedMonster(ModelDB.Monster<SpittingDrake>(), MonsterType.Normal);
					foreach(Figure figure in GameController.Instance.Map.Figures.Where(figure =>
						        figure is Monster monster && (monster.MonsterModel is RendingDrake || monster.MonsterModel is SpittingDrake)))
					{
						ConditionImmunityTrait curseImmunity = new ConditionImmunityTrait(Conditions.Curse);
						await curseImmunity.Activate(figure);
						ScenarioEvents.FigureKilledEvent.Subscribe(this, figure,
							parameters => parameters.Figure == figure,
							async parameters =>
							{
								await curseImmunity.Deactivate(figure);
								ScenarioEvents.FigureKilledEvent.Unsubscribe(this, figure);
							});
					}
				},
				effectButtonParameters: new IconEffectButton.Parameters("res://Content/Monsters/RendingDrake/MapIcon.tres"),
				effectInfoViewParameters: new TextEffectInfoView.Parameters("Spawn one normal Rending Drake and one normal Spitting Drake"),
				effectType: EffectType.SelectableMandatory
			),
			ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
				applyFunction: async applyParameters =>
				{
					await SpawnAlliedMonster(ModelDB.Monster<Lurker>(), MonsterType.Normal);
					await SpawnAlliedMonster(ModelDB.Monster<Lurker>(), MonsterType.Normal);
					await SpawnAlliedMonster(ModelDB.Monster<GiantViper>(), MonsterType.Elite);
					foreach(Figure figure in GameController.Instance.Map.Figures.Where(figure =>
						        figure is Monster monster && (monster.MonsterModel is GiantViper || monster.MonsterModel is Lurker)))
					{
						ConditionImmunityTrait curseImmunity = new ConditionImmunityTrait(Conditions.Curse);
						await curseImmunity.Activate(figure);
						ScenarioEvents.FigureKilledEvent.Subscribe(this, figure,
							parameters => parameters.Figure == figure,
							async parameters =>
							{
								await curseImmunity.Deactivate(figure);
								ScenarioEvents.FigureKilledEvent.Unsubscribe(this, figure);
							});
					}
				},
				effectButtonParameters: new IconEffectButton.Parameters("res://Content/Monsters/Lurker/MapIcon.tres"),
				effectInfoViewParameters: new TextEffectInfoView.Parameters("Spawn two normal Lurkers and one elite Giant Viper"),
				effectType: EffectType.SelectableMandatory
			),
		], hintText: "Choose a group of monsters to spawn as allies");

		tempRule.Remove();
	}

	private async GDTask SpawnAlliedMonster(MonsterModel monsterModel, MonsterType monsterType)
	{
		await SpawnMonster(null, monsterModel, monsterType, _markerAHexes, GameController.Instance.SavedScenario.ScenarioLevel - 1,
			Alignment.Characters, Alignment.Monsters);
	}
}