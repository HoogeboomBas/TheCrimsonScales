using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario065 : SoloScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario065.tscn";

	public override int ScenarioNumber => 65;
	public override string Name => "Beyond the Grave";
	public override ClassModel ClassModel => ModelDB.Class<SpiritCallerModel>();
	protected override List<ScenarioRequirement> Requirements { get; } = [new SoloScenarioRequirement(ModelDB.Class<SpiritCallerModel>())];

	public override string IntroductionText =>
		"""
		You have been called. It is not entirely unexpected, but you do not feel fully prepared for this moment. Although you have grown into a skilled controller of the spirits, the Haunted Ghoul is a monster skilled in dark magic, with powers that you can only dream of acquiring.

		You always knew that one day you would have to face her—it has long been written that there can only be one true spirit caller, and when there is a challenger of sufficient skill they must face off—so you seek out her tomb, knowing you may not make it out again.
		""";

	public override string ConclusionText =>
		"You strike the Haunted Ghoul and the last of her energy leaves her. She crumbles to the ground, fading as she does so into nothing. Or nearly nothing. A small potion lies on the floor, all that is left of the Ghoul and her power. You pick it up and place it in your satchel. You are the one true Spirit Caller now.";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<VermlingShaman>()
	];

	public override List<SavedReward> Rewards =>
	[
		new SoloScenarioReward(ModelDB.Item<SpiritLibation>())
	];

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());

		List<Hex> markerAHexes = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.Hex).ToList();
		NPC angrySpirit1 = await SpawnNPC(markerAHexes[0], 6, "Angry Spirit", "res://Content/Scenarios/NPCs/Spirits/AngrySpirit", 50,
			[MoveAbility.Builder().WithDistance(3).Build(), AttackAbility.Builder().WithDamage(1).Build()],
			textParameters => $"{Icons.Inline(Icons.Move, textParameters)}3, {Icons.Inline(Icons.Attack, textParameters)}1", Alignment.Monsters);
		NPC angrySpirit2 = await SpawnNPC(markerAHexes[1], 6, "Angry Spirit", "res://Content/Scenarios/NPCs/Spirits/AngrySpirit", 50,
			[MoveAbility.Builder().WithDistance(3).Build(), AttackAbility.Builder().WithDamage(1).Build()],
			textParameters => $"{Icons.Inline(Icons.Move, textParameters)}3, {Icons.Inline(Icons.Attack, textParameters)}1", Alignment.Monsters);
		SubscribeNPC(angrySpirit1);
		SubscribeNPC(angrySpirit2);

		object subscriber = new object();
		ScenarioCheckEvents.FigureFocusCheckEvent.Subscribe(this, subscriber,
			parameters => parameters.AbilityState.Performer is NPC,
			parameters =>
			{
				parameters.SetFocusFigure(GameController.Instance.CharacterManager.FirstAlive());
			});

		AddScenarioRule(
			"Enemy spirits will only focus on the Spirit Caller. Enemy spirits can only be targeted by your spirits. You cannot affect enemy spirits with normal abilities. Vermling Shamans can perform Healing actions on enemy spirits.");
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters roomRevealedParameters)
	{
		await base.OnRoomRevealed(roomRevealedParameters);

		if(roomRevealedParameters.Room == GameController.Instance.Map.Rooms[1])
		{
			List<Hex> markerBHexes = GameController.Instance.Map.GetMarkers(Marker.Type.b).Select(marker => marker.Hex).ToList();
			NPC vileSpirit1 = await SpawnNPC(markerBHexes[1], 4, "Vile Spirit", "res://Content/Scenarios/NPCs/Spirits/VileSpirit", 50, [
					MoveAbility.Builder().WithDistance(2).Build(),
					AttackAbility.Builder().WithDamage(2).WithRange(2).WithConditions(Conditions.Poison1).Build()
				], textParameters =>
					$"{Icons.Inline(Icons.Move, textParameters)}2, {Icons.Inline(Icons.Attack, textParameters)}2, {Icons.Inline(Icons.Range, textParameters)}2, {Icons.Inline(Icons.InlineCondition(Conditions.Poison1, textParameters))}",
				Alignment.Monsters);
			NPC vileSpirit2 = await SpawnNPC(markerBHexes[1], 4, "Vile Spirit", "res://Content/Scenarios/NPCs/Spirits/VileSpirit", 50, [
					MoveAbility.Builder().WithDistance(2).Build(),
					AttackAbility.Builder().WithDamage(2).WithRange(2).WithConditions(Conditions.Poison1).Build()
				], textParameters =>
					$"{Icons.Inline(Icons.Move, textParameters)}2, {Icons.Inline(Icons.Attack, textParameters)}2, {Icons.Inline(Icons.Range, textParameters)}2, {Icons.Inline(Icons.InlineCondition(Conditions.Poison1, textParameters))}",
				Alignment.Monsters);
			SubscribeNPC(vileSpirit1);
			SubscribeNPC(vileSpirit2);

			await ShowText(
				"The first trial—The Angry Spirits—were no match for your own powers. You move into the second chamber to be greeted by her trademark—ethereal Vile Spirits that poison the very air, as well as anything that gets in their way. You know if she is unleashing these, you are getting close to her.");
		}
		else if(roomRevealedParameters.Room == GameController.Instance.Map.Rooms[1])
		{
			NPC hauntedGhoulSpirit = await SpawnNPC(GameController.Instance.Map.GetMarker(Marker.Type.c).Hex, 10, "Haunted Ghoul Spirit",
				"res://Content/Scenarios/NPCs/Spirits/HauntedGhoulSpirit", 50, [AttackAbility.Builder().WithDamage(3).WithRange(4).Build()],
				textParameters => $"{Icons.Inline(Icons.Attack, textParameters)}3, {Icons.Inline(Icons.Range, textParameters)}4", Alignment.Monsters);
			SubscribeNPC(hauntedGhoulSpirit);
			await new ShieldTrait(1).Activate(hauntedGhoulSpirit);

			await ShowText(
				"You push through into the final room, and there she is. Slowly rising from her ancient tomb, the Haunted Ghoul speaks in faint, wavering tones. “You have done well, but I am not ready to pass on... just yet.” She fires a charge of energy from deep within her that flies just above your head. The battle is on.");
		}
	}

	private void SubscribeNPC(NPC npc)
	{
		ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(this, npc,
			parameters => parameters.PotentialTarget == npc && npc.EnemiesWith(parameters.Performer) && !Spirit.CountsAsSpirit(parameters.Performer),
			parameters =>
			{
				parameters.SetCannotBeTargeted();
			});
		ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(this, npc,
			parameters => parameters.PotentialTarget == npc && npc.EnemiesWith(parameters.Performer) && !Spirit.CountsAsSpirit(parameters.Performer),
			parameters =>
			{
				parameters.SetCannotBeFocused();
			});
	}
}