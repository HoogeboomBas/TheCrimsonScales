using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario012 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario012.tscn";

	public override int ScenarioNumber => 12;
	public override string Name => "Uncovering the Source";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SailScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario015>(true)];

	public override string IntroductionText =>
		"""
		You follow Sankas’s instructions across the heavily overgrown island, spending hours cutting through the thorny undergrowth. This hidden source has clearly lain undisturbed for many years— if it is still here.

		Finally, you stumble upon a pile of nests in a clearing. According to the instructions, you’re getting very close to the spot where the power source lays buried, and you smile with enthusiasm.

		You hear a hissing sound and glance up from your map. To the left, enormous snakes begin to slither out of the bushes toward you. To the right, red scaly lizards with elongated claws stomp around and flicker their tongues at you.

		These creatures are extremely aggressive, and seem to have been imbued with some sort of latent energy from the power source, but nothing will hold you back from claiming it, and the prize that will go along with it.
		""";

	public override string ConclusionText =>
		"""
		Finally! After fighting off many of the island creatures and the demons that surrounded you, you grab shovels from your packs and, with tired arms, begin to dig.

		As you get further down, the energy is such that there’s a crackle in the air, like a continuous static charge. Before long, your blade hits something metallic, and you uncover a strange cube—lead bands surrounding it in all dimensions, with a green-blue glow pulsing from between them, shifting in color and intensity. It is extremely heavy, and you reluctantly begin to heave the cube back to the ship, when you suddenly find yourself face-to-face with more snakes again, with other creatures slowly beginning to surround you.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<FlameDemon>(),
		ModelDB.Monster<GiantViper>(),
		ModelDB.Monster<RendingDrake>(),
		ModelDB.Monster<SpittingDrake>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new UnlockScenarioReward(ModelDB.Scenario<Scenario015>())
	];

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		ItemModel item = await AbilityCmd.SelectItem(character, ItemState.Available, ItemType.Small,
			hintText: $"Select one {Icons.HintText(Icons.GetItem(ItemType.Small))} to {Icons.HintText(Icons.LoseCard)}");

		if(item != null)
		{
			await item.SetItemState(ItemState.Consumed);
		}
	}

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());
		await AddGoal(new LootGoalTreasuresGoal());
	}
}