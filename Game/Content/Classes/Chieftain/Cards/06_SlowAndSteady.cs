using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SlowAndSteady : ChieftainCardModel<SlowAndSteady.CardTop, SlowAndSteady.CardBottom>
{
	public override string Name => "Slow and steady";
	public override int Level => 1;
	public override int Initiative => 93;
	protected override int AtlasIndex => 6;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Giant Tortoise")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/giant_tortoise_AI.png")
				.WithHealth(6, new SummonHealthSquare(this, new Vector2(0.44711232f, 0.1984234f)))
				.WithMove(1, new SummonMoveSquare(this, new Vector2(0.67825043f, 0.1984234f)))
				.WithAttack(1, new SummonAttackSquare(this, new Vector2(0.44711232f, 0.27472314f)))
				.WithTraits(
					new ShieldTrait(1),
					new MountTrait(
						async (owner, mount) =>
						{
							ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Subscribe(mount, this,
								canApply: parameters =>
									parameters.Figure == owner ||
									parameters.Figure == mount,
								parameters =>
								{
									parameters.SetImmuneToForcedMovement();
								}
							);
							await GDTask.CompletedTask;
						},
						async (owner, mount) =>
						{
							ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Unsubscribe(mount, this);

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
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.6201599f, 0.7623622f)))
				.Build()),
		];
	}
}