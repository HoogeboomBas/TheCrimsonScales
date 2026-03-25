using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Electromagnetism : BrightsparkCardModel<Electromagnetism.CardTop, Electromagnetism.CardBottom>
{
	public override string Name => "Electromagnetism";
	public override int Level => 3;
	public override int Initiative => 84;
	protected override int AtlasIndex => 17;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Metal Detector")
				.WithTexturePath("res://Content/Classes/Brightspark/MetalDetector.png")
				.WithHealth(6, new SummonHealthSquare(this, new Vector2(0.4474074f, 0.2185185f)))
				.WithMove(1, new SummonMoveSquare(this, new Vector2(0.67767775f, 0.2179894f)))
				.WithTraits(new AtEndOfTurnTrait(async figure =>
					{
						await new ActionState(figure, [
							LootAbility.Builder().WithRange(1)
								.WithCustomGetLootObtainer(state => ((Summon)state.Performer).CharacterOwner).Build()
						]).Perform();
					}, $"Perform {Icons.Inline(Icons.Loot)}1"))
				.Build()
			),
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(2, new PullCircle(this, new Vector2(0.46054077f, 0.6676248f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.6696296f, 0.6656084f)))
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(Element.Air,
						applyFunction: async applyParameters =>
						{
							((PullAbility.State)applyParameters.AbilityState).AbilityAdjustRange(1);
							((PullAbility.State)applyParameters.AbilityState).AbilityAdjustPull(1);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Pull)}, +1{Icons.Inline(Icons.Range)}")))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithPush(1)
				.Build())
		];
	}
}