using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario063 : SoloScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario063.tscn";

	public override int ScenarioNumber => 63;
	public override string Name => "A Cornered Viper";
	public override ClassModel ClassModel => ModelDB.Class<MirefootModel>();
	protected override List<ScenarioRequirement> Requirements { get; } = [new SoloScenarioRequirement(ModelDB.Class<MirefootModel>())];

	public override string IntroductionText =>
		"""
		You finally track down a rare Ghost Viper, whose venom is the key ingredient to a simple but powerful poison. You know the stories about the spirits inhabiting this swamp, and that large statue you just passed looked rather disconcerting. But you managed to corner the beast you have been tracking without disturbing anything, and now you just need to subdue and capture it.

		Suddenly you hear someone not too far away, bumbling through the scrub. This idiot is going to get you both killed. And if the creatures of this bog don’t kill him, you’ll do it yourself.
		""";

	public override string ConclusionText =>
		"""
		Well, that was a mess. You can always trust an Inox to ruin everything. The Ghost Viper is dead now, so you’ll need to track down another one. However, not all is lost. Somehow that idiot was in possession of a nifty little mortar that you can use to mix herbs with in the field.

		He clearly must have stolen it from someone brighter, but whoever it was is long gone, and you know better than to look a gift horse in the mouth.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<UnseenBearOfTheMarsh>(),
		ModelDB.Monster<GiantViper>(),
		ModelDB.Monster<LostRanger>(),
		ModelDB.Monster<LivingCorpse>(),
		ModelDB.Monster<LivingSpirit>(),
		ModelDB.Monster<EternalGuardian>()
	];

	public override List<SavedReward> Rewards =>
	[
		new SoloScenarioReward(ModelDB.Item<FieldMortar>())
	];


	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());

		AddScenarioRule(textParameters =>
			$"""
			 The Lost Ranger is an enemy to all figures and treats all figures as enemies. It acts at initiative 30 each round, performing {Icons.Inline(Icons.Attack, textParameters)}3, {Icons.Inline(Icons.Range, textParameters)}5.

			 The Unseen Bears of the Marsh and may target figures who have {Icons.InlineCondition(Conditions.Invisible, textParameters)}.
			 """);

		ScenarioCheckEvents.CanTargetInvisibleCheckEvent.Subscribe(this,
			parameters => parameters.Figure is Monster monster && monster.MonsterModel is UnseenBearOfTheMarsh,
			parameters =>
			{
				parameters.SetCanTargetInvisible();
			});
	}
}