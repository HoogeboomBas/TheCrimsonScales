using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class PreparedRations : ChieftainCardModel<PreparedRations.CardTop, PreparedRations.CardBottom>
{
	public override string Name => "Prepared Rations";
	public override int Level => 1;
	public override int Initiative => 91;
	protected override int AtlasIndex => 10;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Pack Mule")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/pack_mule_AI.png")
				.WithHealth(4, new SummonHealthSquare(this, new Vector2(0.44591248f, 0.18810727f)))
				.WithMove(3, new SummonMoveSquare(this, new Vector2(0.6773092f, 0.18810727f)))
				.WithTraits(
					new RetaliateTrait(1),
					new MountTrait(
						async (owner, mount) =>
						{
							ScenarioEvents.RoundEndedEvent.Subscribe(owner, this,
								parameters => true,
								async parameters =>
								{
									ActionState actionState = new ActionState(owner,
										[HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()]);
									await actionState.Perform();
								}
							);
							await GDTask.CompletedTask;
						},
						async (owner, mount) =>
						{
							ScenarioEvents.RoundEndedEvent.Unsubscribe(owner, this);
							await GDTask.CompletedTask;
						}
					)
				)
				.Build()
			),
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6211601f, 0.7209304f)))
				.Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2, new HealCircle(this, new Vector2(0.49397123f, 0.81769234f)))
				.WithTarget(Target.Self)
				.Build()),
		];
	}
}