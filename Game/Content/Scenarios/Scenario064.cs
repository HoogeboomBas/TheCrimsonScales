using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario064 : SoloScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario064.tscn";

	public override int ScenarioNumber => 64;
	public override string Name => "Glowing Crystals";
	public override ClassModel ClassModel => ModelDB.Class<LuminaryModel>();
	protected override List<ScenarioRequirement> Requirements { get; } = [new SoloScenarioRequirement(ModelDB.Class<LuminaryModel>())];

	public override string IntroductionText =>
		"""
		That feeling is back. The caverns where you gained your powers have grown new crystals. But these are not the same. These are bad crystals. These will take your powers. They must be destroyed. But the others, the other Lurkers. They do not understand. They want the powers, even though they are bad powers. You do not want to hurt your brothers. But they cannot have the bad powers. You must destroy the bad crystals. You will have to destroy everything in that cavern, even your brothers. They might have the bad powers already.

		Strange glowing crystals, containing similar, yet different, properties to the original crystals that gave Luminaries their elemental mastery have begun to grow inside one of the caverns on the coast that forms the Lurkers’ habitat. Unsure what properties these crystals will bring, but with an instinct that they are not good for you, you decide to destroy the crystals, and all who have come into contact with them. This may not be popular within the Lurker colony, but you feel strongly that it must be done, and that the influence you have in the Lurker colony will overcome their doubts.
		""";

	public override string ConclusionText =>
		"The bad crystals are gone. They are no longer a threat. Some creatures died, including your brothers. This is bad. But the crystals were bad too. And now they are gone. So things will be better. Your brothers gave their lives for a good cause. So that is ok. This cavern does not feel nice any more though. It holds bad memories. You will not come back. Unless the crystals do.";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<Lurker>(),
		ModelDB.Monster<DeepTerror>()
	];

	public override List<SavedReward> Rewards =>
	[
		new SoloScenarioReward(ModelDB.Item<LuminousCarapace>())
	];

	private CustomScenarioGoal _goal;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal(countObjectives: false));
		_goal = await AddGoal(new CustomScenarioGoal(_ => "Destroy all glowing crystals."));

		List<Objective> glowingCrystals = GameController.Instance.Map.GetChildrenOfType<Objective>();

		foreach(Objective glowingCrystal in glowingCrystals)
		{
			glowingCrystal.Init(1, "Glowing Crystal");
			await new AllDamageImmunityTrait().Activate(glowingCrystal);
		}

		ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(this,
			parameters => parameters.PotentialTarget is Objective,
			parameters =>
			{
				parameters.SetCannotBeFocused();
			}
		);

		ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(this,
			parameters => parameters.PotentialTarget is Objective,
			parameters =>
			{
				parameters.SetCannotBeTargeted();
			}
		);

		ScenarioEvents.AbilityEndedEvent.Subscribe(this,
			parameters => parameters.AbilityState.GetCustomValue<bool>(parameters.Performer, "Glow Ability") &&
			              parameters.AbilityState is IAOEAbilityState aoeAbilityState && aoeAbilityState.GetRedAOEHexes().Any(),
			async parameters =>
			{
				IAOEAbilityState aoeAbilityState = (IAOEAbilityState)parameters.AbilityState;
				foreach(Objective glowingCrystal in aoeAbilityState.GetRedAOEHexes().SelectMany(hex => hex.GetHexObjectsOfType<Objective>()))
				{
					if(!glowingCrystal.IsDestroyed)
					{
						await glowingCrystal.Destroy();
						await _goal.AdjustProgress(1);
					}
				}
			});

		AddScenarioRule(
			$"The Glowing Crystals and can only be targeted with {Icons.Inline(LuminaryCardSide.GlowIconPath)} abilities. Whenever you perform a {Icons.Inline(LuminaryCardSide.GlowIconPath)} ability, destroy all Glowing Crystals within the targeted hexes. Only one Glowing Crystal needs to be targeted in order to destroy the connected crystals.");
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters roomRevealedParameters)
	{
		await base.OnRoomRevealed(roomRevealedParameters);

		if(roomRevealedParameters.Room == GameController.Instance.Map.Rooms[2])
		{
			await _goal.SetMaxProgress(8);
		}
	}
}